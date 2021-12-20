import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedModule } from "app/shared/shared.module";
import { LineupCalendarioComponent } from "./calendario/lineup-calendario.component";
import { LineupEmbarqueComponent } from "./embarque/lineup-embarque.component";
import { LineUpRoutingModule } from "./lineup-routing.module";
import { LineupComponent } from "./lineup.component";
import { CargandoMuelleComponent } from './cargando-muelle/cargando-muelle.component';

@NgModule({
    imports: [
        LineUpRoutingModule,
        CommonModule,
        SharedModule
    ],
    declarations: [
        LineupComponent,
        LineupEmbarqueComponent,
        LineupCalendarioComponent,
        CargandoMuelleComponent
    ]
})
export class LineUpModule {}