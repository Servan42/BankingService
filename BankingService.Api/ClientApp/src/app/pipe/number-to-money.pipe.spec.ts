import { NumberToMoneyPipe } from './number-to-money.pipe';

describe('NumberToMoneyPipe', () => {
  it('create an instance', () => {
    const pipe = new NumberToMoneyPipe();
    expect(pipe).toBeTruthy();
  });
});
