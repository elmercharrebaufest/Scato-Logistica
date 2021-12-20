import { AgenciaControlPrivado } from "./agencia-control-privado";
import { AgenteControlPrivado } from "./agente-control-privado";
import { CargaComercial } from "./carga-comercial";
import { Estiba } from "./estiba";
import { PlanoDeCargaBodega } from "./plano-de-carga-bodega";

export class PlanoDeCarga {
    id: number;
    estiba: Estiba[];
    agenciaControlPrivado: AgenciaControlPrivado[];
    agentesControlPrivado: AgenteControlPrivado[];
    cargasComerciales: CargaComercial[];
    planoDeCargaBodegas: PlanoDeCargaBodega[];
    observaciones: string;
    filePathPlano: string | ArrayBuffer;
    planoDeCargaArchivoPlanoNombre: string;
    filePathSecuencia: string | ArrayBuffer;
    planoDeCargaArchivoSecuenciaNombre: string;
    caladoSalida: number;
    defensasMoviles: boolean;
    cargado: boolean;
    enviado: boolean;
    fumigacion: boolean;
    empresaFumigadora : string;
    usuario: string;
}