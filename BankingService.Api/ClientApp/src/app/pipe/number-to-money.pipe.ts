import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'numberToMoney',
  standalone: true
})
export class NumberToMoneyPipe implements PipeTransform {

  transform(value: number | undefined): string {
    if (!value || isNaN(value))
      return 'Invalid input';

    const roundedValue = Math.round(value * 100) / 100;
    const roundedString = roundedValue.toFixed(2);
    // Convert to string with thousands separator
    let formattedValue = roundedString.replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
    if(value > 0)
      formattedValue = '+' + formattedValue;
    return `${formattedValue} €`;
  }

}
