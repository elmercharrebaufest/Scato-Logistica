using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCTGWebService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorAltaCTG : ProcesadorComando<DarDeAltaCTG>
    {
        private readonly CTGServicePortType serviceAfipCTG;
        private readonly IAccesoWsCtg accesoWsCtg;
        public ProcesadorAltaCTG(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CTGServicePortType serviceAfipCTG, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.serviceAfipCTG = serviceAfipCTG;
            this.accesoWsCtg = accesoWsCtg;
        }

        public override Resultado Ejecutar(DarDeAltaCTG comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////
            var resultado = new Resultado();

            try
            {
                var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                if (centro == null) { throw new Exception(String.Format(Textos.Error_Requerido, Textos.Centro)); }
                if (centro.Localidad == null) { throw new Exception(String.Format(Textos.Error_Requerido, Textos.Error_LocalidadCentro)); }

                if (string.IsNullOrEmpty(comando.Dto.DestinoCuit)) { throw new Exception(String.Format(Textos.Error_Requerido, Textos.Destino)); }
                if (string.IsNullOrEmpty(comando.Dto.DestinoLocalidadCodigoSap)) { throw new Exception(String.Format(Textos.Error_Requerido, Textos.Error_LocalidadDestino)); }

                var destinoLocalidadCodigoAfip = comando.Dto.DestinoLocalidadCodigoSap;
                var destinoCuit = comando.Dto.DestinoCuit;

                var destinoBoca = Repositorio.Obtener<BocaDestino>(comando.Dto.BocaDestinoId);
                if (destinoBoca != null)
                {
                    destinoLocalidadCodigoAfip = destinoBoca.Localidad.CodigoAfip;
                    destinoCuit = destinoBoca.Proveedor.Cuil;
                }
                var transportista = Repositorio.Obtener<Transportista>(comando.Dto.TransportistaId ?? 0);
                if (transportista == null) { throw new Exception(String.Format(Textos.Error_Requerido, Textos.Transportista)); }
                var destinatario = Repositorio.Obtener<Proveedor>(comando.Dto.DestinatarioId);
                if (destinatario == null) { throw new Exception(String.Format(Textos.Error_Requerido, Textos.CartaPorte_Destinatario)); }
                var material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
                if (material == null) { throw new Exception(String.Format(Textos.Error_Requerido, Textos.Material)); }
                Log.Debug("ProcesadorAltaCTG - Creo la autorizacion");
                // Obtengo la autorizacion
                var auth = accesoWsCtg.ObtenerAuthType(centro.Cuit.Replace("-", string.Empty), resultado);
                Log.Debug("ProcesadorAltaCTG - armo consulta");
                var request = new solicitarCTGInicialRequest(
                    new solicitarCTGInicialRequestType
                    {
                        auth = auth,
                        datosSolicitarCTGInicial = new datosSolicitarCTGInicialType
                        {
                            cartaPorte = (long)Convert.ToDouble(comando.Dto.NroCartaPorte),
                            codigoEspecie = material.CodigoEspecie.HasValue ? material.CodigoEspecie.Value : 0,
                            codigoCosecha = comando.Dto.Cosecha.Replace("-", String.Empty),
                            cuitDestino = (long)Convert.ToDouble(destinoCuit.Replace("-", String.Empty)),
                            cuitDestinatario = (long)Convert.ToDouble(destinatario.Cuil.Replace("-", String.Empty)),
                            cuitTransportista = (long)Convert.ToDouble(transportista.Cuit.Replace("-", String.Empty)),
                            codigoLocalidadDestino = Convert.ToInt32(destinoLocalidadCodigoAfip),
                            codigoLocalidadOrigen = Convert.ToInt32(centro.Localidad.CodigoAfip),
                            pesoNeto = comando.Vehiculo.PesoNetoOrigen.HasValue
                                            ? comando.Vehiculo.PesoNetoOrigen.Value
                                            : 0,
                            patente = comando.Vehiculo.Patente,
                            cantHoras = 1,
                            kmARecorrer = Convert.ToUInt32(comando.Dto.KmRecorrer),
                            cuitCanjeadorSpecified = false,
                            cantHorasSpecified = true,
                            cuitTransportistaSpecified = true,
                            turno = comando.Dto.Cupo
                        }
                    });
                Log.Debug("ProcesadorAltaCTG - Inicio la consulta");
                var response = serviceAfipCTG.solicitarCTGInicial(request);
                Log.Debug("ProcesadorAltaCTG - Realizo la consulta ");

                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsCtg"] == "1")
                    {

                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = "ProcesadorAltaCTG",
                            Fecha = DateTime.Now,
                            Comentario = request.ToXml(),
                            NombreUsuario = "",
                            WorkflowInstanceId = comando.WorkflowId,
                        });
                        Repositorio.GuardarCambios();
                    }
                }
                catch (Exception e)
                {
                    Log.Debug("Error al loguear request Afip CTG", e.Message);
                }

                if (response.response.arrayErrores.Any())
                {
                    resultado.Errores.Add("CodigoDeAlta", response.response.arrayErrores.FirstOrDefault());
                }
                else if (response.response.datosSolicitarCTGResponse.arrayControles != null && response.response.datosSolicitarCTGResponse.arrayControles.Any() && response.response.datosSolicitarCTGResponse.datosSolicitarCTG == null)
                {
                    var firstOrDefault = response.response.datosSolicitarCTGResponse.arrayControles.FirstOrDefault();
                    if (firstOrDefault != null)
                    {
                        resultado.Errores.Add("CodigoDeAlta", firstOrDefault.descripcion);
                        Log.Error("ProcesadorAltaCTG -" + firstOrDefault.descripcion);
                    }
                }
                else
                {
                    // Si no hay errores, registro la Alta del CTG
                    var datos = response.response?.datosSolicitarCTGResponse;
                    var cartaPorte = Repositorio.Obtener<CartaPorte>(comando.Dto.Id);

                    if (datos?.datosSolicitarCTG == null)
                    {
                        resultado.Errores.Add("CodigoDeAlta",
                            "Error, el servicio de AFIP devolvió una respuesta vacía.");
                        return resultado;
                    }

                    string ctg = ObtenerCtgDesdeAlta(datos);

                    if (string.IsNullOrWhiteSpace(ctg))
                    {
                        ctg = ObtenerCtgDesdeConsultaPorPatente(
                            serviceAfipCTG,
                            auth,
                            comando.Vehiculo.Patente,
                            comando.Dto.NroCartaPorte);

                        if (string.IsNullOrWhiteSpace(ctg))
                        {
                            resultado.Errores.Add("CodigoDeAlta",
                                "Error, el servicio de AFIP no devolvió un CTG válido.");
                            return resultado;
                        }
                    }

                    if (!DateTime.TryParse(datos.datosSolicitarCTG.fechaEmision, out var fechaEmision))
                    {
                        resultado.Errores.Add("CodigoDeAlta",
                            "Error al parsear la fecha de emisión del CTG.");
                        return resultado;
                    }

                    Repositorio.Agregar(new AltaCTG
                    {
                        CartaPorte = cartaPorte,
                        CodigoCTG = ctg,
                        Fecha = fechaEmision,
                        WorkflowId = comando.WorkflowId
                    });

                    cartaPorte.CTG = ctg;
                    cartaPorte.TarifaReferencia = datos.datosSolicitarCTG.tarifaReferencia;

                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("CodigoDeAlta", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }
            return resultado;
        }

        private string ObtenerCtgDesdeAlta(datosSolicitarCTGResponseType datos)
        {
            var ctg = datos.datosSolicitarCTG.ctg;
            return ctg > 0
                ? ctg.ToString(CultureInfo.InvariantCulture)
                : null;
        }

        private string ObtenerCtgDesdeConsultaPorPatente(
            CTGServicePortType service,
            authType auth,
            string patente,
            string nroCartaPorte)
        {
            var request = new consultarCTGActivosPorPatenteRequest
            {
                request = new consultarCTGActivosPorPatenteRequestType
                {
                    auth = auth,
                    patente = patente
                }
            };

            var response = service.consultarCTGActivosPorPatente(request);

            var ctgResponse = response?.response?
                .arrayConsultarCTGActivosPorPatenteResponse?
                .FirstOrDefault(r => r.cartaPorte == nroCartaPorte);

            return ctgResponse?.ctg;
        }

    }
}