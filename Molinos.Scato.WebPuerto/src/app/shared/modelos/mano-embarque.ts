import { CeldaManoDeEmbarque } from "./celda-mano-embarque";
import { SentidoManoDeEmbarque } from "./sentido-mano-embarque";
import { MaterialPuerto } from '@ScatoModels/material-puerto';

export class ManosDeEmbarque{
    id: number;
    mano: number;
    observaciones: string;
    moduloDeCargaManosDeEmbarqueDetalle: ManosDeEmbarqueDetalle[];
}

export class ManosDeEmbarqueDetalle{
    id: number;
    celdaManoDeEmbarque: CeldaManoDeEmbarque;
    sentidoManoDeEmbarque: SentidoManoDeEmbarque;
    porcentajePorMano: number;
    aperturaPorton: boolean;
    masProduccion: boolean;
    materialPuerto: MaterialPuerto;
}