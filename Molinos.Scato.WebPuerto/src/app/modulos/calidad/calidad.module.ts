import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedModule } from "app/shared/shared.module";
import { CargaModule } from "../carga/carga.module";
import { CalidadRoutingModule } from "./calidad-routing.module";
import { CalidadComponent } from "./calidad.component";
import { LiquidosComponent } from "./liquidos/liquidos.component";
import { NIRComponent } from "./solidos/nir/nir.component";
import { SolidosComponent } from "./solidos/solidos.component";
import { PlanillaTurnosSolidosComponent } from './solidos/planilla-turnos-solidos/planilla-turnos-solidos.component';
import { PlanillaTurnosLiquidosCalidadComponent } from './liquidos/planilla-turnos-liquidos-calidad/planilla-turnos-liquidos-calidad.component';

@NgModule({
    imports: [
        CommonModule,
        CalidadRoutingModule,
        SharedModule,
        CargaModule
    ],
    declarations: [
        CalidadComponent,
        NIRComponent,
        SolidosComponent,
        LiquidosComponent,
        PlanillaTurnosSolidosComponent,
        PlanillaTurnosLiquidosCalidadComponent
    ],
})

export class CalidadModule {}
