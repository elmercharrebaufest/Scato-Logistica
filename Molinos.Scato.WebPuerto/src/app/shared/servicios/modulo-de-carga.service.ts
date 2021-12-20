import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
import { SentidoManoDeEmbarque } from '@ScatoModels/sentido-mano-embarque';
import { CeldaManoDeEmbarque } from '@ScatoModels/celda-mano-embarque';
import { ModuloDeCarga } from '@ScatoModels/modulo-carga';
import { ModuloDeCargaListado } from '@ScatoModels/modulo-carga-listado';
import { MotivosFallasBalanza } from '@ScatoModels/motivo-balanza';

@Injectable({
  providedIn: 'root'
})
export class ModuloDeCargaService {
  url: string = environment.apiUrl;

  constructor(
    private http: HttpClient,
  ) {

  }

  obtenerListadoSentidoManoDeEmbarque(): Observable<SentidoManoDeEmbarque[]> {
    return this.http.get<SentidoManoDeEmbarque[]>(`${this.url}ModuloDeCarga/ListarSentidoManoDeEmbarques`, { 'withCredentials': true });
  }

  obtenerListadoCeldaManoDeEmbarque(): Observable<CeldaManoDeEmbarque[]> {
    return this.http.get<CeldaManoDeEmbarque[]>(`${this.url}ModuloDeCarga/ListarCeldaManoDeEmbarques`, { 'withCredentials': true });
  }

  guardarModuloDeCarga(moduloDeCarga: ModuloDeCarga){    
    return this.http.post(`${this.url}ModuloDeCarga/GuardarModuloDeCarga`, moduloDeCarga, { 'withCredentials' : true});
  }
  
  obtenerModuloDeCarga(moduloDeCargaId: number) {
    return this.http.get<ModuloDeCarga>(`${this.url}ModuloDeCarga/ObtenerModuloDeCarga?id=`+moduloDeCargaId, { 'withCredentials' : true});
  }

  modificarCargadoPlanoDeCarga(planoDeCargaId: any): Observable<any>{
    return this.http.post(`${this.url}PlanoDeCarga/ModificarCargadoPlanoDeCarga?planoDeCargaId=${parseInt(planoDeCargaId)}`, {'withCredentials': true});
  }

  obtenerUltimaHabilitacionDeTanques(): Observable<ModuloDeCargaListado> {
    return this.http.get<ModuloDeCargaListado>(`${this.url}ModuloDeCarga/ObtenerUltimaHabilitacionDeTanques`, { 'withCredentials': true });
  }

  obtenerListadoMotivosFallasBalanza(): Observable<MotivosFallasBalanza> {
    return this.http.get<MotivosFallasBalanza>(`${this.url}ModuloDeCarga/ListarMotivosFallasBalanza`, { 'withCredentials': true });
  }
}