import { Test, TestingModule } from '@nestjs/testing';
import { MatchListenerService } from './match-listener.service';

describe('MatchListenerService', () => {
  let service: MatchListenerService;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [MatchListenerService],
    }).compile();

    service = module.get<MatchListenerService>(MatchListenerService);
  });

  it('should be defined', () => {
    expect(service).toBeDefined();
  });
});
