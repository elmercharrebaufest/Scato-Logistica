import { ModuloDeCargaHabilitacionDeTanques } from "./habilitacion-tanques";

export class ModuloDeCargaListado{
    id: number;
    moduloDeCargaHabilitacionDeTanques: ModuloDeCargaHabilitacionDeTanques;

    constructor(
        id,
        moduloDeCargaHabilitacionDeTanques){
    this.id = id;
    this.moduloDeCargaHabilitacionDeTanques = moduloDeCargaHabilitacionDeTanques;
    }
}