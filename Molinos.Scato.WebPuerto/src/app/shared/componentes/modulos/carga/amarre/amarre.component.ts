import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-amarre',
  templateUrl: './amarre.component.html',
  styleUrls: ['./amarre.component.css']
})
export class AmarreComponent implements OnInit {
  public solidosForm: FormGroup;
  constructor(
    private _builder: FormBuilder
  ) { }

  ngOnInit(): void {
    this.newForm();
  }

  newForm(){
    this.solidosForm = this._builder.group({
      amarro: this.initAmarre(),
      desamarro: this.initAmarre(),
      habilitacion: this.initFechaHora()
    })
  }

  initFechaHora(){
    return this._builder.group({
      id: '',
      fecha: '',
      hora: '',
    });
  }

  initAmarre(){
    return this._builder.group({
      id: '',
      fecha: '',
      hora: '',
      viento: '',
      direccion: ''
    });
  }

  validarAMPM(event){
    
  }

  guardarAmarre(){
    
  }
}
