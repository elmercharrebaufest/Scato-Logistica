import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'ship-spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.css']
})
export class SpinnerComponent implements OnInit {
  @Input() message: string = "";
  @Input() src: string = 'ship';
  constructor() { }

  ngOnInit(): void {
  }

}
