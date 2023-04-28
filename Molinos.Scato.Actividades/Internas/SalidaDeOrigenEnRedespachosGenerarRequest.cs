using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using System;
using System.Activities;
using System.Configuration;
using System.Globalization;

namespace Molinos.Scato.Actividades.Internas
{
    public class SalidaDeOrigenEnRedespachosGenerarRequest : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        //[RequiredArgument]
        //public InArgument<int> AlmacenReceptorId { get; set; }
        [RequiredArgument]
        public InArgument<int> Cantidad { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroEmisorId { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroReceptorId { get; set; }

        [RequiredArgument]
        public InArgument<string> ClaseExp { get; set; }

        [RequiredArgument]
        public InArgument<int> TransportistaId { get; set; }

        public InArgument<string> NroDocumento { get; set; }
        public InArgument<string> Lote { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FechaContab { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FechaDoc { get; set; }

        [RequiredArgument]
        public InArgument<int> Kilometros { get; set; }

        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }

        [RequiredArgument]
        public InArgument<int> ChoferId { get; set; }

        [RequiredArgument]
        public InArgument<string> Patente { get; set; }

        public InArgument<string> PatenteAcoplado { get; set; }

        [RequiredArgument]
        public InArgument<int> Precinto1Id { get; set; }

        [RequiredArgument]
        public InArgument<int> Precinto2Id { get; set; }

        [RequiredArgument]
        public InArgument<TipoDocumentoIngreso> TipoDoc { get; set; }

        public OutArgument<Mov975Request> Request { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();
            Mov975Request request = null;
            try
            {
                var cantidad = Cantidad.Get<int>(context);
                var centroEmisorId = CentroEmisorId.Get<int>(context);
                var centroReceptorId = CentroReceptorId.Get<int>(context);
                var claseExp = ClaseExp.Get<string>(context);
                var transportistaId = TransportistaId.Get<int>(context);

                var lote = Lote.Get<string>(context);
                var fechaContab = FechaContab.Get<DateTime>(context);
                var fechaDoc = FechaDoc.Get<DateTime>(context);
                var kilometros = Kilometros.Get<int>(context);
                var materialId = MaterialId.Get<int>(context);
                var choferId = ChoferId.Get<int>(context);
                var patente = Patente.Get<string>(context);
                var patenteAcoplado = PatenteAcoplado.Get<string>(context) ?? "";
                var precinto1Id = Precinto1Id.Get<int>(context);
                var precinto2Id = Precinto2Id.Get<int>(context);

                var asignacion = srvRepositorio.ObtenerAsignacionDePuestoComando(InstanceId.Get<Guid>(context).ToString("D"));
                var almacenEmisor = srvRepositorio.ObtenerAlmacen(asignacion.AlmacenId);
                var materialPorCentro = srvRepositorio.ObtenerMaterialPorCentro(centroReceptorId, materialId);
                AlmacenDto almacenReceptor = null;
                if (materialPorCentro != null && materialPorCentro.AlmacenPredId.HasValue)
                {
                    almacenReceptor = srvRepositorio.ObtenerAlmacen(materialPorCentro.AlmacenPredId.Value);
                }
                var centroEmisor = srvRepositorio.ObtenerCentro(centroEmisorId);
                var centroReceptor = srvRepositorio.ObtenerCentro(centroReceptorId);
                var transportista = srvRepositorio.ObtenerTransportista(transportistaId);
                var chofer = srvRepositorio.ObtenerChofer(choferId);
                var material = srvRepositorio.ObtenerMaterial(materialId);
                var precinto1 = srvRepositorio.ObtenerPrecinto(precinto1Id);
                var precinto2 = srvRepositorio.ObtenerPrecinto(precinto2Id);
                var recorrido = srvRepositorio.ObtenerDatosDeInstanciaAltaCTGPorGuid(InstanceId.Get<Guid>(context));
                var cartaPorte = srvRepositorio.ObtenerCartaDePortePorrecorrido(recorrido.Id);

                var almacenEmisorSap = almacenEmisor != null ? almacenEmisor.CodigoSAP : "";
                var almacenReceptorSap = string.Empty;

                if (almacenEmisor != null && almacenEmisor.EsSojaEPA)
                    almacenReceptorSap = almacenEmisorSap;
                else
                    almacenReceptorSap = almacenReceptor != null ? almacenReceptor.CodigoSAP : "REDE";

                var centroEmisorSap = centroEmisor != null ? centroEmisor.CodigoSAP : "";
                var centroReceptorSap = centroReceptor != null ? centroReceptor.CodigoSAP : "";
                var materialeSap = material != null ? material.CodigoSAP : "";
                var unindadDeMedida = material != null ? material.UnidadDeMedidad : "";
                var precintonum1 = precinto1 != null ? precinto1.NumeroPrecinto : "";
                var precintonum2 = precinto2 != null ? precinto2.NumeroPrecinto : "";
                var factorConversion = material != null ? material.FactorConversion : null;

                var nroDocumento = NroDocumento.Get<string>(context);
                nroDocumento = cartaPorte?.Cpe ?? false ? nroDocumento.PadLeft(12, '0') : nroDocumento;
                nroDocumento = nroDocumento != null && nroDocumento.IndexOf("-", StringComparison.Ordinal) == -1 ? nroDocumento.Substring(0, 4) + "-" + nroDocumento.Substring(4, 8) : nroDocumento;

                if (!string.IsNullOrEmpty(cartaPorte?.IntermediarioFleteCuil) && !string.IsNullOrEmpty(transportista?.Cuit))
                {
                    request = SetMov975Request(cantidad, claseExp, lote, fechaContab, fechaDoc, kilometros, patente, patenteAcoplado,
                        chofer, almacenEmisorSap, almacenReceptorSap, centroEmisorSap, centroReceptorSap, materialeSap,
                        unindadDeMedida, precintonum1, precintonum2, factorConversion, nroDocumento,
                        cartaPorte?.IntermediarioFleteCuil?.Replace("-", ""), cartaPorte?.IntermediarioFlete);
                }
                else
                {
                    request = SetMov975Request(cantidad, claseExp, lote, fechaContab, fechaDoc, kilometros, patente, patenteAcoplado,
                        chofer, almacenEmisorSap, almacenReceptorSap, centroEmisorSap, centroReceptorSap, materialeSap,
                        unindadDeMedida, precintonum1, precintonum2, factorConversion, nroDocumento,
                        transportista.Cuit.Replace("-", ""), transportista.RazonSocial.Truncate(35));
                }

                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                    {
                        var srv = context.GetExtension<IServicioComandos>();
                        srv.Ejecutar(new CrearControlRecorrido
                        {
                            Dto = new ControlRecorridoDto
                            {
                                Actividad = "SalidaDeOrigenEnRedespachosGenerarRequest",
                                Fecha = DateTime.Now,
                                Comentario = request.ToXml(),
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
            catch (Exception e)
            {
                resultado.Errores.Add("", e.Message);
            }
            Request.Set(context, request);
            Resultado.Set(context, resultado);
        }

        private static Mov975Request SetMov975Request(int cantidad, string claseExp, string lote, DateTime fechaContab, DateTime fechaDoc,
            int kilometros, string patente, string patenteAcoplado, ChoferDto chofer, string almacenEmisorSap,
            string almacenReceptorSap, string centroEmisorSap, string centroReceptorSap, string materialeSap, string unindadDeMedida,
            string precintonum1, string precintonum2, decimal? factorConversion, string nroDocumento,
            string transportistaCuit, string transportistaNombre)
        {
            return new Mov975Request(new Mov975
            {
                AlmEmisor = almacenEmisorSap,
                AlmReceptor = almacenReceptorSap,
                Cantidad = factorConversion == null ? cantidad.ToString(CultureInfo.InvariantCulture) : (cantidad / factorConversion.Value).ToString(CultureInfo.InvariantCulture),
                CentroEmisor = centroEmisorSap,
                CentroReceptor = centroReceptorSap,
                ClaseExpedicion = claseExp,
                CUITTransp = transportistaCuit,
                NombreTransportista = transportistaNombre,
                NroDocumento = nroDocumento, //Si es CPE se envia el nro de CTG sin formato caso contrario NroCartaPorte formateado.
                Lote = lote,
                FechaContab = fechaContab.ToString("yyyy-MM-dd"),
                FechaDoc = fechaDoc.ToString("yyyy-MM-dd"),
                Kilometros = Convert.ToDecimal(kilometros),
                Material = materialeSap,
                DocChofer = chofer.NumeroDeDocumento.Replace("-", string.Empty),
                NombreChofer = chofer.NombreCompleto,
                Patente1 = patente,
                Patente2 = patenteAcoplado,
                Precinto1 = precintonum1,
                Precinto2 = precintonum2,
                TipoDoc = chofer.TipoDocumentoIdentidadCodigoSap,
                UniMed = unindadDeMedida
            });
        }
    }
}