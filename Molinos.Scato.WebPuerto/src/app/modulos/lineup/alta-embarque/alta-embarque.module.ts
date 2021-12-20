import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedModule } from "app/shared/shared.module";
import { AltaEmbarqueRoutingModule } from "./alta-embarque-routing.module";
import { AltaEmbarqueComponent } from "./alta-embarque.component";

@NgModule({
    imports: [
        AltaEmbarqueRoutingModule,
        CommonModule,
        SharedModule
    ],
    declarations: [
        AltaEmbarqueComponent
    ]
})
export class AltaEmbarqueModule {}