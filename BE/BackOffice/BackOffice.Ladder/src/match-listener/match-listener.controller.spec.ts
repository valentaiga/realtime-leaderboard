import { Test, TestingModule } from '@nestjs/testing';
import { MatchListenerController } from './match-listener.controller';
import { MatchListenerService } from './match-listener.service';

describe('MatchListenerController', () => {
  let controller: MatchListenerController;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [MatchListenerController],
      providers: [MatchListenerService],
    }).compile();

    controller = module.get<MatchListenerController>(MatchListenerController);
  });

  it('should be defined', () => {
    expect(controller).toBeDefined();
  });
});
