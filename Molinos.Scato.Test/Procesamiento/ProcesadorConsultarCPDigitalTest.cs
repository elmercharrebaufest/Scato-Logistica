using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorConsultarCPDigitalTest
    {
        private ProcesadorConsultarCPDigital target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<IServicioComandos> servicioComandosMock;
        private Mock<CpePortType> serviceAfipMock;
        private Mock<IAccesoWsCtg> accesoWsCtgMock;
        private Mock<IKernel> kernelMock;
        private NullLogger log;

        private const int CentroId = 1;
        private const long CtgEstable = 100_000_001L;   // prefijo "100" (no "01") → OPERADOR
        private const long CtgInestable = 200_000_001L;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            servicioComandosMock = new Mock<IServicioComandos>();
            serviceAfipMock = new Mock<CpePortType>();
            accesoWsCtgMock = new Mock<IAccesoWsCtg>();
            kernelMock = new Mock<IKernel>();
            log = new NullLogger();

            target = new ProcesadorConsultarCPDigital(
                repositorioMock.Object,
                conversorMock.Object,
                log,
                servicioComandosMock.Object,
                serviceAfipMock.Object,
                accesoWsCtgMock.Object,
                kernelMock.Object);

            SetupRepositorioMinimal();
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // CacheConFallbackAfip — búsqueda por CTG directo con caché
        // ──────────────────────────────────────────────────────────────────────────────

        [Test]
        public void CuandoEstadoEnCacheEsAC_NoDebeReConsultarARCA()
        {
            var cpe = CrearCpeEntidad(CtgEstable, "AC");
            SetupObtenerMasReciente(cpe);

            target.Ejecutar(new ConsultarCPDigital { NroCtg = CtgEstable, CentroId = CentroId, IncluirImagen = false });

            accesoWsCtgMock.Verify(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>()), Times.Never(),
                "Estado AC es estable — no debe llamar a ARCA");
        }

        [Test]
        public void CuandoEstadoEnCacheEsCO_DebeReConsultarARCA()
        {
            // "CO" (contingencia) ya NO está en EstadosSinReConsulta → la contingencia puede
            // haberse resuelto y el dato en ARCA/AFIP quedar más actualizado que el caché.
            var cpe = CrearCpeEntidad(CtgEstable, "CO");
            SetupObtenerMasReciente(cpe);

            accesoWsCtgMock.Setup(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>()))
                .Throws(new Exception("ARCA no disponible"));

            target.Ejecutar(new ConsultarCPDigital { NroCtg = CtgEstable, CentroId = CentroId, IncluirImagen = false });

            accesoWsCtgMock.Verify(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>()), Times.Once(),
                "Estado CO (contingencia) es inestable — debe re-consultar ARCA/AFIP");
        }

        [Test]
        public void CuandoEstadoEnCacheEsCF_NoDebeReConsultarARCA()
        {
            var cpe = CrearCpeEntidad(CtgEstable, "CF");
            SetupObtenerMasReciente(cpe);

            target.Ejecutar(new ConsultarCPDigital { NroCtg = CtgEstable, CentroId = CentroId, IncluirImagen = false });

            accesoWsCtgMock.Verify(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>()), Times.Never(),
                "Estado CF (confirmado arribo) es estable — no debe llamar a ARCA");
        }

        [Test]
        public void CuandoEstadoEnCacheEsAN_NoDebeReConsultarARCA()
        {
            // "AN" (anulado) está en EstadosSinReConsulta → no debe re-consultar
            var cpe = CrearCpeEntidad(CtgEstable, "AN");
            SetupObtenerMasReciente(cpe);

            target.Ejecutar(new ConsultarCPDigital { NroCtg = CtgEstable, CentroId = CentroId, IncluirImagen = false });

            accesoWsCtgMock.Verify(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>()), Times.Never(),
                "Estado AN (anulado) es estable — no debe llamar a ARCA");
        }

        [Test]
        public void CuandoEstadoEsInestableYARCACae_DebeRetornarCacheComoFallback()
        {
            // Ante falla de ARCA durante re-query, el camión no debe quedar sin respuesta
            var cpe = CrearCpeEntidad(CtgInestable, "RE");
            SetupObtenerMasReciente(cpe);

            accesoWsCtgMock.Setup(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>()))
                .Throws(new Exception("Timeout ARCA"));

            var resultado = target.Ejecutar(new ConsultarCPDigital { NroCtg = CtgInestable, CentroId = CentroId, IncluirImagen = false })
                as ResultadoCartaPorteElectronica;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.Cpe, Is.Not.Null, "El fallback debe retornar la CPE del caché");
            Assert.That(resultado.Cpe.EstadoCpe, Is.EqualTo("RE"), "El estado del caché debe preservarse en el fallback");
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // BuscarCPEPorPatenteEnCache — búsqueda por patente
        // ──────────────────────────────────────────────────────────────────────────────

        [Test]
        public void CuandoBuscaPorPatenteYEstadoEsCF_NoDebeReConsultarARCA()
        {
            var patente = "AB123CD";
            var cpe = CrearCpeEntidad(CtgEstable, "CF");
            cpe.Dominio = patente;
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns(new List<CartaPorteElectronica> { cpe });

            target.Ejecutar(new ConsultarCPDigital { Patente = patente, CentroId = CentroId, IncluirImagen = false });

            accesoWsCtgMock.Verify(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>()), Times.Never(),
                "Estado CF en caché por patente es estable — no debe re-consultar ARCA");
        }

        [Test]
        public void CuandoBuscaPorPatenteYEstadoEsAN_NoDebeReConsultarARCA()
        {
            var patente = "AB123CD";
            var cpe = CrearCpeEntidad(CtgEstable, "AN");
            cpe.Dominio = patente;
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns(new List<CartaPorteElectronica> { cpe });

            target.Ejecutar(new ConsultarCPDigital { Patente = patente, CentroId = CentroId, IncluirImagen = false });

            accesoWsCtgMock.Verify(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>()), Times.Never(),
                "Estado AN (anulado) en caché por patente es estable — no debe re-consultar ARCA");
        }

        [Test]
        public void CuandoBuscaPorPatenteYEstadoEsRE_DebeIntentarReConsultarARCA()
        {
            var patente = "ZZ999ZZ";
            var cpe = CrearCpeEntidad(CtgInestable, "RE");
            cpe.Dominio = patente;
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns(new List<CartaPorteElectronica> { cpe });

            accesoWsCtgMock.Setup(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>()))
                .Throws(new Exception("ARCA no disponible"));

            target.Ejecutar(new ConsultarCPDigital { Patente = patente, CentroId = CentroId, IncluirImagen = false });

            accesoWsCtgMock.Verify(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>()), Times.Once(),
                "Estado RE (rechazado) en caché por patente es inestable — debe re-consultar ARCA");
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // Flujo ferroviario (Tren) — bug: NRE en consulta por operativo + RuntimeBinderException
        // en la conversión de transporte (TransporteFerroviariaRespuesta no tiene dominio/
        // cuitChofer/fechaHoraPartida/codigoTurno/tarifaReferencia).
        // ──────────────────────────────────────────────────────────────────────────────

        [Test]
        public void CuandoConsultaFerroviariaPorCtgDirecto_NoDebeRetornarErrorGenerico()
        {
            const long ctgTren = 100_000_002L;
            SetupTransportistaEncontrado();
            accesoWsCtgMock.Setup(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>())).Returns(new Auth());
            serviceAfipMock.Setup(s => s.consultarCPEFerroviaria(It.IsAny<consultarCPEFerroviariaRequest>()))
                .Returns(new consultarCPEFerroviariaResponse(CrearDetalleFerroviaria(ctgTren, "AC")));

            var resultado = target.Ejecutar(new ConsultarCPDigital
            {
                NroCtg = ctgTren,
                CentroId = CentroId,
                TipoVehiculo = (int)TipoVehiculo.Tren,
                ConsultaFerroviarioPorCtg = true,
                ForzarConsultaAfip = true,
                IncluirImagen = false
            }) as ResultadoCartaPorteElectronica;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False,
                "La consulta ferroviaria por CTG directo no debe fallar con el error genérico");
        }

        [Test]
        public void CuandoConsultaFerroviariaPorCtgDirecto_DebePoblarLaCpeDelResultado()
        {
            // Antes del fix, ConvertirResponseAFIPenCartaPorteElectronica accedía a propiedades
            // inexistentes en TransporteFerroviariaRespuesta (dominio/cuitChofer/fechaHoraPartida/
            // codigoTurno/tarifaReferencia), lo que producía RuntimeBinderException silenciada
            // y dejaba resultado.Cpe en null.
            const long ctgTren = 100_000_003L;
            SetupTransportistaEncontrado();
            accesoWsCtgMock.Setup(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>())).Returns(new Auth());
            serviceAfipMock.Setup(s => s.consultarCPEFerroviaria(It.IsAny<consultarCPEFerroviariaRequest>()))
                .Returns(new consultarCPEFerroviariaResponse(CrearDetalleFerroviaria(ctgTren, "AC")));

            var resultado = target.Ejecutar(new ConsultarCPDigital
            {
                NroCtg = ctgTren,
                CentroId = CentroId,
                TipoVehiculo = (int)TipoVehiculo.Tren,
                ConsultaFerroviarioPorCtg = true,
                ForzarConsultaAfip = true,
                IncluirImagen = false
            }) as ResultadoCartaPorteElectronica;

            Assert.That(resultado.Cpe, Is.Not.Null, "La conversión de la respuesta ferroviaria debe completarse y poblar la CPE");
            Assert.That(resultado.Cpe.EstadoCpe, Is.EqualTo("AC"));
        }

        [Test]
        public void CuandoConsultaFerroviariaPorCtgDirecto_DebeResolverPatenteDesdeNroVagon()
        {
            // TransporteFerroviariaRespuesta no tiene "dominio" (a diferencia de automotor).
            // El nro de vagón AFIP hace las veces de dominio/patente para el vehículo ferroviario
            // — paridad con el comportamiento legacy (pre-refactor sep-2025), que sí lo resolvía así.
            const long ctgTren = 100_000_003L;
            SetupTransportistaEncontrado();
            SetupConversorMapeandoVehiculos();
            accesoWsCtgMock.Setup(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>())).Returns(new Auth());
            serviceAfipMock.Setup(s => s.consultarCPEFerroviaria(It.IsAny<consultarCPEFerroviariaRequest>()))
                .Returns(new consultarCPEFerroviariaResponse(CrearDetalleFerroviaria(ctgTren, "AC")));

            var resultado = target.Ejecutar(new ConsultarCPDigital
            {
                NroCtg = ctgTren,
                CentroId = CentroId,
                TipoVehiculo = (int)TipoVehiculo.Tren,
                ConsultaFerroviarioPorCtg = true,
                ForzarConsultaAfip = true,
                IncluirImagen = false
            }) as ResultadoCartaPorteElectronica;

            Assert.That(resultado.Cpe?.Vehiculos?.FirstOrDefault()?.Patente, Is.EqualTo("3"),
                "La patente del vagón debe derivarse del nroVagon devuelto por AFIP");
        }

        [Test]
        public void CuandoConsultaFerroviariaPorOperativoConVarioVagones_NoDebeLanzarNullReferenceException()
        {
            // Reproduce el bug reportado: BuscarCPEPorCTGEnAFIPFerroviariaPorOperativo asignaba
            // resultado.Cpe.Vehiculos sin que resultado.Cpe estuviera poblado.
            const long nroOperativo = 500_000_001L;
            const long ctgVagon1 = 100_000_010L;
            const long ctgVagon2 = 100_000_011L;

            SetupTransportistaEncontrado();
            accesoWsCtgMock.Setup(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>())).Returns(new Auth());

            serviceAfipMock.Setup(s => s.consultaCPEFerroviariaPorNroOperativo(It.IsAny<consultaCPEFerroviariaPorNroOperativoRequest>()))
                .Returns(new consultaCPEFerroviariaPorNroOperativoResponse(CrearResumenOperativo(ctgVagon1, ctgVagon2)));

            serviceAfipMock.Setup(s => s.consultarCPEFerroviaria(It.Is<consultarCPEFerroviariaRequest>(r => r.solicitud.nroCTG == ctgVagon1)))
                .Returns(new consultarCPEFerroviariaResponse(CrearDetalleFerroviaria(ctgVagon1, "AC")));
            serviceAfipMock.Setup(s => s.consultarCPEFerroviaria(It.Is<consultarCPEFerroviariaRequest>(r => r.solicitud.nroCTG == ctgVagon2)))
                .Returns(new consultarCPEFerroviariaResponse(CrearDetalleFerroviaria(ctgVagon2, "AC")));

            Assert.DoesNotThrow(() => target.Ejecutar(new ConsultarCPDigital
            {
                NroCtg = nroOperativo,
                CentroId = CentroId,
                TipoVehiculo = (int)TipoVehiculo.Tren,
                ConsultaFerroviarioPorCtg = false,
                ForzarConsultaAfip = true,
                IncluirImagen = false
            }));
        }

        [Test]
        public void CuandoConsultaFerroviariaPorOperativoConVarioVagones_DebePoblarVehiculosDeLaCpe()
        {
            const long nroOperativo = 500_000_002L;
            const long ctgVagon1 = 100_000_020L;
            const long ctgVagon2 = 100_000_021L;

            SetupTransportistaEncontrado();
            SetupConversorMapeandoVehiculos();
            accesoWsCtgMock.Setup(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>())).Returns(new Auth());

            serviceAfipMock.Setup(s => s.consultaCPEFerroviariaPorNroOperativo(It.IsAny<consultaCPEFerroviariaPorNroOperativoRequest>()))
                .Returns(new consultaCPEFerroviariaPorNroOperativoResponse(CrearResumenOperativo(ctgVagon1, ctgVagon2)));

            serviceAfipMock.Setup(s => s.consultarCPEFerroviaria(It.Is<consultarCPEFerroviariaRequest>(r => r.solicitud.nroCTG == ctgVagon1)))
                .Returns(new consultarCPEFerroviariaResponse(CrearDetalleFerroviaria(ctgVagon1, "AC")));
            serviceAfipMock.Setup(s => s.consultarCPEFerroviaria(It.Is<consultarCPEFerroviariaRequest>(r => r.solicitud.nroCTG == ctgVagon2)))
                .Returns(new consultarCPEFerroviariaResponse(CrearDetalleFerroviaria(ctgVagon2, "AC")));

            var resultado = target.Ejecutar(new ConsultarCPDigital
            {
                NroCtg = nroOperativo,
                CentroId = CentroId,
                TipoVehiculo = (int)TipoVehiculo.Tren,
                ConsultaFerroviarioPorCtg = false,
                ForzarConsultaAfip = true,
                IncluirImagen = false
            }) as ResultadoCartaPorteElectronica;

            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.Cpe, Is.Not.Null, "El primer vagón exitoso debe poblar la CPE base del resultado");
            Assert.That(resultado.Cpe.Vehiculos, Is.Not.Null.And.Count.EqualTo(2),
                "Deben registrarse los vehículos (vagones) de ambas CTGs del operativo");
            Assert.That(resultado.Cpe.Vehiculos.Select(v => v.Patente), Is.EqualTo(new[] { "20", "21" }),
                "La patente de cada vagón debe derivarse del nroVagon de AFIP (paridad con el comportamiento legacy)");
        }

        // Nota: el refresco del Pdf cacheado (regresión PR 5099: `if (pdfNuevo != null)
        // cartaPorte.Pdf = pdfNuevo`) ya no vive en ProcesadorConsultarCPDigital — se extrajo a
        // CartaPorteElectronicaCacheHelper.Actualizar (compartido con ProcesadorCachearCPEAfip).
        // La cobertura de ese comportamiento está en CartaPorteElectronicaCacheHelperTest
        // (CuandoExisteCPECacheadaYNoCambioNingunValor_* / Cuando...CambioElEstado_*).

        // ──────────────────────────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────────────────────────

        private CartaPorteElectronica CrearCpeEntidad(long nroCTG, string estado) => new CartaPorteElectronica
        {
            Id = 1,
            NroCTG = nroCTG,
            Estado = estado,
            Dominio = "AA000AA",
            Localidad = 1,
            Provincia = 1,
            FechaEmision = DateTime.Now.AddDays(-1),
            FechaCP = DateTime.Now.AddDays(-1),
            FechaVto = DateTime.Now.AddDays(30),
            TipoCartaPorte = (int)TipoVehiculo.Camión,
            CuitTransportista = 0,
            CuitRepresentanteEntregador = 0,
            CuitRepresentanteRecibidor = 0
        };

        private void SetupObtenerMasReciente(CartaPorteElectronica cpe)
        {
            repositorioMock.Setup(r => r.ObtenerMasReciente<CartaPorteElectronica>(
                    It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>(),
                    It.IsAny<Expression<Func<CartaPorteElectronica, DateTime>>>()))
                .Returns(cpe);
        }

        /// <summary>
        /// Mocks mínimos para que ConvertirCartaPorteDto no lance NullReferenceException.
        /// No valida lógica de negocio — solo evita que el camino estable crashee.
        /// </summary>
        private void SetupRepositorioMinimal()
        {
            var centro = new Centro { Id = CentroId, Cuit = "30-12345678-9", Descripcion = "Centro Test" };

            repositorioMock.Setup(r => r.Obtener<Centro>(CentroId)).Returns(centro);
            repositorioMock.Setup(r => r.Obtener<Categoria>(It.IsAny<Expression<Func<Categoria, bool>>>()))
                .Returns((Categoria)null);
            repositorioMock.Setup(r => r.Obtener<ConfiguracionGeneral>(It.IsAny<Expression<Func<ConfiguracionGeneral, bool>>>()))
                .Returns(new ConfiguracionGeneral { Valor = "7" });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(new List<Proveedor>());
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Entregador, bool>>>()))
                .Returns(new List<Entregador>());
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Transportista, bool>>>()))
                .Returns(new List<Transportista>());
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Localidad, bool>>>()))
                .Returns(new List<Localidad> { new Localidad { CodigoAfip = "1", Descripcion = "Loc Test" } });
            repositorioMock.Setup(r => r.Obtener<Material>(It.IsAny<Expression<Func<Material, bool>>>()))
                .Returns((Material)null);
            repositorioMock.Setup(r => r.Obtener<Chofer>(It.IsAny<Expression<Func<Chofer, bool>>>()))
                .Returns((Chofer)null);
            conversorMock.Setup(c => c.Convertir<Dominio.Entidades.CartaPorte, CartaPorteDto>(
                    It.IsAny<Dominio.Entidades.CartaPorte>()))
                .Returns(new CartaPorteDto());
        }

        /// <summary>
        /// Hace que la búsqueda de Transportista siempre encuentre un resultado, evitando el
        /// error de negocio "Transportista no encontrado" (no relacionado al bug bajo prueba).
        /// </summary>
        private void SetupTransportistaEncontrado()
        {
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Transportista, bool>>>()))
                .Returns(new List<Transportista> { new Transportista { Cuit = "30-11111111-1" } });
        }

        /// <summary>
        /// El mock por defecto del Conversor ignora la entidad y siempre retorna una CartaPorteDto
        /// vacía. En el flujo de vagones ferroviarios (ProcesarVagonesFerroviarios), esto dejaría
        /// Cpe.Vehiculos en null y el vagón nunca se agregaría a la lista. Este helper simula el
        /// mapeo real de entidad.Vehiculos -> CartaPorteDto.Vehiculos.
        /// </summary>
        private void SetupConversorMapeandoVehiculos()
        {
            conversorMock.Setup(c => c.Convertir<Dominio.Entidades.CartaPorte, CartaPorteDto>(
                    It.IsAny<Dominio.Entidades.CartaPorte>()))
                .Returns((Dominio.Entidades.CartaPorte entidad) => new CartaPorteDto
                {
                    Vehiculos = entidad.Vehiculos?.Select(v => new VehiculoDto { Patente = v.Patente }).ToList()
                });
        }

        /// <summary>
        /// Construye una respuesta AFIP realista de detalle ferroviario (misma forma usada por
        /// consultarCPEFerroviaria), con TransporteFerroviariaRespuesta — el tipo que carece de
        /// dominio/cuitChofer/fechaHoraPartida/codigoTurno/tarifaReferencia.
        /// </summary>
        private DetalleFerroviariaRespuesta CrearDetalleFerroviaria(long nroCTG, string estado) => new DetalleFerroviariaRespuesta
        {
            cabecera = new CabeceraRespuesta
            {
                tipoCartaPorte = (int)TipoVehiculo.Tren,
                sucursal = 1,
                nroOrden = 1,
                nroCTG = nroCTG,
                fechaEmision = DateTime.Now.AddDays(-1),
                estado = estado,
                fechaInicioEstado = DateTime.Now.AddDays(-1),
                fechaVencimiento = DateTime.Now.AddDays(30)
            },
            origen = new OrigenFerroviariaRespuesta
            {
                cuit = 0,
                codProvincia = 1,
                codLocalidad = 1,
                domicilio = "Ruta Test km 1",
                planta = 1
            },
            correspondeRetiroProductor = false,
            retiroProductor = new RetiroProductorRespuesta { cuitRemitenteComercialProductor = 0 },
            intervinientes = new IntervinientesFerroviariaRespuesta(),
            datosCarga = new DatosCargaFerroviariaRespuesta { codGrano = 1, cosecha = 2024, pesoBruto = 50000, pesoTara = 20000 },
            destino = new DestinoRespuesta { cuit = 0, codProvincia = 1, codLocalidad = 1, planta = 1 },
            destinatario = new DestinatarioRespuesta { cuit = 0 },
            transporte = new TransporteFerroviariaRespuesta
            {
                // cuitTransportista queda resuelto porque el repositorio de Transportista está
                // mockeado para devolver siempre un resultado no vacío (ver SetupTransportistaMinimal).
                cuitTransportista = 30111111111,
                nroVagon = nroCTG % 1000,
                nroVagonSpecified = true,
                fechaHoraPartidaTren = DateTime.Now,
                fechaHoraPartidaTrenSpecified = true,
                kmRecorrer = 100,
                cuitPagadorFlete = 0,
                cuitConductor = 20222222222,
                tarifa = 1000,
                cuitIntermediarioFlete = 0,
                mercaderiaFumigada = false
            },
            pdf = null,
            errores = null
        };

        private CartaPorteFerroviariaResumenRespuesta CrearResumenOperativo(params long[] ctgsVagones) => new CartaPorteFerroviariaResumenRespuesta
        {
            cartaPorte = ctgsVagones.Select(ctg => new ConsultaOperativoCPEFerroviariaRespuesta { nroCTG = ctg }).ToArray(),
            pdf = null,
            errores = null
        };
    }
}
