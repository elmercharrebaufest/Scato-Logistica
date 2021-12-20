import { MaterialPuerto } from "./material-puerto";

export class ElementoGrafico {
    id: number;
    tipo: string;
    forma: string;
    x: number;
    y: number;
    width: number;
    height: number;
    radioX: number;
    radioY: number;
    materialPuerto: MaterialPuerto;
    rotacion: boolean;

    constructor(id, tipo, x, y, forma, width, height, radioX, radioY, materialPuerto, rotacion){
        this.id = id;
        this.tipo = tipo;
        this.forma = forma;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.radioX = radioX;
        this.radioY = radioY;
        this.materialPuerto = materialPuerto;
        this.rotacion = rotacion;
    }
}