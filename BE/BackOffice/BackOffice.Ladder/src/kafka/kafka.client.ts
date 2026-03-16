/* eslint-disable @typescript-eslint/no-unsafe-member-access,@typescript-eslint/no-unsafe-assignment,@typescript-eslint/no-unsafe-return */
/* @typescript-eslint/no-explicit-any */
// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-nocheck
// custom-kafka.client.ts
import { ClientKafka } from '@nestjs/microservices/client';
import { KafkaOptions } from '@nestjs/microservices';
import { Kafka, Producer, RecordMetadata } from 'kafkajs';
import { Logger } from '@nestjs/common';
import { CustomKafkaSerializer } from './kafka.serializer';
import { CustomKafkaDeserializer } from './kafka.deserializer';

export class CustomKafkaClient extends ClientKafka {
  protected readonly logger = new Logger(CustomKafkaClient.name);
  private producer: Producer;

  constructor(options: KafkaOptions['options']) {
    super(options);

    // Override with custom serializer/deserializer
    this.serializer = new CustomKafkaSerializer();
    this.deserializer = new CustomKafkaDeserializer();

    // Initialize Kafka with custom configuration
    const kafka = new Kafka({
      clientId: options.client?.clientId || 'nestjs-custom-producer',
      brokers: options.client?.brokers || ['localhost:9092'],
      ...options.client,
    });

    // Create producer
    this.producer = kafka.producer(options.producer || {});
  }

  async connect(): Promise<void> {
    try {
      await this.producer.connect();
      this.logger.log('Custom Kafka client connected successfully');
    } catch (error) {
      this.logger.error(`Failed to connect Kafka client: ${error.message}`);
      throw error;
    }
  }

  async close(): Promise<void> {
    await this.producer.disconnect();
    this.logger.log('Custom Kafka client disconnected');
  }

  async publish(packet: any): Promise<any> {
    const { pattern, data, headers } = packet;

    try {
      // Serialize the data
      const serializedPacket = await this.serializer.serialize(data, {
        headers,
      });

      // Send to Kafka
      const result: RecordMetadata[] = await this.producer.send({
        topic: pattern,
        messages: [
          {
            value: serializedPacket.data,
            headers: serializedPacket.headers,
          },
        ],
      });

      return result[0]; // Return first record metadata
    } catch (error) {
      this.logger.error(
        `Error publishing message to ${pattern}: ${error.message}`,
      );
      throw error;
    }
  }

  // Helper method to send raw Buffer
  async sendRaw(
    topic: string,
    data: Buffer,
    headers?: any,
  ): Promise<RecordMetadata> {
    return this.publish({
      pattern: topic,
      data,
      headers,
    });
  }
}
