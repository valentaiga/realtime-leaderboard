import { Controller } from '@nestjs/common';
import { MessagePattern, Payload } from '@nestjs/microservices';
import { unpack } from 'msgpackr';
import { type } from 'node:os';
import { decode } from '@msgpack/msgpack';

interface MatchStatusDeserializedData {
  matchId: string;
  matchStartedEvent: {
    Team1: number[];
    Team2: number[];
    startedAt: Date;
  };
  matchFinishedEvent: {
    Winners: number[];
    Losers: number[];
    startedAt: Date;
    finishedAt: Date;
  };
}

@Controller('match-listener')
export class MatchListenerController {
  @MessagePattern('match_status')
  test(@Payload() message: any): any {
    try {
      // const buffer = new Buffer(message);
      // // const decoder = new Decoder();
      // // decoder.decode(message);
      // const uint8array = new Uint8Array(message);
      // console.log('match_status message', JSON.parse(message));
      console.log(decode(message));
    } catch (error) {
      console.log(error);
    }
  }
}
