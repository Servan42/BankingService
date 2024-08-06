import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-import-dialog',
  standalone: true,
  imports: [],
  templateUrl: './import-dialog.component.html',
  styleUrl: './import-dialog.component.css'
})
export class ImportDialogComponent {

  report: string = '';

  constructor(private dialogRef: MatDialogRef<ImportDialogComponent>, @Inject(MAT_DIALOG_DATA) data: any) {
    this.report = data.report;
  }

  onClose() {
    this.dialogRef.close();
  }
}
