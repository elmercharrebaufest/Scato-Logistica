export class Balanza {
    id: number;
    numeroBalanza: string;
    pesoBruto: number;
    pesoTara: number;
    pesoNeto: number;
    capacidad: string;
    fecha: Date;
    enviadoASap: boolean;
    cargaInicial: Carga;
    cargaInicial_Id: number;
    cargaInicial_NumeroBalanza: string;
}

export class Carga{
    id: number;
    numeroBalanza: string;
    vapor: string;
    material: string;
    bodega: string;
    exportador: string;
    destino: string;
    pesoProgramado: number;
    toneladasAW: number;
    fecha: Date;
    fechaInicio: Date;
    idFin: number;
    error: number;
    errorMensaje: string;
    enviadoASap: boolean;
    tipo: string;
    porcentajeDeCarga: number;
    pediente: boolean;
    materialId: number;
}