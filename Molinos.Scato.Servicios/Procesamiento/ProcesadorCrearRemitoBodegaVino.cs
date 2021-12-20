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
    public class ProcesadorCrearRemitoBodegaVino : ProcesadorComando<CrearRemitoBodegaVino>
    {
        public ProcesadorCrearRemitoBodegaVino(IRepositorio repositorio, IConversor conversor, ILogger log)
            :base(repositorio,conversor,log)
        {
        }

        public override Resultado Ejecutar(CrearRemitoBodegaVino comando)
        {

            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearRemitoBodegaVino para la instancia workflow {0}",
                         comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(w => w.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
                    var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
                    var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
                    var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);
                    var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
                    var tipoDeDocumentoIngreso = TipoDocumentoIngreso.RemitoBodegaVino;
                    var recorrido = new Recorrido
                        {
                            InstanciaWorkflow = comando.InstanciaWorkflowId,
                            Workflow = workflow,
                            Chofer = chofer,
                            Centro = centro,
                            Patente = comando.Orden.Patente,
                            Transportista = transportista,
                            TipoComercial = tipoComercial,
                            WorkflowDefinicion = workflowDefinicion,
                            Material = material,
                            TipoDocumentoIngreso = tipoDeDocumentoIngreso,
                            NumeroDocumentoIngreso = comando.Orden.NroRemito,
                            FechaInicio = DateTime.Now,
                            PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen,
                            PesoTaraOrigen = comando.Orden.PesoTaraOrigen
                        };
                    Repositorio.Agregar(recorrido);
                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);

                    var remitoBodegaVino = new RemitoBodegaVino
                        {
                            Chofer = chofer,
                            TipoComercial = tipoComercial,
                            Transportista = transportista,
                            Material = material,
                            Recorrido = recorrido,
                            NroRemito = comando.Orden.NroRemito,
                            Patente = comando.Orden.Patente,
                            MarcaCamion = comando.Orden.MarcaCamion,
                            ModeloCamion = comando.Orden.ModeloCamion,
                            OrdenDeCompra = comando.Orden.OrdenDeCompra,
                            PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen,
                            PesoTaraOrigen = comando.Orden.PesoTaraOrigen,
                            PesoNetoOrigen = comando.Orden.PesoNetoOrigen,
                            TipoVehiculoBodega = Repositorio.Obtener<TipoVehiculoBodega>(comando.Orden.TipoVehiculoBodegaId),
                            Proveedor = Repositorio.Obtener<Proveedor>(comando.Orden.ProveedorId),
                            Posicion = comando.Orden.Posicion

                        };

                    Repositorio.Agregar(remitoBodegaVino);
                    Repositorio.GuardarCambios();
                    resultado.Id = remitoBodegaVino.Id;

                }
            }
            catch (Exception)
            {
                Log.Error("Ocurrió un error al intentar crear el Remito Bodega Vino para el workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.RemitoBodegaVino_Error);
            }

            return resultado;
        }

    }
}
