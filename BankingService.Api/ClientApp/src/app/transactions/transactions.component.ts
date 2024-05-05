import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ImportComponent } from './import/import.component';
import { FiltersComponent } from './filters/filters.component';
import { TransactionHeadersComponent } from './transaction-headers/transaction-headers.component';
import { TransactionItemComponent } from './transaction-item/transaction-item.component';
import { Transaction, mockTransactions } from '../model/transaction';
import { FilterTransactionsPipe } from '../pipe/filter-transactions.pipe';
import { TransactionFilters } from '../model/transaction-filters';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    ImportComponent,
    FiltersComponent,
    TransactionHeadersComponent,
    TransactionItemComponent,
    FilterTransactionsPipe
  ]
})
export class TransactionsComponent implements OnInit {

  transactions: Transaction[] = mockTransactions;
  filters: TransactionFilters | undefined;

  constructor() { }

  ngOnInit() {
  }

  onFiltersChanged(filters: TransactionFilters) {
    this.filters = filters;
  }

}
