/* eslint-disable */
//@ts-nocheck
//

// import { Deserializer } from '@nestjs/microservices';
// import { unpack } from 'msgpackr';
// import { KafkaMessage } from 'kafkajs';
// import * as fs from 'node:fs';
// import path from 'node:path';
// import { decode } from '@msgpack/msgpack';
//
// export class MessagePackDeserializer implements Deserializer {
//   deserialize(message: any): any {
//     // const uint8Arr = new Uint8Array();
//     // // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment
//     // uint8Arr[0] = message.value;
//     // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access,@typescript-eslint/no-unsafe-argument
//     console.log(decode(Buffer.from(message.value, 'binary')));
//     // fs.writeFileSync(
//     //   path.join(__dirname, 'newfile.txt'),
//     //   JSON.stringify(message),
//     // );
//     // throw new Error('asd');
//     // console.log(message, uint8Arr);
//     // try {
//     //   return {
//     //     // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment,@typescript-eslint/no-unsafe-member-access
//     //     pattern: message.topic,
//     //     // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment
//     //     data: unpack(uint8Arr),
//     //   };
//     // } catch (error) {
//     //   console.error('Failed to deserialize MessagePack message:', error);
//     //   throw error;
//     // }
//   }
// }

// custom-messagepack-deserializer.ts
import {
  ConsumerDeserializer,
  IncomingEvent,
  IncomingRequest,
} from '@nestjs/microservices';
import { decode } from '@msgpack/msgpack';

export class MessagePackIncomingDeserializer implements ConsumerDeserializer {
  deserialize(
    value: any,
    // options?: Record<string, any>,
  ): IncomingRequest | IncomingEvent {
    // const kafkaMessage = JSON.parse(value.toString());
    // console.log(kafkaMessage);
    // Deserialize MessagePack value
    if (value.value) {
      console.log('yeees');
      const buffer =
        typeof value.value === 'string'
          ? Buffer.from(value.value, 'utf-8')
          : value.value;

      value.value = decode(buffer);
    }

    return {
      pattern: value.topic,
      data: value,
    };
  }
}
