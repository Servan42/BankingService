import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ImportComponent } from './import/import.component';
import { FiltersComponent } from './filters/filters.component';
import { TransactionHeadersComponent } from './transaction-headers/transaction-headers.component';
import { TransactionItemComponent } from './transaction-item/transaction-item.component';

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
    TransactionItemComponent
  ]
})
export class TransactionsComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
