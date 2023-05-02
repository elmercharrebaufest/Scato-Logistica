using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var camionesPermitidos = new List<TipoVehiculo> { TipoVehiculo.Camión, TipoVehiculo.CamiónC, TipoVehiculo.CamiónD, TipoVehiculo.CamiónE };

            try
            {
                Log.Debug($"ProcesadorInformarPesadaCircular: WorkflowInstanceId: {comando.WorkflowInstanceId}");
                var recorrido = Repositorio.ObtenerProyeccion<Recorrido, dynamic>(x => x.InstanciaWorkflow == comando.WorkflowInstanceId,
                    x => new
                    {
                        x.NumeroDocumentoIngreso,
                        x.Centro.InformaCircular,
                        x.TipoDocumentoIngreso,
                        x.TipoVehiculo
                    });
                Log.Debug($"ProcesadorInformarPesadaCircular= InformaCircular: {recorrido.InformaCircular}, NumeroDocumentoIngreso: {recorrido.NumeroDocumentoIngreso} WorkflowInstanceId: {comando.WorkflowInstanceId}");
                var permitido = camionesPermitidos.Any(x => x == recorrido.TipoVehiculo);

                if (recorrido.InformaCircular && recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && permitido)
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