import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ReportInput } from '../../model/report-input';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-date-selector',
  templateUrl: './date-selector.component.html',
  styleUrls: ['./date-selector.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule]
})
export class DateSelectorComponent implements OnInit {

  @Output() reportInput = new EventEmitter<ReportInput>();

  inputFormGroup = new FormGroup({
    startDate: new FormControl<string>(''),
    endDate: new FormControl<string>(''),
    minAmount: new FormControl<number>(-100, Validators.max(0))
  })

  constructor() { }

  ngOnInit() {
    this.setDates();
    this.onSubmit();
  }

  setDates() {
    var currentDate = new Date();
    this.inputFormGroup.get('startDate')?.setValue(currentDate.getFullYear() + '-' + currentDate.getMonth().toString().padStart(2, '0') + '-01');
    this.inputFormGroup.get('endDate')?.setValue(currentDate.getFullYear() + '-' + (currentDate.getMonth() + 1).toString().padStart(2, '0') + '-01');
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
