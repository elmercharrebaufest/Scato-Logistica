import { EventEmitter, Injectable, Output } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class TurnosService {
    @Output() sendTurnos = new EventEmitter<any>();
    @Output() sendTnTotal = new EventEmitter<number>();
    @Output() sendExportadores = new EventEmitter<any>();
    @Output() sendBodega = new EventEmitter<any>();
    private turnos: any;
    private tnTotales: number = 0;
    private formExportadores: any;
    private bodega: any;

    setTurnos(turno: any){
        this.turnos = turno;
        this.sendTurnos.emit(turno);
    }

    getTurnos(){
        return this.turnos;
    }

    setTnTotales(tn: number){
        this.tnTotales = tn;
        this.sendTnTotal.emit(tn)
    }

    getTnTotales(){
        return this.tnTotales;
    }

    setExportadores(formExportadores: any){
        this.formExportadores = formExportadores;
        this.sendExportadores.emit(formExportadores);
    }

    getExportadores(){
        return this.formExportadores;
    }

    setBodega(bodega){
        this.bodega = bodega;
        this.sendBodega.emit(bodega);
    }

    getBodega(){
        return this.bodega;
    }
}