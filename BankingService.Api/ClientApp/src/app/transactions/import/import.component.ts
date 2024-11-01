import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { ImportService } from '../../services/import.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from "@angular/material/dialog";
import { ImportDialogComponent } from './import-dialog/import-dialog.component';
import { BackupConfirmationComponent } from './backup-confirmation-dialog/backup-confirmation.component';

@Component({
  selector: 'app-import',
  templateUrl: './import.component.html',
  styleUrls: ['./import.component.css'],
  standalone: true,
  imports: [MatIconModule],
})
export class ImportComponent implements OnInit {
  constructor(private importService: ImportService, private snackBar: MatSnackBar, private dialog: MatDialog) {}

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
          next: (s) => this.openImportDialog(s),
          complete: () => {
            this.snackBar.open('Bank file imported successfully.', '✔', { duration: 5000 });
            this.uploadComplete.emit();
          }
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
          next: (s) => this.openImportDialog(s),
          complete: () => {
            this.snackBar.open('Paypal file imported successfully.', '✔', { duration: 5000 });
            this.uploadComplete.emit();
          }
        });
    }
  }

  openImportDialog(reportData: string) {
    this.dialog.open(ImportDialogComponent, {
      data: { report: reportData },
    });
  }

  onBackupButtonClicked() {
    this.dialog.open(BackupConfirmationComponent);
  }
}
