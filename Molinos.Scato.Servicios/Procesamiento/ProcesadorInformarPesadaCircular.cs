using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorInformarPesadaCircular : ProcesadorComando<InformarPesadaCircular>
    {
        private readonly IServicioCircular servicioCircular;
        private readonly IServicioRepositorio servicioRepositorio;

        public ProcesadorInformarPesadaCircular(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioCircular servicioCircular, IServicioRepositorio servicioRepositorio)
            : base(repositorio, conversor, log)
        {
            this.servicioCircular = servicioCircular;
            this.servicioRepositorio = servicioRepositorio;
        }

        public override Resultado Ejecutar(InformarPesadaCircular comando)
        {
            var resultado = new ResultadoCircular();

            try
            {
                Log.Debug($"ProcesadorInformarPesadaCircular: WorkflowInstanceId: {comando.WorkflowInstanceId}");
                var recorrido = Repositorio.ObtenerProyeccion<Recorrido, dynamic>(x => x.InstanciaWorkflow == comando.WorkflowInstanceId, x => new { x.NumeroDocumentoIngreso, InformaCircular = x.Centro.InformaCircular, x.TipoDocumentoIngreso });
                Log.Debug($"ProcesadorInformarPesadaCircular= InformaCircular: {recorrido.InformaCircular}, NumeroDocumentoIngreso: {recorrido.NumeroDocumentoIngreso} WorkflowInstanceId: {comando.WorkflowInstanceId}");
                if (recorrido.InformaCircular && recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte)
                {
                    servicioCircular.InformarPesoCircular(recorrido.NumeroDocumentoIngreso, comando.TipoPesada, comando.Peso);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error al procesar ProcesadorInformarPesadaCircular del camión a Circular App: {e.Message}");
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}