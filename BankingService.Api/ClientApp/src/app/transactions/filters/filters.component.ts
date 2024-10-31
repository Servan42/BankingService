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

  filterCategoryButtonText: string = 'Filter category...';
  filterTypeButtonText: string = 'Filter type...';

  types: string[] = [];
  categories: string[] = [];

  constructor(private dbService: TransactionService) {}

  ngOnInit() {
    this.dbService.getTypesNames().subscribe((types) => (this.types = types));
    this.dbService
      .getCategoriesNames()
      .subscribe((categories) => (this.categories = categories));
  }

  onMenuItemClicked(): void {
    if(this.filters.type == this.NO_FILTER)
      this.filters.type = undefined;

    if(this.filters.category == this.NO_FILTER)
      this.filters.category = undefined;

    this.filterOutput.emit({... this.filters });

    if(this.filters.category == undefined)
      this.filters.category = this.NO_FILTER;
    if(this.filters.type == undefined)
      this.filters.type = this.NO_FILTER;
  }

  onClearFilter(): void {
    this.filters.type = undefined;
    this.filters.category = undefined;
    this.filters.search = undefined;
    this.filters.startDate = undefined;
    this.filters.endDate = undefined;
    this.filterOutput.emit({... this.filters });
    this.filters.type = this.NO_FILTER;
    this.filters.category = this.NO_FILTER;
  }

  isAnyFilterSelected(): boolean {
    return (this.filters.type != undefined && this.filters.type != this.NO_FILTER)
      || (this.filters.category != undefined && this.filters.category != this.NO_FILTER)
      || this.filters.search != undefined
      || this.filters.startDate != undefined
      || this.filters.endDate != undefined;
  }

  onSearchFilterInput(): void {
    this.filterOutput.emit({... this.filters });
  }

  onEndDateSelected() :void {
    this.filterOutput.emit({ ...this.filters });
  }
}
