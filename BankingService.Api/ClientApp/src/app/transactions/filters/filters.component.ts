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

  typesMenuItemClicked(type: string): void {
    this.filters.type = type;
    this.filterOutput.emit({... this.filters });
    this.filterTypeButtonText = 'Filter: ' + type;
  }

  categoryMenuItemClicked(category: string): void {
    this.filters.category = category;
    this.filterCategoryButtonText = 'Filter: ' + category;
    this.filterOutput.emit({... this.filters });
  }

  onClearFilter(): void {
    this.filterCategoryButtonText = 'Filter category...';
    this.filterTypeButtonText = 'Filter type...';

    this.filters.type = undefined;
    this.filters.category = undefined;
    this.filters.search = undefined;
    this.filters.startDate = undefined;
    this.filters.endDate = undefined;
    this.filterOutput.emit({... this.filters });
  }

  onSearchFilterInput(): void {
    this.filterOutput.emit({... this.filters });
  }

  onEndDateSelected() :void {
    this.filterOutput.emit({ ...this.filters });
  }
}
