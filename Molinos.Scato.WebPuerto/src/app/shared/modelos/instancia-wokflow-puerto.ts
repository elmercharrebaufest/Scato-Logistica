import { Embarque } from './embarque';
import { LineUp } from './lineUp';

export class InstanciaWorkflowPuerto {
  id: string;
  proximaAccion: string;
  fechaUltimaModificacion: Date;
  embarque: Embarque;
  lineUp: LineUp;
}
