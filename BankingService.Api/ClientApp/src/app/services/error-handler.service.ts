import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, throwError } from 'rxjs';
import { ErrorSnackbarComponent } from '../sncackbars/error-snackbar/error-snackbar.component';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {

  constructor(private snackBar: MatSnackBar) { }

  handleError(error: HttpErrorResponse) : Observable<never> {
    if (error.status === 0) {
      console.error('A client-side or network error occurred:', error.error);
      this.snackBar.open('A client-side or network error occurred.', '✔', { duration: 10000 })
    } else {
      this.snackBar.openFromComponent(ErrorSnackbarComponent, { data: error })
    }
    return throwError(() => new Error('An HTTP error occured.'));
  }
}
