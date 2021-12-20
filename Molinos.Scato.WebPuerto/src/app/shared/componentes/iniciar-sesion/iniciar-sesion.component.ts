import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AutenticadorService } from '@ScatoServicios/autenticador.service';
import { SessionService } from '@ScatoServicios/session.service';
import { Usuario } from '@ScatoInterfaces/usuario';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-iniciar-sesion',
  templateUrl: './iniciar-sesion.component.html',
  styleUrls: ['./iniciar-sesion.component.css']
})
export class IniciarSesionComponent implements OnInit {

  constructor(
    private router: Router,
    private messageService: MessageService,
    private serviceauth: AutenticadorService,
    private session: SessionService) { }

  ngOnInit(): void {
    this.autenticar();
  }

  autenticar() {
    this.serviceauth.autenticarUsuario().subscribe(
      (res: Usuario) => {
        if (res) {
          this.session.clear();
          res.permisos = [600, 601, 602, 603, 604, 605, 606, 607, 608, 609,
            610, 611, 612, 613, 614, 615, 616, 617, 618, 619,
            620, 621, 622, 623, 624, 625, 626, 627, 628, 629,
            630, 631, 632, 633, 634, 635, 636, 637, 638, 639,
            640, 641, 642, 643, 644, 645, 646, 647, 648, 649]
          res.autenticado = true;
          this.session.setUser(res);
          this.router.navigateByUrl('/lineup');
        } else {
          this.messageService.add({ severity: 'error', detail: 'Error al iniciar sesión', summary: 'No se ha encontrado el usuario' })
        }
      }
    )
  }
}