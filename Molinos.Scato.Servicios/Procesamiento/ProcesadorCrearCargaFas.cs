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
    public class ProcesadorCrearCargaFas : ProcesadorComando<CrearOrdenCargaFas>
    {
        public ProcesadorCrearCargaFas(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearOrdenCargaFas comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearCargaFas para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
                    var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
                    var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
                    var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
                    var cliente = Repositorio.Obtener<Cliente>(comando.Orden.ClienteId);
                    var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);
                    var localidadDestino = Repositorio.Obtener<Localidad>(comando.Orden.LocalidadDestinoId);

                    var recorrido = new Recorrido { InstanciaWorkflow = comando.InstanciaWorkflowId, Usuario = comando.Usuario, FechaInicio = DateTime.Now, Workflow = workflow, Chofer = chofer, Centro = centro, Patente = comando.Orden.PatenteCamion, Transportista = transportista, TipoComercial = tipoComercial, TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaFas, Material = material, WorkflowDefinicion = workflowDefinicion, NumeroDocumentoIngreso = comando.Orden.NumeroOrden, DatosProximaActividad = "Visteo",TipoVehiculo = comando.Orden.TipoVehiculo, VehiculoDemorado = comando.Orden.VehiculoDemorado, MotivoDemora = comando.Orden.MotivoDemora, Rechazado = comando.Orden.Rechazado};
                    Repositorio.Agregar(recorrido);
                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);

                    var orden = new OrdenCargaFas
                        {
                            Cliente = comando.Orden.VehiculoDemorado ? null : cliente,
                            Chofer = chofer,
                            Id = comando.Orden.Id,
                            Material = material,
                            PatenteAcoplado = comando.Orden.PatenteAcoplado,
                            PatenteCamion = comando.Orden.PatenteCamion,
                            TipoComercial = tipoComercial,
                            Transportista = comando.Orden.VehiculoDemorado ? null : transportista,
                            ValidaCompliance = comando.Orden.VehiculoDemorado ? false : comando.Orden.ValidaCompliance,
                            NumeroOrden = comando.Orden.VehiculoDemorado ? "" : comando.Orden.NumeroOrden,
                            Recorrido = recorrido,
                            KmRecorrer = comando.Orden.KmARecorrer,
                            LocalidadDestino = comando.Orden.VehiculoDemorado ? null : localidadDestino,
                            DerivadoGranarioHabilitado = comando.Orden.DerivadoGranarioHabilitado,
                            PlantaDGDestino = comando.Orden.PlantaDGDestino,
                            OrdenDomicilioDestino = comando.Orden.OrdenDomicilioDestino,
                            PagadorFlete = Repositorio.Obtener<Cliente>(comando.Orden.PagadorFleteId),
                            Corredor = Repositorio.Obtener<Proveedor>(comando.Orden.CorredorId),
                            Comisionista = Repositorio.Obtener<Cliente>(comando.Orden.ComisionistaId),
                            Remitente = Repositorio.Obtener<Cliente>(comando.Orden.RemitenteId),
                            CuitDestinatario = comando.Orden.CuitDestinatario,
                            TipoDomicilioDestino = comando.Orden.TipoDomicilioDestino,
                            IntermediarioFlete = Repositorio.Obtener<Proveedor>(comando.Orden.IntermediarioFleteId),
                    };

                    if (!comando.Orden.VehiculoDemorado && Repositorio.Existe<OrdenCargaFas>(x => x.Id == comando.Orden.Id))
                    {
                        orden = Repositorio.Obtener<OrdenCargaFas>(x => x.Id == comando.Orden.Id);
                    }

                    Repositorio.GuardarCambios();

                    resultado.Id = comando.Orden.Id != 0 ? comando.Orden.Id : (int)orden.GetType().GetProperty("Id").GetValue(orden, null);

                    Repositorio.Agregar(orden);
                    Repositorio.GuardarCambios();
                    Log.Info("Se creó exitosamente la la orden de carga Fas para el workflow {0}", comando.NombreWorkflow);
                    resultado.Id = (int)orden.GetType().GetProperty("Id").GetValue(orden, null);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear la orden de carga FAS para la instancia de workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.OrdenCargaFas_Error);
            }

            return resultado;
        }
    }
}
