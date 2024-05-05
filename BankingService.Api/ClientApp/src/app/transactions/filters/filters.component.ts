import { Component, OnInit } from '@angular/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import {
  MAT_DATE_LOCALE,
  provideNativeDateAdapter,
} from '@angular/material/core';
import { MatMenuModule } from '@angular/material/menu';
import { DatabaseService } from '../../services/database.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-filters',
  templateUrl: './filters.component.html',
  styleUrls: ['./filters.component.css'],
  standalone: true,
  providers: [
    provideNativeDateAdapter(),
    { provide: MAT_DATE_LOCALE, useValue: 'fr-FR' },
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
  selectedCategoryFilter: string | undefined;
  selectedTypeFilter: string | undefined;
  searchFilter: string | undefined;
  startDateFilter: Date | undefined;
  endDateFilter: Date | undefined;

  filterCategoryButtonText: string = 'Filter category...';
  filterTypeButtonText: string = 'Filter type...';

  types: string[] = [];
  categories: string[] = [];

  constructor(private dbService: DatabaseService) {}

  ngOnInit() {
    this.dbService.getTypesNames().subscribe((types) => (this.types = types));
    this.dbService
      .getCategoriesNames()
      .subscribe((categories) => (this.categories = categories));
  }

  menuItemClicked(s: string) {}

  typesMenuItemClicked(type: string): void {
    this.selectedTypeFilter = type;
    this.filterTypeButtonText = 'Filter: ' + type;
  }

  categoryMenuItemClicked(category: string): void {
    this.selectedCategoryFilter = category;
    this.filterCategoryButtonText = 'Filter: ' + category;
  }

  onClearFilter(): void {
    this.filterCategoryButtonText = 'Filter category...';
    this.filterTypeButtonText = 'Filter type...';
    this.selectedTypeFilter = undefined;
    this.selectedCategoryFilter = undefined;
    this.searchFilter = undefined;
    this.startDateFilter = undefined;
    this.endDateFilter = undefined;
  }
}
