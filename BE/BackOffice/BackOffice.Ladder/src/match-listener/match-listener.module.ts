import { Module } from '@nestjs/common';
import { MatchListenerService } from './match-listener.service';
import { MatchListenerController } from './match-listener.controller';
import { ClientsModule, Transport } from '@nestjs/microservices';
import { ConfigModule, ConfigService } from '@nestjs/config';
import { getHostname } from '../utils/hostname';
import { MessagePackIncomingDeserializer } from '../utils/msg-deserializer';
import { CustomKafkaClient } from '../kafka/kafka.client';
// import { MsgPackDeserializer } from '../utils/msg-deserializer';

@Module({
  imports: [
    ClientsModule.registerAsync([
      {
        name: 'MATCH_LISTENER',
        imports: [ConfigModule],
        inject: [ConfigService],
        useFactory: (config: ConfigService) => ({
          transport: Transport.KAFKA,
          options: {
            client: {
              clientId: getHostname(),
              brokers: config.get<string>('KAFKA_BROKERS')!.split(','),
            },
            consumer: {
              groupId: config.get<string>('KAFKA_GROUP_ID')!,
            },
          },
          // deserializer: new MessagePackIncomingDeserializer(), // <-- custom deserializer

          // clientFactory: (options) => new CustomKafkaClient(options),
        }),
      },
    ]),
  ],
  providers: [MatchListenerService],
  controllers: [MatchListenerController],
})
export class MatchListenerModule {}
