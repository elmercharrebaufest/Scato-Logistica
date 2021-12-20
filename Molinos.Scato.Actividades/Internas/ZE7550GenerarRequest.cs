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
using System.Linq;
using System.Text.RegularExpressions;

namespace Molinos.Scato.Actividades.Internas
{
    public class ZE7550GenerarRequest : CodeActivity
    {
        [RequiredArgument]
        public InArgument<CartaPorteDto> CartaPorte { get; set; }
        [RequiredArgument]
        public InArgument<VehiculoDto> Vehiculo { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaEgreso { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoTara { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoBruto { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoNeto { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaPesoTara { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaPesoBruto { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaPesoNeto { get; set; }
        [RequiredArgument]
        public InArgument<int> CamaraId { get; set; }
        [RequiredArgument]
        public InArgument<bool> CamionRechazado { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        public OutArgument<Resultado> ResultadoRequest { get; set; }
        public OutArgument<Z_SDMF_RFC_ZE7550Request> Request { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            Z_SDMF_RFC_ZE7550Request request = null;
            try
            {
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var cartaPorte = CartaPorte.Get<CartaPorteDto>(context);
                var fechaEgreso = FechaEgreso.Get<DateTime>(context);
                var pesoTara = PesoTara.Get<int>(context);
                var vehiculo = Vehiculo.Get<VehiculoDto>(context);
                var pesoBruto = PesoBruto.Get<int>(context);
                var pesoNeto = PesoNeto.Get<int>(context);
                var fechaPesoNeto = FechaPesoNeto.Get<DateTime>(context);
                var fechaPesoBruto = FechaPesoBruto.Get<DateTime>(context);
                var fechaPesoTara = FechaPesoTara.Get<DateTime>(context);
                var camionRechazado = CamionRechazado.Get<bool>(context);
                var instanceId = InstanceId.Get<Guid>(context);

                var centro = srvRepositorio.ObtenerCentro(CentroId.Get<int>(context));

                var camaraExcepcion = srvRepositorio.ObtenerCamaraDeExcepcionDescuento(instanceId, cartaPorte.MaterialId, centro.Id);
                var camara = camaraExcepcion ?? srvRepositorio.ObtenerCamara(CamaraId.Get<int>(context));
                var asignacion = srvRepositorio.ObtenerAsignacionDePuestoComando(instanceId.ToString("D"));
                var balanza = srvRepositorio.ObtenerBalanza(asignacion.BalanzaTaraId ?? 0);
                var almacen = srvRepositorio.ObtenerAlmacen(asignacion.AlmacenId);
                var calado = srvRepositorio.ObtenerCaladoPorGuid(instanceId);
                var carAnalizadas = calado != null ? srvRepositorio.ObtenerAnalisisDeCalidadPorCaladoId(calado.Id) : null;
                var kilosNetosDescontados = pesoNeto - (calado != null && calado.CaladosPorCaracteristica != null && calado.CaladosPorCaracteristica.Any() ? srvRepositorio.TotalKilosDescuentos(calado, carAnalizadas, pesoNeto) : 0);

                var firmasCuit = srvRepositorio.ListarCuitfirmas();

                var material = srvRepositorio.ObtenerMaterialPorCentro(centro.Id, cartaPorte.MaterialId);
                var chofer = cartaPorte.Chofer.Apellido + " " + cartaPorte.Chofer.Nombre;
                var tieneAnalisisInterno = srvRepositorio.ObtenerNumeroAleatorio() < (material.AnalisisInterno ?? 0);


                var data = new Z_SDMF_RFC_ZE7550
                {
                    IM_ZE7550 = new ZMPES5170[]
                    {
                        new ZMPES5170
                        {
                            NUMCARPOR = cartaPorte.NroCartaPorteSAP, //CPE
                            ACOPLADO = vehiculo.PatenteAcoplado,
                            AGENTE_DE_COMPRA=PadProveedor(cartaPorte.AgenteComprasCodigoSap),
                            ALMACEN = almacen != null ? almacen.CodigoSAP.Substring(Math.Max(0, almacen.CodigoSAP.Length - 4)) : null,
                            ANALISIS_INT = tieneAnalisisInterno ? "S" : "N",
                            APARCERIA = cartaPorte.Aparceria ? "S" : "N",
                            BALANZA = balanza != null ? balanza.CodigoCabezal.Substring(Math.Max(0, balanza.CodigoCabezal.Length - 4)) : null,
                            //BOLSAS=,
                            BRUTO = pesoBruto,
                            BRUTO_ORIGEN = vehiculo.PesoBrutoOrigen ?? 0,
                            CALIDAD_ESPECIAL = cartaPorte.TrigoEspecial ? "X" : string.Empty,
                            CAMARA_A_PRESENT = camara != null ? camara.CodigoSAP : null,
                            CARATULA = cartaPorte.Caratula != null ? cartaPorte.Caratula.ToString() : null,
                            CARGADOR = PadProveedor(cartaPorte.TitularCartaPorteCodigoSap),
                            CCPP_REF = "",
                            CENTRO = centro.CodigoSAP.Substring(Math.Max(0, centro.CodigoSAP.Length - 4)),
                            CHOFER = chofer.Length > 20 ? chofer.Substring(0,20) : chofer,
                            CLASIFICACION = cartaPorte.TipoCategoria,
                            //CLIPPER=,
                            CONTRATO = cartaPorte.AcuerdoMarco,
                            //CONTRVEND=,
                            CORREDOR = PadProveedor(cartaPorte.CorredorCodigoSap),
                            COSECHA = cartaPorte.Cosecha,
                            CTG = cartaPorte.Cpe ? cartaPorte.CpeSap : cartaPorte.CTG, //CPE
                            CUENTAORDEN = firmasCuit.Any(x => x == cartaPorte.DestinatarioCuil) ? PadProveedor(cartaPorte.RtteComercialCodigoSap) : PadProveedor(cartaPorte.DestinatarioCodigoSap),
                            CUIT_DESTINATARI = cartaPorte.DestinatarioCuil != null ? cartaPorte.DestinatarioCuil.Replace("-", "") : "",
                            DESTINO = cartaPorte.DestinoCodigoSap.Substring(Math.Max(0, cartaPorte.DestinoCodigoSap.Length - 4)),
                            //DUPLICADO=,
                            ENTRADA_O_SALIDA = cartaPorte.TipoComercialSentido,
                            ESTABLECIMIENTO = cartaPorte.CodEstab,
                            ESTADO = camionRechazado ? "R" : "A",
                            //FECHAACT=,
                            //FECHA_ACT_APLIC=,
                            //FECHA_ACT_CAL=,
                            //FECHA_ACT_CAL1=,
                            //FECHA_ACT_CAL2=,
                            //FECHA_ACT_GASTOS=,
                            //FECHA_ACT_STOCK=,
                            FECHA_ALTA = cartaPorte.FechaEmision.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                            FECHA_BRUTO = fechaPesoBruto == null ? null : fechaPesoBruto.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                            FECHA_CALADO = calado != null && calado.FechaCreacion.HasValue ? calado.FechaCreacion.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
                            FECHA_EGRESO =  fechaEgreso == null ? null : fechaEgreso.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                            FECHA_INGRESO = cartaPorte.FechaEmision.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                            FECHA_NETO = fechaPesoNeto == null ? null : fechaPesoNeto.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                            FECHA_TARA = fechaPesoTara == null ? null : fechaPesoTara.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                            FECHA_CTG=  fechaEgreso == null ? null : fechaEgreso.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                            //FECHA_SUBIDA=,
                            //FECHA_VALIDACION =,
                            //GRADO=,
                            //GRADO_CAMARA=,
                            //HORAACT=,
                            //HORA_ACT_APLI=,
                            //HORA_ACT_CAL=,
                            //HORA_ACT_CAL1=,
                            //HORA_ACT_CAL2=,
                            //HORA_ACT_GASTOS=,
                            //HORA_ACT_STOCK=,
                            //HORA_VALIDACION=,
                            HORA_BRUTO = fechaPesoBruto == null ? null : fechaPesoBruto.ToString("HHmmss", CultureInfo.InvariantCulture),
                            HORA_CALADO = calado != null && calado.FechaCreacion.HasValue ? calado.FechaCreacion.Value.ToString("HHmmss", CultureInfo.InvariantCulture) : null,
                            HORA_EGRESO = fechaEgreso == null ? null : fechaEgreso.ToString("HHmmss", CultureInfo.InvariantCulture),
                            HORA_INGRESO = cartaPorte.FechaEmision.ToString("HHmmss", CultureInfo.InvariantCulture),
                            HORA_NETO = fechaPesoNeto == null ? null : fechaPesoNeto.ToString("HHmmss", CultureInfo.InvariantCulture),
                            HORA_TARA = fechaPesoTara == null ? null : fechaPesoTara.ToString("HHmmss", CultureInfo.InvariantCulture),
                            //IMP_1116A=,
                            INTER_FLETE = cartaPorte.IntermediarioFleteCodigoSap,
                            //KG_REALES=,
                            KM_RECOR = cartaPorte.KmRecorrer ?? 0,
                            MATERIAL = cartaPorte.MaterialCodigoSap,
                            //MRP_PAG_FLETE=,
                            MUESTRA_CONJUNTO = calado != null && calado.MuestraConjunto.HasValue ? calado.MuestraConjunto.ToString() : null,
                            NETO = pesoNeto,
                            NETO_DESCONTADO = ((int)decimal.Round(kilosNetosDescontados)),
                            NETO_ORIGEN = vehiculo.PesoNetoOrigen ?? 0,
                            //NOMBRE_ARCH=,
                            //NO_VALIDA_CG=,
                            //NO_VALIDA_CTG=,
                            NRODOCHOFER = cartaPorte.Chofer.NumeroDeDocumento.ToString(CultureInfo.InvariantCulture),
                            //NRO_ARCH=,
                            //OBS_TRANSPORT=,
                            PESADA = balanza != null ? (balanza.Modalidad == Modalidad.Automática ? "A" : "M") : null,
                            PATENTE = vehiculo.Patente,
                            PRESTADOR = PadProveedor(cartaPorte.PrestadorCodigoSap),
                            PROCEDENCIA = cartaPorte.ProcedenciaCodigoSap,
                            PROVEEDOR = PadProveedor(cartaPorte.TitularCartaPorteCodigoSap),
                            PROV_PROC = cartaPorte.ProvinciaCodigoSap,
                            REMITENTE_COM = PadProveedor(cartaPorte.RtteComercialCodigoSap),
                            //RT=,
                            SECUENCIA = cartaPorte.Cpe ? cartaPorte.SecuenciaSap : vehiculo.NumeroVehiculo.ToString(CultureInfo.InvariantCulture), //CPE 
                            //STATUS_APLIC=,
                            //STATUS_CAL=,
                            //STATUS_CAL1=,
                            //STATUS_CAL2=,
                            //STATUS_CP=,
                            //STATUS_GASTOS=,
                            //STATUS_STOCK=,
                            TARA = pesoTara,
                            TARA_ORIGEN = vehiculo.PesoTaraOrigen ?? 0,
                            TIPODOCHOFER = cartaPorte.Chofer.TipoDocumentoIdentidadCodigoSap,
                            TIPO_COMERCIAL = cartaPorte.TipoComercialCodigoSap,
                            TIP_VEHI = cartaPorte.TipoVehiculo == TipoVehiculo.Tren ? "T" : "C",
                            TRANSPORTISTA = cartaPorte.TransportistaCUIT.Length > 0 ? cartaPorte.TransportistaCUIT.Replace("-", "").Substring(2, 8) : cartaPorte.TransportistaCUIT,
                            //TRANSPORT_ANT=,
                            //UDATE=,
                            //USERNAME=,
                            //UTIME=,
                            //VALIDADO=,
                            VARIEDAD = cartaPorte.Variedad= cartaPorte.Variedad,
                            //ERR_APLIC=,
                            //ERR_CAL=,
                            //ERR_CAL1=,
                            //ERR_CAL2=,
                            //ERR_GASTOS=,
                            //ERR_STOCK=,
                            //ERR_VALID =
                            CUIT_SOLICITANTE = cartaPorte.Cpe ? cartaPorte.TitularCartaPorteCuil.Replace("-", "") : string.Empty
                        }
                    }                        
                };

                request = new Z_SDMF_RFC_ZE7550Request(data);

                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                    {
                        var srv = context.GetExtension<IServicioComandos>();
                        srv.Ejecutar(new CrearControlRecorrido
                            {
                                Dto = new ControlRecorridoDto
                                    {
                                        Actividad = "Z7550GenerarRequest",
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
                    resultado.Errores.Add("ControlRecorrido", "ULTIMO " + e.Message);
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
