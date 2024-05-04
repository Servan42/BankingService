import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-import',
  templateUrl: './import.component.html',
  styleUrls: ['./import.component.css'],
  standalone: true
})
export class ImportComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  // https://blog.angular-university.io/angular-file-upload/
  onBankFileSelected($event: any){
    const file = $event.target.files[0];
  }
}
