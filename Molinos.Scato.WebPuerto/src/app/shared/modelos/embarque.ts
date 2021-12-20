import { AgenciaMaritimaPuerto } from './agencia-maritima-puerto';
import { ATAPuerto } from './ata-puerto';
import { CoordinadorPuerto } from './coordinador-puerto';
import { Destino } from './destino';
import { MaterialPuertoCantidad } from './material-puerto-cantidad';
import { MotivosLimpieza } from './motivo-limpieza';
import { TipoDeBuquePuerto } from './tipo-de-buque-puerto';
import { UbicacionDeBuquePuerto } from './ubicacion-de-buque-puerto';

export class Embarque {
  id: number;
  nombreBuque: string;
  agencia: string;
  agencias: AgenciaMaritimaPuerto[];
  coordinador: string;
  coordinadores: CoordinadorPuerto[];
  fechaRecalada: Date;
  horaRecalada: string;
  obligacionCarga: Date;
  senasa: boolean;
  observaciones: string;
  vicentin: boolean;
  otrosMuelles: boolean;
  materialesPuertoCantidad: MaterialPuertoCantidad[]
  centroId: number;
  patente: string;
  tipoBuque: string;
  tipoDeBuque: TipoDeBuquePuerto;
  freeboard: number;
  noryon: boolean;
  sanBenito: boolean;
  ubicacion: number;
  ubicacionDeBuque: UbicacionDeBuquePuerto;
  ata: ATAPuerto[];
  esLiquido: boolean;
  fechaDesdeLimpieza: Date;
  horaDesdeLimpieza: string;
  fechaHastaLimpieza: Date;
  horaHastaLimpieza: string;
  motivosLimpieza: MotivosLimpieza[];
  observacionesLimpieza: string;
  destinoBuque: number;
  destino: Destino;
  porteNeto: number;
  porteBruto: number;
  eslora: number;
  manga: number;
  puntal: number;
  fechaLibrePlatica: Date;
  horaLibrePlatica: string;
  vapor: Vapor;
  estadoBuque: EstadoBuque;
}

export class Vapor{
  id: number;
  nombre: string;
}
export class EstadoBuque{
  id: number;
  descripcion: string;
}
