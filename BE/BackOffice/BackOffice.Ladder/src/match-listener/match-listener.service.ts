import { Inject, Injectable } from '@nestjs/common';
import { ClientProxy } from '@nestjs/microservices';

@Injectable()
export class MatchListenerService {
  constructor(@Inject('MATCH_LISTENER') private readonly client: ClientProxy) {}
}
