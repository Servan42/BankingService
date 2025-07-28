import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, throwError } from 'rxjs';
import { ErrorSnackbarComponent } from '../sncackbars/error-snackbar/error-snackbar.component';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {

  constructor(private readonly snackBar: MatSnackBar, private readonly router: Router) { }

  handleError(error: HttpErrorResponse) : Observable<never> {
    if (error.status === 0) {
      console.error('A client-side or network error occurred:', error.error);
      this.snackBar.open('A client-side or network error occurred.', '✔', { duration: 10000 })
    } else if (error.status === 401) {
      this.snackBar.open('You are not authorized to perform this action.', '✔', { duration: 10000 });
      this.router.navigateByUrl('/login');
    } else {
      this.snackBar.openFromComponent(ErrorSnackbarComponent, { data: error })
    }
    return throwError(() => new Error('An HTTP error occured.'));
  }
}
