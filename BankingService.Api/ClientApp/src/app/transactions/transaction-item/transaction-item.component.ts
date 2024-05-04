import { Component, Input, OnInit } from '@angular/core';
import { Transaction } from '../../model/transaction';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-transaction-item',
  templateUrl: './transaction-item.component.html',
  styleUrls: ['./transaction-item.component.css'],
  standalone: true,
  imports: [
    FormsModule,
    CommonModule
  ]
})
export class TransactionItemComponent implements OnInit {

  @Input() transaction!: Transaction;
  isEditableCommentDisabled: boolean = true;
  editButtonText: string = 'Edit';

  constructor() { }

  ngOnInit() {
  }

  onEditTransactionClicked(): void{
    if(this.isEditableCommentDisabled === true) {
      this.editButtonText = 'Save';
      // TODO save to db
    } else {
      this.editButtonText = 'Edit';
    }
    this.isEditableCommentDisabled = !this.isEditableCommentDisabled;
  }

}
