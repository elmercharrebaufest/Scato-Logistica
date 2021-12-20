import { Component, ElementRef, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbTypeahead } from '@ng-bootstrap/ng-bootstrap';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { forkJoin } from 'rxjs';
import { Tipoalerta } from '@ScatoEnums/tipo-alerta';
import { Alerta } from '@ScatoModels/alerta';
import { MaterialPuertoCantidad } from '@ScatoModels/material-puerto-cantidad';
import { UbicacionDeBuquePuerto } from '@ScatoModels/ubicacion-de-buque-puerto';
import { ConfirmationDialogService } from '@ScatoServicios/confirmation-dialog.service';
import { PlanoDeCargaService } from '@ScatoServicios/plano-de-carga.service';
import { AgenciaMaritimaPuerto } from '@ScatoModels/agencia-maritima-puerto';
import { EmbarqueService } from '@ScatoServicios/embarque.service';
import { AlertService } from '@ScatoServicios/alert.service';
import { MaterialPuerto } from '@ScatoModels/material-puerto';
import { TipoDeBuquePuerto } from '@ScatoModels/tipo-de-buque-puerto';
import { CoordinadorPuerto } from '@ScatoModels/coordinador-puerto';
import { ATAPuerto } from '@ScatoModels/ata-puerto';
import { Destino } from '@ScatoModels/destino';
import { MotivosLimpieza } from '@ScatoModels/motivo-limpieza';
import { WorkflowService } from '@ScatoServicios/workflow.service'
import { ModuloDeCargaService } from '@ScatoServicios/modulo-de-carga.service'

@Component({
  selector: 'app-alta-embarque',
  templateUrl: './alta-embarque.component.html',
  styleUrls: ['./alta-embarque.component.css']
})

export class AltaEmbarqueComponent implements OnInit {
  mostrarSpinner: boolean = true;
  @ViewChild('modalABM') modalABM: TemplateRef<any>;

  embarqueId: number;
  embarqueForm: FormGroup;
  submitted = false;
  listadoMateriales: MaterialPuerto[] = []
  tipoDeBuquePuerto: TipoDeBuquePuerto[];
  ubicacionDeBuquePuerto: UbicacionDeBuquePuerto[];
  agenciasList: AgenciaMaritimaPuerto[];
  coordinadoresList: CoordinadorPuerto[];
  ataList: ATAPuerto[];
  pantallaSeleccionada: string;
  opcionABMSeleccionada: string;
  tituloABM: string;
  motivosLimpiezaList: MotivosLimpieza[];
  destinoPuerto: Destino[];
  PlanoDeCargaId: number;
  private state: string;

  @ViewChild('horaRecalada') horaRecalada: ElementRef;
  @ViewChild('horaDesdeLimpieza') horaDesdeLimpieza: ElementRef;
  @ViewChild('horaHastaLimpieza') horaHastaLimpieza: ElementRef;
  @ViewChild('horaLibrePlatica') horaLibrePlatica: ElementRef;
  @ViewChild('instance', { static: true }) instance: NgbTypeahead;

  constructor(private formBuilder: FormBuilder,
    private embarqueService: EmbarqueService,
    private confirmationDialogService: ConfirmationDialogService,
    private router: Router, private route: ActivatedRoute,
    private modalService: NgbModal,
    private alertService: AlertService,
    private planoDeCargaService: PlanoDeCargaService,
    private workflowService: WorkflowService,
    private moduloCargaService: ModuloDeCargaService) {
    this.state = this.route.snapshot.params.state;
    this.embarqueId = this.route.snapshot.params.id ? this.route.snapshot.params.id : 0;
  }

  ngOnInit(): void {
    this.cargarCombos();
    this.embarqueForm = this.formBuilder.group({
      id: [],
      nombreBuque: [''],
      agencia: [],
      coordinador: [],
      fechaRecalada: ['', [this.dateValidator.bind(this)]],
      obligacionCarga: ['', [this.dateValidator.bind(this)]],
      senasa: [false],
      observaciones: [],
      vicentin: [false],
      noryon: [false],
      sanBenito: [true],
      otrosMuelles: [false],
      patente: [],
      tipoBuque: [''],
      tipoDeBuque: [''],
      freeboard: [],
      ubicacion: [''],
      ubicacionDeBuque: [''],
      materialesPuertoCantidad: this.formBuilder.array([]),
      meridiemRecalada: [],
      horaREcalada: [],
      agencias: [],
      agenciasList: [],
      coordinadores: [],
      coordinadoresList: [],
      ata: [],
      ataList: [],
      ubicacionDeBuquePuerto: [],
      tipoDeBuquePuerto: [],
      esLiquido: [false],
      fechaDesdeLimpieza: ['', [this.dateValidator.bind(this)]],
      horaDesdeLimpieza: [],
      fechaHastaLimpieza: ['', [this.dateValidator.bind(this)]],
      horaHastaLimpieza: [],
      motivosLimpieza: [],
      motivosLimpiezaList: [],
      observacionesLimpieza: [],
      destinoBuque: [''],
      destino: [''],
      destinoPuerto: [],
      porteNeto: [],
      porteBruto: [],
      eslora: [],
      manga: [],
      puntal: [],
      fechaLibrePlatica: ['', [this.dateValidator.bind(this)]],
      horaLibrePlatica: [],
    });

    if (this.state === 'modulo-carga'){
      this.embarqueForm.get('sanBenito').disable();
      this.embarqueForm.get('noryon').disable();
      this.embarqueForm.get('vicentin').disable();
      this.embarqueForm.get('otrosMuelles').disable();
    }

    forkJoin([this.embarqueService.obtenerListadoTipoDeBuquePuerto(),
    this.embarqueService.obtenerListadoUbicacionDeBuquePuerto(),
    this.planoDeCargaService.obtenerDestinos()
    ]).subscribe(([res1, res2, res3
    ]) => {
      this.tipoDeBuquePuerto = res1;
      this.ubicacionDeBuquePuerto = res2;
      this.destinoPuerto = res3;
      this.cargarListadoMateriales();
    }, err => { console.log(err); });
  }

  cargarEmbarqueEditar() {
    if (!this.tipoDeBuquePuerto)
      this.embarqueService.obtenerListadoTipoDeBuquePuerto().subscribe(res => { this.tipoDeBuquePuerto = res; });

    if (!this.ubicacionDeBuquePuerto)
      this.embarqueService.obtenerListadoUbicacionDeBuquePuerto().subscribe(res => { this.ubicacionDeBuquePuerto = res; });

    if (!this.destinoPuerto)
      this.planoDeCargaService.obtenerDestinos().subscribe(res => { this.destinoPuerto = res; });
    if (this.embarqueId != 0) {
      this.embarqueService.obtenerEmbarque(this.embarqueId).subscribe(
        res => {
          var filtered = this.listadoMateriales.filter(
            function (e) {
              return this.indexOf(e.id) < 0;
            },
            res.materialesPuertoCantidad.map(x => x.materialId)
          );
          filtered.map(x => new MaterialPuertoCantidad({
            materialId: x.id,
            descripcionCorta: x.descripcionCorta,
            cantidad: 0,
            color: x.color
          })).forEach(x => {
            res.materialesPuertoCantidad.push(x);
          });
          this.embarqueForm.patchValue(res);
          this.checkLiquidOrSolid(res.materialesPuertoCantidad.find(x => x.cantidad != 0));

          if (res.tipoBuque != null && res.tipoBuque != '' && typeof this.tipoDeBuquePuerto != 'undefined')
            this.embarqueForm.get('tipoDeBuque').setValue(
              this.tipoDeBuquePuerto.find(x => x.nombre == res.tipoBuque.toString()));
          else
            this.embarqueForm.get('tipoDeBuque').setValue('');

          if (res.obligacionCarga != null)
            this.embarqueForm.get('obligacionCarga').setValue(new Date(res.obligacionCarga).toISOString().slice(0, 10));
          else
            this.embarqueForm.get('obligacionCarga').setValue('');

          this.embarqueForm.get('fechaRecalada').setValue('');
          this.horaRecalada.nativeElement.value = '';
          this.embarqueForm.get('meridiemRecalada').setValue('');
          if (res.fechaRecalada != null) {
            this.embarqueForm.get('fechaRecalada').setValue(new Date(res.fechaRecalada).toISOString().slice(0, 10));
            //this.horaRecalada.nativeElement.value = formatDate(res.fechaRecalada, 'HH:mm', 'es-ar');
            this.horaRecalada.nativeElement.value = res.horaRecalada != null ? res.horaRecalada.length > 2 ? res.horaRecalada : '' : '';
            if (res.horaRecalada != null && res.horaRecalada.length == 2)
              this.embarqueForm.get('meridiemRecalada').setValue(res.horaRecalada);
          }

          // HORAS A LA ESPERA DE LIMPIEZA
          if (res.fechaDesdeLimpieza != null)
            this.embarqueForm.get('fechaDesdeLimpieza').setValue(new Date(res.fechaDesdeLimpieza).toISOString().slice(0, 10));
          else
            this.embarqueForm.get('fechaDesdeLimpieza').setValue('');

          this.horaDesdeLimpieza.nativeElement.value = res.horaDesdeLimpieza != null ? res.horaDesdeLimpieza : '';

          if (res.fechaHastaLimpieza != null)
            this.embarqueForm.get('fechaHastaLimpieza').setValue(new Date(res.fechaHastaLimpieza).toISOString().slice(0, 10));
          else
            this.embarqueForm.get('fechaHastaLimpieza').setValue('');

          this.horaHastaLimpieza.nativeElement.value = res.horaHastaLimpieza != null ? res.horaHastaLimpieza : '';

          // SHIP PARTICULAR
          if (res.fechaLibrePlatica != null)
            this.embarqueForm.get('fechaLibrePlatica').setValue(new Date(res.fechaLibrePlatica).toISOString().slice(0, 10));
          else
            this.embarqueForm.get('fechaLibrePlatica').setValue('');

          this.horaLibrePlatica.nativeElement.value = res.horaLibrePlatica != null ? res.horaLibrePlatica : '';

          if (res.ubicacion != null && res.ubicacion != 0 && typeof this.ubicacionDeBuquePuerto != 'undefined')
            this.embarqueForm.get('ubicacionDeBuque').setValue(
              this.ubicacionDeBuquePuerto.find(x => x.id == res.ubicacion));
          else
            this.embarqueForm.get('ubicacionDeBuque').setValue('');

          if (res.destino && res.destino != null && res.destino.id != null && res.destino.id != 0 && typeof this.destinoPuerto != 'undefined')
            this.embarqueForm.get('destino').setValue(
              this.destinoPuerto.find(x => x.id == res.destino.id));
          else
            this.embarqueForm.get('destino').setValue('');
          //--------------------
          this.embarqueService.obtenerListadoAgenciasMaritimas().subscribe(res1 => {
            res.agencias = res1.map(x => new AgenciaMaritimaPuerto(x.id, x.nombre));
          });
          if (this.embarqueForm.value['agencias'] != null) {
            this.embarqueService.obtenerListadoAgenciasMaritimas().subscribe(res1 => {
              this.embarqueForm.get('agenciasList').setValue(
                res1.filter(x => x.id == this.embarqueForm.value['agencias'].id).map(x => new AgenciaMaritimaPuerto(x.id, x.nombre)));
            });
          }

          this.embarqueService.obtenerListadoCoordinadores().subscribe(res1 => {
            res.coordinadores = res1.map(x => new CoordinadorPuerto(x.id, x.nombre));
          });
          if (this.embarqueForm.value['coordinadores'] != null) {
            this.embarqueService.obtenerListadoCoordinadores().subscribe(res1 => {
              this.embarqueForm.get('coordinadoresList').setValue(
                res1.filter(x => x.id == this.embarqueForm.value['coordinadores'].id).map(x => new CoordinadorPuerto(x.id, x.nombre)));
            });
          }

          this.embarqueService.obtenerListadoATAPuerto().subscribe(res1 => {
            res.ata = res1.map(x => new ATAPuerto(x.id, x.nombre));
          });
          if (this.embarqueForm.value['ata'] != null) {
            this.embarqueService.obtenerListadoATAPuerto().subscribe(res1 => {
              this.embarqueForm.get('ataList').setValue(
                res1.filter(x => x.id == this.embarqueForm.value['ata'].id).map(x => new ATAPuerto(x.id, x.nombre)));
            });
          }

          this.embarqueService.obtenerListadoMotivosLimpieza().subscribe(res1 => {
            res.motivosLimpieza = res1.map(x => new MotivosLimpieza(x.id, x.nombre));
          });
          if (this.embarqueForm.value['motivosLimpieza'] != null) {
            this.embarqueService.obtenerListadoMotivosLimpieza().subscribe(res1 => {
              this.embarqueForm.get('motivosLimpiezaList').setValue(
                res1.filter(x => x.id == this.embarqueForm.value['motivosLimpieza'].id).map(x => new MotivosLimpieza(x.id, x.nombre)));
            });
          }

          this.mostrarSpinner = false;
        },
        errmess => {
          this.confirmationDialogService.confirm('¡Error!', 'Error al cargar el embarque: ' + <any>errmess.error, 'Cerrar', '', null, null, Tipoalerta.Error);
          this.mostrarSpinner = false;
        }
      );

    } else {
      this.mostrarSpinner = false;
    }
  }

  get materialesPuertoCantidadFormArray(): FormArray {
    return this.embarqueForm.get("materialesPuertoCantidad") as FormArray
  }
  get f() { return this.embarqueForm.controls; }

  dateValidator(control: AbstractControl): { [key: string]: boolean } | null {
    var dateToValidate = new Date(control.value);
    if (this.isInvalidDate(dateToValidate)) {
      return { 'dateError': true };
    }
    return null;

  }

  isInvalidDate(date: Date): boolean {
    return date.getFullYear() < 2000 || date.getFullYear() > 2100;
  }

  finalizarAlta() {
    this.submitted = true;
    if (this.embarqueForm.invalid)
      return;
    if (this.invalidRequiredMaterial()) {
      this.embarqueForm.controls['materialesPuertoCantidad'].setErrors({ 'error': true });
      return;
    }

    this.mostrarSpinner = true;

    this.embarqueForm.get('fechaRecalada').setValue(
      this.embarqueForm.value.fechaRecalada + ' ' + this.horaRecalada.nativeElement.value);

    this.embarqueForm.get('horaREcalada').setValue(
      this.horaRecalada.nativeElement.value ?
        this.horaRecalada.nativeElement.value :
        this.embarqueForm.value.meridiemRecalada);

    this.embarqueForm.get('fechaDesdeLimpieza').setValue(
      this.embarqueForm.value.fechaDesdeLimpieza + ' ' + this.horaDesdeLimpieza.nativeElement.value);

    this.embarqueForm.get('horaDesdeLimpieza').setValue(
      this.horaDesdeLimpieza.nativeElement.value ?
        this.horaDesdeLimpieza.nativeElement.value : '');

    this.embarqueForm.get('fechaHastaLimpieza').setValue(
      this.embarqueForm.value.fechaHastaLimpieza + ' ' + this.horaHastaLimpieza.nativeElement.value);

    this.embarqueForm.get('horaHastaLimpieza').setValue(
      this.horaHastaLimpieza.nativeElement.value ?
        this.horaHastaLimpieza.nativeElement.value : '');

    this.embarqueForm.get('fechaLibrePlatica').setValue(
      this.embarqueForm.value.fechaLibrePlatica + ' ' + this.horaLibrePlatica.nativeElement.value);

    this.embarqueForm.get('horaLibrePlatica').setValue(
      this.horaLibrePlatica.nativeElement.value ?
        this.horaLibrePlatica.nativeElement.value : '');

    this.embarqueForm.value.agencias =
      this.embarqueForm.value.agenciasList != null && this.embarqueForm.value.agenciasList.length > 0 ?
        this.agenciasList.find(x => x.id == this.embarqueForm.value.agenciasList[0].id) : '';

    this.embarqueForm.value.motivosLimpieza =
      this.embarqueForm.value.motivosLimpiezaList != null && this.embarqueForm.value.motivosLimpiezaList.length > 0 ?
        this.motivosLimpiezaList.find(x => x.id == this.embarqueForm.value.motivosLimpiezaList[0].id) : '';

    this.embarqueForm.value.coordinadores =
      this.embarqueForm.value.coordinadoresList != null && this.embarqueForm.value.coordinadoresList.length > 0 ?
        this.coordinadoresList.find(x => x.id == this.embarqueForm.value.coordinadoresList[0].id) : '';

    this.embarqueForm.value.ata =
      this.embarqueForm.value.ataList != null && this.embarqueForm.value.ataList.length > 0 ?
        this.ataList.find(x => x.id == this.embarqueForm.value.ataList[0].id) : '';

    this.embarqueForm.value.esLiquido = this.listadoMateriales.find(x => x.id == this.materialesPuertoCantidadFormArray.controls.find(x => x.value.cantidad > 0).value.materialId).esLiquido;
    this.state === 'modulo-carga' ? this.embarqueForm.value['sanBenito'] = true : '';
    this.embarqueService.altaEmbarque(this.embarqueForm.value)
      .subscribe((res: any) => {
        if (this.state && this.state.toLowerCase().trim() === 'modulo-carga') { //Si venimos del modulo de carga => /:state = modulo-carga, mostramos el confirm solo con el boton volver
          setTimeout(() => {
            this.workflowService.listarEmbarquesEnLineUp().subscribe(listado => {
              this.PlanoDeCargaId = listado.find(x => x.id == res).planoDeCargaId;
              this.mostrarSpinner = false;
              this.moduloCargaService.modificarCargadoPlanoDeCarga(this.PlanoDeCargaId).subscribe(y => {
                this.openConfirmationDialog('¡Felicitaciones!',
                  'Ha cargado con éxito un nuevo Buque al Line UP',
                  'Volver a Modulo de Carga')
              });
            });
          }, 1000);
        } else {
          this.mostrarSpinner = false;
          this.openConfirmationDialog('¡Felicitaciones!',
            'Ha cargado con éxito un nuevo Buque al Line UP',
            'Ver line up',
            'Cargar otro buque');
        }
      },
        errmess => {
          this.confirmationDialogService.confirm('¡Error!', 'Error al crear el embarque: ' + <any>errmess.error, 'Cerrar', '', null, null, Tipoalerta.Error);
          this.mostrarSpinner = false;
        });
  }

  invalidRequiredMaterial() {
    var material = this.materialesPuertoCantidadFormArray.controls.find(x => x.value.cantidad > 0);
    return material == null;
  }

  public openConfirmationDialog(titulo: string, texto: string, button1: string = 'OK', button2: string = 'Cancel') {
    if (this.state && this.state.toLowerCase().trim() === 'modulo-carga') { //Si venimos del modulo de carga => /:state = modulo-carga, nos devuelve al mismo modulo
      this.confirmationDialogService.confirm(titulo, texto, button1, '')
        .then((confirmed) => {
          if (confirmed)
            this.router.navigate(['/carga']);
        })
        .catch(() => window.location.reload());
    } else {
      this.confirmationDialogService.confirm(titulo, texto, button1, button2)
        .then((confirmed) => {
          if (confirmed)
            this.router.navigate(['/lineup']);
          else
            window.location.reload();
        })
        .catch(() => window.location.reload());
    }
  }

  cargarListadoMateriales() {
    this.embarqueService.obtenerListadoMateriales()
      .subscribe(res => {
        res.forEach(element => {
          this.listadoMateriales = res;
          this.materialesPuertoCantidadFormArray.push(this.formBuilder.group({
            materialId: [element.id],
            descripcionCorta: [element.descripcionCorta],
            cantidad: [],
            color: [element.color]
          }))
        });
        this.cargarEmbarqueEditar();
      },
        errmess => {
          this.confirmationDialogService.confirm('¡Error!', 'Error al cargar los materiales: ' + <any>errmess.error, 'Cerrar', '', null, null, Tipoalerta.Error);
          this.mostrarSpinner = false;
        });
  }

  cargarCombos() {
    this.embarqueService.obtenerListadoCoordinadores().subscribe(res => {
      this.coordinadoresList = res.map(x => new CoordinadorPuerto(x.id, x.nombre));
    });

    this.embarqueService.obtenerListadoATAPuerto().subscribe(res => {
      this.ataList = res.map(x => new ATAPuerto(x.id, x.nombre));
    });

    this.embarqueService.obtenerListadoAgenciasMaritimas().subscribe(res => {
      this.agenciasList = res.map(x => new AgenciaMaritimaPuerto(x.id, x.nombre));
    });

    this.embarqueService.obtenerListadoMotivosLimpieza().subscribe(res => {
      this.motivosLimpiezaList = res.map(x => new MotivosLimpieza(x.id, x.nombre));
    });
  }

  public isEditing() {
    return this.route.snapshot.queryParamMap.get('id') != null;
  }

  public modificarEmbarque() {
    this.submitted = true;
    if (this.embarqueForm.invalid)
      return;
    if (this.invalidRequiredMaterial()) {
      this.embarqueForm.controls['materialesPuertoCantidad'].setErrors({ 'error': true });
      return;
    }

    this.mostrarSpinner = true;
    this.embarqueForm.get('fechaRecalada').setValue(
      this.embarqueForm.value.fechaRecalada + ' ' + this.horaRecalada.nativeElement.value);

    this.embarqueForm.get('horaREcalada').setValue(
      this.horaRecalada.nativeElement.value ?
        this.horaRecalada.nativeElement.value :
        this.embarqueForm.value.meridiemRecalada);

    this.embarqueForm.get('fechaDesdeLimpieza').setValue(
      this.embarqueForm.value.fechaDesdeLimpieza + ' ' + this.horaDesdeLimpieza.nativeElement.value);

    this.embarqueForm.get('horaDesdeLimpieza').setValue(
      this.horaDesdeLimpieza.nativeElement.value ? this.horaDesdeLimpieza.nativeElement.value : '');

    this.embarqueForm.get('fechaHastaLimpieza').setValue(
      this.embarqueForm.value.fechaHastaLimpieza + ' ' + this.horaHastaLimpieza.nativeElement.value);

    this.embarqueForm.get('horaHastaLimpieza').setValue(
      this.horaHastaLimpieza.nativeElement.value ? this.horaHastaLimpieza.nativeElement.value : '');

    this.embarqueForm.get('fechaLibrePlatica').setValue(
      this.embarqueForm.value.fechaLibrePlatica + ' ' + this.horaLibrePlatica.nativeElement.value);

    this.embarqueForm.get('horaLibrePlatica').setValue(
      this.horaLibrePlatica.nativeElement.value ? this.horaLibrePlatica.nativeElement.value : '');

    this.embarqueForm.value.motivosLimpieza =
      this.embarqueForm.value.motivosLimpiezaList != null && this.embarqueForm.value.motivosLimpiezaList.length > 0 ?
        this.motivosLimpiezaList.find(x => x.id == this.embarqueForm.value.motivosLimpiezaList[0].id) : '';

    this.embarqueForm.value.agencias =
      this.embarqueForm.value.agenciasList != null && this.embarqueForm.value.agenciasList.length > 0 ?
        this.agenciasList.find(x => x.id == this.embarqueForm.value.agenciasList[0].id) : '';

    this.embarqueForm.value.coordinadores =
      this.embarqueForm.value.coordinadoresList != null && this.embarqueForm.value.coordinadoresList.length > 0 ?
        this.coordinadoresList.find(x => x.id == this.embarqueForm.value.coordinadoresList[0].id) : '';

    this.embarqueForm.value.ata =
      this.embarqueForm.value.ataList != null && this.embarqueForm.value.ataList.length > 0 ?
        this.ataList.find(x => x.id == this.embarqueForm.value.ataList[0].id) : '';

    this.embarqueForm.value.esLiquido = this.listadoMateriales.find(x => x.id == this.materialesPuertoCantidadFormArray.controls.find(x => x.value.cantidad > 0).value.materialId).esLiquido;

    this.embarqueService.modificarEmbarque(this.embarqueForm.value)
      .subscribe((res: any) => {
        this.mostrarSpinner = false;
        this.openConfirmationDialog('¡Felicitaciones!',
          'Ha modificado con éxito el buque',
          'Ver line up',
          'Seguir modificando');
      },
        errmess => {
          this.alertService.mostrar(new Alerta(<any>errmess.error, Tipoalerta.Error));
          this.mostrarSpinner = false;
        });
  }

  public trackByFn(index: any, item: any) {
    return index;
  }

  public checkLiquidOrSolid(materialCantidad) {

    if (materialCantidad == null)
      return;

    var material = this.listadoMateriales.find(x => x.id == materialCantidad.materialId);
    if (material.esLiquido) {
      if (materialCantidad.cantidad != null && materialCantidad.cantidad > 0)
        this.disabledInputMaterial(true, false);
      else if (this.emptyValuesQuantity(materialCantidad))
        this.disabledInputMaterial(false, false);
    }
    else if (!material.esLiquido) {
      if (materialCantidad.cantidad != null && materialCantidad.cantidad > 0)
        this.disabledInputMaterial(true, true);
      else if (this.emptyValuesQuantity(materialCantidad))
        this.disabledInputMaterial(false, true);
    }
  }

  emptyValuesQuantity(materialCantidad) {
    var material = this.materialesPuertoCantidadFormArray.controls.find(x => x.value.cantidad > 0 && x.value.materialId != materialCantidad.materialId);
    return material == null;
  }

  disabledInputMaterial(disabled, esLiquido) {
    var materiales = this.listadoMateriales.filter(x => x.esLiquido === esLiquido);
    materiales.forEach(element => {
      var control = this.materialesPuertoCantidadFormArray.controls.find(x => x.value.materialId == element.id);
      if (disabled)
        control.get('cantidad').disable();
      else
        control.get('cantidad').enable();
    });
  }

  public numberOnly(event): boolean {
    var charCode = (event.which) ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57))
      return false;
    return true;
  }

  public decimalOnly(event): boolean {
    var charCode = (event.which) ? event.which : event.keyCode;
    if ((charCode > 47 && charCode < 58) || charCode == 46)
      return true;
    return false;
  }

  public onChangeVicentin(e) {
    if (!this.embarqueForm.value.noryon && !this.embarqueForm.value.sanBenito
      && !this.embarqueForm.value.otrosMuelles) {
      this.embarqueForm.get('vicentin').setValue(true);
    }
  }

  public onChangeSanBenito(e) {
    if (!this.embarqueForm.value.noryon && !this.embarqueForm.value.vicentin
      && !this.embarqueForm.value.otrosMuelles) {
      this.embarqueForm.get('sanBenito').setValue(true);
    }
  }

  public onChangeNoryon(e) {
    if (!this.embarqueForm.value.sanBenito && !this.embarqueForm.value.vicentin
      && !this.embarqueForm.value.otrosMuelles) {
      this.embarqueForm.get('noryon').setValue(true);
    }
  }

  public onChangeotrosMuelles(e) {
    if (!this.embarqueForm.value.sanBenito && !this.embarqueForm.value.vicentin
      && !this.embarqueForm.value.noryon) {
      this.embarqueForm.get('otrosMuelles').setValue(true);
    }
  }


  public validarAMPM(event) {
    if (event) {
      if (event.length > 0) {
        this.embarqueForm.get('meridiemRecalada').setValue('');
      }
    }
  }

  public onChangeAMPM(event) {
    this.horaRecalada.nativeElement.value = '';
  }

  open(content) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' }).result.then((result) => {
    }, (reason) => {
    });
  }

  datosABMPuerto(): string {
    var list = this.pantallaSeleccionada == 'Ata' ? 'ataList' :
      this.pantallaSeleccionada == 'Agencia' ? 'agenciasList' :
        this.pantallaSeleccionada == 'Coordinador' ? 'coordinadoresList' : 'motivosLimpiezaList';

    if (this.embarqueForm.get([list]).value.length > 0) {
      var numero = this.embarqueForm.get([list]).value[0].id;
      return this[list] != null ? this[list].find(x => x.id == numero).nombre.toString() : '';
    }
    else
      return '';
  }

  ABM(pantalla, opcion) {
    this.pantallaSeleccionada = pantalla;
    this.opcionABMSeleccionada = opcion;
    this.tituloABM =
      (this.opcionABMSeleccionada == 'Agregar' ? 'Agregar nuevo registro ' : 'Editar ') +
      (this.pantallaSeleccionada == 'Coordinador' ? 'Coordinador de Puerto' :
        this.pantallaSeleccionada == 'Ata' ? 'ATA de Puerto' :
          this.pantallaSeleccionada == 'Agencia' ? 'Agencia Maritima de Puerto' : 'Motivo Limpieza');

    return this.modalService.open(this.modalABM);
  }

  //**CONTROL DE BOTONES DE LOS ABM***//
  submitABM(accion) {
    var condicion: string = this.pantallaSeleccionada
    switch (condicion) {
      case 'Coordinador':
        var abm: CoordinadorPuerto = new CoordinadorPuerto('', '');
        var list = 'coordinadoresList';
        var opcionABM = this.opcionABMSeleccionada == 'Agregar' ?
          'agregarCoordinadorPuerto' : accion == 'Guardar' ?
            'modificarCoordinadorPuerto' : 'eliminarCoordinadorPuerto';
        var obtener = 'obtenerListadoCoordinadores';
        var modelo = CoordinadorPuerto;
        var mensaje1 = this.opcionABMSeleccionada == 'Agregar' ?
          'Ha cargado con éxito un nuevo Coordinador de Puerto' : accion == 'Guardar' ?
            'Ha modificado con éxito el Coordinador de Puerto' : 'Ha eliminado con éxito el Coordinador de Puerto';
        var mensaje2 = accion == 'Guardar' ? 'Los datos de este Coordinador de Puerto ya existen' :
          'Los datos de este Coordinador de Puerto NO existen';
        var mensaje3 = 'Debe Inidcar un Nombre para el Coordinador de Puerto';
        var mensaje4 = 'No se puede eliminar el Coordinador de Puerto, ya que está asociado a un Embarque';
        break;

      case 'Ata':
        var abm: ATAPuerto = new ATAPuerto('', '');
        var list = 'ataList';
        var opcionABM = this.opcionABMSeleccionada == 'Agregar' ?
          'agregarATAPuerto' : accion == 'Guardar' ?
            'modificarATAPuerto' : 'eliminarATAPuerto';
        var obtener = 'obtenerListadoATAPuerto';
        var modelo = ATAPuerto;
        var mensaje1 = this.opcionABMSeleccionada == 'Agregar' ?
          'Ha cargado con éxito una nueva ATA de Puerto' : accion == 'Guardar' ?
            'Ha modificado con éxito la ATA de Puerto' : 'Ha eliminado con éxito la ATA de Puerto';
        var mensaje2 = accion == 'Guardar' ? 'Los datos de esta ATA de Puerto ya existen' :
          'Los datos de esta ATA de Puerto NO existen';
        var mensaje3 = 'Debe Inidcar un Nombre para la ATA de Puerto';
        var mensaje4 = 'No se puede eliminar la ATA de Puerto, ya que está asociada a un Embarque';
        break;

      case 'Agencia':
        var abm: AgenciaMaritimaPuerto = new AgenciaMaritimaPuerto('', '');
        var list = 'agenciasList';
        var opcionABM = this.opcionABMSeleccionada == 'Agregar' ?
          'agregarAgenciaMaritimaPuerto' : accion == 'Guardar' ?
            'modificarAgenciaMaritimaPuerto' : 'eliminarAgenciaMaritimaPuerto';
        var obtener = 'obtenerListadoAgenciasMaritimas';
        var modelo = AgenciaMaritimaPuerto;
        var mensaje1 = this.opcionABMSeleccionada == 'Agregar' ?
          'Ha cargado con éxito una nueva Agencia Maritima de Puerto' : accion == 'Guardar' ?
            'Ha modificado con éxito la Agencia Maritima de Puerto' : 'Ha eliminado con éxito la Agencia Maritima de Puerto';
        var mensaje2 = accion == 'Guardar' ? 'Los datos de esta Agencia Maritima de Puerto ya existen' :
          'Los datos de esta Agencia Maritima de Puerto NO existen';
        var mensaje3 = 'Debe Inidcar un Nombre para la Agencia Maritima de Puerto';
        var mensaje4 = 'No se puede eliminar la Agencia Maritima de Puerto, ya que está asociada a un Embarque';
        break;

      case 'Motivo':
        var abm: MotivosLimpieza = new MotivosLimpieza('', '');
        var list = 'motivosLimpiezaList';
        var opcionABM = this.opcionABMSeleccionada == 'Agregar' ?
          'agregarMotivoLimpieza' : accion == 'Guardar' ?
            'modificarMotivoLimpieza' : 'eliminarMotivoLimpieza';
        var obtener = 'obtenerListadoMotivosLimpieza';
        var modelo = MotivosLimpieza;
        var mensaje1 = this.opcionABMSeleccionada == 'Agregar' ?
          'Ha cargado con éxito un nuevo Motivo de Limpieza' : accion == 'Guardar' ?
            'Ha modificado con éxito un Motivo de Limpieza' : 'Ha eliminado con éxito un Motivo de Limpieza';
        var mensaje2 = accion == 'Guardar' ? 'Los datos de este Motivo de Limpieza ya existen' :
          'Los datos de este Motivo de Limpieza NO existen';
        var mensaje3 = 'Debe Inidcar un Nombre para el Motivo de Limpieza';
        var mensaje4 = 'No se puede eliminar el Motivo de Limpieza, ya que está asociada a un Embarque';
        break;
    }

    //**Guardar de Agregar y Modificar***//
    if (accion == 'Guardar') {
      if (this.opcionABMSeleccionada == 'Modificar-Eliminar')
        abm.id = this.embarqueForm.get([list]).value[0].id;

      abm.nombre = (<HTMLInputElement>document.getElementById("nombre")).value;

      if (abm.nombre.replace(/\s/g, "").length > 0) {
        if (this.opcionABMSeleccionada == 'Agregar')
          var datosUnicos = this[list].find(x => x.nombre == abm.nombre);
        else
          var datosUnicos = this[list].find(x => x.id != abm.id && x.nombre == abm.nombre);

        if (typeof datosUnicos == 'undefined') {
          this.embarqueService[opcionABM](abm).subscribe(res => {
            this.modalService.dismissAll();

            this.embarqueService[obtener]().subscribe(res => {
              this[list] = res.map(x => new modelo(x.id, x.nombre));
            });

            if (this.opcionABMSeleccionada == 'Agregar') {
              this.embarqueService[obtener]().subscribe(res => {
                this.embarqueForm.get([list]).setValue(
                  res.filter(x => x.nombre == abm.nombre).map(x => new modelo(x.id, x.nombre)));
              });
            }
            else {
              this.embarqueService[obtener]().subscribe(res => {
                this.embarqueForm.get([list]).setValue(
                  res.filter(x => x.id == abm.id).map(x => new modelo(x.id, x.nombre)));
              });
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
      abm.id = this.embarqueForm.get([list]).value[0].id;

      var datosUnicos = this[list].find(x => x.id != abm.id);

      if (typeof datosUnicos != 'undefined') {
        this.embarqueService[opcionABM](abm.id).subscribe(res => {
          this.modalService.dismissAll();

          if (res) {
            this.embarqueService[obtener]().subscribe(res => {
              this[list] = res.map(x => new modelo(x.id, x.nombre));
            });

            this.embarqueForm.get([list]).setValue('');

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
}
