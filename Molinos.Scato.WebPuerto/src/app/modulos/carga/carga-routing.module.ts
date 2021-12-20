import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CargaComponent } from "./carga.component";

const routes: Routes = [
    {
        path: '',
        component: CargaComponent
    },
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CargaRoutingModule {}