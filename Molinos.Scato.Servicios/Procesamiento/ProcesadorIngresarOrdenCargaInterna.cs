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
    public class ProcesadorIngresarOrdenCargaInterna : ProcesadorComando<CrearOrdenCargaInterna>
    {
        public ProcesadorIngresarOrdenCargaInterna(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearOrdenCargaInterna comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorIngresarOrdenCargaInterna para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
                    var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
                    var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
                    var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
                    var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);
                    var almacen = Repositorio.Obtener<Almacen>(comando.Orden.Almacen_Id);
                    var calle = Repositorio.Obtener<Calle>(comando.Orden.Calle_Id);

                    var recorrido = new Recorrido { InstanciaWorkflow = comando.InstanciaWorkflowId,Usuario = comando.Usuario, Workflow = workflow, Chofer = chofer, Centro = centro, Patente = comando.Orden.PatenteCamion, Transportista = transportista, TipoComercial = tipoComercial, TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna, Material = material, NumeroDocumentoIngreso = comando.Orden.NumeroOrden, FechaInicio = DateTime.Now, WorkflowDefinicion = workflowDefinicion, TipoVehiculo = comando.Orden.TipoVehiculo, Almacen = almacen, Calle = calle};
                    Repositorio.Agregar(recorrido);
                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);

                    var ordenCargaInterna = new OrdenCargaInterna
                        {
                            Chofer = chofer,
                            Destino = Repositorio.Obtener<Cliente>(comando.Orden.DestinoId),
                            FechaEmision = comando.Orden.FechaEmision,
                            Material = material,
                            NumeroOrden = comando.Orden.NumeroOrden,
                            PatenteAcoplado = comando.Orden.PatenteAcoplado,
                            PatenteCamion = comando.Orden.PatenteCamion,
                            TipoComercial = tipoComercial,
                            Transportista = transportista,
                            Recorrido = recorrido,
                            FechaCreacion = DateTime.Now,
                            KmRecorrer = comando.Orden.KmARecorrer,
                            LocalidadDestino = Repositorio.Obtener<Localidad>(comando.Orden.LocalidadDestinoId)
                        };

                    Repositorio.Agregar(ordenCargaInterna);
                    Repositorio.GuardarCambios();
                    resultado.Id = (int)ordenCargaInterna.GetType().GetProperty("Id").GetValue(ordenCargaInterna, null);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear la orden de carga interna para la instancia de workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.OrdenCargaInterna_Error);
            }

            return resultado;
        }
    }
}
