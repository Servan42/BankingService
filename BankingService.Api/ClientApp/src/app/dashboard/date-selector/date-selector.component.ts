import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ReportInput } from '../../model/report-input';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MonthToTextPipe } from '../../pipe/month-to-text.pipe';

@Component({
  selector: 'app-date-selector',
  templateUrl: './date-selector.component.html',
  styleUrls: ['./date-selector.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, MonthToTextPipe]
})
export class DateSelectorComponent implements OnInit {

  @Output() reportInput = new EventEmitter<ReportInput>();

  inputFormGroup = new FormGroup({
    startDate: new FormControl<string>(''),
    endDate: new FormControl<string>(''),
    minAmount: new FormControl<number>(-100, Validators.max(0))
  })

  months: number[] = Array.from({length: 12}, (_, index) => index + 1);
  selectedMonth: number = 0;

  constructor() { }

  ngOnInit() {
    this.setDates(new Date());
  }

  setDates(startDate: Date) {
    let year = startDate.getFullYear();
    let month = startDate.getMonth() + 1;

    this.selectedMonth = month;

    this.inputFormGroup.get('startDate')?.setValue(year + '-' + month.toString().padStart(2, '0') + '-01');
    if(month === 12){
      year++;
      month = 0;
    }
    this.inputFormGroup.get('endDate')?.setValue(year + '-' + (month + 1).toString().padStart(2, '0') + '-01');
    this.onSubmit();
  }

  onMonthSelected(month: number) {
    let currentDate = new Date();
    this.setDates(new Date(currentDate.getFullYear(), month - 1, 1));
  }

  onSubmit() {
    let input: ReportInput = {
      startDate: this.getStartDate(),
      endDate: this.getEndDate(),
      highestOperationMinAmount: this.getMinAmount()
    };
    this.reportInput.emit(input);
  }

  private getMinAmount(): number {
    return this.inputFormGroup.get('minAmount')?.value ?? -100;
  }

  private getEndDate(): Date {
    return new Date(this.inputFormGroup.get('endDate')?.value ?? '');
  }

  private getStartDate(): Date {
    return new Date(this.inputFormGroup.get('startDate')?.value ?? '');
  }
}
