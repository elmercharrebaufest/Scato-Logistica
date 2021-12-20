import { CanActivate, CanActivateChild } from "@angular/router";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { SessionService } from "@ScatoServicios/session.service";

@Injectable()
export class LoginGuard implements CanActivate {
  user: any;
  constructor(private router: Router, private session: SessionService) {
  }

  canActivate() {
    this.user = this.session.getUser();
    if (this.user && this.user.autenticado)
      return true
    else{
      this.router.navigate(['/login']);
      return false
    }
  }
}