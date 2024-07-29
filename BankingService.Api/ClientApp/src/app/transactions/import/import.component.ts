import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { ImportService } from '../../services/import.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-import',
  templateUrl: './import.component.html',
  styleUrls: ['./import.component.css'],
  standalone: true,
  imports: [MatIconModule],
})
export class ImportComponent implements OnInit {
  constructor(private importService: ImportService, private snackBar: MatSnackBar) {}

  @Output() uploadComplete = new EventEmitter();

  ngOnInit() {}

  // https://blog.angular-university.io/angular-file-upload/
  onBankFileSelected($event: any) {
    const file: File = $event.target.files[0];

    const inputElement = $event.target as HTMLInputElement;
    inputElement.value = '';

    if (file) {
      this.importService
        .importBankFile(file)
        .subscribe({
          next: (s) => this.snackBar.open(s, '✔', { duration: 5000 }),
          complete: () => this.uploadComplete.emit()
        });
    }
  }

  onPayalFileSelected($event: any) {
    const file: File = $event.target.files[0];

    const inputElement = $event.target as HTMLInputElement;
    inputElement.value = '';

    if (file) {
      this.importService
        .importPaypalFile(file)
        .subscribe({
          complete: () => {
            this.snackBar.open('Paypal file imported successfully.', '✔', { duration: 5000 });
            this.uploadComplete.emit();
          }
        });
    }
  }
}
