import { Component, Input, OnInit } from '@angular/core';
import { Transaction } from '../../model/transaction';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DatabaseService } from '../../services/database.service';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'app-transaction-item',
  templateUrl: './transaction-item.component.html',
  styleUrls: ['./transaction-item.component.css'],
  standalone: true,
  imports: [FormsModule, CommonModule, MatMenuModule],
})
export class TransactionItemComponent implements OnInit {
  @Input() transaction!: Transaction;

  isEditableCommentDisabled: boolean = true;
  editButtonText: string = 'Edit';
  categories: string[] = [];

  constructor(private dbService: DatabaseService) {}

  ngOnInit() {}

  onEditTransactionClicked(): void {
    if (this.isEditableCommentDisabled === true) {
      this.dbService
        .getCategoriesNames()
        .subscribe((x) => (this.categories = x));
      this.editButtonText = 'Save';
    } else {
      this.editButtonText = 'Edit';
      // TODO save to db
    }
    this.isEditableCommentDisabled = !this.isEditableCommentDisabled;
  }

  categoryMenuItemClicked(newCategory: string): void {
    this.transaction.category = newCategory;
  }
}
