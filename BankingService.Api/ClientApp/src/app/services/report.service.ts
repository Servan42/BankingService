import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Transaction } from '../model/transaction';
import { TransactionReport, mockReport } from '../model/transaction-report';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  
  constructor() { }
  
  GetLastReport() : Observable<TransactionReport> {
    console.log('DB CALLED! (GetLastReport)');
    return of(mockReport);
  }

}
