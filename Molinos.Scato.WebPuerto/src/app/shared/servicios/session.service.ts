import { Injectable } from '@angular/core';
import { Usuario } from '../interfaces/usuario';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  

  constructor() { }

    setUser(user: Usuario){
      sessionStorage.setItem("user", JSON.stringify(user));
    }
 
    getUser() : any {
      return JSON.parse(sessionStorage.getItem("user"));
    }
    
    clear() {
      sessionStorage.clear();
    }
}
