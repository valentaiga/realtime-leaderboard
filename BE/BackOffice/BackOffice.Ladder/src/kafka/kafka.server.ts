/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* @typescript-eslint/no-explicit-any */
// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-nocheck

import { ServerKafka } from '@nestjs/microservices/server';
import { KafkaOptions } from '@nestjs/microservices';
import { Consumer, EachMessagePayload, Kafka } from 'kafkajs';
import { Logger } from '@nestjs/common';
import { CustomKafkaDeserializer } from './kafka.deserializer';
import { CustomKafkaSerializer } from './kafka.serializer';
import { isObservable, lastValueFrom } from 'rxjs';

export class CustomKafkaServer extends ServerKafka {
  protected readonly logger = new Logger(CustomKafkaServer.name);
  private consumer: Consumer;

  constructor(options: KafkaOptions['options']) {
    super(options);

    // Override with custom serializer/deserializer
    this.serializer = new CustomKafkaSerializer();
    this.deserializer = new CustomKafkaDeserializer();

    // Initialize Kafka with custom configuration
    const kafka = new Kafka({
      clientId: options.client?.clientId || 'nestjs-custom-client',
      brokers: options.client?.brokers || ['localhost:9092'],
      ...options.client,
    });

    // Create consumer
    this.consumer = kafka.consumer({
      groupId: options.consumer?.groupId || 'nestjs-custom-group',
      ...options.consumer,
    });

    // Store options for later use
    this.options = options;
  }

  async listen(callback: () => void): Promise<void> {
    try {
      await this.consumer.connect();

      // Get all registered patterns/topics
      const registeredPatterns = Array.from(this.messageHandlers.keys());
      this.logger.log(`Registered patterns: ${registeredPatterns.join(', ')}`);

      // Subscribe to all registered patterns/topics
      for (const pattern of registeredPatterns) {
        await this.consumer.subscribe({
          topic: pattern,
          fromBeginning: this.options?.subscribe?.fromBeginning || false,
        });
        this.logger.log(`Subscribed to topic: ${pattern}`);
      }

      // Start consuming messages
      await this.consumer.run({
        eachMessage: async (payload: EachMessagePayload) => {
          await this.handleMessage(payload);
        },
      });

      callback();
      this.logger.log('Custom Kafka server started successfully');
    } catch (error) {
      this.logger.error(`Failed to start Kafka server: ${error.message}`);
      throw error;
    }
  }

  private async handleMessage({
    topic,
    partition,
    message,
  }: EachMessagePayload): Promise<void> {
    const handler = this.messageHandlers.get(topic);

    if (!handler) {
      this.logger.warn(`No handler found for topic: ${topic}`);
      return;
    }

    try {
      // Create Kafka context
      const kafkaContext = {
        topic,
        partition,
        message: {
          value: message.value,
          key: message.key,
          timestamp: message.timestamp,
          attributes: message.attributes,
          offset: message.offset,
          headers: message.headers,
        },
      };

      // Process with deserializer (optional, depends on your needs)
      const packet = await this.deserializer.deserialize(message.value, {
        topic,
        partition,
        headers: message.headers,
      });

      // Execute handler - THIS IS THE FIXED PART
      // Instead of calling handler directly with lastValueFrom, we need to handle different return types
      let response;

      // Check if handler returns Observable
      const handlerResult = handler(packet.data, kafkaContext);

      if (isObservable(handlerResult)) {
        // If it's an Observable, convert to Promise
        response = await lastValueFrom(handlerResult);
      } else if (handlerResult instanceof Promise) {
        // If it's already a Promise
        response = await handlerResult;
      } else {
        // If it's a synchronous value
        response = handlerResult;
      }

      this.logger.debug(`Message processed successfully for topic: ${topic}`);
    } catch (error) {
      this.logger.error(
        `Error handling message from topic ${topic}: ${error.message}`,
      );
      this.logger.error(error.stack);
    }
  }

  async close(): Promise<void> {
    await this.consumer.disconnect();
    this.logger.log('Custom Kafka server disconnected');
  }
}
