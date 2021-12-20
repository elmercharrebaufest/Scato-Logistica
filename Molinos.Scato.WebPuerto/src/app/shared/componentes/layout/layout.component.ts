import { Component, OnInit } from '@angular/core';
import { Router, RouterEvent, Event } from '@angular/router';
import { SessionService } from '@ScatoServicios/session.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css']
})
export class LayoutComponent implements OnInit {
  variable: boolean = true;
  opened: boolean = false;
  animate: boolean = true;
  keyClose: boolean = false;
  private user: any;
  rutaActual: string;

  constructor(
    private router: Router,
    private session: SessionService
  ) {
    this.user = this.session.getUser();
    this.rutaActual = this.router.url.replace('/', '');
    router.events.pipe(
      filter((e: Event): e is RouterEvent => e instanceof RouterEvent)
    ).subscribe((e: RouterEvent) => {
      this.rutaActual = e.url.replace('/', '');
    });
  }

  ngOnInit(): void {
  }

  change(param) {
    this.variable = param;
  }

  toggleCloseFocusOut() {
    this.opened = false;
  }

  toggleOpened(): void {
    this.opened = !this.opened;
  }

  toggleAnimate(): void {
    this.animate = !this.animate;
  }

  toggleKeyClose(): void {
    this.keyClose = !this.keyClose;
  }

  goHome() {
    if (this.user.permisos.find(x => x === 600))
      this.router.navigate(['/lineup']);
    if (this.user.permisos.find(x => x === 630))
      this.router.navigate(['/carga'])
    if (this.user.permisos.find(x => x === 640))
      this.router.navigate(['/calidad'])
    this.opened = false;
  }

  showSubmenu(menu: HTMLElement, submenu: HTMLElement) {
    this.closeSubmenus(menu, submenu);
    submenu.classList.contains('submenu-active') ? submenu.classList.remove('submenu-active') : submenu.classList.add('submenu-active');
  }

  closeSubmenus(menu: HTMLElement, submenu?: HTMLElement) {
    for (let child of menu.children) {
      if (child != submenu)
        child.classList.contains('submenu-active') ? child.classList.remove('submenu-active') : '';
    }
    if (!submenu)
      this.opened = false;
  }

  tienePermiso(permiso: number) {
    return this.user.permisos.find(x => x === permiso);
  }

  openSidebar() {
    this.opened = true;
  }

  closeSidebar() {
    this.opened = false;
  }
}
