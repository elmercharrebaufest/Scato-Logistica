import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { PermisosScato } from '@ScatoEnums/permisos-scato';
import { EstadoPuerto } from '@ScatoModels/estado-puerto';
import { AutenticadorService } from '@ScatoServicios/autenticador.service';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { LineupService } from '@ScatoServicios/lineup.service';

@Component({
  selector: 'app-estados-puerto-content',
  templateUrl: './estados-puerto-content.component.html',
  styleUrls: ['./estados-puerto-content.component.css']
})
export class EstadosPuertoContentComponent implements OnInit {
  estadoPuerto: EstadoPuerto;
  permisosScato: typeof PermisosScato = PermisosScato;
  estadoCalado: string;
  estadoUbicacion: string;
  estadoAltura: string;

  constructor(
    private lineupService: LineupService,
    public autenticadorService: AutenticadorService,
    private _procesoService:DatosEmbarquesProcesoService
    ) { }

  ngOnInit(): void {
    this.lineupService.obtenerEstadoPuerto().subscribe(x => { 
      this.estadoPuerto = x; this.cargarEstadosPuerto(); this.enviarEstadoAlturaValor();});
  }

  public cargarEstadosPuerto(){
    this.estadoCalado = this.estadoPuerto.calado == null ? '' : this.estadoPuerto.calado;
    this.estadoUbicacion = this.estadoPuerto.ubicacion == null ? '' : this.estadoPuerto.ubicacion;
    this.estadoAltura = this.estadoPuerto.alturaDelRio == null ? '' : this.estadoPuerto.alturaDelRio;
  }

  public modificarEstadosPuerto(){
    var estado : EstadoPuerto = this.estadoPuerto;
    if (this.estadoCalado != (this.estadoPuerto.calado == null ? '' : this.estadoPuerto.calado)){
      estado.calado = this.estadoCalado;
      estado.fechaCalado = new Date();
    }

    if (this.estadoUbicacion != (this.estadoPuerto.ubicacion == null ? '' : this.estadoPuerto.ubicacion)){
      estado.ubicacion = this.estadoUbicacion;
      estado.fechaUbicacion = new Date();
    }

    if (this.estadoAltura != (this.estadoPuerto.alturaDelRio == null ? '' : this.estadoPuerto.alturaDelRio)){
      estado.alturaDelRio = this.estadoAltura;
      estado.fechaAlturaRio = new Date();
      this.enviarEstadoAlturaValor();
    }     

    this.lineupService.modificarEstadosPuerto(estado).subscribe(ret => console.log('EstadoPuertoModificado'));
  }

  enviarEstadoAlturaValor(){
    this._procesoService.setEstadoAltura(Number(this.estadoAltura))
  }
}