import { CommonModule } from '@angular/common';
import { Component, inject, Inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_SNACK_BAR_DATA, MatSnackBarAction, MatSnackBarActions, MatSnackBarLabel, MatSnackBarRef } from '@angular/material/snack-bar';

@Component({
  selector: 'app-error-snackbar',
  standalone: true,
  imports: [
    MatButtonModule,
    MatSnackBarLabel,
    MatSnackBarActions,
    MatSnackBarAction,
    CommonModule
  ],
  templateUrl: './error-snackbar.component.html',
  styleUrl: './error-snackbar.component.css'
})
export class ErrorSnackbarComponent implements OnInit {

  message!: string;
  isInternalServerError: boolean = false;
  isBusinessError: boolean = false;
  snackBarRef = inject(MatSnackBarRef);

  constructor(@Inject(MAT_SNACK_BAR_DATA) public data: any) { }

  ngOnInit(): void {
    switch (this.data.status) {
      case 400:
        this.isBusinessError = true;
        break;
        case 500:
          this.isInternalServerError = true;
        break;
      default:
        break;
    }
    this.message = this.data.error;
  }
}


