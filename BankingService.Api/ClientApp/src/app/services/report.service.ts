import { Injectable } from '@angular/core';
import { Observable, map, of } from 'rxjs';
import { Transaction } from '../model/transaction';
import { TransactionReport, mockReport } from '../model/transaction-report';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ReportInput } from '../model/report-input';

const ENDPOINT = environment.apiUrl + '/api/Report/';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  constructor(private httpClient: HttpClient) {}

  getReport(reportInput: ReportInput): Observable<TransactionReport> {
    console.log('DB CALLED! (getReport)');
    let params = '?startDate=' + reportInput.startDate.toISOString() + '&endDate=' + reportInput.endDate.toISOString() + '&highestOperationMinAmount=' + reportInput.highestOperationMinAmount;
    return this.httpClient
      .get<TransactionReport>(ENDPOINT + params)
      .pipe(
        map((r) => ({
          ...r,
          startDate: new Date(r.startDate),
          endDate: new Date(r.endDate),
          highestOperations: r.highestOperations.map((t) => ({...t, date: new Date(t.date)})),
          treasuryGraphData: r.treasuryGraphData.map((data) => ({...data, dateTime: new Date(data.dateTime)}))
        }))
      );
    // return of(mockReport);
  }
}
