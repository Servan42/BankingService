import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dateToString',
  standalone: true
})
export class DateToStringPipe implements PipeTransform {

  transform(value: Date): string {
    return value.toLocaleDateString();
  }

}
