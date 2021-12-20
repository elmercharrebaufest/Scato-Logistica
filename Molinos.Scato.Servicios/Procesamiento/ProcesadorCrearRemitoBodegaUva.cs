using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearRemitoBodegaUva : ProcesadorComando<CrearRemitoBodegaUva>
    {
        public ProcesadorCrearRemitoBodegaUva(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearRemitoBodegaUva comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearRemitoBodegaUva para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var descargas = new List<DescargaDeBines>();


                    var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
                    var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
                    var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
                    var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
                    var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);
                    var tipoDocumentoIngreso = comando.EsPropia ? TipoDocumentoIngreso.RemitoBodegaUvaPropia : TipoDocumentoIngreso.RemitoBodegaUvaTerceros;
                    var recorrido = new Recorrido { InstanciaWorkflow = comando.InstanciaWorkflowId, Workflow = workflow, Chofer = chofer, Centro = centro, Patente = comando.Orden.Patente, Transportista = transportista, TipoComercial = tipoComercial, TipoDocumentoIngreso = tipoDocumentoIngreso, Material = material, NumeroDocumentoIngreso = comando.Orden.NroRemito, FechaInicio = DateTime.Now, WorkflowDefinicion = workflowDefinicion, PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen, PesoTaraOrigen = comando.Orden.PesoTaraOrigen};
                    Repositorio.Agregar(recorrido);
                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);

                    var remitoBodegaUva = new RemitoBodegaUva
                    {
                        Chofer = chofer,
                        Material = material,
                        NroRemito = comando.Orden.NroRemito,
                        Patente = comando.Orden.Patente,
                        TipoComercial = tipoComercial,
                        Transportista = transportista,
                        Recorrido = recorrido,
                        Cosecha = comando.Orden.Cosecha,
                        MarcaCamion = comando.Orden.MarcaCamion,
                        ModeloCamion = comando.Orden.ModeloCamion,
                        Proveedor = Repositorio.Obtener<Proveedor>(comando.Orden.ProveedorId),
                        Vinedo = Repositorio.Obtener<Vinedo>(comando.Orden.VinedoId),
                        OrdenDeCompra = comando.Orden.OrdenDeCompra,
                        PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen,
                        PesoTaraOrigen = comando.Orden.PesoTaraOrigen,
                        PesoNetoOrigen = comando.Orden.PesoNetoOrigen,
                        TipoCosecha = comando.Orden.TipoCosecha ?? TipoCosecha.Manual,
                        TipoVehiculoBodega = Repositorio.Obtener<TipoVehiculoBodega>(comando.Orden.TipoVehiculoBodegaId),
                        DescargasDeBines = descargas,
                        Posicion = comando.Orden.Posicion
                    };

                    foreach (var descargaDeBines in comando.Orden.DescargasDeBines)
                    {
                        var descarga = new DescargaDeBines
                        {
                            Tipo = Repositorio.Obtener<Material>(descargaDeBines.TipoId),
                            CantidadBines = descargaDeBines.CantidadBines,
                            Cuartel = Repositorio.Obtener<Cuartel>(descargaDeBines.CuartelId),
                            RemitoBodegaUva = remitoBodegaUva
                        };
                        descargas.Add(descarga);
                    }

                    Repositorio.Agregar(remitoBodegaUva);
                    Repositorio.GuardarCambios();
                    resultado.Id = remitoBodegaUva.Id;
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear el remito bodega uva para la instancia de workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.RemitoBodegaUva_Error);
            }

            return resultado;
        }
    }
}
