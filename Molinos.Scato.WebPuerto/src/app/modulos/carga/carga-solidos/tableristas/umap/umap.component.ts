import { Component, OnInit } from '@angular/core';
import { FormArray, FormGroup } from '@angular/forms';
import { FormUmap } from '@ScatoModels/form-umap';

@Component({
  selector: 'app-umap',
  templateUrl: './umap.component.html',
  styleUrls: ['./umap.component.css']
})
export class UmapComponent implements OnInit {
  public forms: FormGroup[];
  constructor() { }

  ngOnInit(): void {
    this.forms = [new FormUmap().formulario];
  }

  guardarUMAP(){
    for (let form of this.forms){
      console.log(form)
    }
  }

  addTimeRow(){
    let newTime = new FormUmap().formulario;
    this.forms.push(newTime);
  }

  deleteHorario(form: FormGroup){
    let index = this.forms.indexOf(form);
    this.forms.splice(index, 1);
  }
}
