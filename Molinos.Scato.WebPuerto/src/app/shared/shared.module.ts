import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedComponentModule } from "./componentes/shared-components.module";
import { LoginGuard } from "./seguridad/login.guard";
import { RoleGuard } from "./seguridad/role.guard";

@NgModule({
    imports: [
        CommonModule,
        SharedComponentModule,
    ],
    exports: [
        CommonModule,
        SharedComponentModule,
    ],
    providers: [RoleGuard, LoginGuard]
})
export class SharedModule { }