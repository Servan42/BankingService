import { Component, Input, OnInit } from '@angular/core';
import { TransactionReport } from '../../model/transaction-report';
import { NumberToMoneyPipe } from '../../pipe/number-to-money.pipe';
import { CommonModule } from '@angular/common';
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-misc-data',
  templateUrl: './misc-data.component.html',
  styleUrls: ['./misc-data.component.css'],
  standalone: true,
  imports: [NumberToMoneyPipe, CommonModule, MatSlideToggleModule, FormsModule]
})
export class MiscDataComponent implements OnInit {

  @Input() report : TransactionReport | undefined;

  slideChecked: boolean = false;
  constructor() { }

  ngOnInit() {
  }

  isBalancePositive(withCosts: boolean): boolean {
    if(!this.report)
      return false;

    if(withCosts){
      return this.report.balance > 0;
    } else {
      return this.report.balanceWithoutSavings > 0;
    }
  }
}
