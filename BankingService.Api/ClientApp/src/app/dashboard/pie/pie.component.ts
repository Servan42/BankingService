import { Component, OnInit, NgModule, Input } from '@angular/core';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { myPieData } from '../../model/pie-data';

@Component({
  selector: 'app-pie',
  templateUrl: './pie.component.html',
  styleUrls: ['./pie.component.css'], 
  standalone: true,
  imports: [NgxChartsModule]
})
export class PieComponent implements OnInit {

  @Input() inputData: myPieData[] = [];
  constructor() { }

  ngOnInit() {
  }

}
