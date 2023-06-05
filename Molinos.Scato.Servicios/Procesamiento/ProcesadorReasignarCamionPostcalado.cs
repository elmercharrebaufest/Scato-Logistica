using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorReasignarCamionPostcalado : ProcesadorComando<ReasignarCamionPostcalado>
    {
        private IAdministradorDeCalles administradorDeCalles;
        private IServicioComandos servicioComandos;

        public ProcesadorReasignarCamionPostcalado(IRepositorio repositorio, IConversor conversor, ILogger log, IAdministradorDeCalles administradorDeCalles, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.administradorDeCalles = administradorDeCalles;
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(ReasignarCamionPostcalado comando)
        {
            var resultado = new Resultado();
            CallePorRecorrido calleRecorrido = null;
            
            try
            {
                if (comando.CargaDeCupoId.HasValue)
                {
                    calleRecorrido = Repositorio.Obtener<CallePorRecorrido>(x => x.CargaDeCupo.Id == comando.CargaDeCupoId && x.FechaEgreso == null);
                }
                else
                {
                    calleRecorrido = Repositorio.ObtenerProyeccion<Recorrido, CallePorRecorrido>(x => x.InstanciaWorkflow == comando.InstanciaWorkflow, x => x.CallePorRecorridos.FirstOrDefault(w => w.Recorrido.Id == x.Id && w.FechaEgreso == null));
                }
                Validar(comando, resultado, calleRecorrido);
                if (!resultado.HayErrores)
                {
                    var calle = Repositorio.Obtener<Calle>(x => x.Id == comando.CalleId);                    

                    if(calle.TipoCalle == Dominio.Enums.TipoCalle.RechazadosDemorados)
                    {
                        servicioComandos.Ejecutar(
                            new CrearCallePorRecorrido
                            {
                                TipoCalle = Dominio.Enums.TipoCalle.RechazadosDemorados,
                                InstanciaWorkflow = comando.InstanciaWorkflow,
                                FlagReasignacionCalle = true,
                                CalleReasignacion = calle
                            });
                    } 
                    else
                    {
                        //calleRecorrido.Recorrido.Calle.Id = comando.CalleId;
                        calleRecorrido.Calle = calle;
                        Repositorio.GuardarCambios();
                    }                    
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Ocurrió un error al reasignar la calle del camión con IntanceWorkFlowId : {comando?.InstanciaWorkflow} ");
                resultado.Error("", Textos.Error_ActualizarGenerico);
            }

            return resultado;
        }
        protected void Validar(ReasignarCamionPostcalado comando, Resultado resultado, CallePorRecorrido callePorRecorrido)
        {
            if (!Repositorio.Existe<Calle>(x => x.Id == comando.CalleId))
            {
                resultado.Error("CalleId", Textos.Calle_Inexistente);
            }
            if (!comando.CargaDeCupoId.HasValue && !Repositorio.Existe<Recorrido>(x => x.InstanciaWorkflow == comando.InstanciaWorkflow))
            {
                resultado.Error("InstanciaWorkflow", Textos.Recorrido_Inexistente);
            }
            if (!administradorDeCalles.ObtenerEspacioDisponibleEnCalle(comando.CalleId))
            {
                resultado.Error("CantidadCamiones", "La calle seleccionada se encuentra ocupada en su totalidad, seleccionar otra opción.");
            }
            if (callePorRecorrido != null && callePorRecorrido.Calle.Id == comando.CalleId)
            {
                resultado.Error("CalleId", "La calle seleccionada debe ser distinta a la actual.");
            }
        }
    }
}
