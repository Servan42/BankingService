import { Injectable } from '@angular/core';
import { Observable, catchError, map, of, throwError } from 'rxjs';
import { TransactionReport, mockReport } from '../model/transaction-report';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ReportInput } from '../model/report-input';
import { ErrorHandlerService } from './error-handler.service';

const ENDPOINT = environment.apiUrl + '/api/Report/';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  constructor(private httpClient: HttpClient, private errorHandler: ErrorHandlerService) {}

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
        catchError(err => this.errorHandler.handleError(err))
      );
    // return of(mockReport);
  }
}
