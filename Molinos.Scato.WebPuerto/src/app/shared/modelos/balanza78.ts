import { MotivosFallasBalanza } from "./motivo-balanza";

export class Balanza78 {
    id?: number;
    nombreBuque: string;
    numeroBalanza: number;
    fecha: string;
    hora: string;
    toneladas: number;
    producto: string;
    bodega: number;
    porcentajeCarga: number;
    totalProducto: number;
    motivosFallasBalanza: MotivosFallasBalanza[];
    // motivoBalanza: MotivoBalanza[];
    observaciones: string;
}