import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { RoleGuard } from "app/shared/seguridad/role.guard";
import { LineupComponent } from "./lineup.component";

const routes: Routes =[
    {
        path: '',
        component: LineupComponent
    },
    {
        path: 'plano-de-carga',
        canActivateChild: [RoleGuard],
        loadChildren: () => import('./plano-de-carga/plano-carga.module').then(m => m.PlanoCargaModule)
    },
    {
        path: 'alta-embarque',
        canActivateChild: [RoleGuard],
        loadChildren: () => import('./alta-embarque/alta-embarque.module').then(m => m.AltaEmbarqueModule)
    },
    
]
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LineUpRoutingModule {}