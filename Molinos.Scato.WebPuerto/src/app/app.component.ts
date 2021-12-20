import { Component } from '@angular/core';
import { AutenticadorService } from '@ScatoServicios/autenticador.service';
import { PrimeNGConfig } from 'primeng/api'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'web-puerto';

  constructor(
    private primeConfig: PrimeNGConfig,
    private auth: AutenticadorService
    ) { 
      this.auth.autenticarUsuario();
    }

  ngOnInit() {
    this.primeConfig.ripple = true;
  }
}
