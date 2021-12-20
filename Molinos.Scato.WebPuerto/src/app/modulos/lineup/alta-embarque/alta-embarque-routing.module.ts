import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AltaEmbarqueComponent } from "./alta-embarque.component";

const routes: Routes =[
    {
        path: '',
        component: AltaEmbarqueComponent
    },
    {
        path: ':id/:state',
        component: AltaEmbarqueComponent
    }
]
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AltaEmbarqueRoutingModule {}