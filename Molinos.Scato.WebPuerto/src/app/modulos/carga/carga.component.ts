import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { WorkflowService } from '@ScatoServicios/workflow.service';
import { EmbarqueNav } from '@ScatoModels/embarque-nav';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';


@Component({
  selector: 'app-carga',
  templateUrl: './carga.component.html',
  styleUrls: ['./carga.component.css']
})
export class CargaComponent implements OnInit, OnDestroy {
  mostrarSpinner: boolean = true;
  mostrarTabs: boolean = false;
  mostrarPlano: boolean = false;
  mostrarCargas: boolean = false;
  embarquesEnLineUp: EmbarqueNav[];
  embarqueSelected: EmbarqueNav;
  unsubscribe: Subject<any>;

  constructor(
    private workflowService: WorkflowService,
    private _procesoService: DatosEmbarquesProcesoService
  ) {
    this.unsubscribe = new Subject();
  }

  ngOnInit(): void {
    this.subscribeEmbarques();
  }

  subscribeEmbarques() {
    this._procesoService.disposeData();
    try {
      this.workflowService.listarEmbarquesEnLineUp()
        .pipe(takeUntil(this.unsubscribe))
        .subscribe(res => {
          this.embarquesEnLineUp = res;
          this._procesoService.setEmbarquesList(this.embarquesEnLineUp);
          this.mostrarTabs = true;
        });
    } catch (e) {
      console.log(e);
      console.log("Error en listarEmbarquesEnLineUp");
    }
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