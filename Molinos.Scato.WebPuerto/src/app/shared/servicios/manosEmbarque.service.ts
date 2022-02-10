import { EventEmitter, Injectable, Output } from '@angular/core';


@Injectable({
  providedIn: 'root'
})
export class ManosEmbarqueService {
  @Output() removerManoDeEmbarque = new EventEmitter<object>();
  @Output() removerResaltadoSilos = new EventEmitter<object>();
  @Output() resaltarSilo = new EventEmitter<string>();
  @Output() agregarManoDeEmbarque = new EventEmitter<object>();
  @Output() agregarTabique = new EventEmitter<object>();
  @Output() removerTabique = new EventEmitter<string>();
}
