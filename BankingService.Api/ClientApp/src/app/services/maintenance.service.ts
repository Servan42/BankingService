import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ErrorHandlerService } from './error-handler.service';
import { catchError, Observable } from 'rxjs';

const ENDPOINT = environment.apiUrl + '/api/Maintenance/';

@Injectable({
  providedIn: 'root'
})
export class MaintenanceService {
  constructor(private httpClient: HttpClient, private errorHandler: ErrorHandlerService) {}

  backupDb(): Observable<any> {
    return this.httpClient
      .post(ENDPOINT + "BackupDB", {})
      .pipe(
        catchError(err => this.errorHandler.handleError(err))
      );
  }
}
