using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento.AFIP
{
    // Regresión: esta lógica vivía en ProcesadorConsultarCPDigital.ProcesarErroresAfip /
    // ConvertirResponseAFIPenCartaPorteElectronica antes de extraerse a CpeAfipRespuestaMapper
    // para compartirla con ProcesadorCachearCPEAfip. Los casos acá replican exactamente el
    // comportamiento previo al refactor.
    [TestFixture]
    public class CpeAfipRespuestaMapperTest
    {
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            log = new NullLogger();
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // TieneErrorBloqueante
        // ──────────────────────────────────────────────────────────────────────────────

        [Test]
        public void CuandoRespuestaEsNull_RetornaTrueYAgregaErrorNoExistenSolicitudes()
        {
            var resultado = new ResultadoCartaPorteElectronica();

            var tieneErrorBloqueante = CpeAfipRespuestaMapper.TieneErrorBloqueante(null, resultado, log);

            Assert.That(tieneErrorBloqueante, Is.True);
            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Errores[Constantes.AFIPCodigoDeError.NoExistenSolicitudes], Is.EqualTo("No se obtuvo respuesta desde AFIP"));
        }

        [Test]
        public void CuandoErroresEsNull_RetornaFalse()
        {
            var respuesta = new DetalleAutomotorRespuesta { errores = null };
            var resultado = new ResultadoCartaPorteElectronica();

            var tieneErrorBloqueante = CpeAfipRespuestaMapper.TieneErrorBloqueante(respuesta, resultado, log);

            Assert.That(tieneErrorBloqueante, Is.False);
            Assert.That(resultado.HayErrores, Is.False);
        }

        [Test]
        public void CuandoErroresEsVacio_RetornaFalse()
        {
            var respuesta = new DetalleAutomotorRespuesta { errores = new CodigoDescripcion[0] };
            var resultado = new ResultadoCartaPorteElectronica();

            var tieneErrorBloqueante = CpeAfipRespuestaMapper.TieneErrorBloqueante(respuesta, resultado, log);

            Assert.That(tieneErrorBloqueante, Is.False);
            Assert.That(resultado.HayErrores, Is.False);
        }

        [Test]
        public void CuandoSoloHayErrorNoBloqueante_RetornaFalseYNoAgregaError()
        {
            var respuesta = new DetalleAutomotorRespuesta
            {
                errores = new[]
                {
                    new CodigoDescripcion { codigo = Constantes.AFIPCodigoDeError.ErrorPDFNoGenerado, descripcion = "No se pudo generar el PDF" }
                }
            };
            var resultado = new ResultadoCartaPorteElectronica();

            var tieneErrorBloqueante = CpeAfipRespuestaMapper.TieneErrorBloqueante(respuesta, resultado, log);

            Assert.That(tieneErrorBloqueante, Is.False);
            Assert.That(resultado.HayErrores, Is.False);
        }

        [Test]
        public void CuandoHayErrorBloqueanteConCodigoNoExistenSolicitudes_AgregaMensajeNoSeEncuentranDatos()
        {
            var respuesta = new DetalleAutomotorRespuesta
            {
                errores = new[]
                {
                    new CodigoDescripcion { codigo = Constantes.AFIPCodigoDeError.NoExistenSolicitudes, descripcion = "detalle original de AFIP" }
                }
            };
            var resultado = new ResultadoCartaPorteElectronica();

            var tieneErrorBloqueante = CpeAfipRespuestaMapper.TieneErrorBloqueante(respuesta, resultado, log);

            Assert.That(tieneErrorBloqueante, Is.True);
            Assert.That(resultado.Errores[Constantes.AFIPCodigoDeError.NoExistenSolicitudes], Is.EqualTo("No se encuentran datos"));
        }

        [Test]
        public void CuandoHayErrorBloqueanteDistintoDeNoExisteSolicitudes_AgregaDescripcionDelPrimerError()
        {
            var respuesta = new DetalleAutomotorRespuesta
            {
                errores = new[]
                {
                    new CodigoDescripcion { codigo = "999", descripcion = "Error inesperado de AFIP" }
                }
            };
            var resultado = new ResultadoCartaPorteElectronica();

            var tieneErrorBloqueante = CpeAfipRespuestaMapper.TieneErrorBloqueante(respuesta, resultado, log);

            Assert.That(tieneErrorBloqueante, Is.True);
            Assert.That(resultado.Errores[Constantes.AFIPCodigoDeError.NoExistenSolicitudes], Is.EqualTo("Error inesperado de AFIP"));
        }

        [Test]
        public void CuandoHayErrorBloqueanteMezcladoConNoBloqueante_SigueSiendoBloqueante()
        {
            var respuesta = new DetalleAutomotorRespuesta
            {
                errores = new[]
                {
                    new CodigoDescripcion { codigo = Constantes.AFIPCodigoDeError.ErrorPDFNoGenerado, descripcion = "No se pudo generar el PDF" },
                    new CodigoDescripcion { codigo = "999", descripcion = "Error inesperado de AFIP" }
                }
            };
            var resultado = new ResultadoCartaPorteElectronica();

            var tieneErrorBloqueante = CpeAfipRespuestaMapper.TieneErrorBloqueante(respuesta, resultado, log);

            Assert.That(tieneErrorBloqueante, Is.True);
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // ConvertirResponseAFIPenCartaPorteElectronica
        // ──────────────────────────────────────────────────────────────────────────────

        [Test]
        public void CuandoResponseEsNull_RetornaNull()
        {
            var carta = CpeAfipRespuestaMapper.ConvertirResponseAFIPenCartaPorteElectronica(null, log);

            Assert.That(carta, Is.Null);
        }

        [Test]
        public void CuandoLaRespuestaNoTieneLaEstructuraEsperada_CapturaLaExcepcionYRetornaNull()
        {
            // Un objeto sin las propiedades dinámicas esperadas (cabecera/origen/etc.) dispara
            // RuntimeBinderException al acceder vía dynamic; debe capturarse y devolver null,
            // nunca propagar la excepción (comportamiento preservado del código legacy).
            var respuestaMalformada = new object();

            var carta = CpeAfipRespuestaMapper.ConvertirResponseAFIPenCartaPorteElectronica(respuestaMalformada, log);

            Assert.That(carta, Is.Null);
        }

        [Test]
        public void CuandoRespuestaAutomotorValida_MapeaCabeceraOrigenYTransporte()
        {
            var respuesta = new DetalleAutomotorRespuesta
            {
                cabecera = new CabeceraRespuesta { tipoCartaPorte = 74, sucursal = 1, nroOrden = 111, nroCTG = 222, estado = "AC" },
                origen = new OrigenAutomotorRespuesta { cuit = 30000000001, codProvincia = 1, codLocalidad = 2, nroRenspa = "12.345.6.78901/A0" },
                retiroProductor = new RetiroProductorRespuesta { cuitRemitenteComercialProductor = 0 },
                intervinientes = new IntervinientesAutomotorRespuesta(),
                datosCarga = new DatosCargaAutomotorRespuesta { codGrano = 23, pesoBruto = 45000, pesoTara = 15000, cosecha = 2024 },
                destino = new DestinoRespuesta { cuit = 30000000002, codProvincia = 1, codLocalidad = 3, planta = 1 },
                destinatario = new DestinatarioRespuesta { cuit = 30000000003 },
                transporte = new TransporteAutomotorRespuesta
                {
                    cuitTransportista = 30000000004,
                    dominio = new[] { "AB123CD" },
                    cuitChofer = 20000000005,
                    codigoTurno = "T1",
                    tarifa = 1000,
                    tarifaReferencia = 900
                },
                errores = null
            };

            var carta = CpeAfipRespuestaMapper.ConvertirResponseAFIPenCartaPorteElectronica(respuesta, log);

            Assert.That(carta, Is.Not.Null);
            Assert.That(carta.NroCTG, Is.EqualTo(222));
            Assert.That(carta.Estado, Is.EqualTo("AC"));
            Assert.That(carta.NroRenspa, Is.EqualTo("12.345.6.78901/A0"), "El renspa del origen automotor debe mapearse");
            Assert.That(carta.Dominio, Is.EqualTo("AB123CD"), "El dominio automotor viene directo de transporte.dominio");
            Assert.That(carta.Material, Is.EqualTo(23));
            Assert.That(carta.PesoBruto, Is.EqualTo(45000));
        }

        [Test]
        public void CuandoRespuestaFerroviaria_ResuelvePatenteDesdeNroVagonYNoUsaRenspa()
        {
            // TransporteFerroviariaRespuesta / OrigenFerroviariaRespuesta no tienen dominio/nroRenspa;
            // el mapeo debe resolver el "dominio" a partir de nroVagon y dejar NroRenspa en null,
            // sin lanzar RuntimeBinderException (ver comentarios del código de producción).
            var respuesta = new DetalleFerroviariaRespuesta
            {
                cabecera = new CabeceraRespuesta { tipoCartaPorte = 75, nroCTG = 333, estado = "AC" },
                origen = new OrigenFerroviariaRespuesta { cuit = 30000000001, codProvincia = 1, codLocalidad = 2 },
                retiroProductor = new RetiroProductorRespuesta { cuitRemitenteComercialProductor = 0 },
                intervinientes = new IntervinientesFerroviariaRespuesta(),
                datosCarga = new DatosCargaFerroviariaRespuesta { codGrano = 23, pesoBruto = 45000, pesoTara = 15000, cosecha = 2024 },
                destino = new DestinoRespuesta { cuit = 30000000002, codProvincia = 1, codLocalidad = 3, planta = 1 },
                destinatario = new DestinatarioRespuesta { cuit = 30000000003 },
                transporte = new TransporteFerroviariaRespuesta
                {
                    nroVagon = 987654,
                    nroVagonSpecified = true,
                    cuitConductor = 20000000005
                },
                errores = null
            };

            var carta = CpeAfipRespuestaMapper.ConvertirResponseAFIPenCartaPorteElectronica(respuesta, log);

            Assert.That(carta, Is.Not.Null);
            Assert.That(carta.NroRenspa, Is.Null, "El origen ferroviario no tiene renspa");
            Assert.That(carta.Dominio, Is.EqualTo("987654"), "El nro de vagón hace de dominio/patente en ferroviario");
            Assert.That(carta.CuitChofer, Is.EqualTo(20000000005));
        }

        [Test]
        public void CuandoRespuestaFerroviariaSinNroVagonEspecificado_DominioQuedaVacio()
        {
            var respuesta = new DetalleFerroviariaRespuesta
            {
                cabecera = new CabeceraRespuesta { tipoCartaPorte = 75, nroCTG = 334, estado = "AC" },
                origen = new OrigenFerroviariaRespuesta { cuit = 30000000001 },
                retiroProductor = new RetiroProductorRespuesta { cuitRemitenteComercialProductor = 0 },
                intervinientes = new IntervinientesFerroviariaRespuesta(),
                datosCarga = new DatosCargaFerroviariaRespuesta(),
                destino = new DestinoRespuesta(),
                destinatario = new DestinatarioRespuesta(),
                transporte = new TransporteFerroviariaRespuesta { nroVagonSpecified = false },
                errores = null
            };

            var carta = CpeAfipRespuestaMapper.ConvertirResponseAFIPenCartaPorteElectronica(respuesta, log);

            Assert.That(carta.Dominio, Is.EqualTo(string.Empty));
        }
    }
}
