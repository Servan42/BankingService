import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Transaction } from '../model/transaction';

@Injectable({
  providedIn: 'root'
})
export class DatabaseService {

  constructor() { }
  
  getCategoriesNames(): Observable<string[]> {
    console.log('DB CALLED! (getCategoriesNames)');
    const mockCategories = [
      'Food',
      'Salary',
      'Entertainment',
      'Utilities'
    ]

    return of(mockCategories);
  }

  getTypesNames(): Observable<string[]> {
    console.log('DB CALLED! (getTypesNames)');
    const mockTypes = [
      'Expense',
      'Income',
    ]

    return of(mockTypes);
  }

  updateTransaction(transaction: Transaction): void {
    console.log('DB CALLED! (updateTransaction)', transaction);
  }

}
