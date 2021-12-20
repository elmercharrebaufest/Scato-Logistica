import { Component, HostListener, OnInit } from '@angular/core';

@Component({
  selector: 'app-toggle-menu-acciones',
  templateUrl: './toggle-menu-acciones.component.html',
  styleUrls: ['./toggle-menu-acciones.component.css']
})
export class ToggleMenuAccionesComponent implements OnInit {

  showMenu = false;
  
  constructor() { }

  ngOnInit(): void {
    
  }

  public toggleMenu(){
    this.showMenu  = !this.showMenu;
  }

  private wasInside = false;
  @HostListener('click')
  clickInside() {
    this.wasInside = true;
  }
  @HostListener('document:click')
  clickout() {
    if (!this.wasInside) {
      if (this.showMenu) {
        this.toggleMenu();
      }
    }
    this.wasInside = false;
  }
}
