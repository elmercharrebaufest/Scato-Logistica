using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorInformarSalidaCircular : ProcesadorComando<InformarSalidaCircular>
    {
        private readonly IServicioCircular servicioCircular;
        private readonly IServicioRepositorio servicioRepositorio;

        public ProcesadorInformarSalidaCircular(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioCircular servicioCircular, IServicioRepositorio servicioRepositorio)
            : base(repositorio, conversor, log)
        {
            this.servicioCircular = servicioCircular;
            this.servicioRepositorio = servicioRepositorio;
        }
        public override Resultado Ejecutar(InformarSalidaCircular comando)
        {
            var resultado = new ResultadoCircular();

            try
            {
                Log.Debug($"ProcesadorInformarSalidaCircular: WorkflowInstanceId: {comando.WorkflowInstanceId}");
                var recorrido = Repositorio.ObtenerProyeccion<Recorrido, dynamic>(x => x.InstanciaWorkflow == comando.WorkflowInstanceId, x => new { x.NumeroDocumentoIngreso, InformaCircular = x.Centro.InformaCircular, x.Rechazado, NoTieneCalado = x.Calado.CaladosPorCaracteristica.FirstOrDefault() == null });
                Log.Debug($"ProcesadorInformarSalidaCircular = InformaCircular: {recorrido.InformaCircular},  NumeroDocumentoIngreso: {recorrido.NumeroDocumentoIngreso}, WorkflowInstanceId: {comando.WorkflowInstanceId}");

                if (recorrido.InformaCircular)
                {
                    servicioCircular.CamionSalioDePlanta(recorrido.NumeroDocumentoIngreso, recorrido.Rechazado && recorrido.NoTieneCalado);
                }

            }
            catch (Exception e)
            {
                Log.Error(e, $"Error al procesar ProcesadorInformarSalidaCircular del camión a Circular App: {e.Message}");
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}
