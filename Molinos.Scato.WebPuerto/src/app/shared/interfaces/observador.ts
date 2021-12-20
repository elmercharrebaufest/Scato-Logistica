import { InstanciaWorkflowPuerto } from '../modelos/instancia-wokflow-puerto';

export interface Observador {
  Actualizar(subject?: any);
  ListarEmbarques(): InstanciaWorkflowPuerto[];
}
