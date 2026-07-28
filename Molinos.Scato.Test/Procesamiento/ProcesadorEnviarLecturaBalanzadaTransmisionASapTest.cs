using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorEnviarLecturaBalanzadaTransmisionASapTest
    {
        private ProcesadorEnviarLecturaBalanzadaTransmisionASap target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<ZSDWS_SCATO> servicioSapMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private NullLogger log;

        private Balanzada balanzadaBase;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            servicioSapMock = new Mock<ZSDWS_SCATO>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            log = new NullLogger();

            target = new ProcesadorEnviarLecturaBalanzadaTransmisionASap(
                repositorioMock.Object, conversorMock.Object, log,
                servicioSapMock.Object, orquestadorMock.Object);

            balanzadaBase = new Balanzada
            {
                Id = 1,
                NumeroBalanza = "B01",
                PesoNeto = 1000,
                CargaInicial = new Carga
                {
                    Id = 10,
                    Material = new MaterialPuerto
                    {
                        CodigoSAP = "MAT001",
                        Almacen = new Almacen { CodigoSAP = "ALM001" }
                    },
                    Exportador = new Exportador { Id = 1, Almacen = new Almacen { CodigoSAP = "ALM002" } }
                },
                CargaInicial_Id = 10,
                Fecha = DateTime.Today
            };

            repositorioMock
                .Setup(r => r.Obtener<Balanzada>(It.IsAny<Expression<Func<Balanzada, bool>>>()))
                .Returns(balanzadaBase);

            repositorioMock
                .Setup(r => r.Obtener<Exportador>(It.IsAny<Expression<Func<Exportador, bool>>>()))
                .Returns(balanzadaBase.CargaInicial.Exportador);

            repositorioMock
                .Setup(r => r.Obtener<BalanzaPuerto>(It.IsAny<Expression<Func<BalanzaPuerto, bool>>>()))
                .Returns(new BalanzaPuerto { CodigoBalanza = "B01", OffSetPlc = 0, CodigoDispositivo = "DEV01" });

            repositorioMock
                .Setup(r => r.Obtener<Carga>(It.IsAny<Expression<Func<Carga, bool>>>()))
                .Returns((Carga)null);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────────

        private void ConfigurarConfiguracionMov311(string valor)
        {
            repositorioMock
                .Setup(r => r.ObtenerPrimero<ConfiguracionGeneral>(It.IsAny<Expression<Func<ConfiguracionGeneral, bool>>>()))
                .Returns(new ConfiguracionGeneral
                {
                    Pantalla = Constantes.ConfiguracionGeneral.Pantalla.ServicioSap,
                    Nombre = Constantes.ConfiguracionGeneral.ServicioSap.HabilitarEnvioMov311,
                    Valor = valor
                });
        }

        private void ConfigurarSinConfiguracion()
        {
            repositorioMock
                .Setup(r => r.ObtenerPrimero<ConfiguracionGeneral>(It.IsAny<Expression<Func<ConfiguracionGeneral, bool>>>()))
                .Returns((ConfiguracionGeneral)null);
        }

        private void ConfigurarRespuestaSapExitosa()
        {
            servicioSapMock
                .Setup(s => s.Z_SDMF_RFC_MOV_311(It.IsAny<Z_SDMF_RFC_MOV_311Request>()))
                .Returns(new Z_SDMF_RFC_MOV_311Response1
                {
                    Z_SDMF_RFC_MOV_311Response = new Z_SDMF_RFC_MOV_311Response { IM_MESSAGE = "" }
                });
        }

        private void ConfigurarRespuestaSapConError(string mensaje)
        {
            servicioSapMock
                .Setup(s => s.Z_SDMF_RFC_MOV_311(It.IsAny<Z_SDMF_RFC_MOV_311Request>()))
                .Returns(new Z_SDMF_RFC_MOV_311Response1
                {
                    Z_SDMF_RFC_MOV_311Response = new Z_SDMF_RFC_MOV_311Response { IM_MESSAGE = mensaje }
                });
        }

        private EnviarLecturaBalanzadaTransmisionASap ComandoBase()
        {
            return new EnviarLecturaBalanzadaTransmisionASap { Id = 1, NumeroBalanza = "B01" };
        }

        // ─── Tests: comportamiento predeterminado (sin registro en BD) ─────────────

        [Test]
        public void SinConfiguracionEnBD_DebeLlamarASap()
        {
            ConfigurarSinConfiguracion();
            ConfigurarRespuestaSapExitosa();

            target.Ejecutar(ComandoBase());

            servicioSapMock.Verify(
                s => s.Z_SDMF_RFC_MOV_311(It.IsAny<Z_SDMF_RFC_MOV_311Request>()),
                Times.Once());
        }

        [Test]
        public void SinConfiguracionEnBD_RespuestaExitosa_MarcaBalanzadaComoEnviada()
        {
            ConfigurarSinConfiguracion();
            ConfigurarRespuestaSapExitosa();

            target.Ejecutar(ComandoBase());

            Assert.That(balanzadaBase.EnviadoASap, Is.True);
        }

        // ─── Tests: habilitado explícitamente (Valor = 'true') ────────────────────

        [Test]
        public void ConfiguracionHabilitada_DebeLlamarASap()
        {
            ConfigurarConfiguracionMov311("true");
            ConfigurarRespuestaSapExitosa();

            target.Ejecutar(ComandoBase());

            servicioSapMock.Verify(
                s => s.Z_SDMF_RFC_MOV_311(It.IsAny<Z_SDMF_RFC_MOV_311Request>()),
                Times.Once());
        }

        [Test]
        public void ConfiguracionHabilitada_RespuestaExitosa_MarcaBalanzadaComoEnviada()
        {
            ConfigurarConfiguracionMov311("true");
            ConfigurarRespuestaSapExitosa();

            target.Ejecutar(ComandoBase());

            Assert.That(balanzadaBase.EnviadoASap, Is.True);
        }

        [Test]
        public void ConfiguracionHabilitada_RespuestaConError_NoMarcaComoEnviada()
        {
            ConfigurarConfiguracionMov311("true");
            ConfigurarRespuestaSapConError("Error de SAP");

            var resultado = target.Ejecutar(ComandoBase());

            Assert.That(balanzadaBase.EnviadoASap, Is.False);
            Assert.That(resultado.HayErrores, Is.True);
        }

        // ─── Tests: deshabilitado explícitamente (Valor = 'false') ───────────────

        [Test]
        public void ConfiguracionDeshabilitada_NoDebeLlamarASap()
        {
            ConfigurarConfiguracionMov311("false");

            target.Ejecutar(ComandoBase());

            servicioSapMock.Verify(
                s => s.Z_SDMF_RFC_MOV_311(It.IsAny<Z_SDMF_RFC_MOV_311Request>()),
                Times.Never());
        }

        [Test]
        public void ConfiguracionDeshabilitada_NoDebeMarcaComoEnviada()
        {
            ConfigurarConfiguracionMov311("false");

            target.Ejecutar(ComandoBase());

            Assert.That(balanzadaBase.EnviadoASap, Is.False);
        }

        [Test]
        public void ConfiguracionDeshabilitada_NoDebeGenerarErrores()
        {
            ConfigurarConfiguracionMov311("false");

            var resultado = target.Ejecutar(ComandoBase());

            Assert.That(resultado.HayErrores, Is.False);
        }
    }
}
