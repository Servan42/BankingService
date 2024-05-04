import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DatabaseService {
  
  constructor() { }
  
  getCategoriesNames(): Observable<string[]> {
    console.log('DB CALLED! (getCategoriesNames)');
    const mockCategories = [
      'Food',
      'Salary',
      'Entertainment',
      'Utilities'
    ]

    return of(mockCategories);
  }

}
