import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { ImportService } from '../../services/import.service';

@Component({
  selector: 'app-import',
  templateUrl: './import.component.html',
  styleUrls: ['./import.component.css'],
  standalone: true,
  imports: [MatIconModule],
})
export class ImportComponent implements OnInit {
  constructor(private importService: ImportService) {}

  @Output() uploadComplete = new EventEmitter();

  ngOnInit() {}

  // https://blog.angular-university.io/angular-file-upload/
  onBankFileSelected($event: any) {
    const file: File = $event.target.files[0];

    if (file) {
      this.importService
        .importBankFile(file)
        .subscribe({
          error: (e) => alert(e.error),
          complete: () => this.uploadComplete.emit()
        });
    }
  }

  onPayalFileSelected($event: any) {
    const file: File = $event.target.files[0];

    if (file) {
      this.importService
        .importPaypalFile(file)
        .subscribe({
          error: (e) => alert(e.error),
          complete: () => this.uploadComplete.emit()
        });
    }
  }
}
