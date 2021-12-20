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
    public class ProcesadorCrearOrdenDeDescargaFason : ProcesadorComando<CrearOrdenDeDescargaFason>
    {
        public ProcesadorCrearOrdenDeDescargaFason(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearOrdenDeDescargaFason comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearOrdenDeDescargaFason para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
                    var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
                    var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
                    var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                    var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
                    var cliente = Repositorio.Obtener<Cliente>(comando.Orden.ClienteId);
                    var procedencia = Repositorio.Obtener<Localidad>(comando.Orden.ProcedenciaId);
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);

                    var recorrido = new Recorrido { InstanciaWorkflow = comando.InstanciaWorkflowId, Usuario = comando.Usuario, FechaInicio = DateTime.Now, Workflow = workflow, Chofer = chofer, Centro = centro, Patente = comando.Orden.PatenteCamion, Transportista = transportista, TipoComercial = tipoComercial, TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenDeDescargaFason, NumeroDocumentoIngreso = comando.Orden.Numero, NumeroDocumentoIngresoLegal = (!string.IsNullOrEmpty(comando.Orden.NumeroRemito) ? comando.Orden.NumeroRemito.Replace('R', '-') : ""), WorkflowDefinicion = workflowDefinicion, Material = material,TipoVehiculo = comando.Orden.TipoVehiculo};
                    Repositorio.Agregar(recorrido);
                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);

                    var ordenDeDescargaFason = new OrdenDeDescargaFason
                    {
                        Numero = comando.Orden.Numero,
                        FechaOD = comando.Orden.FechaOD,
                        TipoComercial = tipoComercial,
                        Cliente = cliente,
                        Material = material,
                        Transportista = transportista,
                        Chofer = chofer,
                        Procedencia = procedencia,
                        PatenteCamion = comando.Orden.PatenteCamion,
                        PatenteAcoplado = comando.Orden.PatenteAcoplado,
                        NumeroRemito = comando.Orden.NumeroRemito,
                        PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen,
                        PesoTaraOrigen = comando.Orden.PesoTaraOrigen,
                        PesoNetoOrigen = comando.Orden.PesoNetoOrigen,
                        Recorrido = recorrido
                    };

                    Repositorio.Agregar(ordenDeDescargaFason);
                    Repositorio.GuardarCambios();
                    resultado.Id = comando.Orden.Id != 0 ? comando.Orden.Id : ordenDeDescargaFason.Id;
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear el OrdenDeDescargaFason para la instancia de workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.OrdenDeDescarga_Error);
            }

            return resultado;
        }
    }
}
