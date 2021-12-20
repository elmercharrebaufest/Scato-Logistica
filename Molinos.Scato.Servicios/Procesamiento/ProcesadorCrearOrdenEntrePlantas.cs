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
    public class ProcesadorCrearOrdenEntrePlantas : ProcesadorComando<CrearOrdenEntrePlantas>
    {
        public ProcesadorCrearOrdenEntrePlantas(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearOrdenEntrePlantas comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearOrdenEntrePlantas para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
                    var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
                    var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
                    var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                    var centroDestino = Repositorio.Obtener<Centro>(comando.Orden.CentroDestinoId);
                    var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);

                    var recorrido = new Recorrido { InstanciaWorkflow = comando.InstanciaWorkflowId, Usuario = comando.Usuario, FechaInicio = DateTime.Now, Workflow = workflow, Chofer = chofer, Centro = centro, Patente = comando.Orden.PatenteCamion, Transportista = transportista, TipoComercial = tipoComercial, TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenEntrePlantas, NumeroDocumentoIngreso = comando.Orden.Numero, WorkflowDefinicion = workflowDefinicion, Material = material,TipoVehiculo = comando.Orden.TipoVehiculo};
                    Repositorio.Agregar(recorrido);
                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);

                    var ordenEntrePlantas = new OrdenEntrePlantas
                    {
                        Numero = comando.Orden.Numero,
                        Fecha = comando.Orden.Fecha,
                        TipoComercial = tipoComercial,
                        Material = material,
                        Transportista = transportista,
                        Chofer = chofer,
                        CentroDestino = centroDestino,
                        PatenteCamion = comando.Orden.PatenteCamion,
                        PatenteAcoplado = comando.Orden.PatenteAcoplado,
                        Recorrido = recorrido,
                        CodigoAnexo = comando.Orden.CodigoAnexo,
                        KmRecorrer = comando.Orden.KmRecorrer
                    };
                    Repositorio.Agregar(ordenEntrePlantas);
                    Repositorio.GuardarCambios();
                    resultado.Id = comando.Orden.Id != 0 ? comando.Orden.Id : (int)ordenEntrePlantas.GetType().GetProperty("Id").GetValue(ordenEntrePlantas, null);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear el OrdenEntrePlantas para la instancia de workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.OrdenEntrePlantas_Error);
            }

            return resultado;
        }
    }
}
