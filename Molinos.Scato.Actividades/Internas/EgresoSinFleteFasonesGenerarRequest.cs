using System;
using System.Activities;
using System.Configuration;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class EgresoSinFleteFazonesGenerarRequest : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> TransportistaId { get; set; }
        [RequiredArgument]
        public InArgument<int> DestinoId { get; set; }
        [RequiredArgument]
        public InArgument<int> Cantidad { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        public InArgument<DateTime> FechaCon { get; set; }
        public InArgument<DateTime> FechaDoc { get; set; }
        public InArgument<int> MaterialId { get; set; }
        public InArgument<int> ChoferId { get; set; }
        public InArgument<string> PatenteCamion { get; set; }
        public InArgument<string> PatenteAcoplado { get; set; }
        public InArgument<string> NumRemito  { get; set; }

        public OutArgument<EgresoSinFleteFazonesRequest> Request { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            EgresoSinFleteFazonesRequest request = null;
            try
            {
                var transportistaId = TransportistaId.Get<int>(context);
                var destinoId = DestinoId.Get<int>(context);
                var cantidad = Cantidad.Get<int>(context);
                var centroId = CentroId.Get<int>(context);

                var fechaCon = FechaCon.Get<DateTime>(context);
                var fechaDoc = FechaDoc.Get<DateTime>(context);
                var materialId = MaterialId.Get<int>(context);
                var choferId = ChoferId.Get<int>(context);
                var patenteCamion = PatenteCamion.Get<string>(context);
                var patenteAcoplado = PatenteAcoplado.Get<string>(context);

                var almacen = srvRepositorio.ObtenerAlmacenPredeterminado(centroId, materialId);
                var transportista = srvRepositorio.ObtenerTransportista(transportistaId);
                var destino = srvRepositorio.ObtenerCliente(destinoId);
                var material = srvRepositorio.ObtenerMaterial(materialId);
                var chofer = srvRepositorio.ObtenerChofer(choferId);
                var centro = srvRepositorio.ObtenerCentro(centroId);
                var remito = NumRemito.Get<string>(context);

                request = new EgresoSinFleteFazonesRequest(new EgresoSinFleteFazones
                {
                    Almacen = almacen == null ? string.Empty : almacen.CodigoSAP,
                    CUITCliente = destino == null ? string.Empty : destino.Cuit.Replace("-", string.Empty),
                    CUITD = transportista == null ? string.Empty : transportista.Cuit.Replace("-", string.Empty),
                    Cantidad = cantidad.ToString(),
                    Centro = centro == null ? string.Empty : centro.CodigoSAP,
                    FechaCon = fechaCon.ToString("yyyy-MM-dd"),
                    FechaDoc = fechaDoc.ToString("yyyy-MM-dd"),
                    Material = material == null ? string.Empty : material.CodigoSAP,
                    NomChofer = chofer == null ? string.Empty : chofer.NombreCompleto,
                    NomTransportista = transportista == null ? string.Empty : transportista.RazonSocial,
                    NumDocu = chofer == null ? string.Empty : chofer.NumeroDeDocumento,
                    Patente = patenteCamion,
                    PatenteRemolque = patenteAcoplado,
                    TipoDocu = chofer == null ? string.Empty : chofer.TipoDocumentoIdentidadCodigoSap                   
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
                                        Actividad = "EgresoSinFleteFasonesGenerarRequest",
                                        Fecha = DateTime.Now,
                                        Comentario = request.ToXml(),
                                        NombreUsuario = "",
                                        WorkflowInstanceId = context.WorkflowInstanceId,
                                    }
                            });
                    }
                }
                catch (Exception e)
                {
                    resultado.Errores.Add("ControlRecorrido", e.Message);
                }
                
            }
            catch (Exception e)
            {
                resultado.Errores.Add("", e.Message);

            }
            Request.Set(context, request);
            Resultado.Set(context, resultado);
        }
    }
}
