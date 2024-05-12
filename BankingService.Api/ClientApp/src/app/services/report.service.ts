import { Injectable } from '@angular/core';
import { Observable, map, of } from 'rxjs';
import { Transaction } from '../model/transaction';
import { TransactionReport, mockReport } from '../model/transaction-report';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

const ENDPOINT = environment.apiUrl + '/api/Report/';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  constructor(private httpClient: HttpClient) {}

  GetLastReport(): Observable<TransactionReport> {
    console.log('DB CALLED! (GetLastReport)');
    let params =
      '?startDate=2024-03&endDate=2024-04&highestOperationMinAmount=-100';
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
