import { Component, inject, Inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_SNACK_BAR_DATA, MatSnackBarAction, MatSnackBarActions, MatSnackBarLabel, MatSnackBarRef } from '@angular/material/snack-bar';

@Component({
  selector: 'app-error-snackbar',
  standalone: true,
  imports: [MatButtonModule, MatSnackBarLabel, MatSnackBarActions, MatSnackBarAction],
  templateUrl: './error-snackbar.component.html',
  styleUrl: './error-snackbar.component.css'
})
export class ErrorSnackbarComponent implements OnInit {

  message!: string;
  snackBarRef = inject(MatSnackBarRef);

  constructor(@Inject(MAT_SNACK_BAR_DATA) public data: any) { }

  ngOnInit(): void {
    this.message = this.formatErrorMessage();
  }

  formatErrorMessage(): string {

    if (!this.data) {
      return 'An unknown error occurred. (null error)';
    }

    if (this.data.error && typeof this.data.error === 'string') {
      return this.data.error;
    }

    if (this.data.message) {
      return this.data.message;
    }

    if (typeof this.data === 'string') {
      return this.data;
    }

    console.log(this.data);
    return "An unknown error occurred.";
  }
}


