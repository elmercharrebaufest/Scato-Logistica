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
    public class ProcesadorCrearOrdenDeCargaContenedor : ProcesadorComando<CrearOrdenDeCargaContenedor>
    {
        public ProcesadorCrearOrdenDeCargaContenedor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearOrdenDeCargaContenedor comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearOrdenDeCargaContenedor para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
                    var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
                    var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
                    var destino = Repositorio.Obtener<Cliente>(comando.Orden.DestinoId);
                    var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);
                    var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
                    var contenedorEntrada = Repositorio.Obtener<TaraContenedor>(comando.Orden.ContenedorEntradaId);
                    var contenedorSalida = Repositorio.Obtener<TaraContenedor>(comando.Orden.ContenedorSalidaId);
                    var recorrido = new Recorrido { InstanciaWorkflow = comando.InstanciaWorkflowId, Usuario = comando.Usuario, FechaInicio = DateTime.Now, Workflow = workflow, Chofer = chofer, Centro = centro, Patente = comando.Orden.PatenteCamion, Transportista = transportista, TipoComercial = tipoComercial, TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenDeCargaContenedor, NumeroDocumentoIngreso = comando.Orden.OrdenDeCargaContenedor, WorkflowDefinicion = workflowDefinicion,TipoVehiculo = comando.Orden.TipoVehiculo};
                    Repositorio.Agregar(recorrido);
                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);

                    var ordenDeCargaContenedor = new OrdenDeCargaContenedor
                    {
                        NroOrdenDeCargaContenedor = comando.Orden.OrdenDeCargaContenedor,
                        TipoComercial = tipoComercial,
                        Transportista = transportista,
                        Chofer = chofer,
                        PatenteCamion = comando.Orden.PatenteCamion,
                        PatenteAcoplado = comando.Orden.PatenteAcoplado,
                        Destino = destino,
                        Material = material,
                        ContenedorEntrada = contenedorEntrada,
                        ContenedorSalida = contenedorSalida,
                        Recorrido = recorrido,
                        Fecha = DateTime.Now
                    };

                    if (Repositorio.Existe<OrdenDeCargaContenedor>(x => x.NroOrdenDeCargaContenedor == comando.Orden.OrdenDeCargaContenedor))
                    {
                        ordenDeCargaContenedor = Repositorio.Obtener<OrdenDeCargaContenedor>(x => x.NroOrdenDeCargaContenedor == comando.Orden.OrdenDeCargaContenedor);
                    }
                    Repositorio.Agregar(ordenDeCargaContenedor);
                    Repositorio.GuardarCambios();
                    resultado.Id = comando.Orden.Id != 0 ? comando.Orden.Id : (int)ordenDeCargaContenedor.GetType().GetProperty("Id").GetValue(ordenDeCargaContenedor, null);
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
