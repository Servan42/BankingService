import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

const ENDPOINT = environment.apiUrl + '/api/Import/';

@Injectable({
  providedIn: 'root',
})
export class ImportService {
  constructor(private httpClient: HttpClient) {}

  importBankFile(file: File): Observable<string> {
    return this.importFile(file, true);
  }

  importPaypalFile(file: File): Observable<string> {
    return this.importFile(file, false);
  }

  private importFile(file: File, isBankFile: boolean): Observable<string> {
    console.log('DB CALLED! (importFile)', file);
    const formData = new FormData();
    formData.append('formFile', file);
    return this.httpClient.post(ENDPOINT + 'ImportFile?isBankFile=' + isBankFile, formData, { responseType: 'text'});
  }
}
