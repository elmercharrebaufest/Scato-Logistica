using System;
using System.Activities;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class FletesDobleTramoGenerarRequest : CodeActivity
    {
        [RequiredArgument]
        public InArgument<CartaPorteDto> CartaPorte { get; set; }
        [RequiredArgument]
        public InArgument<VehiculoDto> Vehiculo { get; set; }
        public InArgument<int> AlmacenId { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaEgreso { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoNeto { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        public OutArgument<FletesDobleTramoRequest> Request { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            FletesDobleTramoRequest request = null;
            try
            {
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var cartaPorte = CartaPorte.Get<CartaPorteDto>(context);
                var fechaEgreso = FechaEgreso.Get<DateTime>(context);
                var vehiculo = Vehiculo.Get<VehiculoDto>(context);
                var pesoNeto = PesoNeto.Get<int>(context);
                var centro = srvRepositorio.ObtenerCentro(CentroId.Get<int>(context));
                
                var data = new FletesDobleTramo
                    {
                        Almacen = "PLAN",
                        Cargador = PadProveedor(cartaPorte.TitularCartaPorteCodigoSap),
                        Centro = centro.CodigoSAP.Substring(Math.Max(0, centro.CodigoSAP.Length - 4)),
                        Chofer = cartaPorte.Chofer.Apellido + " " + cartaPorte.Chofer.Nombre,
                        EntradaOSalida = cartaPorte.TipoComercialSentido,
                        FechaEgreso = fechaEgreso.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                        FechaIngreso = cartaPorte.FechaEmision.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                        KmRecorridos = cartaPorte.KmRecorrer.HasValue ? cartaPorte.KmRecorrer.Value.ToString("0") : null,
                        Material = cartaPorte.MaterialCodigoSap,
                        Neto = pesoNeto.ToString(CultureInfo.InvariantCulture),
                        CartaPorte = cartaPorte.NroCartaPorte,
                        Patente = vehiculo.Patente,
                        Procedencia = cartaPorte.ProcedenciaCodigoSap,
                        ProvProc = cartaPorte.ProvinciaCodigoSap,
                        Transportista = cartaPorte.TransportistaCUIT.Replace("-", ""),
                        HoraEgreso = fechaEgreso.ToString("HHmmss", CultureInfo.InvariantCulture),
                        HoraIngreso = cartaPorte.FechaEmision.ToString("HHmmss", CultureInfo.InvariantCulture),
                        Secuencia = vehiculo.NumeroVehiculo.ToString(CultureInfo.InvariantCulture),
                    };
       
                request = new FletesDobleTramoRequest(data);


                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                    {
                        var serializer = new XmlSerializer(typeof(FletesDobleTramoRequest));
                        var ms = new MemoryStream();
                        serializer.Serialize(ms, request);

                        var srv = context.GetExtension<IServicioComandos>();
                        srv.Ejecutar(new CrearControlRecorrido
                            {
                                Dto = new ControlRecorridoDto
                                    {
                                        Actividad = "FletesDobleTramoGenerarRequest",
                                        Fecha = DateTime.Now,
                                        Comentario = Encoding.Default.GetString(ms.ToArray()),
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
            Request.Set(context,request);
            Resultado.Set(context, resultado);
        }

        private string PadProveedor(string codigoSap)
        {
            if (String.IsNullOrEmpty(codigoSap))
            {
                return string.Empty;
            }
            // Si el código de proveedor no es numérico no hace el padd con ceros. Este es el caso de los corrredores.
            return Numeric.IsMatch(codigoSap) ? codigoSap.PadLeft(10, '0') : codigoSap;
        }

        private static readonly Regex Numeric = new Regex(@"^\d+$");
    }
}
