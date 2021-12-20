using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresosBodegaGenerarRequest : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        public InArgument<int?> OrdenUvasId { get; set; }
        public InArgument<string> VinedoCalidad { get; set; }
        public InArgument<string> Cosecha { get; set; }
        public InArgument<string> VinedoINV { get; set; }
        public InArgument<string> OrdenDeCompra { get; set; }
        public InArgument<string> NroRemito { get; set; }
        public InArgument<string> EsPropiaPTercerosT { get; set; }
        public InArgument<string> VinedoSubZona { get; set; }
        public InArgument<string> Variedad { get; set; }
        public InArgument<string> VinedoZona { get; set; }               
        public InArgument<DateTime> FechaPesoTara { get; set; }
        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        public InArgument<DateTime> Fecha { get; set; }
        public InArgument<string> PosDocumento { get; set; }
        public InArgument<string> Ciu { get; set; }
        public InArgument<string> CentroOperativo { get; set; }

        public OutArgument<ICollection<IngresosBodegaAsincronicoDto>> Requests { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();          
            var resultado = new Resultado();
            var requests = new List<IngresosBodegaAsincronicoDto>();
            try
            {
                var instanceId = InstanceId.Get<Guid>(context);
                var centroId = CentroId.Get<int>(context);
                var ordenUvasId = OrdenUvasId.Get<int?>(context);
                var materialId = MaterialId.Get<int>(context);
                var fechaPesoTara = FechaPesoTara.Get<DateTime>(context);
                var fecha = Fecha.Get<DateTime>(context);
                var posDocumento = PosDocumento.Get<string>(context);
                var vinedoCalidad = VinedoCalidad.Get<string>(context);
                var cosecha = Cosecha.Get<string>(context);
                var vinedoInv = VinedoINV.Get<string>(context);
                var ordenCompra = OrdenDeCompra.Get<string>(context);
                var nroRemito = NroRemito.Get<string>(context);
                var esPoT = EsPropiaPTercerosT.Get<string>(context);
                var vinedoSubZona = VinedoSubZona.Get<string>(context);
                var variedad = Variedad.Get<string>(context);
                var vinedoZona = VinedoZona.Get<string>(context);
                
                var centro = srvRepositorio.ObtenerCentro(centroId);
                var material = srvRepositorio.ObtenerMaterial(materialId);

                var almacenes = srvRepositorio.ObtenerDistribucionDeAlmacenes(instanceId);

                var centroOperativo = CentroOperativo.Get<string>(context);

                IList<DescargaDeBinesDto> descargaBines = new List<DescargaDeBinesDto>();
                var remitoBodegaUva = new RemitoBodegaUvaDto();
                if (ordenUvasId.HasValue)
                {               
                    remitoBodegaUva = srvRepositorio.ObtenerRemitoBodegaUva(ordenUvasId.Value);
                    descargaBines = remitoBodegaUva.DescargasDeBines;
                }

                var tenorNum = srvRepositorio.ObtenerTenorAzucarinoNumerico(instanceId);
                string tenor = null;
                if (tenorNum != null)
                {
                    tenor = ((int) tenorNum).ToString(CultureInfo.InvariantCulture);
                }
                var ciu = Ciu.Get<string>(context);
                var centroActivoSap = (centro != null ? centro.CodigoSAP : "");
                var centroSap = !string.IsNullOrEmpty(centroOperativo) ? centroOperativo : centroActivoSap;
                var centroInv = centro != null ? centro.NumeroINV : "";
                var materialSapTerceros = material != null && material.CodigoSAP != null ? material.CodigoSAP.PadLeft(18, '0') : "";
                var materialSap = material != null ? material.CodigoSAP : "";
                var esUvaTerceros = esPoT == "T";
                posDocumento = posDocumento != null ? posDocumento.TrimStart('0') : "";

                if (almacenes != null && almacenes.DistribucionesDeAlmacenes != null && almacenes.DistribucionesDeAlmacenes.Any())
                {
                    foreach (var distribucion in almacenes.DistribucionesDeAlmacenes)
                    {
                        requests.Add(new IngresosBodegaAsincronicoDto
                            {
                                IngresosBodega = new IngresosBodegaRequest
                                    {
                                        IngresosBodega = new IngresosBodega
                                            {
                                                Bins = string.Empty,
                                                Calidad = vinedoCalidad, //orden.VinedoCalidad,
                                                Cantidad = distribucion.Litros,//Kilos o litros? de cada almacen?
                                                Centro = centroActivoSap,
                                                Ciu = ciu,
                                                Cosecha = cosecha, //orden.Cosecha,
                                                Cuartel = string.Empty,
                                                FechaContabilizacion = fechaPesoTara.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                                FechaDocumento = fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                                Finca = centroInv,
                                                Inv = vinedoInv, //orden.VinedoINV,
                                                Material = materialSap,
                                                NumDocumento = ordenCompra, //orden.OrdenDeCompra,
                                                NumNota = nroRemito != null ? nroRemito.Replace("-", string.Empty) : null, //orden.NroRemito.Replace("-", string.Empty),
                                                PosDocumento = posDocumento,
                                                Propio = esPoT, //orden.EsPropiaPesTercerosT,
                                                Subzona = vinedoSubZona, //orden.VinedoSubZona,
                                                Tanque = distribucion.AlmacenSap, //almacenSap
                                                Tenor = tenor,//caractarsticaCalidaddel calado o analisis si tuvo sino vacio
                                                Varietal = variedad, //orden.Variedad,
                                                Zona = vinedoZona //orden.VinedoZona
                                            }
                                    }
                            });

                        try
                        {
                            if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                            {
                                var srv = context.GetExtension<IServicioComandos>();
                                srv.Ejecutar(new CrearControlRecorrido
                                    {
                                        Dto = new ControlRecorridoDto
                                            {
                                                Actividad = "IngresosBodegaGenerarRequest",
                                                Fecha = DateTime.Now,
                                                Comentario = requests.Last().IngresosBodega.ToXml(),
                                                NombreUsuario = "",
                                                WorkflowInstanceId = context.WorkflowInstanceId,
                                            }
                                    });
                            }
                        }
                        catch
                        {
                            
                        }
                    }
                }
                else if (descargaBines != null && descargaBines.Any())
                {
                    var cantidadDebines = descargaBines.Sum(x => x.CantidadBines);
                    foreach (var descargaBin in descargaBines)
                    {
                        requests.Add(new IngresosBodegaAsincronicoDto
                        {
                            IngresosBodega = new IngresosBodegaRequest
                                    {
                                        IngresosBodega = new IngresosBodega
                                        {
                                            Bins = descargaBin.EsGranel.HasValue ? (descargaBin.EsGranel.Value ? Decimal.ToInt32((decimal)descargaBin.CantidadBines / 100 * remitoBodegaUva.PesoNetoBodega.Value).ToString() : descargaBin.CantidadBines.ToString()) : string.Empty,
                                            Calidad = vinedoCalidad ?? "", //orden.VinedoCalidad,
                                            Cantidad = descargaBin.EsGranel.HasValue ? Decimal.ToInt32(descargaBin.EsGranel.Value ? (decimal)descargaBin.CantidadBines / 100 * remitoBodegaUva.PesoNetoBodega.Value : ((decimal)remitoBodegaUva.PesoNetoBodega.Value * descargaBin.CantidadBines) / cantidadDebines) : 0,//Kilos o litros? de cada almacen?
                                            Centro = centroActivoSap,
                                            Ciu = ciu,
                                            Cosecha = cosecha, //orden.Cosecha,
                                            Cuartel = descargaBin.Cuartel ?? string.Empty,
                                            FechaContabilizacion = fechaPesoTara.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                            FechaDocumento = fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                            Finca = esUvaTerceros ? vinedoInv : centroSap,
                                            Inv = vinedoInv, //orden.VinedoINV,
                                            Material = esUvaTerceros ? materialSapTerceros : materialSap,
                                            NumDocumento = ordenCompra, //orden.OrdenDeCompra,
                                            NumNota = nroRemito != null ? nroRemito.Replace("-", string.Empty) : null, //orden.NroRemito.Replace("-", string.Empty),
                                            PosDocumento = posDocumento,
                                            Propio = esPoT, //orden.EsPropiaPesTercerosT,
                                            Subzona = vinedoSubZona, //orden.VinedoSubZona,
                                            Tanque = string.Empty, //almacen
                                            Tenor = tenor,//caractarsticaCalidaddel calado o analisis si tuvo sino vacio
                                            Varietal = variedad, //orden.Variedad,
                                            Zona = vinedoZona //orden.VinedoZona
                                        }
                                    },
                            TipoBinId = esUvaTerceros ? descargaBin.TipoId : (int?)null
                        });

                        try
                        {
                            if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                            {
                                var srv = context.GetExtension<IServicioComandos>();
                                srv.Ejecutar(new CrearControlRecorrido
                                {
                                    Dto = new ControlRecorridoDto
                                    {
                                        Actividad = "IngresosBodegaGenerarRequest",
                                        Fecha = DateTime.Now,
                                        Comentario = requests.Last().IngresosBodega.ToXml(),
                                        NombreUsuario = "",
                                        WorkflowInstanceId = context.WorkflowInstanceId,
                                    }
                                });
                            }
                        }
                        catch
                        {
                        }
                    }

                }
                else
                {
                    resultado.Errores.Add("SinBines",Textos.Error_SinBines);
                }
                                            
            }
            catch (Exception e)
            {
                resultado.Errores.Add("", e.ToString());           
            }
            Requests.Set(context, requests);
            Resultado.Set(context, resultado);
        }
    }
}
