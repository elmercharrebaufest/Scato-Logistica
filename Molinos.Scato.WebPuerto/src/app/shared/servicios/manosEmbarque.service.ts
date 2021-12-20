import { EventEmitter, Injectable, Output } from '@angular/core';
import { EmbarqueNav } from '@ScatoModels/embarque-nav';
import { ModuloDeCarga } from '@ScatoModels/modulo-carga';
import { ModuloDeCargaService } from './modulo-de-carga.service';

@Injectable({
    providedIn: 'root'
})
export class ManosEmbarqueService {
    @Output() removerManoDeEmbarque = new EventEmitter<object>();
    @Output() removerResaltadoSilos = new EventEmitter();
    @Output() resaltarSilo = new EventEmitter<string>();
    @Output() agregarManoDeEmbarque = new EventEmitter<object>();
    @Output() agregarTabique = new EventEmitter<object>();
    @Output() removerTabique = new EventEmitter<string>();
}
