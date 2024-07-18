import { Injectable } from '@angular/core';
import { Observable, catchError, map, of, throwError } from 'rxjs';
import { Transaction } from '../model/transaction';
import { TransactionReport, mockReport } from '../model/transaction-report';
import { environment } from '../../environments/environment';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ReportInput } from '../model/report-input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ErrorSnackbarComponent } from '../sncackbars/error-snackbar/error-snackbar.component';

const ENDPOINT = environment.apiUrl + '/api/Report/';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  constructor(private httpClient: HttpClient, private snackBar: MatSnackBar) {}

  getReport(reportInput: ReportInput): Observable<TransactionReport> {
    console.log('DB CALLED! (getReport)');
    let params = '?startDate=' + reportInput.startDate.toISOString() + '&endDate=' + reportInput.endDate.toISOString() + '&highestTransactionMinAmount=' + reportInput.highestTransactionMinAmount;
    return this.httpClient
      .get<TransactionReport>(ENDPOINT + params)
      .pipe(
        map((r) => ({
          ...r,
          startDate: new Date(r.startDate),
          endDate: new Date(r.endDate),
          highestTransactions: r.highestTransactions.map((t) => ({...t, date: new Date(t.date)})),
          treasuryGraphData: r.treasuryGraphData.map((data) => ({...data, dateTime: new Date(data.dateTime)}))
        })),
        catchError(err => this.handleError(err))
      );
    // return of(mockReport);
  }

  private handleError(error: HttpErrorResponse) : Observable<never> {
    if (error.status === 0) {
      console.error('A client-side or network error occurred:', error.error);
      this.snackBar.open('A client-side or network error occurred.', '✔', { duration: 10000 })
    } else {
      this.snackBar.openFromComponent(ErrorSnackbarComponent, { data: error })
    }
    return throwError(() => new Error('An HTTP error occured.'));
  }
}
