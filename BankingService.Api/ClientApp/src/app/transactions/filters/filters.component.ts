import { Component, OnInit } from '@angular/core';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MAT_DATE_LOCALE, provideNativeDateAdapter} from '@angular/material/core';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'app-filters',
  templateUrl: './filters.component.html',
  styleUrls: ['./filters.component.css'],
  standalone: true,
  providers: [provideNativeDateAdapter(), {provide: MAT_DATE_LOCALE, useValue: 'fr-FR'}],
  imports: [
    MatFormFieldModule, 
    MatDatepickerModule,
    MatMenuModule
  ]
})
export class FiltersComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  menuItemClicked(s: string){
  }

}
