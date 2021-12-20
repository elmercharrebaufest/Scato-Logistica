import { FormControl, FormGroup } from "@angular/forms";

export class FormUmap{
    formulario: FormGroup;

    constructor(){
        this.formulario = new FormGroup({
            fecha_encendido: new FormControl(),
            hora_encendido: new FormControl(),
            fecha_apagado: new FormControl(),
            hora_apagado: new FormControl(),
        });
    }
}