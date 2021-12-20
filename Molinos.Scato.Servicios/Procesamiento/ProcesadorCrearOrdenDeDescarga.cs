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
    public class ProcesadorCrearOrdenDeDescarga : ProcesadorComando<CrearOrdenDeDescarga>
    {
        public ProcesadorCrearOrdenDeDescarga(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearOrdenDeDescarga comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearOrdenDeDescarga para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
                    var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
                    var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
                    var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                    var proveedor = Repositorio.Obtener<Proveedor>(comando.Orden.ProveedorId);
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);
                    
                    var recorrido = new Recorrido { InstanciaWorkflow = comando.InstanciaWorkflowId, Usuario = comando.Usuario, FechaInicio = DateTime.Now, Workflow = workflow, Chofer = chofer, Centro = centro, Patente = comando.Orden.PatenteCamion, Transportista = transportista, TipoComercial = tipoComercial, TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenDeDescarga, NumeroDocumentoIngreso = comando.Orden.Numero, WorkflowDefinicion = workflowDefinicion ,TipoVehiculo = comando.Orden.TipoVehiculo};
                    Repositorio.Agregar(recorrido);

                    var ordenDeDescarga = new OrdenDeDescarga
                    {
                        Numero = comando.Orden.Numero,
                        FechaMovimiento = comando.Orden.FechaMovimiento,
                        TipoComercial = tipoComercial,
                        Proveedor = proveedor,
                        Transportista = transportista,
                        Chofer = chofer,
                        PatenteCamion = comando.Orden.PatenteCamion,
                        PatenteAcoplado = comando.Orden.PatenteAcoplado,
                        Recorrido = recorrido
                    };

                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);

                    Repositorio.Agregar(ordenDeDescarga);
                    Repositorio.GuardarCambios();
                    resultado.Id = comando.Orden.Id != 0 ? comando.Orden.Id : ordenDeDescarga.Id;
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear el OrdenDeDescarga para la instancia de workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.OrdenDeDescarga_Error);
            }

            return resultado;
        }
    }
}
