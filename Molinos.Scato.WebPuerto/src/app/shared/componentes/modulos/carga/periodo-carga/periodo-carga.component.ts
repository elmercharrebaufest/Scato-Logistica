import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-periodo-carga',
  templateUrl: './periodo-carga.component.html',
  styleUrls: ['./periodo-carga.component.css']
})
export class PeriodoCargaComponent implements OnInit {

  periodoCargaForm: FormGroup;
  
  constructor(
    private formBuilder: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initFormulario();
  }
  initFormulario(){
    this.periodoCargaForm = this.formBuilder.group({
      amarro: this.initAmarre(),
      desamarro: this.initAmarre(),
      habilitacion: this.initFechaHora(),
      conexionManguera: this.initFechaHora(),
      desconexionManguera: this.initFechaHora(),
      comienzoCarga: this.initFechaHora(),
      finCarga: this.initFechaHora()
    })
  }

  initFechaHora(){
    return this.formBuilder.group({
      id: '',
      fecha: '',
      hora: '',
    });
  }

  initAmarre(){
    return this.formBuilder.group({
      id: '',
      fecha: '',
      hora: '',
      viento: '',
      direccion: ''
    });
  }

  updatePeriodoCarga(periodoCarga){
    this.periodoCargaForm.patchValue(periodoCarga);
  }

  obtenerDatosPeriodoCarga(){
    return this.periodoCargaForm.getRawValue();
  }

  clForm(){
    console.log(this.periodoCargaForm.getRawValue());
  }
}
