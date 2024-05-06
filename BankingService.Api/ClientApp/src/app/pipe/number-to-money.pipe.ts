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
    // Convert to string with thousands separator
    const formattedValue = roundedValue.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
    return `${formattedValue} €`;
  }

}
