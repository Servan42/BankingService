import { Component, OnInit } from '@angular/core';
import { ReportService } from '../services/report.service';
import { TransactionReport, mockReport } from '../model/transaction-report';
import { CommonModule } from '@angular/common';
import { PieComponent } from './pie/pie.component';
import { myPieData } from '../model/pie-data';
import { MatIconModule } from '@angular/material/icon';
import { MyLineChartComponent } from './my-line-chart/my-line-chart.component';
import { MyLinechartData } from '../model/my-linechart-data';
import { MiscDataComponent } from './misc-data/misc-data.component';
import { DateSelectorComponent } from './date-selector/date-selector.component';
import { ReportInput } from '../model/report-input';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    PieComponent,
    MatIconModule,
    MyLineChartComponent,
    MiscDataComponent,
    DateSelectorComponent
  ]
})
export class DashboardComponent implements OnInit {
  constructor(private reportService: ReportService) { }
  report: TransactionReport | undefined;

  fullPieData: myPieData[] = [];
  noCostsPieData: myPieData[] = [];
  lineData: MyLinechartData[] = [];

  reportInput: ReportInput | undefined;
  hasData = false;

  ngOnInit() {
    // this.loadReport(undefined);
  }

  loadReport(reportInput: ReportInput | undefined) {
    if(!reportInput)
      return;

    this.reportService.getReport(reportInput)
      .subscribe(report => this.prepareData(report));
  }

  prepareData(report: TransactionReport): void {
    if(report.treasuryGraphData.length === 0) {
     this.hasData = false;
     return;
    }

    this.hasData = true;
    this.report = report;
    const newPieData: myPieData[] = [];
    const costData: myPieData[] = [];
    for(let pieData of Object.entries(report.sumPerCategory))
    {
      if(pieData[0].includes('Income') || pieData[0].includes('Epargne'))
        continue;

      if(pieData[0].includes('Charge')) {
        costData.push({name: pieData[0], value: Math.abs(pieData[1])});
        continue;
      }

      newPieData.push({name: pieData[0], value: Math.abs(pieData[1])});
    }
    this.fullPieData = [...newPieData, ...costData];
    this.noCostsPieData = [...newPieData];

    const newLineData: MyLinechartData = {
      name: 'Treasury',
      series: []
    };
    for(let lineData of report.treasuryGraphData) {
      newLineData.series.push({name: lineData.dateTime, value: lineData.value});
    }
    this.lineData = [newLineData];
  }

}

