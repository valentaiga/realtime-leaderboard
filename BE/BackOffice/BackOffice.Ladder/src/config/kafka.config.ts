import { registerAs } from '@nestjs/config';

export default registerAs('kafka', () => ({
  brokers:
    process.env.KAFKA_BROKERS?.split(',') ||
    'localhost:9100,localhost:9101,localhost:9102',
  groupId: process.env.KAFKA_GROUP_ID || 'ladder.group',
}));
