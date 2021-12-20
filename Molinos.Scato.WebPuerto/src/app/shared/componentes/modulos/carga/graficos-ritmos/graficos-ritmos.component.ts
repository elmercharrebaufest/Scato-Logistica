import { Component, Input, OnInit } from '@angular/core';
import { TurnosService } from '@ScatoServicios/turnos.service';

@Component({
  selector: 'app-graficos-ritmos',
  templateUrl: './graficos-ritmos.component.html',
  styleUrls: ['./graficos-ritmos.component.css']
})
export class GraficosRitmosComponent implements OnInit {
  @Input() liquido: boolean;
  valorRitmo: number = 0;
  valorCargando: number = 0;
  valorNeto: number = 0;
  tnTotales: number = 0;
  horaActualizacion: string = '18:00hs';
  turno: any;
  colorRitmo = '';
  cantTurnos: number = 0;
  constructor(
    private _turnosService: TurnosService
  ) {
  }

  ngOnInit(): void {
    if (this.liquido)
      this.subscribeTurnos();
    else
      this.subscribeBalanzas();
  }

  subscribeTurnos() {
    let turnos = this._turnosService.getTurnos();
    if (turnos) {
      let cant = 0;
      for (let turno of turnos.turno) {
        cant += turno.cantidad;
      }
      this.valorRitmo = (this.valorRitmo * this.cantTurnos) + cant;
      this.cantTurnos++;
      this.valorRitmo = Math.round(this.valorRitmo / (this.cantTurnos * 6));
      this.colorRitmo = this.valorRitmo > 1000 ? '#1F8649' : '#F0AD4E';
    }

    let tns = Number(this._turnosService.getTnTotales());
    if (tns)
      this.tnTotales = tns;

    this._turnosService.sendTurnos.subscribe(res => {
      let cant: number = 0;
      for (let turno of res.turno) {
        cant += Number(turno.cantidad);
      }
      this.valorCargando += Number(cant);
      this.valorRitmo = (this.valorRitmo * this.cantTurnos * 6) + Number(cant);
      this.cantTurnos++;
      this.valorRitmo = Math.round(this.valorRitmo / (this.cantTurnos * 6));
      this.colorRitmo = this.valorRitmo > 1000 ? '#1F8649' : '#F0AD4E';
      let fecha = new Date();
      this.horaActualizacion = `${fecha.getHours()}:${fecha.getMinutes()}hs`
    });
    this._turnosService.sendTnTotal.subscribe(res => {
      this.tnTotales = res;
    })
    let fecha = new Date();
    this.horaActualizacion = `${fecha.getHours()}:${fecha.getMinutes()}hs`
  }

  subscribeBalanzas() {

  }
}