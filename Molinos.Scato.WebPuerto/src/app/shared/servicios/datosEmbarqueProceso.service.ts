import { EventEmitter, Injectable, Output } from '@angular/core';
import { EmbarqueNav } from '@ScatoModels/embarque-nav';
import { ModuloDeCarga } from '@ScatoModels/modulo-carga';
import { ModuloDeCargaService } from './modulo-de-carga.service';

@Injectable({
    providedIn: 'root'
})
export class DatosEmbarquesProcesoService {
    // DATOS DE EMBARQUES Y EMBARQUE SELECCIONADO PARA EL PROCESO DE OPERACIONES/TABLERISTAS/RECIBIDORES
    private embarques: EmbarqueNav[];
    private embarqueSelected: EmbarqueNav;
    private planoCargaId: number;
    private moduloDeCargaId: number;
    private embarqueId: number;
    private datosGrafico: Object;
    private moduloDeCarga: ModuloDeCarga;
    private estadoAltura: any;
    @Output() sendEstadoAltura = new EventEmitter<number>();
    @Output() sendEmbarque = new EventEmitter<EmbarqueNav>();

    constructor(private _moduloCargaService: ModuloDeCargaService){}

    //GUARDA LOS DATOS DEL EMBARQUE SELECCIONADO
    /**
     * 
     * @param {number} id id del embarque seleccionado
     * @returns {void}
     * @memberof DatosEmbarquesProcesoService
     */
    setEmbarque(id: number) {
        if (this.embarqueId != id) {
            this.embarqueId = id;
            this.embarqueSelected = this.embarques.find(e => e.id === id);
            this.sendEmbarque.emit(this.embarqueSelected);
            this.planoCargaId = this.embarqueSelected.planoDeCargaId;
            this.moduloDeCargaId = this.embarqueSelected.moduloDeCargaId;
            this._moduloCargaService.obtenerModuloDeCarga(this.moduloDeCargaId).subscribe(
                res => {
                    this.moduloDeCarga = res;
                }
            )
        }
    }

    //GUARDA LA LISTA DE EMBARQUE
    setEmbarquesList(embarques: EmbarqueNav[]) {
        this.embarques = embarques;
    }

    //GUARDA LOS DATOS DE GRAFICO
    setDatosGrafico(datos: object) {
        this.datosGrafico = datos;
    }

    //GUARDA LOS DATOS DE PLANO DE CARGA Y SETEA EL EMBARQUE SELECTED POR EL PLANO
    setPlanoDeCarga(id: number) {
        this.planoCargaId = id;
        if (this.embarques) {
            this.embarqueSelected = this.embarques.find(e => e.planoDeCargaId === this.planoCargaId);
            this.moduloDeCargaId = this.embarqueSelected.moduloDeCargaId;
            this.embarqueId = this.embarqueSelected.id;
        }
    }

    //GUARDA LOS DATOS DE MODULO DE CARGA Y SETEA EL EMBARQUE SELECTED POR EL MODULO
    setModulodDeCarga(id: number) {
        this.moduloDeCargaId = id;
        this.embarqueSelected = this.embarques.find(e => e.planoDeCargaId === this.moduloDeCargaId);
        this.planoCargaId = this.embarqueSelected.planoDeCargaId;
        this.embarqueId = this.embarqueSelected.id;
    }

    //GUARDA LOS DATOS DE ALTURA
    setEstadoAltura(altura: number){
        this.sendEstadoAltura.emit(altura);
        this.estadoAltura = altura;
    }

    //OBTIENE LA LISTA DE EMBARQUES
    getEmbarquesList() {
        return this.embarques;
    }

    //OBTIENE EL EMBARQUE SELECCIONADO
    getEmbarqueSelected() {
        return this.embarqueSelected;
    }

    //OBTIENE EL ID PLANO DE CARGA
    getPlanoDeCargaId() {
        return this.planoCargaId;
    }

    //OBTIENE EL ID MODULO DE CARGA
    getModuloDeCargaId() {
        return this.moduloDeCargaId;
    }

    //OBTIENE EL ID DEL EMBARQUE
    getEmbarqueId() {
        return this.embarqueId;
    }

    //OBTIENE DATOS DE GRAFICO
    getDatosGrafico() {
        return this.datosGrafico;
    }

    //OBTIENE MODULO DE CARGA
    getModuloDeCarga(){
        return this.moduloDeCarga;
    }

    //OBTIENE ESTADO DE ALTURA
    getEstadoAltura(){
        return this.estadoAltura;
    }

    disposeData(){
        this.embarques = undefined;
        this.embarqueSelected = undefined;
        this.planoCargaId = undefined;
        this.moduloDeCargaId = undefined;
        this.embarqueId = undefined;
        this.datosGrafico = undefined;
    }
}
