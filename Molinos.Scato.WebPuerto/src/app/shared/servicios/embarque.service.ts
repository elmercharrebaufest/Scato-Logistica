import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
import { Embarque } from '@ScatoModels/embarque';
import { MaterialPuerto } from '@ScatoModels/material-puerto';
import { AgenciaMaritimaPuerto } from '@ScatoModels/agencia-maritima-puerto';
import { MotivosLimpieza } from '@ScatoModels/motivo-limpieza';
import { CoordinadorPuerto } from '@ScatoModels/coordinador-puerto';
import { ATAPuerto } from '@ScatoModels/ata-puerto';
import { TipoDeBuquePuerto } from '@ScatoModels/tipo-de-buque-puerto';
import { UbicacionDeBuquePuerto } from '@ScatoModels/ubicacion-de-buque-puerto';

@Injectable({
  providedIn: 'root'
})
export class EmbarqueService {

  url: string = environment.apiUrl;

  constructor(private http: HttpClient) { 

  }

  altaEmbarque(embarque: Embarque){
    return this.http.post(`${this.url}Embarque/AltaEmbarque`, embarque, { 'withCredentials' : true});
  }

  modificarEmbarque(embarque: Embarque){
    return this.http.post(`${this.url}Embarque/ModificarEmbarque`, embarque, { 'withCredentials' : true});
  }

  obtenerEmbarque(id: number){
    return this.http.get<Embarque>(`${this.url}Embarque/ObtenerEmbarque?id=`+id, { 'withCredentials' : true});
  }
  obtenerListadoMateriales(): Observable<MaterialPuerto[]>{
    return this.http.get<MaterialPuerto[]>(`${this.url}Embarque/ListarMateriales`, { 'withCredentials' : true});
  }

  obtenerListadoAgenciasMaritimas(): Observable<AgenciaMaritimaPuerto[]> {
    return this.http.get<AgenciaMaritimaPuerto[]>(`${this.url}Embarque/ListarAgenciasMaritimas`, { 'withCredentials': true });
  }

  obtenerListadoMotivosLimpieza(): Observable<MotivosLimpieza[]> {
    return this.http.get<MotivosLimpieza[]>(`${this.url}Embarque/ListarMotivosLimpieza`, { 'withCredentials': true });
  }

  obtenerListadoCoordinadores(): Observable<CoordinadorPuerto[]> {
    return this.http.get<CoordinadorPuerto[]>(`${this.url}Embarque/ListarCoordinadores`, { 'withCredentials': true });
  }

  obtenerListadoATAPuerto(): Observable<ATAPuerto[]> {
    return this.http.get<ATAPuerto[]>(`${this.url}Embarque/ListarATAPuerto`, { 'withCredentials': true });
  }

  obtenerListadoTipoDeBuquePuerto(): Observable<TipoDeBuquePuerto[]> {
    return this.http.get<TipoDeBuquePuerto[]>(`${this.url}Embarque/ListarTipoDeBuquePuerto`, { 'withCredentials': true });
  }

  obtenerListadoUbicacionDeBuquePuerto(): Observable<UbicacionDeBuquePuerto[]> {
    return this.http.get<UbicacionDeBuquePuerto[]>(`${this.url}Embarque/ListarUbicacionDeBuquePuerto`, { 'withCredentials': true });
  }

  agregarAgenciaMaritimaPuerto(agencia: AgenciaMaritimaPuerto) {
    return this.http.post(`${this.url}Embarque/AgregarAgenciaMaritimaPuerto`, agencia, { 'withCredentials': true });
  }
  modificarAgenciaMaritimaPuerto(agencia: AgenciaMaritimaPuerto) {
    return this.http.post(`${this.url}Embarque/ModificarAgenciaMaritimaPuerto`, agencia, { 'withCredentials': true });
  }
  eliminarAgenciaMaritimaPuerto(agenciaId: number) {
    return this.http.post(`${this.url}Embarque/EliminarAgenciaMaritimaPuerto?agenciaId=`+agenciaId, { 'withCredentials': true });
  }

  agregarMotivoLimpieza(motivo: MotivosLimpieza) {
    return this.http.post(`${this.url}Embarque/AgregarMotivosLimpieza`, motivo, { 'withCredentials': true });
  }
  modificarMotivoLimpieza(motivo: MotivosLimpieza) {
    return this.http.post(`${this.url}Embarque/ModificarMotivosLimpieza`, motivo, { 'withCredentials': true });
  }
  eliminarMotivoLimpieza(motivoId: number) {
    return this.http.post(`${this.url}Embarque/EliminarMotivosLimpieza?motivosLimpiezaId=`+motivoId, { 'withCredentials': true });
  }

  agregarCoordinadorPuerto(coordinador: CoordinadorPuerto) {
    return this.http.post(`${this.url}Embarque/AgregarCoordinadorPuerto`, coordinador, { 'withCredentials': true });
  }
  modificarCoordinadorPuerto(coordinador: CoordinadorPuerto) {
    return this.http.post(`${this.url}Embarque/ModificarCoordinadorPuerto`, coordinador, { 'withCredentials': true });
  }
  eliminarCoordinadorPuerto(coordinadorId: number) {
    return this.http.post(`${this.url}Embarque/EliminarCoordinadorPuerto?coordinadorId=`+coordinadorId, { 'withCredentials': true });
  }

  agregarATAPuerto(ATA: ATAPuerto) {
    return this.http.post(`${this.url}Embarque/AgregarATAPuerto`, ATA, { 'withCredentials': true });
  }
  modificarATAPuerto(ATA: ATAPuerto) {
    return this.http.post(`${this.url}Embarque/ModificarATAPuerto`, ATA, { 'withCredentials': true });
  }
  eliminarATAPuerto(ATAId: number) {
    return this.http.post(`${this.url}Embarque/EliminarATAPuerto?ATAId=`+ATAId, { 'withCredentials': true });
  }

  actualizarEstadoBuque(embarqueId: number, orden: number) {
    return this.http.post(`${this.url}Embarque/ActualizarEstadoBuque?embarqueId=${embarqueId}&orden=${orden}`, { 'withCredentials': true });
  }
}
