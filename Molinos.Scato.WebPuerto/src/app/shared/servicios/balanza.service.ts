import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
import { Balanza, Carga } from '@ScatoModels/balanza';

@Injectable({
  providedIn: 'root'
})
export class BalanzaService {
    url: string = environment.apiUrl;
  
    constructor(
      private http: HttpClient,
    ) {}

    listarBalanzadaBuque(buque: number): Observable<Balanza[]>{
        return this.http.get<Balanza[]>(`${this.url}PlanoDeCarga/ListarBalanzadaBuque?buque=${buque}`, { 'withCredentials': true });
    }

    listarCargaBalanzaPuerto(): Observable<Carga[]>{
        return this.http.get<Carga[]>(`${this.url}PlanoDeCarga/ListarCargaBalanzaPuerto`, { 'withCredentials': true });
    }
}