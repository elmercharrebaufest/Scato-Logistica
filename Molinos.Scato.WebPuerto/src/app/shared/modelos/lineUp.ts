import { PlanoDeCarga } from './plano-de-carga';
import { UbicacionDeBuquePuerto } from './ubicacion-de-buque-puerto';

export class LineUp {
  id: number;
  instanciaWorkflow: string;
  //utilizar la ubicaci�n de embarque.ts!
  planoDeCarga: PlanoDeCarga;
  ubicacion: UbicacionDeBuquePuerto[];
  cartaDeSubidaEnviada: boolean;
  cartaDeSubidaAprobada: string;
  cargaEnSap: boolean;
  nominacionDePractico: boolean;
  seguridadPortuaria: boolean;
  inspeccionSenasa: boolean;
  controlSenasa: boolean;
  controlPrivado: boolean;
  amarrador: boolean;
  agenciaContactada: boolean;
  planoDeCargaEnviado: boolean;
  orden: number;
}
