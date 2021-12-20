import { Destino } from "./destino";
import { MaterialPuerto } from "./material-puerto";

export class PlanoDeCargaBodega {
      id : number; 
      bodegaParcel: number;
      cantidad: number;
      materialPuerto: MaterialPuerto;
      condicion: string;
      sfFull: string;
      destino: Destino;
      tanqueDeAbordo: string;
      public constructor(init?:Partial<PlanoDeCargaBodega>) {
            Object.assign(this, init);
      }
}
