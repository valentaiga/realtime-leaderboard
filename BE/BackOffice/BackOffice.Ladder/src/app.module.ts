import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { ConfigModule } from '@nestjs/config';
import { MatchListenerModule } from './match-listener/match-listener.module';

@Module({
  imports: [
    ConfigModule.forRoot({
      isGlobal: true, // This makes ConfigService available everywhere
      envFilePath: '.env',
      // validationSchema: Joi.object({
      //   KAFKA_BROKERS: Joi.string().required(),
      //   KAFKA_CLIENT_ID_PREFIX: Joi.string().required(),
      //   KAFKA_GROUP_ID: Joi.string().required(),
      //   APP_PORT: Joi.number().default(5140),
      //   NODE_ENV: Joi.string()
      //     .valid('development', 'production', 'test')
      //     .default('development'),
      // }),
    }),
    MatchListenerModule,
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
