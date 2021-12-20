import { ChangeDetectorRef, Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { Tipoalerta } from '@ScatoEnums/tipo-alerta';
import { MaterialPuerto } from '@ScatoModels/material-puerto';
import { EmbarqueNav } from '@ScatoModels/embarque-nav';
import { LineasDeEmbarque } from '@ScatoModels/linea-embarque';
import { ConfirmationDialogService } from '@ScatoServicios/confirmation-dialog.service';
import { EmbarqueService } from '@ScatoServicios/embarque.service';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { EstadoTanquesService } from '@ScatoServicios/estado-tanques.service';
import { ModuloDeCarga } from '@ScatoModels/modulo-carga';
import { pairwise, startWith } from 'rxjs/operators';
import { LineasService } from '@ScatoServicios/lineas.service';

@Component({
  selector: 'app-lineas',
  templateUrl: './lineas.component.html',
  styleUrls: ['./lineas.component.css']
})
export class LineasComponent implements OnInit {
  embarque: EmbarqueNav;
  formInitialValues: any;
  tanqueSi: boolean;
  tanquesOption: any;
  tanquesOptionAux: any[] = new Array();
  materialesPuerto: MaterialPuerto[];
  lineas = [{ idLinea: 0, nombreLinea: '' }, { idLinea: 1, nombreLinea: 'Nueva' }, { idLinea: 2, nombreLinea: 'Vieja' },
  { idLinea: 3, nombreLinea: 'Vicentin' }, { idLinea: 4, nombreLinea: 'Biodiesel' }];
  lineasDeEmbarqueForm: FormGroup;
  colorSelected: any[] = new Array();

  moduloDeCarga: ModuloDeCarga;

  constructor(
    private formBuilder: FormBuilder,
    private confirmationDialogService: ConfirmationDialogService,
    private embarqueService: EmbarqueService,
    private _procesoService: DatosEmbarquesProcesoService,
    private _tanquesService: EstadoTanquesService,
    private _lineasService: LineasService,
    private changeDet: ChangeDetectorRef
  ) {

    this._tanquesService.sendData.subscribe(resObj => {
      let tanks = new Array();
      for (var [key, value] of Object.entries(resObj.value)) {
        let tank = { [key]: value, value: key.substr(6), color: (value ? 'tanqueSi' : 'tanqueNo') }
        tanks.push(tank);
      }

      this.tanquesOption = tanks;
    });
  }

  ngOnInit(): void {
    this._procesoService.sendEmbarque.subscribe(
      res => this.embarque = res
    )
    if (!this.embarque)
      this.embarque = this._procesoService.getEmbarqueSelected();
    this.cargarEmbarque(this.embarque.id);

    this.lineasDeEmbarqueForm = this.formBuilder.group({
      lineasEmbarque: this.formBuilder.array([this.initLineasEmbarque()]),
    });

    this.formInitialValues = this.lineasDeEmbarqueForm.getRawValue();
    this.obtenerModuloDeCarga();

    this.lineasDeEmbarqueForm.get('lineasEmbarque')['controls'].forEach((linea, indexLinea) => {
      linea.controls['alturaInicialCM'].valueChanges.pipe(startWith(null as object), pairwise())
        .subscribe(([previous, current]) => {
          linea.controls['alturaInicialMM'].setValue(null, { emitEvent: false });
          if (current) {
            linea.controls['alturaInicialMM'].enable({ emitEvent: false });
          } else {
            linea.controls['alturaInicialMM'].disable({ emitEvent: false });
          }
        });

      linea.controls['alturaInicialMM'].valueChanges.pipe(startWith(null as object), pairwise())
        .subscribe(([previous, current]) => {
          if (current && linea.controls['alturaInicialCM'].value && linea.controls['tkInicial']) {
            this._lineasService.obtenerLlenadoMilimetroPorTanque(linea.controls['alturaInicialCM'].value, current, '0' + linea.controls['tkInicial'].value.value)
              .subscribe(res => {
                linea.controls['litros'].setValue(res, { emitEvent: false });
                if (linea.controls['densidadInicial'].value) {
                  let kilosInicial = (Number(res) * Number(linea.controls['densidadInicial'].value));
                  linea.controls['kilos'].setValue(kilosInicial, { emitEvent: false })
                }
              });
          } else {
            linea.controls['litros'].setValue(null, { emitEvent: false });
          }
        });

      linea.controls['alturaFinalCM'].valueChanges.pipe(startWith(null as object), pairwise())
        .subscribe(([previous, current]) => {
          linea.controls['alturaFinalMM'].setValue(null, { emitEvent: false });
          if (current) {
            linea.controls['alturaFinalMM'].enable({ emitEvent: false });
          } else {
            linea.controls['alturaFinalMM'].disable({ emitEvent: false });
          }
        });

      linea.controls['alturaFinalMM'].valueChanges.pipe(startWith(null as object), pairwise())
        .subscribe(([previous, current]) => {
          console.log(linea.controls['tkInicial'].value.value);
          if (current && linea.controls['alturaFinalCM'].value && linea.controls['tkInicial']) {
            this._lineasService.obtenerLlenadoMilimetroPorTanque(linea.controls['alturaFinalCM'].value, current, '0' + linea.controls['tkInicial'].value.value)
              .subscribe(res => {
                let litrosFinal = res;
                let densidadFinal = linea.controls['densidadFinal'].value;
                if (litrosFinal && densidadFinal && linea.controls['kilos'].value) {
                  let kilosFinal = Number((Number(densidadFinal) * Number(litrosFinal)));
                  linea.controls['tkFinal'].setValue(Number(linea.controls['kilos'].value) - kilosFinal, { emitEvent: false })
                }
              });
          } else {
            linea.controls['tkFinal'].setValue(null, { emitEvent: false });
          }
        });

      linea.controls['temperaturaInicial'].valueChanges.pipe(startWith(null as object), pairwise())
        .subscribe(([previous, current]) => {
          if (current && linea.controls['materialPuerto'].value) {
            linea.controls['temperaturaFinal'].setValue(current, { emitEvent: false });
            this._lineasService.obtenerDensidadPorTemperaturaDeMaterial(linea.controls['materialPuerto'].value.id, current)
              .subscribe(res => {
                linea.controls['densidadInicial'].setValue(res, { emitEvent: false });
                linea.controls['densidadFinal'].setValue(res, { emitEvent: false });
                if (linea.controls['litros'].value) {
                  let kilosInicial = (Number(res) * Number(linea.controls['litros'].value)).toFixed(3);
                  linea.controls['kilos'].setValue(kilosInicial, { emitEvent: false })
                }
              });
          } else {
            linea.controls['temperaturaFinal'].setValue('', { emitEvent: false });
          }
        });

      // linea.controls['temperaturaFinal'].valueChanges.pipe(startWith(null as object), pairwise())
      //   .subscribe(([previous, current]) => {
      //     if(current && linea.controls['materialPuerto'].value){
      //       this._lineasService.obtenerDensidadPorTemperaturaDeMaterial(linea.controls['materialPuerto'].value.id, current)
      //         .subscribe(res => {
      //           linea.controls['densidadFinal'].setValue(res, { emitEvent: false });
      //         });
      //     }
      //   });

      // linea.controls['sarasa'].valueChanges.pipe(startWith(null as object), pairwise())
      //   .subscribe(([previous, current]) => {});
    });
  }

  cargarEmbarque(idEmbarque: number) {
    this.embarqueService.obtenerEmbarque(idEmbarque).subscribe(
      res => {
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
      }, () => {
        this.confirmationDialogService.confirm('¡Error!', `Error al obtener el embarque ${idEmbarque}`, 'Cerrar', '', null, null, Tipoalerta.Error);
      });
  }

  initLineasEmbarque(x: LineasDeEmbarque = null) {
    console.log(this.tanquesOption, x)
    return this.formBuilder.group({
      id: x?.id ?? "",
      linea: x?.linea ?? "",
      tkInicial: x ? this.tanquesOption.find(t => t.value == x.tkInicial) : '',
      materialPuerto: x?.materialPuerto ?? "",
      temperaturaInicial: [{ value: x && x.temperaturaInicial ? x.temperaturaInicial > 0 ? x.temperaturaInicial : "" : "", disabled: false }],
      alturaInicialCM: [{ value: x && x.alturaInicialCM ? x.alturaInicialCM > 0 ? x.alturaInicialCM : "" : "", disabled: false }],
      alturaInicialMM: [{ value: x && x.alturaInicialMM ? x.alturaInicialMM > 0 ? x.alturaInicialMM : "" : "", disabled: true }],
      densidadInicial: [{ value: x && x.densidadInicial ? x.densidadInicial > 0 ? x.densidadInicial : "" : "", disabled: true }],
      temperaturaFinal: [{ value: x && x.temperaturaFinal ? x.temperaturaFinal > 0 ? x.temperaturaFinal : "" : "", disabled: true }],
      litros: [{ value: x && x.litros ? x.litros > 0 ? x.litros : "" : "", disabled: true }],
      densidadFinal: [{ value: x && x.densidadFinal ? x.densidadFinal > 0 ? x.densidadFinal : "" : "", disabled: true }],
      alturaFinalCM: [{ value: x && x.alturaFinalCM ? x.alturaFinalCM > 0 ? x.alturaFinalCM : "" : "", disabled: false }],
      alturaFinalMM: [{ value: x && x.alturaFinalMM ? x.alturaFinalMM > 0 ? x.alturaFinalMM : "" : "", disabled: true }],
      kilos: [{ value: x && x.kilos ? x.kilos > 0 ? x.kilos : "" : "", disabled: true }],
      tkFinal: [{ value: x?.tkFinal ?? "", disabled: true }]
    });
  }

  get lineasEmbarque(): FormArray {
    return this.lineasDeEmbarqueForm.get("lineasEmbarque") as FormArray;
  }

  obtenerModuloDeCarga() {
    this.moduloDeCarga = this._procesoService.getModuloDeCarga();

    if (this.moduloDeCarga.moduloDeCargaLineasDeEmbarque && this.moduloDeCarga.moduloDeCargaLineasDeEmbarque.length > 0) {
      this.lineasEmbarque.clear();

      this.moduloDeCarga.moduloDeCargaLineasDeEmbarque.forEach((x, index) => {
        // this.setClase(x.tkInicial, index);
        this.lineasEmbarque.push(this.initLineasEmbarque(x))
      });
    }
  }

  compareLineas(c1: LineasDeEmbarque, c2: LineasDeEmbarque) {
    return c1 && c2 ? c1.id === c2.id : c1 === c2;
  }
  compareTanque(c1: any, c2: any) {
    return c1 && c2 ? c1.value === c2.value : c1 === c2;
  }

  setClase(index: number) {
    let item = this.tanquesOption?.find(x => x.value == this.lineasDeEmbarqueForm.get('lineasEmbarque')['controls'][index]['controls'].tkInicial?.value?.value)
    return item?.color;
  }

  agregarLineasEmbarque() {
    this.lineasEmbarque.push(this.initLineasEmbarque());
  }

  eliminarLineasEmbarque(pos: number) {
    this.lineasEmbarque.removeAt(pos);
    this.colorSelected.splice(pos, 1);
  }

  obtenerLineasEmbarque() {
    let lineas = this.lineasDeEmbarqueForm.getRawValue().lineasEmbarque;

    lineas.forEach( l => {
      l.tkInicial = l.tkInicial.value;
    });
    
    return lineas;
  }
}