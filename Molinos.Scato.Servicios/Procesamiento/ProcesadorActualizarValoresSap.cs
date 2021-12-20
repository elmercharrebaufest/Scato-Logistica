using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarValoresSap : ProcesadorComando<ActualizarValoresSap>
    {
        public ProcesadorActualizarValoresSap(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarValoresSap comando)
        {
            var resultado = new ResultadoActualizarValoresSap();
            var workflow = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId).Workflow;
            var recorridoIngreso = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
            if (recorridoIngreso == null)
            {
                resultado.Errores.Add("", Textos.Error_Invalido);
                return resultado;
            }
            Vehiculo vehiculoEgreso = null;
            if(recorridoIngreso.TipoVehiculo == TipoVehiculo.Tren)
            {
                vehiculoEgreso = Repositorio.ObtenerMayor<Recorrido, DateTime, Vehiculo>(x => x.Patente == recorridoIngreso.Patente && x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.NumeroDocumentoIngreso == comando.NumeroDeDocumento && x.Centro.Id != comando.CentroId && x.Workflow.TipoDeWorkflow != workflow.TipoDeWorkflow && !x.Rechazado && x.Terminado, x => x.FechaInicio, x => x.Vehiculo);
            }
            else
            {
                vehiculoEgreso = Repositorio.ObtenerMayor<Recorrido, DateTime, Vehiculo>(x => x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.NumeroDocumentoIngreso == comando.NumeroDeDocumento && x.Centro.Id != comando.CentroId && x.Workflow.TipoDeWorkflow != workflow.TipoDeWorkflow && !x.Rechazado && x.Terminado, x => x.FechaInicio, x => x.Vehiculo);
            }
            
            if (vehiculoEgreso != null && !String.IsNullOrEmpty(vehiculoEgreso.DocumentoInternoSap) && !String.IsNullOrEmpty(vehiculoEgreso.NumeroDeDocumentoSap))
            {
                recorridoIngreso.Vehiculo.NumeroDeDocumentoSap = vehiculoEgreso.NumeroDeDocumentoSap;
                recorridoIngreso.Vehiculo.DocumentoInternoSap = vehiculoEgreso.DocumentoInternoSap;
                recorridoIngreso.NumeroDeDocumentoSap = vehiculoEgreso.NumeroDeDocumentoSap;
                recorridoIngreso.DocumentoInternoSap = vehiculoEgreso.DocumentoInternoSap;
                resultado.NumeroDeDocumentoSap = vehiculoEgreso.NumeroDeDocumentoSap;
                resultado.DocumentoInternoSap = vehiculoEgreso.DocumentoInternoSap;
                Repositorio.GuardarCambios();
            }
            else
            {
                resultado.Errores.Add("",Textos.Error_Invalido);
            }

            return resultado;
        }
    }
}
