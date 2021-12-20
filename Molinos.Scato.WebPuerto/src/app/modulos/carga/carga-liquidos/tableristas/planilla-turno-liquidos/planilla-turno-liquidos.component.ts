import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModuloDeCarga } from '@ScatoModels/modulo-carga';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { TurnosService } from '@ScatoServicios/turnos.service';

@Component({
  selector: 'app-planilla-turno-liquidos',
  templateUrl: './planilla-turno-liquidos.component.html',
  styleUrls: ['./planilla-turno-liquidos.component.css']
})
export class PlanillaTurnoLiquidosComponent implements OnInit {
  formTurnos: FormGroup;
  formCorte: FormGroup;
  moduloCarga: ModuloDeCarga;
  exportadores: any[];
  lineas: any[];
  arrLineas: any[];
  partidas: any[];
  producto: any[];
  tanques: any[];
  hoy: any;
  bodegas: any[];
  productos: any[];
  turnos = ['00-06', '06-12', '12-18', '18-24'];
  constructor(
    private _builder: FormBuilder,
    private _modalService: NgbModal,
    private _procesoService: DatosEmbarquesProcesoService,
    private datePipe: DatePipe,
    private _turnosService: TurnosService
  ) { 
    this._turnosService.sendBodega.subscribe( res => {
      this.bodegas = res
      this.getProductos();
    });
  }

  ngOnInit(): void {
    this.newForm()
  }

  newForm() {
    this.formTurnos = this._builder.group({
      diasTurno: this._builder.array([this.initDia()]),
    });
    this.formCorte = this._builder.group({
      motivo: '0',
      inicio: '',
      fin: '',
      total: '',
      observaciones: ''
    });
    this.formCorte.get('total').disable();
    this.getCombos();
  }
  getCombos(){
    this.lineas = this._procesoService.getModuloDeCarga().moduloDeCargaLineasDeEmbarque;
    this.arrLineas = this.lineas;
    this.arrLineas = this.arrLineas.map(l => { return l = l.linea });
    this.arrLineas.filter((value, index, array) => {
      return array.indexOf(value) === index;
    })
    let exportadoresForm = this._turnosService.getExportadores();
    this.exportadores = new Array();
    for (let e of exportadoresForm){
      if (e.exportador) this.exportadores.push(e.exportador.nombre);
    }
    this.hoy = this.datePipe.transform(new Date(), 'dd-MM-yyyy');
    this.bodegas = this._turnosService.getBodega();
    this.getProductos();
  }

  get diasTurno(): FormArray {
    return this.formTurnos.get('diasTurno') as FormArray;
  }

  getTurnos(dia): FormArray {
    return this.diasTurno["controls"][dia]['controls'].turnos as FormArray;
  }

  getTurno(d, t): FormArray {
    return this.getTurnos(d)['controls'][t]['controls'].turno as FormArray;
  }

  getCorteTurnos(d, t): FormArray {
    return this.getTurnos(d)['controls'][t]['controls'].corteTurnos as FormArray;
  }

  openModalCorte(modal, dia, turno) {
    this.formCorte.reset();
    if (this.getTurnos(dia)['controls'][turno].enabled) {
      this._modalService.open(modal, { windowClass: 'window-modal-corte', backdropClass: 'modal-corte' }).result.then(() => {
        this.getCorteTurnos(dia, turno).push(this.initCorte(this.formCorte.getRawValue()));
      })
    }
  }

  calcularTotal() {
    let desde = this.formCorte.get('inicio').value ? this.formCorte.get('inicio').value.split(':') : '',
      hasta = this.formCorte.get('fin').value ? this.formCorte.get('fin').value.split(':') : '',
      f_desde = new Date(),
      f_hasta = new Date(),
      total = new Date();

    f_desde.setHours(desde[0], desde[1], 0, 0);
    f_hasta.setHours(hasta[0], hasta[1], 0, 0);

    total.setHours(f_hasta.getHours() - f_desde.getHours(), f_hasta.getMinutes() - f_desde.getMinutes(), 0, 0);

    if (total.getHours())
      this.formCorte.get('total').setValue(`${total.getHours() < 10 ? '0' + total.getHours() : total.getHours()}:${total.getMinutes() < 10 ? '0' + total.getMinutes() : total.getMinutes()}`)
  }


  getRowSpan(dia: any) {
    let contador = 0;
    for (let turnos of dia.controls.turnos.controls) {
      contador += this.getRowSpanTurno(turnos);
      contador += 2;
    }
    return contador;
  }

  getRowSpanTurno(turno: any) {
    if (turno.controls.corteTurnos.length > 0)
      return turno.controls['turno'].controls.length + 1;
    else
      return turno.controls['turno'].controls.length;
  }

  getTurnoHorario(dia) {
    return this.turnos[dia];
  }

  enviarTurno(dia, turno) {
    if (!this.getTurnos(dia)['controls'][turno]['controls'].enviado.value) {
      if (this.diasTurno['controls'][dia]['controls'].turnos.length < 4) {
        this.getTurnos(dia)['controls'][turno]['controls'].enviado.setValue(true);
        this.getTurnos(dia).push(this.initTurno())
      } else {
        this.getTurnos(dia)['controls'][turno]['controls'].enviado.setValue(true);
        this.diasTurno.push(this.initDia());
      }
      this.sendRitmos(dia, turno);
    }
  }

  sendRitmos(dia, turno){
    this._turnosService.setTurnos(this.getTurnos(dia).controls[turno].value)
  }

  getCantTurno(t){
    let contador = 0;
    for(let turno of t['controls']['turno'].controls){
      contador += turno.controls.cantidad.value ? turno.controls.cantidad.value : 0 ;
    }
    return contador;
  }

  getCantDia(d){
    let contador = 0;
    for (let turno of d['controls']['turnos']['controls']){
      contador += this.getCantTurno(turno);
    }
    return contador;
  }

  getCantTotalABordo(){
    let contador = 0;
    for(let dia of this.formTurnos['controls']['diasTurno']['controls']){
      contador += this.getCantDia(dia);
    }
    return contador;
  }

  initDia() {
    return this._builder.group({
      turnos: this._builder.array([this.initTurno()])
    })
  }

  initTurno() {
    return this._builder.group({
      turno: this._builder.array([this.initLinea(), this.initLinea(), this.initLinea(), this.initLinea()]),
      corteTurnos: this._builder.array([]),
      enviado: false
    })
  }

  initLinea() {
    return this._builder.group({
      exportador: '',
      linea: '',
      partida: 0,
      producto: 0,
      tk: '',
      grados: '',
      medInicial: '',
      medFinal: '',
      destino: '',
      cantidad: '',
    })
  }

  initCorte(corte?: any) {
    return this._builder.group({
      motivo: (corte && corte.motivo ? corte.motivo : '', Validators.required),
      inicio: (corte && corte.inicio ? corte.inicio : '00:00', Validators.required),
      fin: (corte && corte.fin ? corte.fin : '00:00', Validators.required),
      total: (corte && corte.total ? corte.total : '00:00', Validators.required),
      observaciones: (corte && corte.observaciones ? corte.observaciones : '', Validators.required)
    })
  }

  getToneladasParcelDia(bodega: number, d: number){
    let dia = this.getTurnos(d);
    let cantidad = 0;
    for (let turno of dia.controls){
      for (let linea of turno['controls']['turno']['controls']){
        cantidad += (linea.controls.partida.value == bodega ? Number(linea.controls.cantidad.value) : 0);
      }
    }
    return cantidad;
  }

  getToneladasParcelTurno(bodega: number, d: number, t: number){
    let turno = this.getTurno(d, t);
    let cantidad = 0;
    for (let linea of turno.controls){
      cantidad += (linea['controls'].partida.value == bodega ? Number(linea['controls'].cantidad.value) : 0);
    }
    return cantidad;
  }
  
  getToneladasLinea(linea: string){
    const value = linea.toLowerCase();
    let contador = 0;
    this.diasTurno.controls.forEach(dia => {
      dia['controls']['turnos']['controls'].forEach(turno => {
        turno['controls']['turno']['controls'].forEach(linea => {
          contador += (linea.get('linea').value.toLowerCase() == value ? Number(linea.get('cantidad').value) : 0);
        })
      });
    })

    return contador;
  }

  getProductos(){
    this.productos = new Array();
    this.bodegas.forEach(b => {
      if (b.materialPuerto && !this.productos.find(p => p == b.materialPuerto.descripcionCorta)) this.productos.push(b.materialPuerto.descripcionCorta)
    })
  }
}