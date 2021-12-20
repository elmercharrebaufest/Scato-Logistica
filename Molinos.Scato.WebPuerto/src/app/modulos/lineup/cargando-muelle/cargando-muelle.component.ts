import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { InstanciaWorkflowPuerto } from '@ScatoModels/instancia-wokflow-puerto';
import { MaterialPuertoCantidad } from '@ScatoModels/material-puerto-cantidad';
import { BalanzaService } from '@ScatoServicios/balanza.service';
import { Balanza } from '@ScatoModels/balanza';

@Component({
  selector: 'app-cargando-muelle',
  templateUrl: './cargando-muelle.component.html',
  styleUrls: ['./cargando-muelle.component.css']
})
export class CargandoMuelleComponent implements OnInit {
  @Input() instanciaWorkflow: InstanciaWorkflowPuerto;
  balanzas:  Balanza[];
  valorRitmo: number = 80;
  colorRitmo: string = '#28a745';
  constructor(
    private router: Router,
    private balanzaService: BalanzaService) { }

  ngOnInit(): void {
    if(this.instanciaWorkflow)
      this.balanzaService.listarBalanzadaBuque(this.instanciaWorkflow.embarque.vapor.id).subscribe(res => this.balanzas = res);
  }

  public fechaRecaladaCorrecta() {
    var date = new Date();
    
    // if(!this.instanciaWorkflow.embarque.fechaRecalada) return 'warning';
    // if(new Date(this.instanciaWorkflow.embarque.fechaRecalada).getTime() > date.getTime()) return 'success';
    return 'danger';
  }

  public editarEmbarque() {
    this.router.navigate([`/lineup/alta-embarque/${this.instanciaWorkflow.embarque.id}/lineup`]);
  }

  extraeNombre(objeto) : string 
  {
    return objeto != null ? objeto.nombre.toString() : '';
  }

  get filteredMaterialList() : MaterialPuertoCantidad[]
  {return this.instanciaWorkflow.embarque.materialesPuertoCantidad.filter(x => x.cantidad > 0);}
}