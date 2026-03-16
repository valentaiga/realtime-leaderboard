import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { MicroserviceOptions, Transport } from '@nestjs/microservices';
import { ConfigService } from '@nestjs/config';
import { getHostname } from './utils/hostname';
import {
  // MessagePackDeserializer,
  MessagePackIncomingDeserializer,
} from './utils/msg-deserializer';
import { decode } from '@msgpack/msgpack';
import { CustomKafkaServer } from './kafka/kafka.server';
// import { Deserializer } from 'node:v8';
// import { decode, encode } from '@msgpack/msgpack';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);
  const configService = app.get(ConfigService);

  const brokers = configService.get<string>('KAFKA_BROKERS')!.split(',');
  const clientId = getHostname();
  const groupId = configService.get<string>('KAFKA_GROUP_ID')!;

  // const test = { valentin: 'piska' };
  // const serialisedPiska = encode(test); // Buffer.from(JSON.stringify(test));
  // const deserialisedPiska = decode(serialisedPiska);
  // console.log(decode(encode(test)), serialisedPiska, deserialisedPiska);

  // const uint = [
  //   131, 167, 109, 97, 116, 99, 104, 73, 100, 217, 50, 54, 51, 57, 48, 56, 57,
  //   49, 57, 54, 53, 57, 52, 55, 53, 48, 51, 55, 56, 52, 52, 48, 57, 98, 55, 97,
  //   49, 51, 97, 56, 50, 52, 52, 49, 48, 97, 102, 57, 55, 54, 53, 98, 56, 57,
  //   101, 56, 48, 53, 48, 102, 101, 177, 109, 97, 116, 99, 104, 83, 116, 97, 114,
  //   116, 101, 100, 69, 118, 101, 110, 116, 131, 165, 116, 101, 97, 109, 49, 149,
  //   206, 0, 1, 64, 191, 206, 0, 1, 64, 190, 206, 0, 1, 64, 189, 206, 0, 1, 64,
  //   184, 206, 0, 1, 64, 183, 165, 116, 101, 97, 109, 50, 149, 206, 0, 1, 64,
  //   185, 206, 0, 1, 64, 188, 206, 0, 1, 64, 192, 206, 0, 1, 64, 186, 206, 0, 1,
  //   64, 187, 169, 115, 116, 97, 114, 116, 101, 100, 65, 116, 215, 255, 113, 119,
  //   99, 64, 105, 178, 194, 107, 178, 109, 97, 116, 99, 104, 70, 105, 110, 105,
  //   115, 104, 101, 100, 69, 118, 101, 110, 116, 192,
  // ];
  // const uint8Array = new Uint8Array(uint);
  //
  // // Decode
  // const result = decode(uint8Array);
  // console.log(result);

  // app.connectMicroservice<MicroserviceOptions>({
  //   transport: Transport.KAFKA,
  //   options: {
  //     client: {
  //       brokers,
  //       clientId,
  //     },
  //     consumer: {
  //       groupId,
  //       retry: {
  //         retries: 5,
  //       },
  //     },
  //     // Optional: Run configuration
  //     run: {
  //       autoCommit: false,
  //     },
  //     deserializer: new MessagePackIncomingDeserializer(), // <-- custom deserializer
  //   },
  // });

  app.connectMicroservice<MicroserviceOptions>({
    strategy: new CustomKafkaServer({
      client: {
        brokers,
        clientId,
      },
      consumer: {
        groupId,
        retry: {
          retries: 5,
        },
      },
      // Optional: Run configuration
      run: {
        autoCommit: false,
      },
    }),
  });

  // 3. Start all microservices first
  await app.startAllMicroservices();

  await app.listen(process.env.PORT ?? 5140);
}
bootstrap();
