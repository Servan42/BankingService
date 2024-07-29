import { Injectable } from '@angular/core';
import { Observable, catchError, map, of, tap } from 'rxjs';
import { Transaction, mockTransactions } from '../model/transaction';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { ErrorHandlerService } from './error-handler.service';

const ENDPOINT = environment.apiUrl + '/api/Transaction/';

@Injectable({
  providedIn: 'root',
})
export class TransactionService {
  constructor(private httpClient: HttpClient, private errorHandler: ErrorHandlerService) {}

  getAllTransactions(): Observable<Transaction[]> {
    console.log('DB CALLED! (GetAllTransactions)');
    return this.httpClient
      .get<Transaction[]>(ENDPOINT + 'GetAllTransactions')
      .pipe(
        // tap((x) => console.log(x)),
        map((transactions) =>
          transactions.map((transaction) => ({
            ...transaction,
            date: new Date(transaction.date),
          })),
          catchError(err => this.errorHandler.handleError(err))
        )
      );
    // return of(mockTransactions);
  }

  getCategoriesNames(): Observable<string[]> {
    console.log('DB CALLED! (getCategoriesNames)');
    return this.httpClient
      .get<string[]>(ENDPOINT + "GetTransactionCategoriesNames")
      .pipe(
        catchError(err => this.errorHandler.handleError(err))
      );
    // const mockCategories = ['Food', 'Salary', 'Entertainment', 'Utilities'];
    // return of(mockCategories);
  }

  getTypesNames(): Observable<string[]> {
    console.log('DB CALLED! (getTypesNames)');
    return this.httpClient
      .get<string[]>(ENDPOINT + "GetTransactionTypesNames")
      .pipe(
        catchError(err => this.errorHandler.handleError(err))
      );
    // const mockTypes = ['Expense', 'Income'];
    // return of(mockTypes);
  }

  updateTransaction(transaction: Transaction): Observable<any> {
    console.log('DB CALLED! (updateTransaction)');
    return this.httpClient.post(ENDPOINT + "UpdateTransactions", [
      {
        id: transaction.id,
        type: transaction.type,
        category: transaction.category,
        autoComment: transaction.autoComment,
        comment: transaction.comment
      }
    ])
    .pipe(
      catchError(err => this.errorHandler.handleError(err))
    );
  }
}
