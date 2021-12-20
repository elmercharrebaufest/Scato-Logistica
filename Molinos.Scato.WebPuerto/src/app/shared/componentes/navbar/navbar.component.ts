import { Component, Input, OnInit } from '@angular/core';
import { SessionService } from '@ScatoServicios/session.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  @Input() onClickHandler: any;

  constructor(
    public session: SessionService) { }

  ngOnInit(): void {
  }

}
