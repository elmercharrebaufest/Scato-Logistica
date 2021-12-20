import { EventEmitter, Injectable, Output } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class Balanzas78Service {

  @Output() sendDataBalanza7 = new EventEmitter<any>();
  @Output() sendDataBalanza8 = new EventEmitter<any>();

  setBalanza7(form: any){
    this.sendDataBalanza7.emit(form);
  }
  setBalanza8(form: any){
    this.sendDataBalanza8.emit(form);
  }

  // listadoBalanza8 = [
  //   {
  //     id: 5,
  //     nombreBuque: 'embarque01',
  //     numeroBalanza: 8,
  //     fecha: '13/08/2021',
  //     hora: '07:20',
  //     toneladas: 1900,
  //     producto: 'MAIZ',
  //     bodega: 999,
  //     porcentajeCarga: 50,
  //     totalProducto: 1500,
  //     motivoBalanza: [],
  //     observaciones: '',
  //   },
  //   {
  //     id: 7,
  //     nombreBuque: 'embarque01',
  //     numeroBalanza: 8,
  //     fecha: '17/08/2021',
  //     hora: '10:25',
  //     toneladas: 500,
  //     producto: 'MAIZ',
  //     bodega: 888,
  //     porcentajeCarga: 20,
  //     totalProducto: 1000,
  //     motivoBalanza: [],
  //     observaciones: '',
  //   }
  // ];

  // obs = new Observable<Balanza78[]>(
  //   (observer) => {
  //     observer.next(this.listadoBalanza8);
  //   }
  // )

  constructor() { }

  // observablePrueba(): Observable<Balanza78[]> {
  //   return this.obs;
  // }
}
