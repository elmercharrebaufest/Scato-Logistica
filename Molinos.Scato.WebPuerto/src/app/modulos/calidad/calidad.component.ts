import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject, forkJoin } from 'rxjs';
import { InstanciaWorkflowPuerto } from '@ScatoModels/instancia-wokflow-puerto';
import { UbicacionDeBuquePuerto } from '@ScatoModels/ubicacion-de-buque-puerto';
import { EmbarqueService } from '@ScatoServicios/embarque.service';
import { WorkflowService } from '@ScatoServicios/workflow.service';
import { ProcesoCalidadService } from '@ScatoServicios/procesoCalidad.service';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { EmbarqueNav } from '@ScatoModels/embarque-nav';

@Component({
  selector: 'app-calidad',
  templateUrl: './calidad.component.html',
  styleUrls: ['./calidad.component.css']
})
export class CalidadComponent implements OnInit, OnDestroy {
  mostrarSpinner: boolean = true;
  mostrarTabs: boolean = false;
  mostrarPlano: boolean = false;
  mostrarCargas: boolean = false;
  embarquesEnLineUp: EmbarqueNav[] = [];
  embarqueSelected: EmbarqueNav;
  unsubscribe: Subject<any>;

  embarquesEnLineUpSinFiltrar: EmbarqueNav[];
  listadoEmbarques: InstanciaWorkflowPuerto[];

  ubicacionDeBuquePuerto: UbicacionDeBuquePuerto[];

  buqueEnSanBenito: InstanciaWorkflowPuerto | undefined;
  buqueEnNoryon: InstanciaWorkflowPuerto | undefined;
  buqueEnVicentin: InstanciaWorkflowPuerto | undefined;
  buqueEnOtrosMuelles: InstanciaWorkflowPuerto | undefined;

  constructor(
    private workflowService: WorkflowService,
    private embarqueService: EmbarqueService,
    private procesoCalidadService: ProcesoCalidadService,
    private _procesoService: DatosEmbarquesProcesoService) {

    this.unsubscribe = new Subject();

    // ubicacionDeBuquePuerto: {id: 1011, nombre: "Muelle de Carga", orden: 2}
    this.embarqueService.obtenerListadoUbicacionDeBuquePuerto()
      .subscribe(res => {
        this.ubicacionDeBuquePuerto = res;
      });
  }

  ngOnInit(): void {
    this.trabajoOrdenado();
  }

  trabajoOrdenado(){
    forkJoin({
      obtenerListado: this.workflowService.obtenerListado(),
      listarEmbarquesEnLineUp: this.workflowService.listarEmbarquesEnLineUp()
    })
    .subscribe( res => {
      this.listadoEmbarques = res.obtenerListado;
      this.filtrarMuelles();

      this.embarquesEnLineUpSinFiltrar = res.listarEmbarquesEnLineUp;
      this.embarquesEnLineUp.push(this.embarquesEnLineUpSinFiltrar.find(m => m.id == this.buqueEnSanBenito.embarque.id));
      this._procesoService.setEmbarquesList(this.embarquesEnLineUp);

      this.mostrarTabs = true;
    });
  }

  filtrarMuelles() {
    let ubicacion = this.ubicacionDeBuquePuerto ? this.ubicacionDeBuquePuerto.find(x => x.orden == 2).id : '';

    // Filtro los buques de cada muelle. Buque que esta cargando en el muelle
    this.buqueEnSanBenito = this.listadoEmbarques ? this.listadoEmbarques
      .filter(i => i.embarque.sanBenito || (!i.embarque.vicentin && !i.embarque.otrosMuelles && !i.embarque.noryon))
      .find(m => m.embarque.ubicacion == ubicacion) : undefined;
    this.buqueEnNoryon = this.listadoEmbarques ? this.listadoEmbarques.filter(i => i.embarque.noryon)
      .find(m => m.embarque.ubicacion == ubicacion) : undefined;
    this.buqueEnVicentin = this.listadoEmbarques ? this.listadoEmbarques.filter(i => i.embarque.vicentin)
      .find(m => m.embarque.ubicacion == ubicacion) : undefined;
    this.buqueEnOtrosMuelles = this.listadoEmbarques ? this.listadoEmbarques.filter(i => i.embarque.otrosMuelles)
      .find(m => m.embarque.ubicacion == ubicacion) : undefined;

    this.procesoCalidadService.setSanBenito(this.buqueEnSanBenito);
    this.procesoCalidadService.setNoryoun(this.buqueEnNoryon);
    this.procesoCalidadService.setVicentin(this.buqueEnVicentin);
    this.procesoCalidadService.setOtrosMuelles(this.buqueEnOtrosMuelles);
  }

  showPlano(event: boolean) {
    this.embarqueSelected = this._procesoService.getEmbarqueSelected();
    
    setTimeout(() => {
      this.mostrarPlano = event;
    }, 50);
  }

  showCargas(event) {
    this.mostrarCargas = event;
  }

  hideSpinner(event) {
    setTimeout(() => {
      this.mostrarSpinner = event;
    }, 50);
  }

  changeEmbarque() {
    this.mostrarCargas = false;
    this.mostrarSpinner = true;
    this.embarqueSelected = this._procesoService.getEmbarqueSelected();
  }

  ngOnDestroy() {
    this.unsubscribe.complete()
  }
}
