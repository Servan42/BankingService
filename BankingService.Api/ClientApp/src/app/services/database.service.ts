import { Injectable } from '@angular/core';
import { Observable, catchError, map, of, tap } from 'rxjs';
import { Transaction, mockTransactions } from '../model/transaction';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { environment } from '../../environments/environment.development';

const ENDPOINT = environment.apiUrl + '/api/Database/';

@Injectable({
  providedIn: 'root',
})
export class DatabaseService {
  constructor(private httpClient: HttpClient) {}

  getAllTransactions(): Observable<Transaction[]> {
    console.log('DB CALLED! (GetAllTransactions)');
    return this.httpClient
      .get<Transaction[]>(ENDPOINT + 'GetAllOperations')
      .pipe(
        // tap((x) => console.log(x)),
        map((transactions) =>
          transactions.map((transaction) => ({
            ...transaction,
            date: new Date(transaction.date),
          }))
        )
      );
    // return of(mockTransactions);
  }

  getCategoriesNames(): Observable<string[]> {
    console.log('DB CALLED! (getCategoriesNames)');
    return this.httpClient
      .get<string[]>(ENDPOINT + "GetAllCategoriesNames");
    // const mockCategories = ['Food', 'Salary', 'Entertainment', 'Utilities'];
    // return of(mockCategories);
  }

  getTypesNames(): Observable<string[]> {
    console.log('DB CALLED! (getTypesNames)');
    const mockTypes = ['Expense', 'Income'];

    return of(mockTypes);
  }

  updateTransaction(transaction: Transaction): void {
    console.log('DB CALLED! (updateTransaction)', transaction);
  }
}
