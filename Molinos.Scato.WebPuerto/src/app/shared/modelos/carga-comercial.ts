import { Exportador } from "./exportador";
import { MaterialPuerto } from "./material-puerto";

export class CargaComercial {
      id : number; 
      exportador: Exportador;
      materialPuerto: MaterialPuerto;
      cantidad: number;
      public constructor(init?:Partial<CargaComercial>) {
            Object.assign(this, init);
      }
}
