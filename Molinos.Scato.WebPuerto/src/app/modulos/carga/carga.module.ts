import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedModule } from "app/shared/shared.module";
import { CargaLiquidosComponent } from "./carga-liquidos/carga-liquidos.component";
import { LineasComponent } from "./carga-liquidos/operaciones/lineas/lineas.component";
import { TanquesComponent } from "./carga-liquidos/operaciones/tanques/tanques.component";
import { PlanillaEmbarqueComponent } from "./carga-liquidos/tableristas/planilla-embarque/planilla-embarque.component";
import { PlanillaTurnoLiquidosComponent } from "./carga-liquidos/tableristas/planilla-turno-liquidos/planilla-turno-liquidos.component";
import { CargaRoutingModule } from "./carga-routing.module";
import { CargaSolidosComponent } from "./carga-solidos/carga-solidos.component";
import { GraficoCargaComponent } from "./carga-solidos/operaciones/grafico-carga/grafico-carga.component";
import { ManosComponent } from "./carga-solidos/operaciones/manos/manos.component";
import { BalanzasRitmosComponent } from "./carga-solidos/tableristas/balanzas-ritmos/balanzas-ritmos.component";
import { BalanzasComponent } from "./carga-solidos/tableristas/balanzas/balanzas.component";
import { InfoAdicionalComponent } from "./carga-solidos/tableristas/info-adicional/info-adicional.component";
import { UmapComponent } from "./carga-solidos/tableristas/umap/umap.component";
import { CargaComponent } from "./carga.component";
import {ListboxModule} from 'primeng/listbox';
import { PeriodoCargaComponent } from "app/shared/componentes/modulos/carga/periodo-carga/periodo-carga.component";

@NgModule({
    imports: [
        CargaRoutingModule,
        CommonModule,
        SharedModule,
        ListboxModule,
    ],
    declarations: [
        CargaComponent,
        CargaSolidosComponent,
        GraficoCargaComponent,
        ManosComponent,
        BalanzasComponent,
        BalanzasRitmosComponent,
        InfoAdicionalComponent,
        UmapComponent,
        PlanillaEmbarqueComponent,
        CargaLiquidosComponent,
        TanquesComponent,
        LineasComponent,
        PlanillaTurnoLiquidosComponent
    ],
    exports: [
        ManosComponent,
        GraficoCargaComponent,
        TanquesComponent,
        PeriodoCargaComponent,
        CargaLiquidosComponent,
        LineasComponent,
        InfoAdicionalComponent,
    ]
})
export class CargaModule {}