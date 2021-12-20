import { Injectable, Output, EventEmitter } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class EstadoTanquesService {
    @Output() sendData = new EventEmitter<any>();
    private tanks;

    setTank(form: any){
        this.tanks = form;
        this.sendData.emit(form);
    }

    getTanks(){
        return this.tanks;
    }
}
