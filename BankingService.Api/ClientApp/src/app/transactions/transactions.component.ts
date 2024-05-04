import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MAT_DATE_LOCALE, provideNativeDateAdapter} from '@angular/material/core';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.css'],
  standalone: true,
  providers: [provideNativeDateAdapter(), {provide: MAT_DATE_LOCALE, useValue: 'fr-FR'}],
  imports: [
    CommonModule,
    MatFormFieldModule, 
    MatDatepickerModule,
    MatMenuModule
  ]
})
export class TransactionsComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  menuItemClicked(s: string){

  }

}
