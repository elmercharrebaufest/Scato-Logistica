using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class InformarCupoGenerarRequest : CodeActivity
    {
        [RequiredArgument]
        public InArgument<CartaPorteDto> CartaPorte { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoCupo { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaIngreso { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaPesadaTara { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaEgreso { get; set; }
        [RequiredArgument]
        public InArgument<bool> CamionRechazado { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        public OutArgument<Resultado> Resultado { get; set; }
        public OutArgument<Resultado> ResultadoRequest { get; set; }
        public OutArgument<Z_SDMF_Z2200NRequest> Request { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            Z_SDMF_Z2200NRequest request = null;
            try
            {
                var servRepositorio = context.GetExtension<IServicioRepositorio>();
                var cartaDePorte = CartaPorte.Get<CartaPorteDto>(context);
                var codigoCupo = CodigoCupo.Get<string>(context);
                var fechatara = FechaPesadaTara.Get<DateTime>(context);
                var fechaIngreso = FechaIngreso.Get<DateTime>(context);
                var fechaEgreso = FechaEgreso.Get<DateTime>(context);
                var camionRechazado = CamionRechazado.Get<bool>(context);
                var instanceId = InstanceId.Get<Guid>(context);
                var centro = servRepositorio.ObtenerCentro(CentroId.Get<int>(context));

                var corredorSAP = !string.IsNullOrEmpty(cartaDePorte.CorredorVendedorSecundarioCodigoSap) ? cartaDePorte.CorredorVendedorSecundarioCodigoSap : cartaDePorte.CorredorCodigoSap; //Si existe corredor primario y secudario se envía el secundario caso contrario solo se envía el primario.
                var remitentesComerciales = new List<string>();
                if (!string.IsNullOrEmpty(cartaDePorte.RtteComercialCodigoSap)) remitentesComerciales.Add(cartaDePorte.RtteComercialCodigoSap);
                if (!string.IsNullOrEmpty(cartaDePorte.RtteComercialVentaSecundarioCodigoSap)) remitentesComerciales.Add(cartaDePorte.RtteComercialVentaSecundarioCodigoSap);
                if (!string.IsNullOrEmpty(cartaDePorte.RtteComercialVentaSecundario2CodigoSap)) remitentesComerciales.Add(cartaDePorte.RtteComercialVentaSecundario2CodigoSap);

                var rtteComercialSAP = remitentesComerciales.LastOrDefault();

                var corredorSAPDescipcion = !string.IsNullOrEmpty(cartaDePorte.CorredorCodigoSap) && !string.IsNullOrEmpty(cartaDePorte.CorredorVendedorSecundarioCodigoSap) ? cartaDePorte.CorredorVendedorSecundario : cartaDePorte.Corredor; //Si existe corredor primario y secudario se envía el secundario caso contrario solo se envía el primario.
                
                remitentesComerciales = new List<string>();
                if (!string.IsNullOrEmpty(cartaDePorte.RtteComercialCodigoSap)) remitentesComerciales.Add(cartaDePorte.RtteComercial);
                if (!string.IsNullOrEmpty(cartaDePorte.RtteComercialVentaSecundarioCodigoSap)) remitentesComerciales.Add(cartaDePorte.RtteComercialVentaSecundario);
                if (!string.IsNullOrEmpty(cartaDePorte.RtteComercialVentaSecundario2CodigoSap)) remitentesComerciales.Add(cartaDePorte.RtteComercialVentaSecundario2);

                var rtteComercialSAPDescipcion = remitentesComerciales.LastOrDefault();

                request = new Z_SDMF_Z2200NRequest
                    {
                        Z_SDMF_Z2200N = new Z_SDMF_Z2200N
                            {
                                IM_Z2200 = new[]{new ZMPES2200
                                    {
                                        CODIGO = codigoCupo,
                                        CENTRO = centro.CodigoSAP,
                                        CORREDOR = cartaDePorte.Cpe ? corredorSAP : cartaDePorte.CorredorCodigoSap,
                                        AGENTE_DE_COMPRA = cartaDePorte.AgenteComprasCodigoSap,
                                        FECHA_INGRESO = fechaIngreso.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                                        FECHA_TARA = fechatara.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                                        HORA_TARA = fechatara.ToString("HHmmss", CultureInfo.InvariantCulture),
                                        HORA_INRGESO = fechaIngreso.ToString("HHmmss", CultureInfo.InvariantCulture),
                                        FECHA_EGRESO = fechaEgreso.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                                        HORA_EGRESO = fechaEgreso.ToString("HHmmss", CultureInfo.InvariantCulture),
                                        DESTINATARIO = cartaDePorte.DestinatarioCodigoSap,
                                        MATERIAL = cartaDePorte.MaterialCodigoSap,
                                        RECHAZADO = camionRechazado ? "X": null,
                                        REMITENTE_COM = cartaDePorte.Cpe ? rtteComercialSAP : cartaDePorte.RtteComercialCodigoSap,
                                        ESTABLECIMIENTO = servRepositorio.ObtenerCodigoEstablecimientoPorGuid(instanceId),
                                        NUMCARPOR = cartaDePorte.NroCartaPorteSAP, //CPE
                                        PROVEEDOR = cartaDePorte.TitularCartaPorteCodigoSap,
                                        DESCCENTRO = centro.Descripcion,
                                        DESCAGENTE = cartaDePorte.AgenteCompras,
                                        DESCCORREDOR = cartaDePorte.Cpe ? corredorSAPDescipcion : cartaDePorte.Corredor,
                                        DESCDESTINA = cartaDePorte.Destinatario,
                                        DESCMAT = cartaDePorte.Material,
                                        DESCPROV = cartaDePorte.TitularCartaPorte,
                                        DESCREMIT = cartaDePorte.Cpe ? rtteComercialSAPDescipcion : cartaDePorte.RtteComercial,
                                        CUIT_SOLICITANTE = cartaDePorte.TitularCartaPorteCuil.Replace("-", ""), // CPE
                                        SECUENCIA = cartaDePorte.Cpe ? cartaDePorte.SecuenciaSap : string.Empty //CPE
                                    }
                                }
                            }
                    };
                

                if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                {
                    var servComandos = context.GetExtension<IServicioComandos>();
                    servComandos.Ejecutar(new CrearControlRecorrido
                        {
                            Dto =
                                new ControlRecorridoDto
                                    {
                                        Actividad = "InformarCupoGenerarRequest",
                                        Fecha = DateTime.Now,
                                        ActividadXaml = "InformarCupoGenerarRequest",
                                        NombreUsuario = "",
                                        Comentario = request.ToXml(),
                                        WorkflowInstanceId = instanceId
                                    }
                        });
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("", e.Message);
            }
            Resultado.Set(context, resultado);
            Request.Set(context,request);
        }
    }
}
