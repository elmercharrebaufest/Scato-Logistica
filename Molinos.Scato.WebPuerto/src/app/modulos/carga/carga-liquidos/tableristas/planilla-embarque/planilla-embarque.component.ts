import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { TurnosService } from '@ScatoServicios/turnos.service';

@Component({
  selector: 'app-planilla-embarque',
  templateUrl: './planilla-embarque.component.html',
  styleUrls: ['./planilla-embarque.component.css']
})
export class PlanillaEmbarqueComponent implements OnInit {
  lineasEmbarque: FormGroup;
  exportadores: any;
  bodegas: any[];
  lineas: any[];
  productos: any[];
  destinos: any[];
  tanquesAbordo: any[];
  constructor(
    private builder: FormBuilder,
    private turnosService: TurnosService,
    private procesoService: DatosEmbarquesProcesoService
  ) {
    this.turnosService.sendExportadores.subscribe(res => this.exportadores = res);
    this.turnosService.sendBodega.subscribe(res => {
      this.bodegas = res;
      this.getProductos();
      this.getTanqueAbordo();
    });
  }

  ngOnInit(): void {
    this.newForm();
  }

  newForm() {
    this.lineas = this.procesoService.getModuloDeCarga().moduloDeCargaLineasDeEmbarque;
    this.exportadores = this.turnosService.getExportadores().filter(e => e.exportador && e.cantidad);
    this.bodegas = this.turnosService.getBodega();
    this.lineasEmbarque = new FormGroup({
      linea: this.builder.array([this.initLinea(), this.initLinea(), this.initLinea()])
    })
    this.getProductos();
    this.getTanqueAbordo();
  }

  getProductos() {
    this.productos = new Array();
    this.destinos = new Array();
    this.bodegas.forEach(b => {
      if (b.destino && !this.destinos.find(d => d == b.destino.nombre)) this.destinos.push(b.materialPuerto.descripcionCorta);
      if (b.materialPuerto && !this.productos.find(p => p == b.materialPuerto.descripcionCorta)) this.productos.push(b.materialPuerto.descripcionCorta)
    })
  }

  initLinea() {
    return this.builder.group({
      exportador: '',
      partida: 0,
      tksAbordo: 0,
      destino: 0,
      tksTierra: '',
      tn: '',
      producto: 0,
      comenzo: '',
      finalizo: ''
    })
  }

  get linea(): FormArray {
    return this.lineasEmbarque.get('linea') as FormArray;
  }

  autoCompleteParcel(parcel, l: FormGroup) {
    let bodega = this.bodegas.find(b => b.bodegaParcel == parcel);
    l.get('tksAbordo').setValue(bodega.tanqueDeAbordo);
    l.get('destino').setValue(bodega.destino.id);
    l.get('tn').setValue(bodega.cantidad);
    l.get('producto').setValue(bodega.materialPuerto.id);
  }

  getTanqueAbordo(){
    this.tanquesAbordo = new Array();
    this.bodegas.forEach(b => {
      if (this.tanquesAbordo.find(t => t == b.tanqueDeAbordo)) this.tanquesAbordo.push(b.tanqueDeAbordo);
    })
  }
}