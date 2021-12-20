import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
import { MaterialPuerto } from '@ScatoModels/material-puerto';

@Injectable({
  providedIn: 'root'
})
export class LineasService {
    url: string = environment.apiUrl;
  
    constructor(
      private http: HttpClient,
    ) {
  
    }

    obtenerDensidadPorTemperaturaDeMaterial(materialPuertoId: number, temperatura: number): Observable<number> {
        return this.http.get<number>(`${this.url}ModuloDeCarga/ObtenerDensidadPorTemperaturaDeMaterial?materialPuertoId=${materialPuertoId}&grado=${temperatura}`, { 'withCredentials': true });
    }

    obtenerLlenadoMilimetroPorTanque(cm: number, mm: number, tanqueNum: string): Observable<number> {
        return this.http.get<number>(`${this.url}ModuloDeCarga/obtenerLlenadoMilimetroPorTanque?cm=${cm}&mm=${mm}&tanqueNum=${tanqueNum}`, { 'withCredentials': true });
    }
}