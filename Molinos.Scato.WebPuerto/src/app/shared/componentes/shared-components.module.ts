import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FlexLayoutModule } from "@angular/flex-layout";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { CalendarModule } from "angular-calendar";
import { PlanoContentComponent } from "app/modulos/lineup/plano-de-carga/plano-content/plano-content.component";
import { SidebarModule } from "ng-sidebar";
import { TagInputModule } from "ngx-chips";
import { NgxMaterialTimepickerModule } from "ngx-material-timepicker";
import { PerfectScrollbarModule } from "ngx-perfect-scrollbar";
import { AlertComponent } from "./alert/alert.component";
import { CollapseButtonComponent } from "./collapse-button/collapse-button.component";
import { ConfirmationDialogComponent } from "./confirmation-dialog/confirmation-dialog.component";
import { EstadosPuertoContentComponent } from "./estados-puerto/estados-puerto-content.component";
import { LayoutComponent } from "./layout/layout.component";
import { GraficosRitmosComponent } from "./modulos/carga/graficos-ritmos/graficos-ritmos.component";
import { ObsCalidadComponent } from "./modulos/calidad/obs-calidad/obs-calidad.component";
import { NavbarComponent } from "./navbar/navbar.component";
import { NavtabsComponent } from "./navtabs/navtabs.component";
import { ToggleMenuAccionesComponent } from "./toggle-menu-acciones/toggle-menu-acciones.component";
import { ToggleMenuOrdenComponent } from "./toggle-menu-orden/toggle-menu-orden.component";
import { KnobModule } from 'primeng/knob';
import { SpinnerComponent } from './spinner/spinner.component'
import { PeriodoCargaComponent } from "./modulos/carga/periodo-carga/periodo-carga.component";
import { AmarreComponent } from "./modulos/carga/amarre/amarre.component";
import { ToastModule } from 'primeng/toast';
import { IniciarSesionComponent } from './iniciar-sesion/iniciar-sesion.component';
import { NavtabsCalidadComponent } from "./navtabs-calidad/navtabs-calidad.component";

const components = [
    AlertComponent,
    CollapseButtonComponent,
    ConfirmationDialogComponent,
    EstadosPuertoContentComponent,
    LayoutComponent,
    GraficosRitmosComponent,
    ObsCalidadComponent,
    ToggleMenuOrdenComponent,
    ToggleMenuAccionesComponent,
    NavbarComponent,
    NavtabsComponent,
    PlanoContentComponent,
    SpinnerComponent,
    PeriodoCargaComponent,
    AmarreComponent,
    NavtabsCalidadComponent
]
const libs = [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    NgxMaterialTimepickerModule,
    TagInputModule,
    FlexLayoutModule,
    PerfectScrollbarModule,
    SidebarModule,
    CalendarModule,
    RouterModule,
    KnobModule,
    ToastModule
]

@NgModule({
    imports: [
        libs
    ],
    declarations: [
        components,
        IniciarSesionComponent
    ],
    exports: [
        components,
        libs
    ]
})
export class SharedComponentModule { }
