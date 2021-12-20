import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
import { InstanciaWorkflowPuerto } from '@ScatoModels/instancia-wokflow-puerto';
import { EmbarqueNav } from '@ScatoModels/embarque-nav';

@Injectable({
  providedIn: 'root'
})
export class WorkflowService {

  url: string = environment.apiUrl;

  constructor(private http: HttpClient) {

  }

  obtenerListado(): Observable<InstanciaWorkflowPuerto[]> {
    return this.http.get<InstanciaWorkflowPuerto[]>(`${this.url}Workflow/Listar`, { 'withCredentials': true });
  }

  eliminar(id: string) {
    return this.http.get(`${this.url}Workflow/Eliminar?id=` + id, { 'withCredentials': true });
  }

  listarEmbarquesEnLineUp(): Observable<EmbarqueNav[]> {
    return this.http.get<EmbarqueNav[]>(`${this.url}Workflow/ListarEnLineUp`, { 'withCredentials': true });
  }
}
