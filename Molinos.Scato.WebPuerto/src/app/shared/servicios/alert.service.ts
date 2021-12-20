import { Injectable } from '@angular/core';
import { Alerta } from '@ScatoModels/alerta';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  private alerta: Alerta = null;
  subject = new Subject();

  mostrar(alerta: Alerta) {
    this.alerta = alerta;
    this.subject.next(this.alerta);
  }
}
