import { formatDate } from '@angular/common';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { WorkflowService } from '@ScatoServicios/workflow.service';
import { LineupService } from '@ScatoServicios/lineup.service';
import { ConfirmationDialogService } from '@ScatoServicios/confirmation-dialog.service';
import { AutenticadorService } from '@ScatoServicios/autenticador.service';
import { UbicacionDeBuquePuerto } from '@ScatoModels/ubicacion-de-buque-puerto';
import { MaterialPuertoCantidad } from '@ScatoModels/material-puerto-cantidad';
import { InstanciaWorkflowPuerto } from '@ScatoModels/instancia-wokflow-puerto';
import { Observador } from '@ScatoInterfaces/observador';
import { LoadScreen } from '@ScatoInterfaces/load-screen';
import { Tipoalerta } from '@ScatoEnums/tipo-alerta';
import { PermisosScato } from '@ScatoEnums/permisos-scato';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { MessageService } from 'primeng/api';
import { SessionService } from '@ScatoServicios/session.service';
import { Usuario } from '@ScatoInterfaces/usuario';

@Component({
  selector: 'app-lineup-embarque',
  templateUrl: './lineup-embarque.component.html',
  styleUrls: ['./lineup-embarque.component.css']
})
export class LineupEmbarqueComponent implements OnInit {
  @Input() index: number;
  @Input() instanciaWorkflow: InstanciaWorkflowPuerto;
  @Input() observador: Observador;
  @Output() showSpinner = new EventEmitter<boolean>();
  ubicacionDeBuquePuerto: UbicacionDeBuquePuerto[];
  acciones: string[];
  listadoUbicacionDeBuquePuerto: string[];
  showMenu = false;
  fechaCarta: string;
  horaCarta: string;
  permisosScato: typeof PermisosScato = PermisosScato;
  posicionesDeLineUps: number[];
  embarquesPuerto: InstanciaWorkflowPuerto[];
  private user: Usuario;
  constructor(
    private lineUpService: LineupService,
    private workflowService: WorkflowService,
    private router: Router,
    private confirmationDialogService: ConfirmationDialogService,
    private _procesoService: DatosEmbarquesProcesoService,
    private messageService: MessageService,
    private session: SessionService
  ) {
    this.user = this.session.getUser();
  }

  ngOnInit(): void {
    if (this.instanciaWorkflow.lineUp.cartaDeSubidaAprobada) {
      this.fechaCarta = formatDate(this.instanciaWorkflow.lineUp.cartaDeSubidaAprobada, 'yyyy-MM-dd', 'es-ar');
      this.horaCarta = formatDate(this.instanciaWorkflow.lineUp.cartaDeSubidaAprobada, 'HH:mm', 'es-ar');
    }
    this._procesoService.disposeData();
    this.embarquesPuerto = this.observador != null ? this.observador.ListarEmbarques().filter(u => u.embarque.vicentin == this.instanciaWorkflow.embarque.vicentin && u.embarque.noryon == this.instanciaWorkflow.embarque.noryon && u.embarque.sanBenito == this.instanciaWorkflow.embarque.sanBenito && u.embarque.otrosMuelles == this.instanciaWorkflow.embarque.otrosMuelles) : [];
    this.posicionesDeLineUps = Array.from({ length: this.embarquesPuerto.length }, (v, k) => k + 1);
    this.lineUpService.obtenerListadoUbicacionDeBuquePuerto().subscribe(res => { this.ubicacionDeBuquePuerto = res; });

    this.lineUpService.obtenerListadoUbicacionDeBuquePuerto()
      .subscribe(res => {
        this.listadoUbicacionDeBuquePuerto = res.map(u => u.nombre);
      });
  }

  public modificarLineUp(campo: string) {
    if (this.user.permisos.find(p => p === this.permisosScato.LineUp_EditarChecks)) {
      switch (campo) {
        case "CartaDeSubidaEnviada": {
          this.instanciaWorkflow.lineUp.cartaDeSubidaEnviada = !this.instanciaWorkflow.lineUp.cartaDeSubidaEnviada;
          break;
        }
        case "CargaEnSap": {
          this.instanciaWorkflow.lineUp.cargaEnSap = !this.instanciaWorkflow.lineUp.cargaEnSap;
          break;
        }
        case "NominacionDePractico": {
          this.instanciaWorkflow.lineUp.nominacionDePractico = !this.instanciaWorkflow.lineUp.nominacionDePractico;
          break;
        }
        case "SeguridadPortuaria": {
          this.instanciaWorkflow.lineUp.seguridadPortuaria = !this.instanciaWorkflow.lineUp.seguridadPortuaria;
          break;
        }
        case "InspeccionSenasa": {
          this.instanciaWorkflow.lineUp.inspeccionSenasa = !this.instanciaWorkflow.lineUp.inspeccionSenasa;
          break;
        }
        case "ControlSenasa": {
          this.instanciaWorkflow.lineUp.controlSenasa = !this.instanciaWorkflow.lineUp.controlSenasa;
          break;
        }
        case "ControlPrivado": {
          this.instanciaWorkflow.lineUp.controlPrivado = !this.instanciaWorkflow.lineUp.controlPrivado;
          break;
        }
        case "Amarrador": {
          this.instanciaWorkflow.lineUp.amarrador = !this.instanciaWorkflow.lineUp.amarrador;
          break;
        }
        case "AgenciaContactada": {
          this.instanciaWorkflow.lineUp.agenciaContactada = !this.instanciaWorkflow.lineUp.agenciaContactada;
          break;
        }
      }

      this.lineUpService.modificarLineUp(this.instanciaWorkflow.lineUp).subscribe(res => console.log(res));
    }
  }

  public editarEmbarque() {
    this.router.navigate([`/lineup/alta-embarque/${this.instanciaWorkflow.embarque.id}/lineup`]);
  }

  public armarPlanoCarga() {
    if (this.user.permisos.find(p => p === this.permisosScato.PlanoDeCarga_Editar))
      this.router.navigate([`/lineup/plano-de-carga/${this.instanciaWorkflow.embarque.id}`]);
    else
      this.showWarning();
  }

  public eliminarEmbarque() {
    this.openConfirmationDialog('¡Atención!',
      'Está a punto de eliminar por completo un buque',
      'Eliminar buque',
      'Cancelar');
  }

  public openConfirmationDialog(titulo: string, texto: string, button1: string = 'OK', button2: string = 'Cancel') {
    this.confirmationDialogService.confirm(titulo, texto, button1, button2)
      .then((confirmed) => {
        if (confirmed) {
          this.showSpinner.emit(true)
          this.workflowService.eliminar(this.instanciaWorkflow.id)
            .subscribe(res => {
              this.showSpinner.emit(false)
              this.confirmationDialogService.confirm('¡Felicitaciones!', 'Ha eliminado con éxito el embarque del buque "' + this.instanciaWorkflow.embarque.nombreBuque + '"', 'Cerrar', '')
                .then((confirmed) => { if (this.observador) this.observador.Actualizar(this); });
            },
              errmess => {
                this.confirmationDialogService.confirm('¡Error!', 'Error al eliminar el embarque: ' + <any>errmess.error, 'Cerrar', '', null, null, Tipoalerta.Error);
                this.showSpinner.emit(false)
              });
        }
      })
      .catch(() => console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)'));
  }

  public fechaRecaladaCorrecta() {
    var date = new Date();

    if (!this.instanciaWorkflow.embarque.fechaRecalada) return 'warning';
    if (new Date(this.instanciaWorkflow.embarque.fechaRecalada).getTime() > date.getTime()) return 'success';
    return 'danger';
  }

  public onSelectAction(accion) {
    if (this.user.permisos.find(p => p === this.permisosScato.LineUp_EditarUbicacion)) {
      accion = this.numeroUbicacionDeBuquePuerto(accion);
      /**Muelle de Carga**/
      if (accion == 2) {
        this.workflowService.obtenerListado().subscribe(
          listado => {
            if (this.BarcoEnMuelleActualmente(listado)) {
              this.confirmationDialogService.confirm('¡Error!', 'Actualmente ya se encuentra otro buque en muelle', 'Cerrar', '', null, null, Tipoalerta.Error);
              return;
            }
            else {
              this.actualizarUbicacion(accion);
            }
          }
        );
      }
      /**Zarpó**/
      else if (accion == 1) {
        this.confirmationDialogService.confirm('¡Atención!', `Al pasar a Ubicacion "Zarpó", el buque ${this.instanciaWorkflow.embarque.nombreBuque} dejará de mostrarse dentro del line up`, 'Aceptar', 'Cerrar', null, null, Tipoalerta.Warning)
          .then((confirmed) => {
            if (confirmed) {
              this.showSpinner.emit(true)
              this.instanciaWorkflow.embarque.ubicacion = accion;
              this.instanciaWorkflow.lineUp.ubicacion = accion;
              this.lineUpService.modificarLineUp(this.instanciaWorkflow.lineUp).subscribe(x => {
                if (this.observador) {
                  setTimeout(() => {
                    this.observador.Actualizar();
                    this.showSpinner.emit(false)
                  }, 5000);
                }
              });
            }
          })
          .catch(() => window.location.reload());
      }
      else
        this.actualizarUbicacion(accion);
    }
  }

  actualizarUbicacion(accion) {
    this.instanciaWorkflow.embarque.ubicacion = accion;
    this.instanciaWorkflow.lineUp.ubicacion = accion;
    this.lineUpService.modificarLineUp(this.instanciaWorkflow.lineUp).subscribe(x => { if (this.observador) this.observador.Actualizar(); });
  }

  actualizarOrden(posicion) {
    if (this.user.permisos.find(p => p === this.permisosScato.LineUp_EditarPosicion)) {

      var cantidadDeLineUps = this.embarquesPuerto.length;

      if (posicion == 1) {
        this.instanciaWorkflow.lineUp.orden = this.embarquesPuerto[0].lineUp.orden / 2;
      }
      else if (posicion >= cantidadDeLineUps) {
        this.instanciaWorkflow.lineUp.orden = this.embarquesPuerto[cantidadDeLineUps - 1].lineUp.orden + 1;
      }
      else {
        var posicionActual = this.embarquesPuerto.indexOf(this.instanciaWorkflow);
        var moverParaAbajo = posicionActual < posicion;
        var ordenSiguiente = this.embarquesPuerto[posicion].lineUp.orden;
        var ordenActual = this.embarquesPuerto[posicion - 1].lineUp.orden;
        var ordenanerior = this.embarquesPuerto[posicion - 2].lineUp.orden;

        this.embarquesPuerto.forEach((x, index) => { console.log(index + ":" + x.embarque.nombreBuque + " - " + x.lineUp.orden) });
        console.log(ordenanerior + ' - ' + ordenActual);
        console.log("Base:" + this.embarquesPuerto[posicion - 1].embarque.nombreBuque + " " + ordenActual);
        var ordenNuevo = ordenActual + Math.round((moverParaAbajo ? (ordenSiguiente - ordenActual) : -(ordenActual - ordenanerior)) / 2 * 10000) / 10000;
        console.log(ordenActual + "para abajo?" + moverParaAbajo + "? " + (ordenSiguiente - ordenActual) / 2 + " : " + -(ordenActual - ordenanerior) / 2 + "= " + ordenNuevo);


        console.log("Base2:" + this.embarquesPuerto[posicionActual].embarque.nombreBuque + " " + this.embarquesPuerto[posicionActual].lineUp.orden + "=>" + ordenNuevo);

        this.instanciaWorkflow.lineUp.orden = ordenNuevo;
      }
    }

    this.lineUpService.modificarLineUp(this.instanciaWorkflow.lineUp).subscribe(x => { if (this.observador) this.observador.Actualizar(); });
  }

  /**ubicacion == 2 --> Muelle de Carga**/
  BarcoEnMuelleActualmente(listado: InstanciaWorkflowPuerto[]): boolean {
    if (!this.instanciaWorkflow.embarque.vicentin && !this.instanciaWorkflow.embarque.otrosMuelles && !this.instanciaWorkflow.embarque.noryon)
      return listado.find(x => x.embarque.ubicacion == 2 && x.embarque.id != this.instanciaWorkflow.embarque.id
        && !x.embarque.vicentin && !x.embarque.otrosMuelles && !x.embarque.noryon) != null;

    else if (this.instanciaWorkflow.embarque.vicentin)
      return listado.find(x => x.embarque.ubicacion == 2 && x.embarque.id != this.instanciaWorkflow.embarque.id
        && x.embarque.vicentin) != null;
    else if (this.instanciaWorkflow.embarque.noryon)
      return listado.find(x => x.embarque.ubicacion == 2 && x.embarque.id != this.instanciaWorkflow.embarque.id
        && x.embarque.noryon) != null;
    else
      return listado.find(x => x.embarque.ubicacion == 2 && x.embarque.id != this.instanciaWorkflow.embarque.id
        && x.embarque.otrosMuelles) != null;
  }

  public guardarFechaCarta() {
    if (this.user.permisos.find(p => p === this.permisosScato.LineUp_EditarChecks)) {

      console.log(this.fechaCarta + ' ' + this.horaCarta)
      if (this.fechaCarta && this.horaCarta) {
        this.instanciaWorkflow.lineUp.cartaDeSubidaAprobada = this.fechaCarta + ' ' + this.horaCarta;
        this.lineUpService.modificarLineUp(this.instanciaWorkflow.lineUp).subscribe(
          ret => console.log(ret));
      }
    }

  }

  nombreUbicacionDeBuquePuerto(numero): string {
    return numero > 0 && numero != null && this.ubicacionDeBuquePuerto != null ? this.ubicacionDeBuquePuerto.find(x => x.id == numero).nombre.toString() : '';
  }

  numeroUbicacionDeBuquePuerto(nombre): number {
    return nombre.length > 0 && nombre != null && this.ubicacionDeBuquePuerto != null ? this.ubicacionDeBuquePuerto.find(x => x.nombre.toLowerCase().trim() == nombre.toLowerCase().trim()).id : 0;
  }

  extraeNombre(objeto): string {
    return objeto != null ? objeto.nombre.toString() : '';
  }

  get filteredMaterialList(): MaterialPuertoCantidad[] { return this.instanciaWorkflow.embarque.materialesPuertoCantidad.filter(x => x.cantidad > 0); }

  colorDelMuelle() {
    return this.instanciaWorkflow.embarque.sanBenito ?
      'color-muelle-sanbenito' : this.instanciaWorkflow.embarque.vicentin ?
        'color-muelle-vicentin' : this.instanciaWorkflow.embarque.noryon ?
          'color-muelle-nouryon' : 'color-muelle-otros-muelles'
  }

  public get width() {
    return window.innerWidth;
  }

  showWarning() {
    this.messageService.add({ severity: 'error', summary: 'Acceso Denegado', detail: 'No posee permisos para la acción', key: 'access-lineup' });
  }

  hasPermisoRadios() {
    return this.user.permisos.find(p => p === this.permisosScato.LineUp_EditarChecks);
  }

  hasPermisoDeleteEmbarque() {
    return this.user.permisos.find(p => p === this.permisosScato.PreLineUp_EliminarBuque);
  }

  hasPermisoEditEmbarque() {
    return this.user.permisos.find(p => p === this.permisosScato.PreLineUp_EditarBuque);
  }
}
