import { Test, TestingModule } from '@nestjs/testing';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { encode } from '@msgpack/msgpack';

describe('AppController', () => {
  let appController: AppController;

  beforeEach(async () => {
    const app: TestingModule = await Test.createTestingModule({
      controllers: [AppController],
      providers: [AppService],
    }).compile();

    appController = app.get<AppController>(AppController);
  });

  it('serialization test', () => {
    const obj = {
      matchId: 'some message id',
    };

    const serialized = encode(obj);

    console.log(serialized);
  });

  describe('root', () => {
    it('should return "Hello World!"', () => {
      console.log('appController instance', appController);

      expect(appController.getHello()).toBe('Hello World!');
    });
  });
});
