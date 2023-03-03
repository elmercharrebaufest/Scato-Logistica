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
    public class ProcesadorCrearOrdenCargaInternaFason : ProcesadorComando<CrearOrdenCargaInternaFason>
    {
        public ProcesadorCrearOrdenCargaInternaFason(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearOrdenCargaInternaFason comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorIngresarOrdenCargaInternaFason para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
                    var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
                    var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
                    var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
                    var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                    var cliente = Repositorio.Obtener<Cliente>(comando.Orden.ClienteId);
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);
                    var recorrido = new Recorrido { InstanciaWorkflow = comando.InstanciaWorkflowId, Usuario = comando.Usuario, Workflow = workflow, Chofer = chofer, Centro = centro, Patente = comando.Orden.PatenteCamion, Transportista = transportista, TipoComercial = tipoComercial, TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInternaFason, Material = material, NumeroDocumentoIngreso = comando.Orden.NumeroOrden, FechaInicio = DateTime.Now, WorkflowDefinicion = workflowDefinicion,TipoVehiculo = comando.Orden.TipoVehiculo , MotivoDemora = comando.Orden.MotivoDemora , VehiculoDemorado = comando.Orden.Demorado , Rechazado = comando.Orden.Rechazado};
                    Repositorio.Agregar(recorrido);
                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);

                    var ordenCargaInternaFason= new OrdenCargaInternaFason
                    {
                        Chofer = chofer,
                        FechaEmision = comando.Orden.FechaEmision,
                        Material = material,
                        NumeroOrden = comando.Orden.NumeroOrden,
                        PatenteAcoplado = comando.Orden.PatenteAcoplado,
                        PatenteCamion = comando.Orden.PatenteCamion,
                        TipoComercial = tipoComercial,
                        Transportista = transportista,
                        Recorrido = recorrido,
                        Cliente = cliente,
                        KmRecorrer = comando.Orden.KmARecorrer,
                        LocalidadDestino = Repositorio.Obtener<Localidad>(comando.Orden.LocalidadDestinoId),
                        DerivadoGranarioHabilitado = comando.Orden.DerivadoGranarioHabilitado,
                        PlantaDGDestino = comando.Orden.PlantaDGDestino,
                        OrdenDomicilioDestino = comando.Orden.OrdenDomicilioDestino,
                        PagadorFlete = Repositorio.Obtener<Cliente>(comando.Orden.PagadorFleteId),
                        Corredor = Repositorio.Obtener<Proveedor>(comando.Orden.CorredorId),
                        Comisionista = Repositorio.Obtener<Cliente>(comando.Orden.ComisionistaId),
                        Remitente = Repositorio.Obtener<Cliente>(comando.Orden.RemitenteId),
                        IntermediarioFlete = Repositorio.Obtener<Proveedor>(comando.Orden.IntermediarioFleteId),
                    };

                    Repositorio.Agregar(ordenCargaInternaFason);
                    Repositorio.GuardarCambios();
                    resultado.Id = (int)ordenCargaInternaFason.GetType().GetProperty("Id").GetValue(ordenCargaInternaFason, null);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear la orden de carga interna fason para la instancia de workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.OrdenCargaInterna_Error);
            }

            return resultado;
        }
    }
}
