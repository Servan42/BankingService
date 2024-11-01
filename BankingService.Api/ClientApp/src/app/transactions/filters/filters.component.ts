import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import {
  MAT_DATE_LOCALE,
  provideNativeDateAdapter,
} from '@angular/material/core';
import { MatMenuModule } from '@angular/material/menu';
import { TransactionService } from '../../services/transaction.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TransactionFilters } from '../../model/transaction-filters';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-filters',
  templateUrl: './filters.component.html',
  styleUrls: ['./filters.component.css'],
  standalone: true,
  providers: [
    provideNativeDateAdapter(),
    { provide: MAT_DATE_LOCALE, useValue: 'en-CA' },
  ],
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatMenuModule,
    MatIconModule
  ],
})
export class FiltersComponent implements OnInit {
  filters: TransactionFilters = {
    category: undefined,
    type: undefined,
    search: undefined,
    startDate: undefined,
    endDate: undefined,
  };

  @Output() filterOutput = new EventEmitter<TransactionFilters>(undefined);
  @Input() resultCount: number = 0;

  NO_FILTER: string = "NO_FILTER";

  types: string[] = [];
  categories: string[] = [];

  constructor(private dbService: TransactionService) {}

  ngOnInit() {
    this.dbService.getTypesNames().subscribe((types) => (this.types = types));
    this.dbService
      .getCategoriesNames()
      .subscribe((categories) => (this.categories = categories));
    this.setCurrentMonth(true);
  }

  onMenuItemClicked(): void {
    this.emitFilters();
  }

  onClearFilter(): void {
    this.filters.type = this.NO_FILTER;
    this.filters.category = this.NO_FILTER;
    this.filters.search = undefined;
    this.filters.startDate = undefined;
    this.filters.endDate = undefined;
    this.emitFilters();
  }

  isAnyFilterSelected(): boolean {
    return (this.filters.type != undefined && this.filters.type != this.NO_FILTER)
      || (this.filters.category != undefined && this.filters.category != this.NO_FILTER)
      || this.filters.search != undefined
      || this.filters.startDate != undefined
      || this.filters.endDate != undefined;
  }

  onSearchFilterInput(): void {
    this.emitFilters();
  }

  onEndDateSelected() :void {
    this.emitFilters();
  }

  decreaseMonth(): void {
    if(!this.filters.startDate || !this.filters.endDate){
      this.setCurrentMonth(false);
    }
    this.filters.endDate = new Date(this.filters.endDate!.getFullYear(), this.filters.startDate!.getMonth(), -1);
    this.filters.startDate = new Date(this.filters.startDate!.getFullYear(), this.filters.startDate!.getMonth() - 1, 1);
    this.emitFilters();
  }

  increaseMonth(): void {
    if(!this.filters.startDate || !this.filters.endDate){
      this.setCurrentMonth(false);
    }
    this.filters.endDate = new Date(this.filters.endDate!.getFullYear(), this.filters.startDate!.getMonth() + 2, -1);
    this.filters.startDate = new Date(this.filters.startDate!.getFullYear(), this.filters.startDate!.getMonth() + 1, 1);
    this.emitFilters();
  }

  setCurrentMonth(shouldEmit: boolean): void {
    const currentDate = new Date();
    this.filters.startDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    this.filters.endDate = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, -1);
    if (shouldEmit) this.emitFilters();
  }

  private emitFilters() {
    this.filterOutput.emit({
      ...this.filters,
      type: this.filters.type === this.NO_FILTER ? undefined : this.filters.type,
      category: this.filters.category === this.NO_FILTER ? undefined : this.filters.category,
    });
  }
}
