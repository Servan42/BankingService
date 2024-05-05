import { Component, OnInit } from '@angular/core';
import { ReportService } from '../services/report.service';
import { TransactionReport, mockReport } from '../model/transaction-report';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  standalone: true,
  imports: [
    CommonModule
  ]
})
export class DashboardComponent implements OnInit {

  constructor(private reportService: ReportService) { }
  report: TransactionReport | undefined;

  ngOnInit() {
    this.reportService.GetLastReport()
      .subscribe(report => this.report = report);
  }

}
