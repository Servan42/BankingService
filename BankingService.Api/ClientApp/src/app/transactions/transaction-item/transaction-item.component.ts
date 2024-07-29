import { Component, Input, OnInit } from '@angular/core';
import { Transaction } from '../../model/transaction';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TransactionService } from '../../services/transaction.service';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { NumberToMoneyPipe } from '../../pipe/number-to-money.pipe';
import { DateToStringPipe } from '../../pipe/date-to-string.pipe';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-transaction-item',
  templateUrl: './transaction-item.component.html',
  styleUrls: ['./transaction-item.component.css'],
  standalone: true,
  imports: [FormsModule, CommonModule, MatMenuModule, MatIconModule, NumberToMoneyPipe, DateToStringPipe],
})
export class TransactionItemComponent implements OnInit {
  @Input() transaction!: Transaction;

  areEditableFieldsDisabled: boolean = true;
  editButtonText: string = 'Edit';
  categories: string[] = [];
  types: string[] = [];

  constructor(private dbService: TransactionService, private snackBar: MatSnackBar) {}

  ngOnInit() {}

  onEditTransactionClicked(): void {
    this.dbService.getCategoriesNames().subscribe((x) => (this.categories = x));
    this.dbService.getTypesNames().subscribe((x) => (this.types = x));
    this.editButtonText = 'Save';
    this.areEditableFieldsDisabled = false;
  }

  onSaveTransactionClicked(): void {
    this.editButtonText = 'Edit';
    this.dbService
      .updateTransaction(this.transaction)
        .subscribe({
          complete: () => this.snackBar.open('Transaction ' + this.transaction.id + ' updated successfully.', '✔', { duration: 5000 })
      });
    this.areEditableFieldsDisabled = true;
  }

  categoryMenuItemClicked(newCategory: string): void {
    this.transaction.category = newCategory;
  }

  typesMenuItemClicked(newType: string): void {
    this.transaction.type = newType;
  }
}
