import { DatePipe, formatDate } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PermisosScato } from '@ScatoEnums/permisos-scato';
import { Tipoalerta } from '@ScatoEnums/tipo-alerta';
import { Observador } from '@ScatoInterfaces/observador';
import { Usuario } from '@ScatoInterfaces/usuario';
import { Alerta } from '@ScatoModels/alerta';
import { EstadoPuerto } from '@ScatoModels/estado-puerto';
import { InstanciaWorkflowPuerto } from '@ScatoModels/instancia-wokflow-puerto';
import { Mail } from '@ScatoModels/mail';
import { UbicacionDeBuquePuerto } from '@ScatoModels/ubicacion-de-buque-puerto';
import { AlertService } from '@ScatoServicios/alert.service';
import { ConfirmationDialogService } from '@ScatoServicios/confirmation-dialog.service';
import { EmbarqueService } from '@ScatoServicios/embarque.service';
import { LineupService } from '@ScatoServicios/lineup.service';
import { SessionService } from '@ScatoServicios/session.service';
import { WorkflowService } from '@ScatoServicios/workflow.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-lineup',
  templateUrl: './lineup.component.html',
  styleUrls: ['./lineup.component.css'],
  providers: [DatePipe]
})
export class LineupComponent implements OnInit, Observador {
  mostrarSpinner: boolean = true;
  mostrarContent: boolean = false;
  listadoEmbarques: InstanciaWorkflowPuerto[];
  sanBenito: InstanciaWorkflowPuerto[];
  sanBenitoCargandoMuelle: InstanciaWorkflowPuerto;
  noryon: InstanciaWorkflowPuerto[];
  vicentin: InstanciaWorkflowPuerto[];
  otrosMuelles: InstanciaWorkflowPuerto[];
  fechaActualizacion: Date;
  mostrarCalendario: boolean = false;
  estadoPuerto: EstadoPuerto;
  isLoading = false;
  permisosScato: typeof PermisosScato = PermisosScato;
  observador: Observador = this;
  estadoCalado: string;
  estadoUbicacion: string;
  estadoAltura: string;
  ubicacionDeBuquePuerto: UbicacionDeBuquePuerto[];
  private user: Usuario;
  constructor(
    private workflowService: WorkflowService,
    private alertService: AlertService,
    private router: Router,
    private lineupService: LineupService,
    private confirmationDialogService: ConfirmationDialogService,
    private datepipe: DatePipe,
    private embarqueService: EmbarqueService,
    private _messageService: MessageService,
    private session: SessionService
  ) {
    this.user = this.session.getUser()
    this.sanBenito = new Array();
    this.noryon = new Array();
    this.vicentin = new Array();
    this.otrosMuelles = new Array();
    this.embarqueService.obtenerListadoUbicacionDeBuquePuerto().subscribe(res => this.ubicacionDeBuquePuerto = res);
  }

  ngOnInit(): void {
    // servicio InstanciaWorkflowPuerto
    this.cargarWorkflows();
  }

  Actualizar(subject?: any) {
    this.cargarWorkflows(true);
  }

  ListarEmbarques(): InstanciaWorkflowPuerto[] {
    return this.listadoEmbarques;
  }

  private cargarWorkflows(blockUI: boolean = false) {
    this.workflowService.obtenerListado()
      .subscribe(
        ret => {
          this.listadoEmbarques = ret;
          this.fechaActualizacion = new Date();
          if (!blockUI) {
            setTimeout(x => this.cargarWorkflows(), 120000);
          }
          this.filtrarMuelles();
          this.mostrarContent = true;
          this.mostrarSpinner = false;
        },
        errmess => this.alertService.mostrar(new Alerta(<any>errmess.error, Tipoalerta.Error)));
  }

  filtrarMuelles() {
    this.sanBenito = this.listadoEmbarques ? this.listadoEmbarques.filter(i => i.embarque.sanBenito || (!i.embarque.vicentin && !i.embarque.otrosMuelles && !i.embarque.noryon)) : new Array();
    this.sanBenitoCargandoMuelle = this.sanBenito.find(m => m.embarque?.estadoBuque?.descripcion == 'PostOperativo');
    this.noryon = this.listadoEmbarques ? this.listadoEmbarques.filter(i => i.embarque.noryon) : new Array();
    this.vicentin = this.listadoEmbarques ? this.listadoEmbarques.filter(i => i.embarque.vicentin) : new Array();
    this.otrosMuelles = this.listadoEmbarques ? this.listadoEmbarques.filter(i => i.embarque.otrosMuelles) : new Array();
  }

  public altaEmbarque() {
    if (this.user.permisos.find(p => p === this.permisosScato.PreLineUp_CrearBuque)) {
      localStorage.removeItem('embarque');
      this.router.navigate(['/lineup/alta-embarque/0/line-up']);
    }else{
      this._messageService.add({ severity: 'error', summary: 'Acceso Denegado', detail: 'No posee permisos para la acción', key: 'access-lineup'});
    }
  }

  public exportarEmbarques() {
    this.mostrarSpinner = true;
    this.lineupService.exportarEmbarques().subscribe(
      data => {
        const element = document.createElement('a');
        element.href = URL.createObjectURL(data);
        element.download = "Line Up " + formatDate(new Date(), 'yyyy-MM-dd', 'en') + '.xls';
        document.body.appendChild(element);
        element.click();
        this.mostrarSpinner = false;
      }, error => this.alertService.mostrar(new Alerta(<any>error.error, Tipoalerta.Error)));
  }

  public enviarPorMail() {
    var titulo = "Enviar Line Up por mail";
    var text = "Cuerpo del mail:";
    var mail = new Mail("", this.generarBodyModal());
    this.lineupService.obtenerDestinatariosLineUp().subscribe(x => mail.destinatarios = x);
    var button1 = 'Enviar';
    var button2 = 'Cancelar';
    this.confirmationDialogService.confirm(titulo, text, button1, button2, 'xl', mail, null, null, true)
      .then((confirmed) => {

        if (confirmed) {
          this.mostrarSpinner = true;
          this.lineupService.enviarPorMail(mail).subscribe(
            data => {
              this.confirmationDialogService.confirm('¡Felicitaciones!', 'Ha enviado con éxito el excel de lineup por mail', 'Cerrar', '');
              this.mostrarSpinner = false;
            }, error => {
              this.alertService.mostrar(new Alerta(<any>error.error, Tipoalerta.Error));
              this.mostrarSpinner = false;
            })
        }
      })
      .catch(() => console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)'));
  }

  // CARACTERES NO IMPRIMIBLES:
  // Enter: (\n -> <br/>)
  // Tabulador: (\t -> &nbsp;&nbsp;&nbsp;&nbsp;)
  // Negrita: (\f -> <b>) (\f\f -> </b>)
  // Subrayado: (\0 -> <u>) (\0\0 -> </u>)
  private generarBodyModal(): string {
    var fecha = this.datepipe.transform(new Date(), 'dd-MM-yyyy');
    var body = `Adjunto encontrara el archivo de line up generado por el sistema Scato Puerto, creado el dia: ${fecha} por el usuario ${this.user.username}.\n\n`;
    var sanBenito = this.sanBenito;
    if (sanBenito.length > 0) {
      body += `\f\0- San Benito:\0\0\f\f\n`;
      sanBenito.slice(0, 3).forEach((x, index) => {
        body += `\t\f${index + 1}. ${x.embarque.nombreBuque}\f\f - ${x.embarque.materialesPuertoCantidad.map(e => `${e.cantidad.toLocaleString('es-ar')} ${e.descripcionCorta}`).join(",")} - 
      \t\t${x.embarque.observaciones != null ? x.embarque.observaciones.length > 0 ? "Observaciones: " + x.embarque.observaciones + "\n" : "" : ""}`;
      });
    }

    var vicentin = this.vicentin;
    if (vicentin.length > 0) {
      body += `\n\f\0- Vicentin:\0\0\f\f\n`;
      vicentin.slice(0, 3).forEach((x, index) => {
        body += `\t\f${index + 1}. ${x.embarque.nombreBuque}\f\f - ${x.embarque.materialesPuertoCantidad.map(e => `${e.cantidad.toLocaleString('es-ar')} ${e.descripcionCorta}`).join(",")} - 
      \t\t${x.embarque.observaciones != null ? x.embarque.observaciones.length > 0 ? "Observaciones: " + x.embarque.observaciones + "\n" : "" : ""}`;
      });
    }

    var noryon = this.noryon;
    if (noryon.length > 0) {
      body += `\n\f\0- Nouryon:\0\0\f\f\n`;
      noryon.slice(0, 3).forEach((x, index) => {
        body += `\t\f${index + 1}. ${x.embarque.nombreBuque}\f\f - ${x.embarque.materialesPuertoCantidad.map(e => `${e.cantidad.toLocaleString('es-ar')} ${e.descripcionCorta}`).join(",")} - 
      \t\t${x.embarque.observaciones != null ? x.embarque.observaciones.length > 0 ? "Observaciones: " + x.embarque.observaciones + "\n" : "" : ""}`;
      });
    }

    var otrosM = this.otrosMuelles;
    if (otrosM.length > 0) {
      body += `\n\f\0- Otros Muelles:\0\0\f\f\n`;
      otrosM.slice(0, 3).forEach((x, index) => {
        body += `\t\f${index + 1}. ${x.embarque.nombreBuque}\f\f - ${x.embarque.materialesPuertoCantidad.map(e => `${e.cantidad.toLocaleString('es-ar')} ${e.descripcionCorta}`).join(",")} - 
      \t\t${x.embarque.observaciones != null ? x.embarque.observaciones.length > 0 ? "Observaciones: " + x.embarque.observaciones + "\n" : "" : ""}`;
      });
    }

    return body;
  }


  public cambiarVista() {
    this.mostrarCalendario = !this.mostrarCalendario;
  }

  estadoSanBenito() {
    let ubicacion = this.ubicacionDeBuquePuerto ? this.ubicacionDeBuquePuerto.find(x => x.orden == 2).id : '';
    return this.sanBenito && this.sanBenito.find(m => m.embarque.ubicacion == ubicacion) ? 'Operando' : 'No Operando';
  }

  estadoVicentin() {
    let ubicacion = this.ubicacionDeBuquePuerto.find(x => x.orden == 2).id;
    return this.vicentin && this.vicentin.find(m => m.embarque.ubicacion == ubicacion) ? 'Operando' : 'No Operando';
  }

  estadoNoryon() {
    let ubicacion = this.ubicacionDeBuquePuerto.find(x => x.orden == 2).id;
    return this.noryon && this.noryon.find(m => m.embarque.ubicacion == ubicacion) ? 'Operando' : 'No Operando';
  }

  estadoOtros() {
    let ubicacion = this.ubicacionDeBuquePuerto.find(x => x.orden == 2).id;
    return this.otrosMuelles && this.otrosMuelles.find(m => m.embarque.ubicacion == ubicacion) ? 'Operando' : 'No Operando';
  }

  showSpinner(event) {
    this.mostrarSpinner = event;
  }

  goVicentin() {
    if (document.getElementById('muelleVicentin'))
      document.getElementById('muelleVicentin').scrollIntoView()
  }

  goNouryon() {
    if (document.getElementById('muelleNouryon'))
      document.getElementById('muelleNouryon').scrollIntoView()
  }

  goOtrosMuelles() {
    if (document.getElementById('muelleOtrosMuelles'))
      document.getElementById('muelleOtrosMuelles').scrollIntoView()
  }

  hasPermisoAltaEmbarque(){
    return this.user.permisos.find(p => p === this.permisosScato.PreLineUp_CrearBuque);
  }

  hasPermisoMail(){
    return this.user.permisos.find(p => p === this.permisosScato.LineUp_EnviarMail);
  }

  hasPermisoExcel(){
    return this.user.permisos.find(p => p === this.permisosScato.LineUp_Exportar);
  }
}
