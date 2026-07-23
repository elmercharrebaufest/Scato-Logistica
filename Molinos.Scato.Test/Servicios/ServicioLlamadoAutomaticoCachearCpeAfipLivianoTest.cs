using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Servicios.Interfaces;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Servicios
{
    // CachearCpeAFIPSanLorenzoLiviano/CachearCpeAFIPPorCentrosLiviano/CachearCpeAfipLiviano son la
    // versión liviana de CachearCpeAFIPSanLorenzo/CachearCpeAFIPPorCentros/CachearCpeAfip: mismo
    // filtro de centros/CTGs no cacheadas, paralelismo y reintentos, pero ejecutando CachearCPEAfip
    // en lugar de ConsultarCPDigital.
    [TestFixture]
    public class ServicioLlamadoAutomaticoCachearCpeAfipLivianoTest
    {
        private ServicioLlamadoAutomatico target;
        private Mock<IServicioRepositorio> repositorioMock;
        private Mock<IServicioComandos> comandosMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private Mock<IServicioHealthCheck> healthCheckMock;
        private Mock<IServicioNotificarUsuario> notificarUsuarioMock;

        private const int CentroSanLorenzo = 5; // Constantes.Centro.IdSanLorenzo
        private const int OtroCentro = 7;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IServicioRepositorio>();
            comandosMock = new Mock<IServicioComandos>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            healthCheckMock = new Mock<IServicioHealthCheck>();
            notificarUsuarioMock = new Mock<IServicioNotificarUsuario>();

            target = new ServicioLlamadoAutomatico(
                repositorioMock.Object, new NullLogger(), comandosMock.Object,
                orquestadorMock.Object, healthCheckMock.Object, notificarUsuarioMock.Object);

            repositorioMock.Setup(r => r.ObtenerConfiguracionGeneral(
                    Molinos.Scato.Dominio.Constantes.ConfiguracionGeneral.Pantalla.AFIP,
                    Molinos.Scato.Dominio.Constantes.ConfiguracionGeneral.AFIP.ConsultasParalelas,
                    null))
                .Returns(new ConfiguracionGeneralDto { Valor = "1" });
        }

        [Test]
        public void CachearCpeAFIPPorCentros_ConsultaLosCentrosConfigurados()
        {
            ConfigurarCentrosCachearCpeAfip(OtroCentro);

            comandosMock.Setup(c => c.Ejecutar(It.IsAny<ConsultarCPEPorDestino>()))
                .Returns(new ResultadoConsultaCpePorDestino { Cpes = new List<CartaPorteResumenDto>() });
            repositorioMock.Setup(r => r.ObtenerCpesNoCacheadas(It.IsAny<List<CartaPorteResumenDto>>()))
                .Returns(new List<long>());

            target.CachearCpeAFIPPorCentros();

            repositorioMock.Verify(r => r.ListarCentrosPorIds(It.IsAny<IList<int>>()), Times.Once());
            repositorioMock.Verify(r => r.ListarCentrosPorCuitMolinos(), Times.Never());
            comandosMock.Verify(c => c.Ejecutar(It.Is<ConsultarCPEPorDestino>(cmd => cmd.CentroId == OtroCentro)), Times.Once());
        }

        [Test]
        public void CachearCpeAFIPPorCentrosLiviano_ConsultaLosCentrosConfigurados()
        {
            ConfigurarCentrosCachearCpeAfip(OtroCentro);

            comandosMock.Setup(c => c.Ejecutar(It.IsAny<ConsultarCPEPorDestino>()))
                .Returns(new ResultadoConsultaCpePorDestino { Cpes = new List<CartaPorteResumenDto>() });
            repositorioMock.Setup(r => r.ObtenerCpesNoCacheadas(It.IsAny<List<CartaPorteResumenDto>>()))
                .Returns(new List<long>());

            target.CachearCpeAFIPPorCentrosLiviano();

            repositorioMock.Verify(r => r.ListarCentrosPorIds(It.IsAny<IList<int>>()), Times.Once());
            repositorioMock.Verify(r => r.ListarCentrosPorCuitMolinos(), Times.Never());
            comandosMock.Verify(c => c.Ejecutar(It.Is<ConsultarCPEPorDestino>(cmd => cmd.CentroId == OtroCentro)), Times.Once());
        }

        [Test]
        public void CuandoElCentroEsSanLorenzo_NoSeIncluyeEnElLoteLiviano()
        {
            ConfigurarCentrosCachearCpeAfip(CentroSanLorenzo);

            target.CachearCpeAFIPPorCentrosLiviano();

            comandosMock.Verify(c => c.Ejecutar(It.IsAny<ConsultarCPEPorDestino>()), Times.Exactly(0));
        }

        [Test]
        public void CuandoConsultarCPEPorDestinoTieneErrores_ActualizaFechaEstadoCacheadoConElMensajeYNoCacheaNada()
        {
            var resultadoConError = new ResultadoConsultaCpePorDestino();
            resultadoConError.Error("afip", "ARCA no disponible");
            comandosMock.Setup(c => c.Ejecutar(It.IsAny<ConsultarCPEPorDestino>())).Returns(resultadoConError);

            target.CachearCpeAFIPPorCentrosLiviano();

            repositorioMock.Verify(r => r.ActualizarFechaEstadoCacheadoCPECentro(OtroCentro, "ARCA no disponible"), Times.Exactly(1));
            comandosMock.Verify(c => c.Ejecutar(It.IsAny<CachearCPEAfip>()), Times.Exactly(0));
        }

        [Test]
        public void CuandoHayCtgsNoCacheadas_EjecutaCachearCPEAfipPorCadaUnaYLimpiaElError()
        {
            var ctgCamion = 100_000_001L;
            var ctgTren = 200_000_002L;
            var resultadoConsulta = new ResultadoConsultaCpePorDestino
            {
                Cpes = new List<CartaPorteResumenDto>
                {
                    new CartaPorteResumenDto { Ctg = ctgCamion, TipoCartaPorte = 74 },
                    new CartaPorteResumenDto { Ctg = ctgTren, TipoCartaPorte = 75 }
                }
            };
            comandosMock.Setup(c => c.Ejecutar(It.IsAny<ConsultarCPEPorDestino>())).Returns(resultadoConsulta);
            repositorioMock.Setup(r => r.ObtenerCpesNoCacheadas(It.IsAny<List<CartaPorteResumenDto>>()))
                .Returns(new List<long> { ctgCamion, ctgTren });
            comandosMock.Setup(c => c.Ejecutar(It.IsAny<CachearCPEAfip>())).Returns(new ResultadoCachearCPEAfip { Guardado = true });

            target.CachearCpeAFIPPorCentrosLiviano();

            repositorioMock.Verify(r => r.ActualizarFechaEstadoCacheadoCPECentro(OtroCentro, null), Times.Exactly(1));
            comandosMock.Verify(c => c.Ejecutar(It.Is<CachearCPEAfip>(cmd => cmd.NroCTG == ctgCamion && cmd.TipoVehiculo == (int)TipoVehiculo.Camión)), Times.Exactly(1));
            comandosMock.Verify(c => c.Ejecutar(It.Is<CachearCPEAfip>(cmd => cmd.NroCTG == ctgTren && cmd.TipoVehiculo == (int)TipoVehiculo.Tren)), Times.Exactly(1));
        }

        [Test]
        public void CuandoNoHayCtgsNoCacheadas_NoEjecutaCachearCPEAfip()
        {
            comandosMock.Setup(c => c.Ejecutar(It.IsAny<ConsultarCPEPorDestino>()))
                .Returns(new ResultadoConsultaCpePorDestino { Cpes = new List<CartaPorteResumenDto>() });
            repositorioMock.Setup(r => r.ObtenerCpesNoCacheadas(It.IsAny<List<CartaPorteResumenDto>>()))
                .Returns(new List<long>());

            target.CachearCpeAFIPPorCentrosLiviano();

            comandosMock.Verify(c => c.Ejecutar(It.IsAny<CachearCPEAfip>()), Times.Exactly(0));
        }

        [Test]
        public void CuandoCachearCPEAfipFallaLasPrimerasVeces_ReintentaHastaTresVecesEnTotal()
        {
            var ctg = 100_000_003L;
            comandosMock.Setup(c => c.Ejecutar(It.IsAny<ConsultarCPEPorDestino>()))
                .Returns(new ResultadoConsultaCpePorDestino { Cpes = new List<CartaPorteResumenDto> { new CartaPorteResumenDto { Ctg = ctg, TipoCartaPorte = 74 } } });
            repositorioMock.Setup(r => r.ObtenerCpesNoCacheadas(It.IsAny<List<CartaPorteResumenDto>>()))
                .Returns(new List<long> { ctg });

            var resultadoConError = new ResultadoCachearCPEAfip();
            resultadoConError.Error("afip", "ARCA no disponible");
            comandosMock.Setup(c => c.Ejecutar(It.Is<CachearCPEAfip>(cmd => cmd.NroCTG == ctg))).Returns(resultadoConError);

            target.CachearCpeAFIPPorCentrosLiviano();

            comandosMock.Verify(c => c.Ejecutar(It.Is<CachearCPEAfip>(cmd => cmd.NroCTG == ctg)), Times.Exactly(3),
                "Debe reintentar hasta 3 veces (mismo comportamiento que CachearCpeAfip) cuando el comando falla siempre");
        }

        [Test]
        public void CuandoCachearCPEAfipTieneExcepcionEnUnCtg_NoInterrumpeElProcesamientoDeLosDemas()
        {
            var ctgConExcepcion = 100_000_004L;
            var ctgOk = 100_000_005L;
            comandosMock.Setup(c => c.Ejecutar(It.IsAny<ConsultarCPEPorDestino>()))
                .Returns(new ResultadoConsultaCpePorDestino
                {
                    Cpes = new List<CartaPorteResumenDto>
                    {
                        new CartaPorteResumenDto { Ctg = ctgConExcepcion, TipoCartaPorte = 74 },
                        new CartaPorteResumenDto { Ctg = ctgOk, TipoCartaPorte = 74 }
                    }
                });
            repositorioMock.Setup(r => r.ObtenerCpesNoCacheadas(It.IsAny<List<CartaPorteResumenDto>>()))
                .Returns(new List<long> { ctgConExcepcion, ctgOk });

            comandosMock.Setup(c => c.Ejecutar(It.Is<CachearCPEAfip>(cmd => cmd.NroCTG == ctgConExcepcion))).Throws(new Exception("Falla de red"));
            comandosMock.Setup(c => c.Ejecutar(It.Is<CachearCPEAfip>(cmd => cmd.NroCTG == ctgOk))).Returns(new ResultadoCachearCPEAfip { Guardado = true });

            Assert.DoesNotThrow(() => target.CachearCpeAFIPPorCentrosLiviano());

            comandosMock.Verify(c => c.Ejecutar(It.Is<CachearCPEAfip>(cmd => cmd.NroCTG == ctgOk)), Times.Exactly(1));
        }

        [Test]
        public void CachearCpeAFIPSanLorenzoLiviano_ConsultaSoloElCentroSanLorenzo()
        {
            ConfigurarCentrosCachearCpeAfip(CentroSanLorenzo);

            comandosMock.Setup(c => c.Ejecutar(It.IsAny<ConsultarCPEPorDestino>()))
                .Returns(new ResultadoConsultaCpePorDestino { Cpes = new List<CartaPorteResumenDto>() });
            repositorioMock.Setup(r => r.ObtenerCpesNoCacheadas(It.IsAny<List<CartaPorteResumenDto>>()))
                .Returns(new List<long>());

            target.CachearCpeAFIPSanLorenzoLiviano();

            comandosMock.Verify(c => c.Ejecutar(It.Is<ConsultarCPEPorDestino>(cmd => cmd.CentroId == CentroSanLorenzo)), Times.Exactly(1));
            repositorioMock.Verify(r => r.ActualizarFechaEstadoCacheadoCPECentro(CentroSanLorenzo, null), Times.Exactly(1));
        }

        private void ConfigurarCentrosCachearCpeAfip(int centroId)
        {
            repositorioMock.Setup(r => r.ObtenerConfiguracionGeneral(
                    Molinos.Scato.Dominio.Constantes.ConfiguracionGeneral.Pantalla.AFIP,
                    Molinos.Scato.Dominio.Constantes.ConfiguracionGeneral.AFIP.CentrosCachearCPEAfip,
                    null))
                .Returns(new ConfiguracionGeneralDto { Valor = centroId.ToString() });

            repositorioMock.Setup(r => r.ListarCentrosPorIds(It.IsAny<IList<int>>()))
                .Returns(new List<CentroDto> { new CentroDto { Id = centroId } });
        }
    }
}
