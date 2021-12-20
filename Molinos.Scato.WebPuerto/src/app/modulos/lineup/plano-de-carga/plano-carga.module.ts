import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedModule } from "app/shared/shared.module";
import { PlanoCargaRoutingModule } from "./plano-carga-routing.module";
import { PlanoDeCargaComponent } from "./plano-de-carga.component";

@NgModule({
    imports: [
        PlanoCargaRoutingModule,
        CommonModule,
        SharedModule
    ],
    declarations: [PlanoDeCargaComponent]
})
export class PlanoCargaModule {}