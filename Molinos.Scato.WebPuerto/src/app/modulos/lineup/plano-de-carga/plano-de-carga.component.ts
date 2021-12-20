import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LoadScreen } from '@ScatoInterfaces/load-screen';
import { EmbarqueNav } from '@ScatoModels/embarque-nav';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { WorkflowService } from '@ScatoServicios/workflow.service';
import { PlanoContentComponent } from './plano-content/plano-content.component';

@Component({
  selector: 'app-plano-de-carga',
  templateUrl: './plano-de-carga.component.html',
  styleUrls: ['./plano-de-carga.component.css']
})
export class PlanoDeCargaComponent extends LoadScreen implements OnInit {
  mostrarSpinner: boolean = true;
  mostrarNavtabs: boolean = false;
  mostrarPlano: boolean = false;
  embarquesEnLineUp: EmbarqueNav[];
  embarqueId: number;
  estadoAlturaValor: string;
  planoDeCargaId: number;
  @ViewChild(PlanoContentComponent, { static: false }) planoContent: PlanoContentComponent;

  constructor(
    private route: ActivatedRoute,
    private workflowService: WorkflowService,
    private _procesoService: DatosEmbarquesProcesoService,
    private _changeDet: ChangeDetectorRef
  ) {
    super();
  }

  ngOnInit() {
    this.subscribeEmbarques();
  }

  subscribeEmbarques() {
    this.workflowService.listarEmbarquesEnLineUp().subscribe(x => {
      this.embarquesEnLineUp = x;
      this._procesoService.setEmbarquesList(this.embarquesEnLineUp);
      let id = this.route.snapshot.params.id;
      if (id)
        this._procesoService.setEmbarque(Number(id));
      this.mostrarNavtabs = true;
    });
  }
  
  recibirEstadoAlturaValor(estadoAlturaValor) {
    this.estadoAlturaValor = estadoAlturaValor;
    if(this.planoContent)
      this.planoContent.calcularRecomendacionDefensas(this.estadoAlturaValor);
  }

  showPlano(event){
    setTimeout(() => {
      this.mostrarPlano = event;
    }, 50)
  }

  hideSpinner(event){
    this.mostrarSpinner = !event;
  }

  hideSpinnerAux(event) {
    this.mostrarSpinner = event;
  }

  guardarPlanoDeCarga(finalizar: boolean) {
    this._changeDet.detectChanges();
    this.planoContent.guardarPlanoDeCarga(finalizar);
  }
}