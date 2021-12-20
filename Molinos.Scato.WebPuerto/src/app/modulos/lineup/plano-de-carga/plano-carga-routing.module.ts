import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PlanoDeCargaComponent } from "./plano-de-carga.component";

const routes: Routes =[
    {
        path: '',
        component: PlanoDeCargaComponent
    },
    {
        path: ':id',
        component: PlanoDeCargaComponent
    }
]
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PlanoCargaRoutingModule {}