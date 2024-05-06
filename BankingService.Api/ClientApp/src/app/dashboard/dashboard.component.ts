import { Component, OnInit } from '@angular/core';
import { ReportService } from '../services/report.service';
import { TransactionReport, mockReport } from '../model/transaction-report';
import { CommonModule } from '@angular/common';
import { PieComponent } from './pie/pie.component';
import { myPieData } from '../model/pie-data';
import { MatIconModule } from '@angular/material/icon';
import { MyLineChartComponent } from './my-line-chart/my-line-chart.component';
import { MyLinechartData } from '../model/my-linechart-data';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    PieComponent,
    MatIconModule,
    MyLineChartComponent
  ]
})
export class DashboardComponent implements OnInit {

  constructor(private reportService: ReportService) { }
  report: TransactionReport | undefined;

  fullPieData: myPieData[] = [];
  noCostsPieData: myPieData[] = [];
  lineData: MyLinechartData[] = [];

  ngOnInit() {
    this.reportService.GetLastReport()
      .subscribe(report => this.prepareData(report));
  }

  prepareData(report: TransactionReport): void {
    this.report = report;
    const newPieData: myPieData[] = [];
    const costData: myPieData[] = [];
    for(let pieData of Object.entries(report.SumPerCategory))
    {
      if(pieData[0].includes('Income') || pieData[0].includes('Epargne'))
        continue;

      if(pieData[0].includes('Charge')) {
        costData.push({name: pieData[0], value: pieData[1]});
        continue;
      }

      newPieData.push({name: pieData[0], value: pieData[1]});
    }
    this.fullPieData = [...newPieData, ...costData];
    this.noCostsPieData = [...newPieData];

    const newLineData: MyLinechartData = {
      name: 'Treasury',
      series: []
    };
    for(let lineData of report.treasuryGraphData) {
      newLineData.series.push({name: lineData[0], value: lineData[1]});
    }
    this.lineData = [newLineData];
  }
}

