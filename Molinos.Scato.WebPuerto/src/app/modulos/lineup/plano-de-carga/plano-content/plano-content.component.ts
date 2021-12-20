import { Component, OnInit, TemplateRef, ViewChild, Output, EventEmitter } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Tipoalerta } from '@ScatoEnums/tipo-alerta';
import { AgenciaControlPrivado } from '@ScatoModels/agencia-control-privado';
import { AgenteControlPrivado } from '@ScatoModels/agente-control-privado';
import { Alerta } from '@ScatoModels/alerta';
import { Destino } from '@ScatoModels/destino';
import { Embarque } from '@ScatoModels/embarque';
import { EmbarqueNav } from '@ScatoModels/embarque-nav';
import { EstadoPuerto } from '@ScatoModels/estado-puerto';
import { Estiba } from '@ScatoModels/estiba';
import { Exportador } from '@ScatoModels/exportador';
import { Mail } from '@ScatoModels/mail';
import { MaterialPuerto } from '@ScatoModels/material-puerto';
import { Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AlertService } from '@ScatoServicios/alert.service';
import { LineupService } from '@ScatoServicios/lineup.service';
import { EmbarqueService } from '@ScatoServicios/embarque.service';
import { PlanoDeCargaService } from '@ScatoServicios/plano-de-carga.service';
import { ConfirmationDialogService } from '@ScatoServicios/confirmation-dialog.service';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { ProcesoGuardarService } from '@ScatoServicios/procesoGuardar.service';
import { TurnosService } from '@ScatoServicios/turnos.service';
import { SessionService } from '@ScatoServicios/session.service';
import { Usuario } from '@ScatoInterfaces/usuario';

@Component({
  selector: 'app-plano-content',
  templateUrl: './plano-content.component.html',
  styleUrls: ['./plano-content.component.css']
})
export class PlanoContentComponent implements OnInit {
  estadoAlturaValor: number;
  embarqueSelected: EmbarqueNav;
  @Output() showCargas = new EventEmitter<boolean>();
  @Output() hideSpinner = new EventEmitter<boolean>();
  @ViewChild('modalEditarAgenteControlPrivado') modalEditarAgenteControlPrivado: TemplateRef<any>;
  @ViewChild('modalABM') modalABM: TemplateRef<any>;
  mostrarContent: boolean = false;
  estadoPuerto: EstadoPuerto;
  embarque: Embarque;
  recomendacionDefensas: string = '';
  planoDeCargaForm: FormGroup;
  materialesPuerto: MaterialPuerto[];
  destinos: Destino[];
  exportadores: Exportador[];
  estibasList: Estiba[];
  agenciasControlPrivadoList: AgenciaControlPrivado[];
  agentesControlPrivadoList: AgenteControlPrivado[];
  agenteSeleccionado: number;
  pantallaSeleccionada: string;
  opcionABMSeleccionada: string;
  tituloABM: string;
  listadoExportadoresModificado: boolean = false;

  esLiquido: boolean = false;

  filePlano: string | ArrayBuffer;
  fileNamePlano: string = 'Ningun archivo elegido';
  fileSecuencia: string | ArrayBuffer;
  fileNameSecuencia: string = 'Ningun archivo elegido';
  mostrarFumigadora: boolean = false;
  datosGrafico: any;

  state: string;

  private user: Usuario

  constructor(
    private lineupService: LineupService,
    private embarqueService: EmbarqueService,
    private planoDeCargaService: PlanoDeCargaService,
    private router: Router,
    private formBuilder: FormBuilder,
    private confirmationDialogService: ConfirmationDialogService,
    private modalService: NgbModal,
    private alertService: AlertService,
    private _procesoService: DatosEmbarquesProcesoService,
    private _guardarService: ProcesoGuardarService,
    private _turnoService: TurnosService,
    private session: SessionService
  ) {
    this.user = this.session.getUser();
    this._guardarService.sendGuardar.subscribe(
      res => this.guardarPlanoDeCargaContinuacion(res[0], res[1])
    )
    this._procesoService.sendEstadoAltura.subscribe(
      res => {
        this.estadoAlturaValor = res;
        this.calcularRecomendacionDefensas();
      }
    )
    this.state = this.router.url.replace('/', '');
  }

  ngOnInit(): void {
    this.getEmbarqueData();
  }

  getEmbarqueData() {
    this._procesoService.sendEmbarque.subscribe(res => {
      this.embarqueSelected = res;
      this.inicializarFormulario();
    });
    if (!this.embarqueSelected) {
      this.embarqueSelected = this._procesoService.getEmbarqueSelected();
    }
    this.inicializarFormulario();
  }

  inicializarFormulario() {
    this.planoDeCargaForm = this.formBuilder.group({
      id: [this.embarqueSelected.planoDeCargaId],
      defensasMoviles: [],
      observaciones: [],
      caladoSalida: [],
      estiba: [],
      estibasList: [],
      agenciaControlPrivado: [],
      agenciasControlPrivadoList: [],
      agentesControlPrivadoSeleccionado: [],
      agentesControlPrivadoList: [],
      planoDeCargaBodegas: this.formBuilder.array([]),
      cargasComerciales: this.formBuilder.array([]),
      enviado: [false],
      fumigacion: [],
      empresaFumigadora: [],
      FilePathPlano: [],
      planoDeCargaArchivoNombre: [],
      FilePathSecuencia: [],
      planoDeCargaArchivoSecuenciaNombre: [],
      usuarioFinalizacion: [],
    });
    this.cargarEmbarque();
  }

  obtenerPlanoDeCarga() {
    this.planoDeCargaService.obtenerPlanoDeCarga(this.embarqueSelected.planoDeCargaId).subscribe(res => {
      this.planoDeCargaForm.patchValue({
        ...res, planoDeCargaBodegas: this.planoDeCargaForm.get('planoDeCargaBodegas').value
      });
      this._turnoService.setExportadores(this.planoDeCargaForm.controls.cargasComerciales.value);
      if (res.planoDeCargaBodegas.length > 0) {
        for (let index = 0; index < 9; index++) {
          var bodega = res.planoDeCargaBodegas.find(x => x.bodegaParcel == index + 1);
          if (bodega != null) {
            this.planoDeCargaBodegasFormArray.at(index).setValue(bodega);
            this.onChangeCondicion(bodega.condicion, index);
            if (bodega.materialPuerto != null)
              this.planoDeCargaBodegasFormArray.at(index).get('materialPuerto').setValue(
                this.materialesPuerto.find(x => x.id == bodega.materialPuerto.id)
              );
            if (bodega.destino != null)
              this.planoDeCargaBodegasFormArray.at(index).get('destino').setValue(
                this.destinos.find(x => x.id == bodega.destino.id)
              );
            let bodegas = this.planoDeCargaForm.controls.planoDeCargaBodegas.value.filter(b => b.cantidad > 0 || b.condicion || b.destino || b.materialPuerto || b.tanqueDeAbordo);
            this._turnoService.setBodega(bodegas);
          }
        }
      }

      this.planoDeCargaService.obtenerListadoEstibas().subscribe(res1 => {
        res.estiba = res1.map(x => new Estiba(x.id, x.nombre, x.apellido));
      });
      if (this.planoDeCargaForm.value['estiba'] != null) {
        this.planoDeCargaService.obtenerListadoEstibas().subscribe(res1 => {
          this.planoDeCargaForm.get('estibasList').setValue(
            res1.filter(x => x.id == this.planoDeCargaForm.value['estiba'].id).map(x => new Estiba(x.id, x.nombre, x.apellido)));
        });
      }

      this.planoDeCargaService.obtenerListadoAgenciasControlPrivado().subscribe(res1 => {
        res.agenciaControlPrivado = res1.map(x => new AgenciaControlPrivado(x.id, x.nombre));
      });
      if (this.planoDeCargaForm.value['agenciaControlPrivado'] != null) {
        this.planoDeCargaService.obtenerListadoAgenciasControlPrivado().subscribe(res1 => {
          this.planoDeCargaForm.get('agenciasControlPrivadoList').setValue(
            res1.filter(x => x.id == this.planoDeCargaForm.value['agenciaControlPrivado'].id).map(x => new AgenciaControlPrivado(x.id, x.nombre)));
        });
      }

      this.planoDeCargaForm.get('agentesControlPrivadoSeleccionado').setValue(res.agentesControlPrivado);

      this.fileNamePlano = res.planoDeCargaArchivoPlanoNombre != null ? res.planoDeCargaArchivoPlanoNombre : 'Ningun archivo elegido';
      this.fileNameSecuencia = res.planoDeCargaArchivoSecuenciaNombre != null ? res.planoDeCargaArchivoSecuenciaNombre : 'Ningun archivo elegido';
      this.filePlano = res.filePathPlano;
      this.fileSecuencia = res.filePathSecuencia;
      for (let index = 0; index < res.cargasComerciales.length; index++) {
        this.cargasComercialesFormArray.at(index).get('materialPuerto').setValue(
          this.materialesPuerto.find(x => x.id == res.cargasComerciales[index].materialPuerto.id)
        );
      }
      this.mostrarFumigadora = res.fumigacion;

      this.lineupService.obtenerEstadoPuerto().subscribe(x => { this.estadoAlturaValor = Number(x.alturaDelRio); this.calcularRecomendacionDefensas(x); });
      this.mostrarContent = true;
      this.showCargas.emit(true);
    });
  }

  cargarEmbarque() {
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
        this.esLiquido = res.esLiquido;
        this.datosGrafico = {
          nombreBuque: res.nombreBuque,
          fecha: res.fechaRecalada,
          tnAprox: res.materialesPuertoCantidad.reduce((acc, el) => acc + el.cantidad, 0),
          productosACargar: res.materialesPuertoCantidad.reduce((acc, el, i) => i < res.materialesPuertoCantidad.length - 1 ? acc + el.descripcionCorta + ', ' : acc + el.descripcionCorta, ''),
          listaMateriales: res.materialesPuertoCantidad.map(x => ({ id: x.materialId, descripcionCorta: x.descripcionCorta, color: x.color })),
        }
        this._procesoService.setDatosGrafico(this.datosGrafico);
        this.getDestinos();
      }, () => {
        this.confirmationDialogService.confirm('¡Error!', `Error al obtener el embarque ${this.embarqueSelected.id}`, 'Cerrar', '', null, null, Tipoalerta.Error);
      });
  }

  getDestinos() {
    this.planoDeCargaService.obtenerDestinos().subscribe(
      res => {
        this.destinos = res;
        this.getExportadores();
      });
  }

  getExportadores() {
    this.planoDeCargaService.obtenerExportadores().subscribe(
      res => {
        this.exportadores = res;
        this.getListadoEstibas();
      });
  }

  getListadoEstibas() {
    this.planoDeCargaService.obtenerListadoEstibas().subscribe(
      res => {
        this.estibasList = res.map(x => new Estiba(x.id, x.nombre, x.apellido));
        this.getListadoAgenciasControlPrivado();
      });
  }

  getListadoAgenciasControlPrivado() {
    this.planoDeCargaService.obtenerListadoAgenciasControlPrivado().subscribe(res => {
      this.agenciasControlPrivadoList = res.map(x => new AgenciaControlPrivado(x.id, x.nombre));
      this.getListadoAgentesControlPrivado();
    });
  }

  getListadoAgentesControlPrivado() {
    this.planoDeCargaService.obtenerListadoAgentesControlPrivado().subscribe(res => {
      this.agentesControlPrivadoList = res.map(x => new AgenteControlPrivado(x.id, x.nombre, x.apellido));
      this.cargarCargasComerciales();
    });
  }

  cargarCargasComerciales() {
    this.cargasComercialesFormArray.clear();
    for (let index = 0; index < 4; index++) {
      this.cargasComercialesFormArray.push(this.formBuilder.group({
        id: [],
        exportador: [],
        nombre: [],
        materialPuerto: [],
        cantidad: [],
      }))
    };
    this.cargarPlanoDeCargaBodegas();
  }

  cargarPlanoDeCargaBodegas() {
    this.planoDeCargaBodegasFormArray.clear();
    for (let index = 0; index < 9; index++) {
      this.planoDeCargaBodegasFormArray.push(this.formBuilder.group({
        id: [],
        bodegaParcel: [index + 1],
        cantidad: [],
        materialPuerto: [],
        condicion: [""],
        sfFull: [],
        destino: [],
        tanqueDeAbordo: [],
      }))
    };
    this.obtenerPlanoDeCarga();
  }

  get planoDeCargaBodegasFormArray(): FormArray {
    return this.planoDeCargaForm.get("planoDeCargaBodegas") as FormArray
  }

  get cargasComercialesFormArray(): FormArray {
    return this.planoDeCargaForm.get("cargasComerciales") as FormArray
  }

  public trackByFn(index: any, item: any) {
    return index;
  }

  public numberOnly(event): boolean {
    var charCode = (event.which) ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57))
      return false;
    return true;
  }

  calcularRecomendacionDefensas(estadoPuerto?: any) {
    this.estadoPuerto = estadoPuerto;

    if (this.embarque.freeboard == 0 && this.estadoAlturaValor == 0) {
      this.recomendacionDefensas = "Altura y Freeboard desconocidos, no se pueden calcular las defensas móviles";
    } else if (this.embarque.freeboard == 0) {
      this.recomendacionDefensas = "Freeboard desconocido, no se pueden calcular las defensas móviles";
    } else if (this.estadoAlturaValor == 0) {
      this.recomendacionDefensas = "Altura desconocida, no se pueden calcular las defensas móviles";
    } else {
      var result = this.embarque.freeboard +
        (this.estadoAlturaValor ? this.estadoAlturaValor : Number(this.estadoPuerto.alturaDelRio)) - 1
      if (result > 5) {
        this.recomendacionDefensas = "No usar defensas móviles";
        this.planoDeCargaForm.get('defensasMoviles').setValue('No');
      }
      else {
        this.recomendacionDefensas = "Usar defensas móviles";
        this.planoDeCargaForm.get('defensasMoviles').setValue('Si');
      }
    }
  }

  public guardarPlanoDeCarga(finalizar: boolean) {
    if (this.planoDeCargaForm.invalid)
      return;
    //SI EL PLANO DE CARGA YA ESTABA FINALIZADO, Y LE DA GUARDAR, AVISA QUE SE REALIZARON
    //CAMBIOS, POR LO QUE DEBERÍA DARLE FINALIZAR PARA QUE ENVIE EL MAIL
    if (this.planoDeCargaForm.value.enviado && !finalizar) {
      var texto = "Se ha modificado con éxito el plano de carga. Si desea informar los cambios, haga click en FINALIZAR.";
      this.confirmationDialogService.confirm('¡Atención!', texto, 'Cerrar', '', null, null, Tipoalerta.Success)
        .then((confirmed) => {
          if (confirmed)
            this.guardarPlanoDeCargaContinuacion(finalizar);
          else
            return;
        }).catch(() => window.location.reload());
    }
    else
      this.guardarPlanoDeCargaContinuacion(finalizar);
  }

  public guardarPlanoDeCargaContinuacion(finalizar: boolean, moduloCarga: boolean = false) {
    this.hideSpinner.emit(true);
    this.planoDeCargaForm.value.estiba =
      this.planoDeCargaForm.value.estibasList != null && this.planoDeCargaForm.value.estibasList.length > 0 ?
        this.estibasList.find(x => x.id == this.planoDeCargaForm.value.estibasList[0].id) : '';

    this.planoDeCargaForm.value.agenciaControlPrivado =
      this.planoDeCargaForm.value.agenciasControlPrivadoList != null && this.planoDeCargaForm.value.agenciasControlPrivadoList.length > 0 ?
        this.agenciasControlPrivadoList.find(x => x.id == this.planoDeCargaForm.value.agenciasControlPrivadoList[0].id) : '';

    this.planoDeCargaForm.value.agentesControlPrivado =
      this.planoDeCargaForm.get('agentesControlPrivadoSeleccionado').value.length > 0 ?
        this.planoDeCargaForm.get('agentesControlPrivadoSeleccionado').value.map(x => new AgenteControlPrivado(x.id, x.nombre, x.apellido)) : '';

    if (!this.planoDeCargaForm.value.enviado)
      this.planoDeCargaForm.value.enviado = finalizar;

    if (finalizar)
      this.planoDeCargaForm.value.usuarioFinalizacion = this.user.username;
    else
      this.planoDeCargaForm.value.usuarioFinalizacion = null;

    this.planoDeCargaForm.value.filePathPlano = this.filePlano;
    this.planoDeCargaForm.value.planoDeCargaArchivoPlanoNombre = this.fileNamePlano;
    this.planoDeCargaForm.value.filePathSecuencia = this.fileSecuencia;
    this.planoDeCargaForm.value.planoDeCargaArchivoSecuenciaNombre = this.fileNameSecuencia;
    this.planoDeCargaForm.value.usuario = this.user.username;
    this.planoDeCargaForm.value.defensasMoviles = this.planoDeCargaForm.value.defensasMoviles || this.planoDeCargaForm.value.defensasMoviles === 'Si' ? true : false;
    console.log(this.planoDeCargaForm.value)
    try {
      this.planoDeCargaService.guardarPlanoDeCarga(this.planoDeCargaForm.value)
        .subscribe((res: any) => {
          if (!moduloCarga) {
            if (finalizar)
              this.confirmationDialogService.confirm('¡Felicitaciones!', 'Ha cargado con éxito el plano de carga', 'Cerrar', '', null, null, Tipoalerta.Success)
                .then(() => { this.enviarMail(); },
                  error => {
                    this.confirmationDialogService.confirm('¡Error!', 'Error al crear el plano de carga: ' + <any>error.error, 'Cerrar', '', null, null, Tipoalerta.Error);
                  }
                ).catch(() => window.location.reload())
            else {
              this.confirmationDialogService.confirm('¡Felicitaciones!', 'Ha cargado con éxito el plano de carga', 'Volver Line up', '', null, null, Tipoalerta.Success)
                .then((confirmed) => {
                  if (confirmed) {
                    this.router.navigate(['/lineup']);
                  }
                }).catch(() => window.location.reload())
            }
          }
          this.hideSpinner.emit(false);
        },
          errmess => {
            console.log(errmess.error)
            // this.confirmationDialogService.confirm('¡Error!', 'Error al crear el plano de carga: ' + <any>errmess.error, 'Cerrar', '', null, null, Tipoalerta.Error);
          });
    } catch (e) {
      console.log(e);
    }
  }

  enviarMail() {
    var titulo = "Enviar plano de carga por mail";
    var text = "Cuerpo del mail:";
    var inputTitle = "Destinatarios";
    var mail = new Mail(`${this.embarque.nombreBuque}. ${this.embarque.materialesPuertoCantidad[0].descripcionCorta}. Muelle: San Benito. Plano de carga, nominación y adjunto comunicación previa.`);
    this.planoDeCargaService.obtenerBodyPlanoDeCarga(this.embarqueSelected.planoDeCargaId, this.embarque).subscribe(x => { mail.body = x });
    this.planoDeCargaService.obtenerDestinatariosPlanoDeCarga().subscribe(x => mail.destinatarios = x);
    var button1 = 'Enviar';
    var button2 = 'Cancelar';
    this.confirmationDialogService.confirm(titulo, text, button1, button2, 'xl', mail, null, inputTitle, true)
      .then((confirmed) => {
        if (confirmed) {
          this.hideSpinner.emit(true);
          this.planoDeCargaService.enviarPorMail(mail, this.embarqueSelected.planoDeCargaId).subscribe(
            data => {
              this.confirmationDialogService.confirm('¡Felicitaciones!', 'Ha enviado con éxito el mail con el plano de carga', 'Volver Line up', '', null, null, Tipoalerta.Success)
                .then((confirmed) => {
                  if (confirmed)
                    this.router.navigate(['/lineup']);
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

  public formatterExportador = (exp: Exportador) => exp.nombre;

  public searchExportador = (text$: Observable<string>) => text$.pipe(
    debounceTime(200),
    distinctUntilChanged(),
    map(term => this.exportadores.filter(v => v.nombre.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
  )

  public onBlurExportador() {
    this.listadoExportadoresModificado = !this.planoDeCargaForm.value.exportador;
  }

  onFileChangePlano(event) {
    if (event.target.files && event.target.files.length) {
      var file = event.target.files[0];
      var fileReader = new FileReader();

      fileReader.onloadend = (e) => {
        this.filePlano = fileReader.result;
        this.fileNamePlano = file.name;
      }
      fileReader.readAsDataURL(file);
    }
  }

  onFileChangeSecuencia(event) {
    if (event.target.files && event.target.files.length) {
      var file = event.target.files[0];
      var fileReader = new FileReader();

      fileReader.onloadend = (e) => {
        this.fileSecuencia = fileReader.result;
        this.fileNameSecuencia = file.name;
      }
      fileReader.readAsDataURL(file);
    }
  }

  public decimalOnly(event): boolean {
    var charCode = (event.which) ? event.which : event.keyCode;
    if ((charCode > 47 && charCode < 58) || charCode == 46)
      return true;
    return false;
  }

  public onChangeCondicion(valor, i) {
    if (valor == "Full") {
      this.planoDeCargaBodegasFormArray.controls[i].get('sfFull').disable();
      this.planoDeCargaBodegasFormArray.controls[i].get('sfFull').setValue('');
    }
    else
      this.planoDeCargaBodegasFormArray.controls[i].get('sfFull').enable();
  }

  open(content) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' }).result.then((result) => {
    }, (reason) => {
    });
  }

  datosABMPuerto(campo): string {
    var list = this.pantallaSeleccionada == 'Estiba' ? 'estibasList' :
      this.pantallaSeleccionada == 'Agencia' ? 'agenciasControlPrivadoList'
        : 'agentesControlPrivadoList'
    var listDeSeleccionados = this.pantallaSeleccionada == 'Agente' ? 'agentesControlPrivadoSeleccionado' : '';

    if (this.planoDeCargaForm.get((this.pantallaSeleccionada == 'Agente' ?
      [listDeSeleccionados] : [list])).value.length > 0) {
      var numero;
      if (this.pantallaSeleccionada == 'Agente')
        numero = this.agenteSeleccionado;
      else
        numero = this.planoDeCargaForm.get([list]).value[0].id;

      if (campo == 1)
        return this[list] != null ? this[list].find(x => x.id == numero).nombre.toString() : '';
      else
        return this[list] != null ? this[list].find(x => x.id == numero).apellido.toString() : '';
    }
    else
      return '';
  }

  ABM(pantalla, opcion) {
    this.pantallaSeleccionada = pantalla;
    this.opcionABMSeleccionada = opcion;
    this.tituloABM =
      (this.opcionABMSeleccionada == 'Agregar' ? 'Agregar nuevo registro ' : 'Editar ') +
      (this.pantallaSeleccionada == 'Estiba' ? 'Estiba' :
        this.pantallaSeleccionada == 'Agencia' ? 'Agencia de Control Privado' : 'Encargado');
    return this.modalService.open(this.modalABM);
  }

  public descargarArchivo(tipo: string) {
    if (tipo == 'secuencia') {
      if (this.fileNameSecuencia == 'Ningun archivo elegido')
        return;
      var base64 = this.fileSecuencia;
      var imageName = this.fileNameSecuencia;
    }
    else {
      if (this.fileNamePlano == 'Ningun archivo elegido')
        return;
      var base64 = this.filePlano;
      var imageName = this.fileNamePlano;
    }
    const imageBlob = this.dataURItoBlob(base64);

    if (window.navigator.msSaveOrOpenBlob) {
      window.navigator.msSaveBlob(imageBlob, imageName);
    }
    else {
      var elem = window.document.createElement('a');
      elem.href = window.URL.createObjectURL(imageBlob);
      elem.download = imageName;
      document.body.appendChild(elem);
      elem.click();
      document.body.removeChild(elem);
    }
  }

  dataURItoBlob(dataURI) {
    const byteString = window.atob(dataURI);
    const arrayBuffer = new ArrayBuffer(byteString.length);
    const int8Array = new Uint8Array(arrayBuffer);
    for (let i = 0; i < byteString.length; i++) {
      int8Array[i] = byteString.charCodeAt(i);
    }
    const blob = new Blob([int8Array]);
    return blob;
  }

  deleteArchivo(tipo: string) {
    if (tipo == 'secuencia') {
      this.fileSecuencia = null;
      this.fileNameSecuencia = 'Ningun archivo elegido';
    }
    else {
      this.filePlano = null;
      this.fileNamePlano = 'Ningun archivo elegido';
    }
  }

  mostrarFumigadoraEvent(e, mostrar: boolean) {
    this.mostrarFumigadora = mostrar;
    this.planoDeCargaForm.get('empresaFumigadora').setValue("");
  }

  onSelectAgente(event) {
    this.agenteSeleccionado = event.id;
    this.ABM('Agente', 'Modificar-Eliminar');
  }

  calcularTotal() {
    if (this.esLiquido)
      this._turnoService.setTnTotales(this.planoDeCargaBodegasFormArray.value.reduce((prev, next) => prev + +next.cantidad, 0))
    return this.planoDeCargaBodegasFormArray.value.reduce((prev, next) => prev + +next.cantidad, 0);
  }

  //**CONTROL DE BOTONES DE LOS ABM***//
  submitABM(accion) {
    var condicion: string = this.pantallaSeleccionada
    switch (condicion) {
      case 'Estiba':
        var abm: Estiba = new Estiba('', '', '');
        var list = 'estibasList';
        var opcionABM = this.opcionABMSeleccionada == 'Agregar' ?
          'agregarEstiba' : accion == 'Guardar' ?
            'modificarEstiba' : 'eliminarEstiba';
        var obtener = 'obtenerListadoEstibas';
        var modelo = Estiba;
        var mensaje1 = this.opcionABMSeleccionada == 'Agregar' ?
          'Ha cargado con éxito una nueva Estiba' : accion == 'Guardar' ?
            'Ha modificado con éxito la Estiba' : 'Ha eliminado con éxito la Estiba';
        var mensaje2 = accion == 'Guardar' ? 'Los datos de esta Estiba ya existen' :
          'Los datos de esta Estiba NO existen';
        var mensaje3 = 'Debe Inidcar un Nombre para la Estiba';
        var mensaje4 = 'No se puede eliminar la Estiba, ya que está asociada a un Plano de Carga';
        break;

      case 'Agencia':
        var abm2: AgenciaControlPrivado = new AgenciaControlPrivado('', '');
        var list = 'agenciasControlPrivadoList';
        var opcionABM = this.opcionABMSeleccionada == 'Agregar' ?
          'agregarAgenciaControlPrivado' : accion == 'Guardar' ?
            'modificarAgenciaControlPrivado' : 'eliminarAgenciaControlPrivado';
        var obtener = 'obtenerListadoAgenciasControlPrivado';
        var modelo2 = AgenciaControlPrivado;
        var mensaje1 = this.opcionABMSeleccionada == 'Agregar' ?
          'Ha cargado con éxito una nueva Agencia de Control Privado' : accion == 'Guardar' ?
            'Ha modificado con éxito la Agencia de Control Privado' : 'Ha eliminado con éxito la Agencia de Control Privado';
        var mensaje2 = accion == 'Guardar' ? 'Los datos de esta Agencia de Control Privado ya existen' :
          'Los datos de esta Agencia de Control Privado NO existen';
        var mensaje3 = 'Debe Inidcar un Nombre para la Agencia de Control Privado';
        var mensaje4 = 'No se puede eliminar la Agencia de Control Privado, ya que está asociada a un Plano de Carga';
        break;

      case 'Agente':
        var abm: AgenteControlPrivado = new AgenteControlPrivado('', '', '');
        var list = 'agentesControlPrivadoList';
        var listDeSeleccionados = 'agentesControlPrivadoSeleccionado';
        var opcionABM = this.opcionABMSeleccionada == 'Agregar' ?
          'agregarAgenteControlPrivado' : accion == 'Guardar' ?
            'modificarAgenteControlPrivado' : 'eliminarAgenteControlPrivado';
        var obtener = 'obtenerListadoAgentesControlPrivado';
        var modelo = AgenteControlPrivado;
        var mensaje1 = this.opcionABMSeleccionada == 'Agregar' ?
          'Ha cargado con éxito una nuevo Encargado' : accion == 'Guardar' ?
            'Ha modificado con éxito el Encargado' : 'Ha eliminado con éxito el Encargado';
        var mensaje2 = accion == 'Guardar' ? 'Los datos de este Encargado ya existen' :
          'Los datos de este Encargado NO existen';
        var mensaje3 = 'Debe Inidcar un Nombre para el Encargado';
        var mensaje4 = 'No se puede eliminar el Encargado, ya que está asociado a un Plano de Carga';
        break;
    }

    //**Guardar de Agregar y Modificar***//
    if (accion == 'Guardar') {
      if (condicion != 'Agencia') {
        if (this.opcionABMSeleccionada == 'Modificar-Eliminar') {
          if (condicion != 'Agente')
            abm.id = this.planoDeCargaForm.get([list]).value[0].id;
          else
            abm.id = this.agenteSeleccionado;
        }

        abm.nombre = (<HTMLInputElement>document.getElementById("nombre")).value;
        abm.apellido = (<HTMLInputElement>document.getElementById("apellido")).value;
        if (condicion == 'Agente')
          abm.name = abm.nombre + ' ' + abm.apellido;
      }
      else {
        if (this.opcionABMSeleccionada == 'Modificar-Eliminar')
          abm2.id = this.planoDeCargaForm.get([list]).value[0].id;

        abm2.nombre = (<HTMLInputElement>document.getElementById("nombre")).value;
      }

      if ((condicion != 'Agencia' ? abm.nombre.replace(/\s/g, "").length : abm2.nombre.replace(/\s/g, "").length) > 0) {
        if (this.opcionABMSeleccionada == 'Agregar')
          var datosUnicos = this[list].find(x => (condicion != 'Agencia' ?
            x.nombre == abm.nombre && x.apellido == abm.apellido :
            x.nombre == abm2.nombre));
        else
          var datosUnicos = this[list].find(x => (condicion != 'Agencia' ?
            x.id != abm.id && x.nombre == abm.nombre && x.apellido == abm.apellido
            : x.id != abm2.id && x.nombre == abm2.nombre));

        if (typeof datosUnicos == 'undefined') {
          this.planoDeCargaService[opcionABM](condicion != 'Agencia' ? abm : abm2).subscribe(res => {
            this.modalService.dismissAll();

            if (this.opcionABMSeleccionada == 'Agregar') {
              if (condicion != 'Agente') {
                this.planoDeCargaService[obtener]().subscribe(res => {
                  this[list] = res.map(x =>
                    (condicion != 'Agencia' ? new modelo(x.id, x.nombre, x.apellido) : new modelo2(x.id, x.nombre)));
                });

                this.planoDeCargaService[obtener]().subscribe(res => {
                  this.planoDeCargaForm.get([list]).setValue(
                    res.filter(x => x.nombre == (condicion != 'Agencia' ? abm.nombre : abm2.nombre))
                      .map(x => (condicion != 'Agencia' ? new modelo(x.id, x.nombre, x.apellido) : new modelo2(x.id, x.nombre))));
                });
              }
              else { //esto es solo para los agentes, ya que es un input-tag múltiple
                this.planoDeCargaService[obtener]().subscribe(res => {
                  this[list] = res.map(x => new modelo(x.id, x.nombre, x.apellido));

                  var listado = [];
                  for (let index = 0; index < this.planoDeCargaForm.get([listDeSeleccionados]).value.length; index++) {
                    var vid = this.planoDeCargaForm.get([listDeSeleccionados]).value[index].id;
                    var vnombre: string;
                    var vapellido: string;
                    if (vid == abm.id) {
                      vnombre = abm.nombre;
                      vapellido = abm.apellido;
                    }
                    else {
                      vnombre = this.planoDeCargaForm.get([listDeSeleccionados]).value[index].nombre;
                      vapellido = this.planoDeCargaForm.get([listDeSeleccionados]).value[index].apellido;
                    }

                    var datosAnteriores = {
                      id: vid,
                      nombre: vnombre,
                      apellido: vapellido,
                      name: vnombre + ' ' + vapellido
                    }
                    listado.push(datosAnteriores);
                  }

                  var datosUnicos = this[list].find(x => x.nombre == abm.nombre && x.apellido == abm.apellido);
                  vid = datosUnicos.id;
                  if (typeof datosUnicos != 'undefined') {
                    var datosNuevos = {
                      id: vid,
                      nombre: abm.nombre,
                      apellido: abm.apellido,
                      name: abm.nombre + ' ' + abm.apellido
                    }
                    listado.push(datosNuevos);
                  }

                  this.planoDeCargaForm.get([listDeSeleccionados]).setValue(listado);
                });
              }
            }
            else {
              if (condicion != 'Agente') {
                this.planoDeCargaService[obtener]().subscribe(res => {
                  this[list] = res.map(x => (condicion != 'Agencia' ?
                    new modelo(x.id, x.nombre, x.apellido) : new modelo2(x.id, x.nombre)));
                });

                this.planoDeCargaService[obtener]().subscribe(res => {
                  this.planoDeCargaForm.get([list]).setValue(
                    res.filter(x => x.id == (condicion != 'Agencia' ? abm.id : abm2.id))
                      .map(x => (condicion != 'Agencia' ? new modelo(x.id, x.nombre, x.apellido) : new modelo2(x.id, x.nombre))));
                });
              }
              else {//esto es solo para los agentes, ya que es un input-tag múltiple
                this.planoDeCargaService[obtener]().subscribe(res => {
                  this.agentesControlPrivadoList = res.map(x => new modelo(x.id, x.nombre, x.apellido));

                  var listado = [];
                  for (let index = 0; index < this.planoDeCargaForm.get([listDeSeleccionados]).value.length; index++) {
                    var vid = this.planoDeCargaForm.get([listDeSeleccionados]).value[index].id;
                    var vnombre: string;
                    var vapellido: string;
                    if (vid == abm.id) {
                      vnombre = abm.nombre;
                      vapellido = abm.apellido;
                    }
                    else {
                      vnombre = this.planoDeCargaForm.get([listDeSeleccionados]).value[index].nombre;
                      vapellido = this.planoDeCargaForm.get([listDeSeleccionados]).value[index].apellido;
                    }

                    var datosAnteriores = {
                      id: vid,
                      nombre: vnombre,
                      apellido: vapellido,
                      name: vnombre + ' ' + vapellido
                    }
                    listado.push(datosAnteriores);
                  }

                  this.planoDeCargaForm.get([listDeSeleccionados]).setValue(listado);
                });
              }
            }

            this.confirmationDialogService.confirm('¡Felicitaciones!', mensaje1, 'Cerrar', '', null, null, Tipoalerta.Success);
          });
        }
        else {
          this.confirmationDialogService.confirm('¡Error!', mensaje2, 'Cerrar', '', null, null, Tipoalerta.Success);
        }
      }
      else {
        this.confirmationDialogService.confirm('¡Error!', mensaje3,
          'Cerrar', '', null, null, Tipoalerta.Success);
      }
    }
    //**Eliminar***//
    else {
      if (condicion != 'Agencia') {
        if (condicion == 'Agente')
          abm.id = this.agenteSeleccionado;
        else
          abm.id = this.planoDeCargaForm.get([list]).value[0].id;
        var datosUnicos = this[list].find(x => x.id != abm.id);
      }
      else {
        abm2.id = this.planoDeCargaForm.get([list]).value[0].id;
        var datosUnicos = this[list].find(x => x.id != abm2.id);
      }

      if (typeof datosUnicos != 'undefined') {
        this.planoDeCargaService[opcionABM](condicion != 'Agencia' ? abm.id : abm2.id).subscribe(res => {
          this.modalService.dismissAll();

          if (res) {
            this.planoDeCargaService[obtener]().subscribe(res => {
              this[list] = res.map(x => (condicion != 'Agencia' ?
                new modelo(x.id, x.nombre, x.apellido) :
                new modelo2(x.id, x.nombre)));

              if (condicion == 'Agente') {
                var listado = [];
                for (let index = 0; index < this.planoDeCargaForm.get([listDeSeleccionados]).value.length; index++) {
                  var vid = this.planoDeCargaForm.get([listDeSeleccionados]).value[index].id;
                  var vnombre: string;
                  var vapellido: string;
                  if (vid != abm.id) {
                    vnombre = this.planoDeCargaForm.get([listDeSeleccionados]).value[index].nombre;
                    vapellido = this.planoDeCargaForm.get([listDeSeleccionados]).value[index].apellido;

                    var datosAnteriores = {
                      id: vid,
                      nombre: vnombre,
                      apellido: vapellido,
                      name: vnombre + ' ' + vapellido
                    }
                    listado.push(datosAnteriores);
                  }
                }

                this.planoDeCargaForm.get([listDeSeleccionados]).setValue(listado);
              }
              else
                this.planoDeCargaForm.get([list]).setValue('');
            });

            this.confirmationDialogService.confirm('¡Felicitaciones!', mensaje1, 'Cerrar', '', null, null, Tipoalerta.Success);
          }
          else
            this.confirmationDialogService.confirm('¡Error!', mensaje4, 'Cerrar', '', null, null, Tipoalerta.Success);
        });
      }
      else {
        this.confirmationDialogService.confirm('¡Error!', mensaje2, 'Cerrar', '', null, null, Tipoalerta.Success);
      }
    }
  }

  sendExportador(value: any) {
    this._turnoService.setExportadores(value.controls.cargasComerciales.value);
  }

  sendDataParcel(bodegas) {
    let bodegasFull = bodegas.filter(b => b.cantidad > 0 || b.condicion || b.destino || b.materialPuerto || b.tanqueDeAbordo);
    this._turnoService.sendBodega.emit(bodegasFull);
  }
}