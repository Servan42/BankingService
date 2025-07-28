import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ErrorHandlerService } from './error-handler.service';
import { LoginToken } from '../model/login-token';
import { catchError, map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import moment from 'moment';

const ENDPOINT = environment.apiUrl + '/api/Login';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private readonly httpClient: HttpClient, private readonly errorHandler: ErrorHandlerService) { }

  login(username: string, password: string): Observable<void> {
    return this.httpClient
      .get<LoginToken>(ENDPOINT + '?username=' + username + '&password=' + password)
      .pipe(
        map(authResult => this.setSession(authResult)),
        catchError(err => this.errorHandler.handleError(err))
      );
  }

  private setSession(authResult: LoginToken): void {
    localStorage.setItem('token', authResult.token);
    localStorage.setItem('expireDate', JSON.stringify(authResult.expirationDate.valueOf()));
  }

  isLoggedIn(): boolean {
    const expiration = localStorage.getItem("expireDate");
    if(!expiration) {
      return false;
    }
    const expiresAt = JSON.parse(expiration);
    return moment().isBefore((expiresAt));
  }
}
