import { AfterViewInit, Component, EventEmitter, OnInit, Output, QueryList, SimpleChanges, ViewChildren } from '@angular/core';
import { Subject } from 'rxjs';
import { EmbarqueNav } from '@ScatoModels/embarque-nav';
import { InstanciaWorkflowPuerto } from '@ScatoModels/instancia-wokflow-puerto';
import { UbicacionDeBuquePuerto } from '@ScatoModels/ubicacion-de-buque-puerto';
import { ProcesoCalidadService } from '@ScatoServicios/procesoCalidad.service';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';

@Component({
  selector: 'app-navtabs-calidad',
  templateUrl: './navtabs-calidad.component.html',
  styleUrls: ['./navtabs-calidad.component.css']
})
export class NavtabsCalidadComponent implements OnInit, AfterViewInit {
  @Output() showPlano = new EventEmitter<boolean>();
  @Output() changeEmbarque = new EventEmitter<any>();
  @ViewChildren('listadoBotones') listadoBotones: QueryList<any>;
  elementos: EmbarqueNav[];
  embarqueId: number;
  _unsubscribe: Subject<any>;

  ningunBuqueOperativo: boolean = false;
  listadoEmbarques: InstanciaWorkflowPuerto[];
  ubicacionDeBuquePuerto: UbicacionDeBuquePuerto[];

  buqueSanBenito: InstanciaWorkflowPuerto | undefined;
  buqueSanBenito2: EmbarqueNav;
  buqueNoryon: InstanciaWorkflowPuerto | undefined;
  buqueVicentin: InstanciaWorkflowPuerto | undefined;
  buqueOtrosMuelles: InstanciaWorkflowPuerto | undefined;

  embarqueSelected: EmbarqueNav;
  cargado: boolean = false;

  constructor(
    private procesoCalidadService: ProcesoCalidadService,
    private _procesoService: DatosEmbarquesProcesoService
  ) {
    this._unsubscribe = new Subject();
    this.elementos = new Array();

    this.buqueSanBenito = this.procesoCalidadService.getSanBenito();
    
    this.buqueNoryon = this.procesoCalidadService.getNoryoun();
    this.buqueVicentin = this.procesoCalidadService.getVicentin();
    this.buqueOtrosMuelles = this.procesoCalidadService.getOtrosMuelles();

    this._procesoService.setEmbarque(this.buqueSanBenito.embarque.id);

    this.elementos = this._procesoService.getEmbarquesList();
    this.embarqueId = this._procesoService.getEmbarqueId();

    this.buqueSanBenito2 = this.elementos[0];
  }

  ngOnInit(): void {
    this.embarqueSanBenito();
  }

  ngOnChanges(change: SimpleChanges) {
    if (change.elementos) {
      if (this.elementos) {
        this.embarqueSanBenito();
      }
    }
  }

  embarqueSanBenito() {
    if (this.elementos.length > 0) {
      if (!this.embarqueId) {
        this.embarqueId = this.elementos[0].id;
        this._procesoService.setEmbarque(this.elementos[0].id);
      }
    }
  }

  ngAfterViewInit() {
    if (this.elementos.length > 0) {
      var elementoSeleccionado = document.getElementById(this.embarqueId.toString());
      if (elementoSeleccionado) elementoSeleccionado.classList.add("btn-seleccionado");
    }
    this.showPlano.emit(true);

    this.cargado = true;
  }

  onClickHandlerClient(elemento: EmbarqueNav) {
    var elementoDeseleccionado = document.getElementsByClassName("btn-seleccionado")[0];
    if (elementoDeseleccionado != null)
      elementoDeseleccionado.classList.remove("btn-seleccionado")
    var elementoSeleccionado = document.getElementById(elemento.id.toString());
    elementoSeleccionado.classList.add("btn-seleccionado");
    if (this._procesoService.getEmbarqueSelected() != elemento) {
      this._procesoService.setEmbarque(elemento.id);
      this.changeEmbarque.emit(true);
    }
  }

  estadoSanBenito() {
    let msje = `Buque #1 ${this.buqueSanBenito?.embarque.nombreBuque} / CARGANDO`
    return this.buqueSanBenito ? msje : "No hay ningún barco operando";
  }
  estadoVicentin() {
    let msje = `Buque #1 ${this.buqueVicentin?.embarque.nombreBuque} / CARGANDO`
    return this.buqueVicentin ? msje : "No hay ningún barco operando";
  }
  estadoNoryon() {
    let msje = `Buque #1 ${this.buqueNoryon?.embarque.nombreBuque} / CARGANDO`
    return this.buqueNoryon ? msje : "No hay ningún barco operando";
  }
  estadoOtros() {
    let msje = `Buque #1 ${this.buqueOtrosMuelles?.embarque.nombreBuque} / CARGANDO`
    return this.buqueOtrosMuelles ? msje : "No hay ningún barco operando";
  }

  muellesOperativos() {
    return this.buqueSanBenito || this.buqueNoryon || this.buqueVicentin || this.buqueOtrosMuelles ? true : false;
  }

  imgSolidoLiquido(buque: InstanciaWorkflowPuerto) {
    if (buque)
      return buque.embarque.esLiquido ? "icono-liquido" : "icono-solido";
  }

}
