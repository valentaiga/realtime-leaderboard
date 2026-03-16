/* eslint-disable @typescript-eslint/no-unsafe-assignment */
/* @typescript-eslint/no-explicit-any */
// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-nocheck

import {
  Deserializer,
  IncomingEvent,
  IncomingRequest,
} from '@nestjs/microservices';

export class CustomKafkaDeserializer implements Deserializer {
  deserialize(
    value: any,
    options?: Record<string, any>,
  ): IncomingRequest | IncomingEvent {
    // Return the raw message without JSON parsing
    return {
      pattern: options?.topic, // Use topic as pattern
      data: value, // Raw Buffer data
      headers: options?.headers,
    };
  }
}
