import { ChangeDetectorRef, Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { forkJoin } from 'rxjs';
import { GraficoCargaComponent } from './operaciones/grafico-carga/grafico-carga.component';
import { AutenticadorService } from '@ScatoServicios/autenticador.service';
import { SentidoManoDeEmbarque } from '@ScatoModels/sentido-mano-embarque';
import { CeldaManoDeEmbarque } from '@ScatoModels/celda-mano-embarque';
import { ModuloDeCarga } from '@ScatoModels/modulo-carga';
import { Mail } from '@ScatoModels/mail';
import { Embarque } from '@ScatoModels/embarque';
import { MaterialPuerto } from '@ScatoModels/material-puerto';
import { Alerta } from '@ScatoModels/alerta';
import { Tipoalerta } from '@ScatoEnums/tipo-alerta';
import jspdf from 'jspdf';
import html2canvas from 'html2canvas';
import { EmbarqueNav } from '@ScatoModels/embarque-nav';
import { AlertService } from '@ScatoServicios/alert.service';
import { ModuloDeCargaService } from '@ScatoServicios/modulo-de-carga.service';
import { ConfirmationDialogService } from '@ScatoServicios/confirmation-dialog.service';
import { EmbarqueService } from '@ScatoServicios/embarque.service';
import { PlanoDeCargaService } from '@ScatoServicios/plano-de-carga.service';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { ProcesoGuardarService } from '@ScatoServicios/procesoGuardar.service';
import { ManosComponent } from './operaciones/manos/manos.component';
import { SessionService } from '@ScatoServicios/session.service';
import { Usuario } from '@ScatoInterfaces/usuario';
// import { Balanza78 } from '@ScatoModels/balanzadas/balanza78';
// import { BalanzasComponent } from './tableristas/balanzas/balanzas.component';

@Component({
  selector: 'app-carga-solidos',
  templateUrl: './carga-solidos.component.html',
  styleUrls: ['./carga-solidos.component.css']
})
export class CargaSolidosComponent implements OnInit {
  @Output() hideSpinner = new EventEmitter<boolean>();
  @Output() guardarPlano = new EventEmitter<boolean>();
  @ViewChild(GraficoCargaComponent) graficoCarga: GraficoCargaComponent;
  @ViewChild(ManosComponent) manosComponent: ManosComponent;
  // @ViewChild(BalanzasComponent) balanzasComponent: BalanzasComponent;
  embarqueSelected: EmbarqueNav;
  sentidosManoDeEmbarque: SentidoManoDeEmbarque[];
  celdasManoDeEmbarque: CeldaManoDeEmbarque[];
  enviado: boolean;
  usuarioFinalizacion: string;
  embarque: Embarque;
  materialesPuerto: MaterialPuerto[];
  adjunto: any;
  cargaPdf: boolean = false;

  private user: Usuario

  // balanzasEmbarque: Balanza78[];

  constructor(
    private moduloCargaService: ModuloDeCargaService,
    private confirmationDialogService: ConfirmationDialogService,
    private session: SessionService,
    private embarqueService: EmbarqueService,
    private planoDeCargaService: PlanoDeCargaService,
    private alertService: AlertService,
    private _procesoService: DatosEmbarquesProcesoService,
    private _changeDetector: ChangeDetectorRef,
    private _procesoGuardar: ProcesoGuardarService
  ) {
    this.user = this.session.getUser();
   }


  ngOnInit(): void {
    this._procesoService.sendEmbarque.subscribe(
      res => {
        this.embarqueSelected = res;
      }
    )
    if (!this.embarqueSelected)
      this.embarqueSelected = this._procesoService.getEmbarqueSelected();

    this.embarqueService.obtenerEmbarque(this.embarqueSelected.id).subscribe(
      res => {
        this.embarque = res;
        this.materialesPuerto = res.materialesPuertoCantidad.map(m => ({
          id: m.materialId,
          descripcionCorta: m.descripcionCorta,
          descripcion: '',
          almacenDesc: '',
          almacenId: 0,
          codigoSAP: '',
          esLiquido: m.esLiquido,
          color: m.color
        }));
      });
    this.drawGraphic();
  }

  drawGraphic() {
    forkJoin([
      this.moduloCargaService.obtenerListadoSentidoManoDeEmbarque(),
      this.moduloCargaService.obtenerListadoCeldaManoDeEmbarque()
    ]).subscribe(([res1, res2]) => {
      this.sentidosManoDeEmbarque = res1;
      this.celdasManoDeEmbarque = res2;
      this._changeDetector.detectChanges();
      if (this.embarqueSelected.moduloDeCargaId) this.cargarModuloCarga();
    });

    this.hideSpinner.emit(false);
  }

  agregarTabique(tabique, entreColumna, yColumna) {
    this.graficoCarga.agregarTabique(tabique, entreColumna, yColumna);
  }

  agregarManoDeEmbarque(item) {
    let { celda, sentido } = item;
    this.graficoCarga.agregarManoDeEmbarque(celda, sentido);
  }

  cargarModuloCarga() {
    this.moduloCargaService.obtenerModuloDeCarga(this.embarqueSelected.moduloDeCargaId)
      .subscribe(res => {
        this.enviado = res.enviado;
        this.usuarioFinalizacion = res.usuarioFinalizacion;
        this.graficoCarga.limpiarGraficoCarga();
        this.manosComponent.resetForm();
        if (res.moduloDeCargaElementoGrafico) {
          this.graficoCarga.agregarElementosGraficos(res.moduloDeCargaElementoGrafico);
        }
        if (res.moduloDeCargaManosDeEmbarque.length > 0) {
          this.manosComponent.patchManosDeEmbarque(res.moduloDeCargaManosDeEmbarque);
          
          // console.log('res.moduloDeCargaManosDeEmbarque: ', res.moduloDeCargaManosDeEmbarque);
          
        }
        if (res.moduloDeCargaManosDeEmbarque.length > 0) {
          this.manosComponent.patchTabiques(res.moduloDeCargaTabiquesDeEmbarque);
        }
      });
  }

  imprimir(imprimir: boolean = false) {
    this.cargaPdf = true;
    let doc: jspdf = new jspdf('l', 'mm', 'a4', true);

    let textareas: HTMLCollection = document.getElementsByClassName('replaceToDiv');
    while (textareas.length) {
      let div = document.createElement('div');
      div.setAttribute("contenteditable", "true");
      let text = document.createTextNode((<HTMLInputElement>textareas[0]).value);
      div.appendChild(text);
      textareas[0].replaceWith(div);
    }


    html2canvas(document.getElementById('graficoCargaCanva'), { backgroundColor: '#fff' }).then((canvas) => {
      canvas.style.backgroundColor = 'white';
      let img = canvas.toDataURL('image/jpg');
      doc.addImage(img, 'JPG', 15, 15, 260, 160);
      html2canvas(document.getElementById('manosDeEmbarque'), { backgroundColor: '#fff' }).then((canvas2) => {
        doc.addPage('a4', 'l');
        canvas.style.backgroundColor = 'white';
        let img2 = canvas2.toDataURL('image/jpg');
        doc.addImage(img2, 'JPG', 15, 15, 270, 130);

        if (!imprimir) {
          this.cargaPdf = false;
          doc.output('pdfobjectnewwindow');
        } else {
          let file = doc.output('blob');
          this.cargarPDF(file);
        }
      })
    })
  }

  cargarPDF(file) {
    if (file) {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        this.adjunto = reader.result;
        this.enviarMail();
      }
    }
  }

  guardar(finalizar: boolean) {
    // SI LA CARGA YA ESTABA FINALIZADA, Y LE DA GUARDAR, AVISA QUE SE REALIZARON
    // CAMBIOS, POR LO QUE DEBERÍA DARLE FINALIZAR PARA QUE ENVIE EL MAIL
    if (this.enviado && !finalizar) {
      var texto = "Se ha modificado con éxito la carga. Si desea informar los cambios, haga click en FINALIZAR.";
      this.confirmationDialogService.confirm('¡Atención!', texto, 'Cerrar', '', null, null, Tipoalerta.Success)
        .then((confirmed) => {
          if (confirmed)
            this.guardarContinuacion(finalizar);
          else
            return;
        }).catch(() => window.location.reload());
    }
    else
      this.guardarContinuacion(finalizar);
  }

  guardarContinuacion(finalizar: boolean) {
    if (!this.enviado)
      this.enviado = finalizar;

    if (finalizar)
      this.usuarioFinalizacion = this.user.username;
    else
      this.usuarioFinalizacion = null;

    let elementosGraficos = this.graficoCarga.obtenerElementosGraficos();

    // let balanza7 = this.balanzasComponent.obtenerBalanzas7();
    // let balanza8 = this.balanzasComponent.obtenerBalanzas8();
    // this.balanzasEmbarque = balanza7.concat(balanza8);
    // console.log('balanzasEmbarque: ', this.balanzasEmbarque);

    console.log('obtenerManosDeEmbarque(): ', this.manosComponent.obtenerManosDeEmbarque());
    

    // let moduloCarga = new ModuloDeCarga(this.embarqueSelected.moduloDeCargaId, this.enviado, this.usuarioFinalizacion, elementosGraficos,
    //   this.manosComponent.obtenerManosDeEmbarque(), this.manosComponent.obtenerTabiques(), null, null, this.balanzasEmbarque );
    let moduloCarga = new ModuloDeCarga(this.embarqueSelected.moduloDeCargaId, this.enviado, this.usuarioFinalizacion, elementosGraficos,
      this.manosComponent.obtenerManosDeEmbarque(), this.manosComponent.obtenerTabiques());
    this.moduloCargaService.guardarModuloDeCarga(moduloCarga).subscribe(res => {
      if (finalizar)
      this.confirmationDialogService.confirm('¡Felicitaciones!', 'Ha cargado con éxito el modulo de Carga', 'Cerrar', '', null, null, Tipoalerta.Success)
      .then(() => {this.imprimir(finalizar)},
        error => {
          this.confirmationDialogService.confirm('¡Error!', 'Error al crear el modulo de carga: ' + <any>error.error, 'Cerrar', '', null, null, Tipoalerta.Error);
        }).catch(() => window.location.reload())
      else {
        this.confirmationDialogService.confirm('¡Felicitaciones!', 'Ha cargado con éxito el modulo de Carga', 'Cerrar', '', null, null, Tipoalerta.Success)
          .then(() => {},
            error => {
              this.confirmationDialogService.confirm('¡Error!', 'Error al crear el modulo de carga: ' + <any>error.error, 'Cerrar', '', null, null, Tipoalerta.Error);
            }).catch(() => window.location.reload())
      }

      this._procesoGuardar.sendGuardar.emit([finalizar, true]);
      this.cargaPdf = true;
    });
  }

  enviarMail() {
    var titulo = "Enviar carga por mail";
    var text = "Cuerpo del mail:";
    var inputTitle = "Destinatarios";
    var mail = new Mail(`${this.embarque.nombreBuque}. ${this.embarque.materialesPuertoCantidad[0].descripcionCorta}. Muelle: San Benito. Plano de carga, nominación, adjunto comunicación previa y gráfico de celdas.`);
    mail.adjunto = this.adjunto.split("base64,")[1];
    mail.nombre = "GráficoDeCeldas.pdf"
    this.planoDeCargaService.obtenerBodyPlanoDeCarga(this.embarqueSelected.planoDeCargaId, this.embarque).subscribe(x => { mail.body = x });
    this.planoDeCargaService.obtenerDestinatariosPlanoDeCarga().subscribe(x => mail.destinatarios = x);
    var button1 = 'Enviar';
    var button2 = 'Cancelar';
    this.cargaPdf = false;
    this.confirmationDialogService.confirm(titulo, text, button1, button2, 'xl', mail, null, inputTitle, true)
      .then((confirmed) => {
        if (confirmed) {
          this.hideSpinner.emit(true);
          this.planoDeCargaService.enviarPorMail(mail, this.embarqueSelected.planoDeCargaId).subscribe(
            data => {
              this.confirmationDialogService.confirm('¡Felicitaciones!', 'Ha enviado con éxito el mail con la carga', 'Cerrar', '', null, null, Tipoalerta.Success)
                .then((confirmed) => {
                  if (confirmed){
                    this.hideSpinner.emit(false)
                    return
                  }
                }).catch(() => window.location.reload());
            }, error => {
              this.alertService.mostrar(new Alerta(<any>error.error, Tipoalerta.Error));
              this.hideSpinner.emit(false);
            })
        }
        else
          this.hideSpinner.emit(false);
      })
      .catch(() => {
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)');
        this.hideSpinner.emit(false);
      });
  }
}
