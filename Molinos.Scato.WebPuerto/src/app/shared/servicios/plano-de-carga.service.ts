import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
import { Destino } from '@ScatoModels/destino';
import { Exportador } from '@ScatoModels/exportador';
import { Estiba } from '@ScatoModels/estiba';
import { AgenciaControlPrivado } from '@ScatoModels/agencia-control-privado';
import { AgenteControlPrivado } from '@ScatoModels/agente-control-privado';
import { PlanoDeCarga } from '@ScatoModels/plano-de-carga';
import { Mail } from '@ScatoModels/mail';
import { Embarque } from '@ScatoModels/embarque';


@Injectable({
  providedIn: 'root'
})
export class PlanoDeCargaService {

  url: string = environment.apiUrl;

  constructor(private http: HttpClient) { 

  }

  obtenerDestinos(): Observable<Destino[]>{
    return this.http.get<Destino[]>(`${this.url}PlanoDeCarga/ListarDestinos`, { 'withCredentials' : true});
  }

  obtenerExportadores(): Observable<Exportador[]>{
    return this.http.get<Exportador[]>(`${this.url}PlanoDeCarga/ListarExportadores`, { 'withCredentials' : true});
  }

  agregarExportador(exportador: Exportador) {
    return this.http.post(`${this.url}PlanoDeCarga/AgregarExportador`, exportador, { 'withCredentials' : true});
  }
  modificarExportador(exportador: Exportador) {
    return this.http.post(`${this.url}PlanoDeCarga/ModificarExportador`, exportador, { 'withCredentials' : true});
  }
  eliminarExportador(exportadorId: number) {
    return this.http.post(`${this.url}PlanoDeCarga/EliminarExportador?exportadorId=`+exportadorId, { 'withCredentials': true });
  }

  obtenerListadoEstibas(): Observable<Estiba[]>{
    return this.http.get<Estiba[]>(`${this.url}PlanoDeCarga/ListarEstibas`, { 'withCredentials' : true});
  }

  obtenerListadoAgenciasControlPrivado(): Observable<AgenciaControlPrivado[]>{
    return this.http.get<AgenciaControlPrivado[]>(`${this.url}PlanoDeCarga/ListarAgenciasControlPrivado`, { 'withCredentials' : true});
  }

  obtenerListadoAgentesControlPrivado(): Observable<AgenteControlPrivado[]>{
    return this.http.get<AgenteControlPrivado[]>(`${this.url}PlanoDeCarga/ListarAgentesControlPrivado`, { 'withCredentials' : true});
  }
  
  guardarPlanoDeCarga(planoDeCarga: PlanoDeCarga){
    return this.http.post(`${this.url}PlanoDeCarga/GuardarPlanoDeCarga`, planoDeCarga, { 'withCredentials' : true});
  }

  obtenerPlanoDeCarga(planoDeCargaId: number) {
    return this.http.get<PlanoDeCarga>(`${this.url}PlanoDeCarga/ObtenerPlanoDeCarga?id=`+planoDeCargaId, { 'withCredentials' : true});
  }

  agregarEstiba(estiba: Estiba) {
    return this.http.post(`${this.url}PlanoDeCarga/AgregarEstiba`, estiba, { 'withCredentials' : true});
  }
  modificarEstiba(estiba: Estiba) {
    return this.http.post(`${this.url}PlanoDeCarga/ModificarEstiba`, estiba, { 'withCredentials' : true});
  }
  eliminarEstiba(estibaId: number) {
    return this.http.post(`${this.url}PlanoDeCarga/EliminarEstiba?estibaId=`+estibaId, { 'withCredentials': true });
  }

  agregarAgenciaControlPrivado(agencia: AgenciaControlPrivado) {
    return this.http.post(`${this.url}PlanoDeCarga/AgregarAgenciaControlPrivado`, agencia, { 'withCredentials' : true});
  }
  modificarAgenciaControlPrivado(agencia: AgenciaControlPrivado) {
    return this.http.post(`${this.url}PlanoDeCarga/ModificarAgenciaControlPrivado`, agencia, { 'withCredentials' : true});
  }
  eliminarAgenciaControlPrivado(agenciaId: number) {
    return this.http.post(`${this.url}PlanoDeCarga/EliminarAgenciaControlPrivado?agenciaId=`+agenciaId, { 'withCredentials': true });
  }

  agregarAgenteControlPrivado(agente: AgenteControlPrivado) {
    return this.http.post(`${this.url}PlanoDeCarga/AgregarAgenteControlPrivado`, agente, { 'withCredentials' : true});
  }
  modificarAgenteControlPrivado(agente: AgenteControlPrivado) {
    return this.http.post(`${this.url}PlanoDeCarga/ModificarAgenteControlPrivado`, agente, { 'withCredentials' : true});
  }
  eliminarAgenteControlPrivado(agenteId: number) {
    return this.http.post(`${this.url}PlanoDeCarga/EliminarAgenteControlPrivado?agenteId=`+agenteId, { 'withCredentials': true });
  }

  obtenerDestinatariosPlanoDeCarga() {
    return this.http.get<string[]>(`${this.url}PlanoDeCarga/ObtenerDestinatariosPlanoDeCarga`, { 'withCredentials' : true});
  }
  enviarPorMail(mail: Mail, planoDeCargaId : number): any {
    return this.http.post(`${this.url}PlanoDeCarga/EnviarPorMail?planoDeCargaId=`+planoDeCargaId, mail, { 'withCredentials': true });
  }

  obtenerBodyPlanoDeCarga(planoDeCargaId: number, embarque: Embarque) {
    return this.http.post<string>(`${this.url}PlanoDeCarga/ObtenerBodyPlanoDeCarga?planoDeCargaId=`+planoDeCargaId, embarque, { 'withCredentials' : true});
  }

}
