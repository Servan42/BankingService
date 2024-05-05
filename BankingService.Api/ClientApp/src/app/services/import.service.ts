import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ImportService {
  constructor() {}

  importBankFile(file: File): Observable<void> {
    console.log('DB CALLED! (importBankFile)', file);
    // this.fileName = file.name;
    const formData = new FormData();
    formData.append('thumbnail', file);
    // const upload$ = this.http.post('/api/thumbnail-upload', formData);
    // upload$.subscribe();
    return of(undefined);
  }

  importPaypalFile(file: File): Observable<void> {
    console.log('DB CALLED! (importPaypalFile)', file);
    return of(undefined);
  }
}
