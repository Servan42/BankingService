/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MiscDataComponent } from './misc-data.component';

describe('MiscDataComponent', () => {
  let component: MiscDataComponent;
  let fixture: ComponentFixture<MiscDataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MiscDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MiscDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
