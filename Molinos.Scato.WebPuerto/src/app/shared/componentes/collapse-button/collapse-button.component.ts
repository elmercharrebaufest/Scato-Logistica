import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-collapse-button',
  templateUrl: './collapse-button.component.html',
  styleUrls: ['./collapse-button.component.css']
})
export class CollapseButtonComponent {
  @Input() collapseSelectorId: string;
  @Input() isCollapsed: boolean = false;

  toggleCollapse(){
    this.isCollapsed = !this.isCollapsed;
  }
}
