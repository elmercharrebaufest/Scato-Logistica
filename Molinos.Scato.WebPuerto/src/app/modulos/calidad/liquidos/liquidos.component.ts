import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-liquidos',
  templateUrl: './liquidos.component.html',
  styleUrls: ['./liquidos.component.css']
})
export class LiquidosComponent implements OnInit {

  @Output() hideSpinner = new EventEmitter<boolean>();

  constructor() { }

  ngOnInit(): void {
    this.hideSpinner.emit(false);
  }

}