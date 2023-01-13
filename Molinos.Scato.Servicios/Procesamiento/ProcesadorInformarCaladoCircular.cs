using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorInformarCaladoCircular : ProcesadorComando<InformarCaladoCircular>
    {
        private readonly IServicioCircular servicioCircular;
        private readonly IServicioRepositorio servicioRepositorio;

        public ProcesadorInformarCaladoCircular(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioCircular servicioCircular, IServicioRepositorio servicioRepositorio)
            : base(repositorio, conversor, log)
        {
            this.servicioCircular = servicioCircular;
            this.servicioRepositorio = servicioRepositorio;
        }

        public override Resultado Ejecutar(InformarCaladoCircular comando)
        {
            var resultado = new ResultadoCircular();
            try
            {
                Log.Debug($"ProcesadorInformarCaladoCircular: WorkflowInstanceId: {comando.WorkflowInstanceId}");

                var resultadoCalado = servicioRepositorio.ListarAnalisisYCaladoPorCaracteristica(comando.WorkflowInstanceId);
                Log.Debug($"ProcesadorInformarCaladoCircular: WorkflowInstanceId: {comando.WorkflowInstanceId}, resultadoCaladoEncontrados: {resultadoCalado.Count()}");
                var recorrido = Repositorio.ObtenerProyeccion<Recorrido, dynamic>(x => x.InstanciaWorkflow == comando.WorkflowInstanceId, x => new { x.NumeroDocumentoIngreso, x.Rechazado, Calidad = x.CaracteristicasAnalizadasList.Any() ? x.CaracteristicasAnalizadasList.FirstOrDefault().Calidad : TipoCalidad.Desconocida, InformaCircular = x.Centro.InformaCircular, x.TipoDocumentoIngreso });
                Log.Debug($"ProcesadorInformarCaladoCircular= InformaEstadosACircular: {recorrido.InformaCircular}, WorkflowInstanceId: {comando.WorkflowInstanceId}, resultadoCaladoEncontrados: {resultadoCalado.Count()}, Recorrido-CartaPorte: {recorrido.NumeroDocumentoIngreso} Recorrido-Rechazado: {recorrido.Rechazado} Recorrido-Calidad: {Convert.ToString(recorrido.Calidad)} ");

                if (recorrido.InformaCircular && recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte)
                {
                    var estado = recorrido.Rechazado ? "RECHAZADO" : recorrido.Calidad == TipoCalidad.Conforme || comando.EsPostCalado ? "APROBADO" : "DEMORADO";
                    servicioCircular.InformarEstadoCalado(recorrido.NumeroDocumentoIngreso, resultadoCalado, estado);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error al procesar el informe de CALADO del camión a Circular App: {e.Message}");
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}