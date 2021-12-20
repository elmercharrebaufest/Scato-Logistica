using System;
using System.Activities;
using System.Configuration;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;

namespace Molinos.Scato.Actividades.Internas
{
    public class RegistrarCartadePorteGenerarRequest : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<TipoVehiculo> TipoVehiculo { get; set; }
        [RequiredArgument]
        public InArgument<string> CTG { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> Tecnologia { get; set; }
        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<string> ProcedenciaCodigoSap { get; set; }
        [RequiredArgument]
        public InArgument<string> CodEstab { get; set; }
        [RequiredArgument]
        public InArgument<string> NroCartaPorte { get; set; }

        [RequiredArgument]
        public InArgument<string> TitularCartaPorte { get; set; }
        [RequiredArgument]
        public InArgument<string> TitularCartaPorteCuit { get; set; }

        [RequiredArgument]
        public InArgument<string> RtteComercial { get; set; }
        [RequiredArgument]
        public InArgument<string> RtteComercialCuit { get; set; }

        [RequiredArgument]
        public InArgument<string> DestinatarioCuit { get; set; }

        public OutArgument<ParametrosRegistro> Request { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            ParametrosRegistro request = null;
            try
            {
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var tipoVehiculo = TipoVehiculo.Get<TipoVehiculo>(context);
                var instanceId = InstanceId.Get<Guid>(context);
                var cgt = CTG.Get<string>(context) ?? "";
                var centroId = CentroId.Get<int>(context);
                var tecnologia = Tecnologia.Get<string>(context) ?? "";
                var materialId = MaterialId.Get<int>(context);
                var procedenciaCodigoSap = ProcedenciaCodigoSap.Get<string>(context);
                var codEstab = CodEstab.Get<string>(context) ?? "";
                var nroCartaPorte = NroCartaPorte.Get<string>(context);
                var titularCartaPorte = TitularCartaPorte.Get<string>(context) ?? "";
                var titularCartaPorteCuit = TitularCartaPorteCuit.Get<string>(context);
                var rtteComercial = RtteComercial.Get<string>(context) ?? "";
                var rtteComercialCuit = RtteComercialCuit.Get<string>(context);
                var destinatarioCuit = DestinatarioCuit.Get<string>(context);

                var centro = srvRepositorio.ObtenerCentro(centroId);
                var material = srvRepositorio.ObtenerMaterial(materialId);


                var destinoGranos = new DestinoGranos {
                    codigoLocalidadDestino = Convert.ToInt64(centro.LocalidadCodigoSap),
                    numeroPlantaDestino = Convert.ToInt64(centro.CodigoEstablecimiento),
                };

                var granosTransportados = new GranosTransportados
                    {
                        codigoBiotecnologiaDeclarada = tecnologia,
                        codigoEspecie = material.CodigoEspecie.HasValue ? material.CodigoEspecie.Value.ToString(CultureInfo.InvariantCulture) : "",
                        codigoLocalidadProcedencia = !String.IsNullOrEmpty(procedenciaCodigoSap) ? Convert.ToInt64(procedenciaCodigoSap) : 0,
                        codigoLocalidadProcedenciaSpecified = !String.IsNullOrEmpty(procedenciaCodigoSap),
                        establecimiento = codEstab == "999999" ? String.Empty : codEstab
                    };
                
                var intervinientes = new IntervinientesRegistro
                    {
                        destinatario = DocumentoInterviniente(destinatarioCuit),
                        destino = DocumentoInterviniente(centro.Cuit),
                        remitenteComercial = Interviniente(rtteComercialCuit,rtteComercial),
                        titular = IntervinienteRequerido(titularCartaPorteCuit,titularCartaPorte),
                    };


                if (tipoVehiculo != Dominio.Enums.TipoVehiculo.Tren)
                {
                    request = new ParametrosRegistro
                    {
                        Item = new CartaPorteTransporteAutomotorRegistro
                            {
                                ctg = cgt,
                                destinoGranos = destinoGranos,
                                granosTransportados = granosTransportados,
                                intervinientes = intervinientes,
                                numeroCartaPorte = !String.IsNullOrEmpty(nroCartaPorte) ? Convert.ToInt64(nroCartaPorte) : 0,
                                numeroCartaPorteSpecified = !String.IsNullOrEmpty(nroCartaPorte),
                            }

                    };
                }
                else
                {
                    var cantidadVagones = srvRepositorio.ObtenerCantidadVagones(instanceId);
                    request = new ParametrosRegistro
                    {
                        Item = new CartaPorteVagonFerroviarioRegistro
                            {
                                cantidadVagones = cantidadVagones,
                                cantidadVagonesSpecified = cantidadVagones > 0,
                                destinoGranos = destinoGranos,
                                granosTransportados = granosTransportados,
                                intervinientes = intervinientes,
                                numeroCartaPorte = !String.IsNullOrEmpty(nroCartaPorte) ? Convert.ToInt64(nroCartaPorte) : 0,
                                numeroCartaPorteSpecified = !String.IsNullOrEmpty(nroCartaPorte),
                            }
                    };
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
                                        Actividad = "RegistrarCartadePorteGenerarRequest",
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
            Request.Set(context,request);
            Resultado.Set(context, resultado);
        }


        private Interviniente Interviniente(string cuit, string razonSocial)
        {
            Interviniente remitenteComercial;
            if (!String.IsNullOrEmpty(cuit))
            {
                remitenteComercial = new Interviniente
                {
                    cuit = cuit.Replace("-", ""),
                    razonSocial = razonSocial
                };
            }
            else
            {
                remitenteComercial = null;
            }
            return remitenteComercial;
        }

        private IntervinienteRequerido IntervinienteRequerido(string cuit, string razonSocial)
        {
            IntervinienteRequerido remitenteComercial;
            if (!String.IsNullOrEmpty(cuit))
            {
                remitenteComercial = new IntervinienteRequerido
                {
                    cuit = cuit.Replace("-", ""),
                    razonSocial = razonSocial
                };
            }
            else
            {
                remitenteComercial = null;
            }
            return remitenteComercial;
        }

        private DocumentoInterviniente DocumentoInterviniente(string cuit)
        {
            DocumentoInterviniente destinatario;
            if (!String.IsNullOrEmpty(cuit))
            {
                destinatario = new DocumentoInterviniente
                {
                    cuit = cuit.Replace("-", ""),
                };
            }
            else
            {
                destinatario = null;
            }
            return destinatario;
        }
    }
}
