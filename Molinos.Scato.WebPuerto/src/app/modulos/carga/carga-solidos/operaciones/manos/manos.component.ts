import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { pairwise, startWith } from 'rxjs/operators';
import { SentidoManoDeEmbarque } from '@ScatoModels/sentido-mano-embarque';
import { CeldaManoDeEmbarque } from '@ScatoModels/celda-mano-embarque';
import { ManosEmbarqueService } from '@ScatoServicios/manosEmbarque.service';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { MaterialPuerto } from '@ScatoModels/material-puerto';

@Component({
  selector: 'app-manos',
  templateUrl: './manos.component.html',
  styleUrls: ['./manos.component.css']
})
export class ManosComponent implements OnInit {
  @Input() sentidosManoDeEmbarque: SentidoManoDeEmbarque[];
  @Input() celdasManoDeEmbarque: CeldaManoDeEmbarque[];
  manosYTabiquesForm: FormGroup;
  formInitialValues: any;

  datosEmbarque: any;
  productos: MaterialPuerto[] = [];

  constructor(private formBuilder: FormBuilder, 
    private manosEmbarqueService: ManosEmbarqueService,
    private _procesoService: DatosEmbarquesProcesoService) { 

      this.datosEmbarque = this._procesoService.getDatosGrafico();
      this.productos = this.datosEmbarque.listaMateriales      
    }

  ngOnInit(): void {
    this.initFormulario();

    this.manosYTabiquesForm.get('manosDeEmbarque')['controls'].forEach((mano, currentIndexMano) => {
      mano.controls.moduloDeCargaManosDeEmbarqueDetalle.controls.forEach((datoCelda, currentIndexDatoCelda) => {
        datoCelda.controls['celdaManoDeEmbarque'].valueChanges.pipe(startWith(null as object), pairwise())
          .subscribe(([previous, current]) => {
            if (previous && datoCelda.controls['sentidoManoDeEmbarque'].value) {
              this.manosEmbarqueService.removerManoDeEmbarque.emit({celda: previous.nombre, sentido: datoCelda.controls['sentidoManoDeEmbarque'].value.posicion});
            }
            if (current) {
              // VERIFICAR SI EXISTE SILO 31 O 32. SI EXISTE NO PERMITIR MODIFICACION
              if(current.posicion==5 || current.posicion==6){
                let cantidadSiloCurrent = this.buscarSilosRestantes(current.posicion);
                if(cantidadSiloCurrent>1){

                  let texto = "Ya existe el Silo seleccionado";
                  this.confirmationDialogService.confirm('¡Atención!', texto, 'Aceptar', '', null, null, Tipoalerta.Success)
                    .then((confirmed) => {
                      if (confirmed) { } else return;
                    }).catch(() => window.location.reload());

                  // console.log(`Ya existe el Silo${current.posicion} - Ocurrencias: ${cantidadSiloCurrent}`);
                  
                  datoCelda.controls['celdaManoDeEmbarque'].setValue(null, { emitEvent: false });
                  datoCelda.controls['sentidoManoDeEmbarque'].disable({ emitEvent: false });
                  datoCelda.controls['sentidoManoDeEmbarque'].setValue(null, { emitEvent: false });
                }else{
                  datoCelda.controls['sentidoManoDeEmbarque'].enable({ emitEvent: false });
                  datoCelda.controls['sentidoManoDeEmbarque'].setValue(null, { emitEvent: false });
                }
              } else {
                datoCelda.controls['sentidoManoDeEmbarque'].enable({ emitEvent: false });
                datoCelda.controls['sentidoManoDeEmbarque'].setValue(null, { emitEvent: false });
              }
              // this.manosEmbarqueService.resaltarSilo.emit(datoCelda.controls['celdaManoDeEmbarque'].value.posicion);
            } else {
              datoCelda.controls['sentidoManoDeEmbarque'].disable({ emitEvent: false });
            }
            if (current?.posicion) {
              this.validarSentidos(currentIndexMano, currentIndexDatoCelda, current?.posicion);
              if ([11, 12, 13].includes(current?.posicion)) {
                datoCelda.controls['materialPuerto'].disable({ emitEvent: false });
                datoCelda.controls['porcentajePorMano'].disable({ emitEvent: false });
                datoCelda.controls['aperturaPorton'].disable({ emitEvent: false });
                datoCelda.controls['masProduccion'].disable({ emitEvent: false });
              } else {
                datoCelda.controls['materialPuerto'].enable({ emitEvent: false });
                datoCelda.controls['porcentajePorMano'].enable({ emitEvent: false });
                datoCelda.controls['aperturaPorton'].enable({ emitEvent: false });
                datoCelda.controls['masProduccion'].enable({ emitEvent: false });
              }
            }
          });
        datoCelda.controls['sentidoManoDeEmbarque'].valueChanges.pipe(startWith(null as object), pairwise())
          .subscribe(([previous, current]) => {
            if (datoCelda.controls['celdaManoDeEmbarque'].value) {
              if (previous) {
                this.manosEmbarqueService.removerManoDeEmbarque.emit({celda: datoCelda.controls['celdaManoDeEmbarque'].value.nombre, sentido: previous.posicion});
              }
              this.manosEmbarqueService.removerResaltadoSilos.emit();
              // this.manosEmbarqueService.removerManoDeEmbarque.emit(datoCelda.controls['celdaManoDeEmbarque'].value.nombre);
              if (current){
                if(datoCelda.controls['celdaManoDeEmbarque'].value.nombre.includes('Silo')){
                  this.manosEmbarqueService.resaltarSilo.emit(datoCelda.controls['celdaManoDeEmbarque'].value.posicion);
                }
                this.manosEmbarqueService.agregarManoDeEmbarque.emit({celda: datoCelda.controls['celdaManoDeEmbarque'].value.nombre, sentido: current.posicion });
              } 
            }
          });
      });
    });

    this.manosYTabiquesForm.get('tabiques')['controls'].forEach((tabique, index) => {
      tabique.controls['entreColumna'].valueChanges
        .subscribe((current) => {
          let tabiqueVal = tabique.controls['tabique'].value;

          if (current || current == 0) {
            if(Number(current)  > 28 && index == 0){
              tabique.patchValue({ entreColumna: 28, yColumna: 29 });
            } else if(Number(current)  > 11 && index == 1){
              tabique.patchValue({ entreColumna: 11, yColumna: 12 });
            }else if(Number(current)  < 1 && current !== null && current !== undefined){
              tabique.patchValue({ entreColumna: 1, yColumna: 2 });
            } else {
              tabique.patchValue({ yColumna: Number(current) + 1 });
            }
          } else {
            this.manosEmbarqueService.removerTabique.emit(tabiqueVal);
            tabique.patchValue({ yColumna: '' });
          }

          let yColumna = tabique.controls['yColumna'].value;
          if (current && tabique && yColumna) {
            this.manosEmbarqueService.agregarTabique.emit({celda: tabiqueVal, tabiqueDesde: current, tabiqueHasta: yColumna });
          }
        });
    });
  }

  initFormulario() {
    this.manosYTabiquesForm = this.formBuilder.group({
      manosDeEmbarque: this.formBuilder.array([this.initManoDeEmbarque(1), this.initManoDeEmbarque(2)]),
      tabiques: this.formBuilder.array([this.initTabique(20), this.initTabique(7)])
    });
    this.formInitialValues = this.manosYTabiquesForm.getRawValue();
  }

  initManoDeEmbarque(numeroMano) {
    return this.formBuilder.group({
      id: '',
      mano: [{ value: numeroMano, disabled: true }],
      moduloDeCargaManosDeEmbarqueDetalle: this.formBuilder.array([this.initDatosCeldas(), this.initDatosCeldas(), this.initDatosCeldas(), this.initDatosCeldas(), this.initDatosCeldas(), this.initDatosCeldas()]),
      observaciones: ''
    });
  }

  initDatosCeldas() {
    return this.formBuilder.group({
      id: '',
      celdaManoDeEmbarque: null,
      sentidoManoDeEmbarque: [{ value: null, disabled: true }],//this.formBuilder.group({id:'',nombre:''})
      porcentajePorMano: '',
      aperturaPorton: null,
      masProduccion: null,
      materialPuerto: null
    });
  }

  initTabique(numeroTabique) {
    return this.formBuilder.group({
      id: '',
      tabique: [{ value: numeroTabique, disabled: true }],
      entreColumna: '',
      yColumna: [{ value: '', disabled: true }]
    });
  }

  validarSentidos(indexMano, indexDatoCelda, celdaId) {
    let sentidoAMostrar = document.querySelectorAll('[class*="sentido' + indexMano + '-' + indexDatoCelda + '"]');
    for (let sentido of sentidoAMostrar) sentido.classList.remove("hideSentido");
    let validacionSentidos = {
      '1': [1, 2, 3, 4, 9, 11],
      '2': [5, 6, 7, 8, 9, 11],
      '3': [5, 6, 7, 8, 9],
      '4': [5, 6, 7, 8, 9],
      '5': [1, 2, 3, 4, 5, 6, 7, 8, 10],
      '6': [1, 2, 3, 4, 5, 6, 7, 8, 10],
      '7': [1, 2, 3, 4, 5, 6, 7, 8, 10],
      '8': [1, 2, 3, 4, 5, 6, 7, 8, 10],
      '9': [1, 2, 3, 4, 5, 6, 7, 8, 10],
      '10': [1, 2, 3, 4, 5, 6, 7, 8, 10],
      '11': [1, 2, 3, 4, 5, 6, 7, 8, 9],
      '12': [1, 2, 3, 4, 5, 6, 7, 8, 9],
      '13': [1, 2, 3, 4, 5, 6, 7, 8, 9],
      '14': [1, 2, 3, 4, 5, 6, 7, 8, 10],
      '15': [1, 2, 3, 4, 5, 6, 7, 8, 10]
    }
    for (let elemento of validacionSentidos[celdaId]) {
      let sentidoAOcultar = document.querySelector(`.sentido${indexMano}-${indexDatoCelda}-${elemento}`);
      sentidoAOcultar.classList.add('hideSentido');
    }
  }

  compareMateriales(c1: MaterialPuerto, c2: MaterialPuerto) {
    return c1 && c2 ? c1.id === c2.id : c1 === c2;
  }

  compareCeldas(c1: CeldaManoDeEmbarque, c2: CeldaManoDeEmbarque) {
    return c1 && c2 ? c1.posicion === c2.posicion : c1 === c2;
  }

  compareSentidos(c1: SentidoManoDeEmbarque, c2: SentidoManoDeEmbarque) {
    return c1 && c2 ? c1.posicion === c2.posicion : c1 === c2;
  }
  
  resetForm(){
    this.manosYTabiquesForm.reset(this.formInitialValues);
  }

  obtenerManosDeEmbarque(){
    return this.manosYTabiquesForm.getRawValue().manosDeEmbarque;
  }

  obtenerTabiques(){
    return this.manosYTabiquesForm.getRawValue().tabiques;
  }

  patchManosDeEmbarque(manos){
    this.manosYTabiquesForm.get('manosDeEmbarque').patchValue(manos);
  }

  patchTabiques(tabiques){
    this.manosYTabiquesForm.get('tabiques').patchValue(tabiques);
  }
}