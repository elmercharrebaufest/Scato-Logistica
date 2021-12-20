import { Tipoalerta } from "@ScatoEnums/tipo-alerta";

export class Alerta {
  constructor(mensaje: string, tipo: Tipoalerta) {
    this.mensaje = mensaje;
    this.tipo = tipo;

  }

  mensaje: string;
  tipo: Tipoalerta;
}
