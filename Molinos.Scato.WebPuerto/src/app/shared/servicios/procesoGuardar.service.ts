import { EventEmitter, Injectable, Input, Output } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class ProcesoGuardarService {
    @Output() sendGuardar = new EventEmitter<boolean[]>();

}