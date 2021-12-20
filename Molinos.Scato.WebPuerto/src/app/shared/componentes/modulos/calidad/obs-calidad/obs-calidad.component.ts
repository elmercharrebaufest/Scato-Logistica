import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-obs-calidad',
  templateUrl: './obs-calidad.component.html',
  styleUrls: ['./obs-calidad.component.css']
})
export class ObsCalidadComponent implements OnInit {
  obsCalidadForm: FormGroup;
  
  constructor(
    private formBuilder: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initFormulario();
  }
  initFormulario(){
    this.obsCalidadForm = this.formBuilder.group({
      id: '',
      fecha: '',
      hora: '',
      observaciones: ''
    })
  }

  updateObsCalidad(obsCalidad){
    this.obsCalidadForm.patchValue(obsCalidad);
  }

  obtenerDatosObsCalidad(){
    return this.obsCalidadForm.getRawValue();
  }
}
