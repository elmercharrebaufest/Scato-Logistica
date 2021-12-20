import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivateChild, Router } from "@angular/router";
//import { Store } from "@ngrx/store";
import { SessionService } from "@ScatoServicios/session.service";
import { Usuario } from "../interfaces/usuario";

@Injectable()
export class RoleGuard implements CanActivateChild{
    constructor(
        private router: Router,
        private session: SessionService
        ){
        }

    canActivateChild(
        route: ActivatedRouteSnapshot,
        ){
            let permisos = this.session.getUser().permisos;
            let ruta = route.parent.url.toString();    
            // switcheamos sobre la ultima parte de la ruta y preguntamos si tiene el permiso, sino redireccionamosa su home       
            switch (ruta) {
                case "": {
                    if (permisos.find(x => x === 600)){
                        return true;
                    }else{
                        this.navigate(permisos);
                    }
                    break;
                };
                case "lineup": {
                    if (permisos.find(x => x === 600)){
                        return true;
                    }else{
                        this.navigate(permisos);
                    }
                    break;
                }
                case 'alta-embarque': {
                    if (permisos.find(x => x === 610) && route.params.state){
                        return true;
                    }else{
                        this.navigate(permisos);
                    }
                }
                case 'plano-de-carga': {
                    if (permisos.find(x => x === 620) && route.params.id){
                        return true;
                    }else{
                        this.navigate(permisos);
                    }
                }
                case "carga": {
                    if (permisos.find(x => x === 630)){
                        return true;
                    }else{
                        this.navigate(permisos);
                    }
                    break;
                }
                case 'calidad': {
                    if (permisos.find(x => x === 640)){
                        return true;
                    }else{
                        this.navigate(permisos);
                    }
                    break;
                }
            }
        }

    // En caso de que no tenga permisos, redireccionamos a donde si tenga permisos
    navigate(permisos){
        let primerPermiso = permisos.find((p: number)=> p == 600 || p == 630 || p == 640);
        switch(primerPermiso){
            case 600: {
                this.router.navigate(['/lineup']);
                break;
            };
            case 630: {
                this.router.navigate(['/carga']);
                break;
            }
            case 640: {
                this.router.navigate(['/calidad']);
                break;
            }
        }
    }
}
