import { Component, OnInit } from '@angular/core';
import { NgxChartsModule } from '@swimlane/ngx-charts';

@Component({
  selector: 'app-my-line-chart',
  templateUrl: './my-line-chart.component.html',
  styleUrls: ['./my-line-chart.component.css'],
  standalone: true,
  imports: [NgxChartsModule],
})
export class MyLineChartComponent implements OnInit {
  inputData: any[] = [
    {
      name: 'Germany',
      "series": [
        {
          "value": 2080,
          "name": "2016-09-18T10:35:53.581Z"
        },
        {
          "value": 6784,
          "name": "2016-09-17T02:33:53.602Z"
        },
        {
          "value": 4517,
          "name": "2016-09-22T07:58:16.047Z"
        },
        {
          "value": 6315,
          "name": "2016-09-24T06:10:22.209Z"
        },
        {
          "value": 2131,
          "name": "2016-09-18T00:12:37.231Z"
        }
      ],
    },
  ];

  constructor() {}

  ngOnInit() {}
}
