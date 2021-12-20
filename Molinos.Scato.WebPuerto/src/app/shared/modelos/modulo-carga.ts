import { Balanza78 } from "./balanza78";
import { ElementoGrafico } from "./elemento-grafico";
import { ModuloDeCargaHabilitacionDeTanques } from "./habilitacion-tanques";
import { LineasDeEmbarque } from "./linea-embarque";
import { ManosDeEmbarque } from "./mano-embarque";
import { TabiquesDeEmbarque } from "./tabique-embarque";

export class ModuloDeCarga{
    id: number;
    moduloDeCargaElementoGrafico: ElementoGrafico[];
    moduloDeCargaManosDeEmbarque: ManosDeEmbarque[];
    moduloDeCargaTabiquesDeEmbarque: TabiquesDeEmbarque[];
    moduloDeCargaHabilitacionDeTanques: ModuloDeCargaHabilitacionDeTanques;
    moduloDeCargaLineasDeEmbarque: LineasDeEmbarque[];
    moduloDeCargaBalanzas: Balanza78[];
    enviado: boolean;
    usuarioFinalizacion: string;
    cargado?: boolean;

    constructor(
        id,
        enviado,
        usuarioFinalizacion,
        moduloDeCargaElementoGrafico?,
        moduloDeCargaManosDeEmbarque?,
        moduloDeCargaTabiquesDeEmbarque?,
        moduloDeCargaHabilitacionDeTanques?,
        moduloDeCargaLineasDeEmbarque?,
        moduloDeCargaBalanzas?
        ){
    this.id = id;
    this.moduloDeCargaElementoGrafico = moduloDeCargaElementoGrafico;
    this.moduloDeCargaManosDeEmbarque = moduloDeCargaManosDeEmbarque;
    this.moduloDeCargaTabiquesDeEmbarque = moduloDeCargaTabiquesDeEmbarque;
    this.moduloDeCargaHabilitacionDeTanques = moduloDeCargaHabilitacionDeTanques;
    this.moduloDeCargaLineasDeEmbarque = moduloDeCargaLineasDeEmbarque;
    this.moduloDeCargaBalanzas = moduloDeCargaBalanzas;
    this.enviado = enviado;
    this.usuarioFinalizacion = usuarioFinalizacion;
    }
}