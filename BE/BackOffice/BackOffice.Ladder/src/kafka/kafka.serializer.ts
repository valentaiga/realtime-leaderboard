/* eslint-disable @typescript-eslint/no-unsafe-assignment,@typescript-eslint/no-unsafe-member-access */
/* @typescript-eslint/no-explicit-any */
// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-nocheck

import { Serializer, OutgoingResponse } from '@nestjs/microservices';

export class CustomKafkaSerializer implements Serializer {
  serialize(value: any, options?: Record<string, any>): OutgoingResponse {
    // If value is already a Buffer, return it directly
    if (Buffer.isBuffer(value)) {
      return {
        data: value,
        headers: options?.headers,
      };
    }

    // If it's a string, convert to Buffer
    if (typeof value === 'string') {
      return {
        data: Buffer.from(value, 'utf-8'),
        headers: options?.headers,
      };
    }

    // If it's an object with a value property that's a Buffer (common Kafka pattern)
    if (value && typeof value === 'object' && value.value) {
      if (Buffer.isBuffer(value.value)) {
        return {
          data: value.value,
          headers: value.headers || options?.headers,
        };
      }
    }

    // For any other type, you can either:
    // 1. Throw error to enforce raw binary only
    // 2. Convert to Buffer with custom logic
    throw new Error(
      'CustomKafkaSerializer: Only Buffer or string values are allowed',
    );
  }
}
