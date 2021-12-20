import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'environments/environment';
import { SessionService } from './session.service';
import { PermisosScato } from '@ScatoEnums/permisos-scato';

@Injectable({
  providedIn: 'root'
})
export class AutenticadorService {
  url: string = environment.apiUrl;
  // user: any;
  constructor(private http: HttpClient, private sessionService: SessionService) { }

  /**
   * 
   * @returns {Observable<any>} Observable<any>
   * @memberof AutenticadorService
   */
  public autenticarUsuario() {
    return this.http.get(`${this.url}AutenticarUsuario`, { 'withCredentials': true });
  }

  // public getUserName() {
  //   return this.sessionService.getUsername();
  // }

  // public estaAutenticado() {
  //   let permisos = sessionStorage.getItem('username') ? sessionStorage.getItem('permisos').replace('[', '').replace(']', '').split(',') : '';
  //   return permisos ? true : false;
  // }

  // public Is(permiso: PermisosScato): boolean {
  //   if (this.sessionService.getPermisos())
  //     return this.sessionService.getPermisos().some(u => u == permiso);
  //   return false;
  // }
}