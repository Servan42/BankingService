import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { MaintenanceService } from '../../../services/maintenance.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  standalone: true,
  selector: 'app-backup-confirmation',
  templateUrl: './backup-confirmation.component.html',
  styleUrls: ['./backup-confirmation.component.css']
})
export class BackupConfirmationComponent {

  constructor(private dialogRef: MatDialogRef<BackupConfirmationComponent>, private snackBar: MatSnackBar, private maintenanceService: MaintenanceService) { }

  onBackup(){
    this.maintenanceService
      .backupDb()
      .subscribe({
        complete: () => {
          this.snackBar.open('Backup successful.', '✔', { duration: 5000 })
        }
      });
      this.dialogRef.close();
  }

  onClose(){
    this.dialogRef.close();
  }
}
