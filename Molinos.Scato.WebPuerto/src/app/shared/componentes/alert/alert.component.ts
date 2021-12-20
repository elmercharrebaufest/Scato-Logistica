import { AfterViewInit, Component } from '@angular/core';
import { Alerta } from '@ScatoModels/alerta';
import { AlertService } from '@ScatoServicios/alert.service';

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.css']
})
export class AlertComponent implements AfterViewInit {
  alerta: Alerta;
  autoClose: number = 0;

  constructor(private alertService: AlertService) { }

  ngAfterViewInit() {
    this.alertService.subject.subscribe((alerta: Alerta) => {
      this.alerta = alerta;
      if (this.autoClose > 0) {
        clearTimeout(this.autoClose);
      }

      this.autoClose = <any>setTimeout(() => { this.alerta = null; }, 10000);
    })
  }
}
