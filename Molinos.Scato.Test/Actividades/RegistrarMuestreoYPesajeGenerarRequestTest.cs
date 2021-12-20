using System;
using System.Collections.Generic;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class RegistrarMuestreoYPesajeGenerarRequestTest
    {
        private RegistrarMuestreoYPesajeGenerarRequest target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> srvComando;
        private CaladoDto calado;
        private AnalisisDeCalidadDto analisis;

        [SetUp]
        public void SetUp()
        {
            target = new RegistrarMuestreoYPesajeGenerarRequest();
            srvRepositorio = new Mock<IServicioRepositorio>();
            srvComando = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(srvComando.Object);

            
            analisis = new AnalisisDeCalidadDto
                    {
                        CaracteristicasAnalizadas =
                            new List<AnalisisPorCaracteristicaDto>
                                {
                                    new AnalisisPorCaracteristicaDto {CaracteristicaCodigoSap = "23213"}
                                }
                    };

            calado = new CaladoDto { AnalisisDeCalidad = new AnalisisDeCalidadDto(), CaladosPorCaracteristica = new List<CaladoPorCaracteristicaDto> { new CaladoPorCaracteristicaDto { CaracteristicaCodigoSap = "332", DescuentoEnPorcentaje = 0m } } };
            host.InArguments.InstanceId = new Guid();

            host.InArguments.FechaDeDescarga = new DateTime();
            host.InArguments.PesoNeto = 20000;
            host.InArguments.Calado = calado;
            host.InArguments.NroCartaPorte = "00050005";
        }

        [Test]
        public void TestGenerarRequestCamion()
        {
            host.InArguments.TipoVehiculo = TipoVehiculo.Camión;
            calado = new CaladoDto{CaladosPorCaracteristica = new List<CaladoPorCaracteristicaDto>{new CaladoPorCaracteristicaDto()}};
            var caladoConDescuento = calado.TieneDescuentos;
            var caladoPideAnalisis = calado.PideAnalisis;

            srvRepositorio.Setup(x => x.ObtenerCartaDePorteRegistradaServicioMonsanto(It.IsAny<Guid>(),It.IsAny<TipoVehiculo>())).Returns(new CartaDePorteRegistradaServicioMonsantoDto{Id = 2,LaboratorioCuit = "222",LaboratorioRazonSocial = "RFS",RecorridoId = 2,TipoAnalisis = "Liquid"});
            srvRepositorio.Setup(x => x.ObtenerAnalisisDeCalidadPorCaladoId(It.IsAny<int>())).Returns(analisis);
            srvRepositorio.Setup(x => x.ObtenerAnalisisPorVagones(It.IsAny<Guid>())).Returns(new List<AnalisisVagonDto> {new AnalisisVagonDto()});
            srvComando.Setup(x => x.Ejecutar(It.IsAny<CrearControlRecorrido>())).Returns(new Resultado());

            var resultado = host.TestActivity();

            Assert.That(resultado, Is.Not.Null);
            srvComando.Verify(x => x.Ejecutar(It.IsAny<CrearControlRecorrido>()),Times.Exactly(1));
            srvRepositorio.Verify(x => x.ObtenerCartaDePorteRegistradaServicioMonsanto(It.IsAny<Guid>(), It.IsAny<TipoVehiculo>()), Times.Exactly(1));
            srvRepositorio.Verify(x => x.ObtenerAnalisisDeCalidadPorCaladoId(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(x => x.ObtenerAnalisisPorVagones(It.IsAny<Guid>()), Times.Exactly(0));
            Assert.That(caladoPideAnalisis,Is.False);
            Assert.That(caladoConDescuento, Is.False);

        }

        [Test]
        public void TestGenerarRequestTren()
        {
            host.InArguments.TipoVehiculo = TipoVehiculo.Tren;

            srvRepositorio.Setup(x => x.ObtenerCartaDePorteRegistradaServicioMonsanto(It.IsAny<Guid>(), It.IsAny<TipoVehiculo>())).Returns(new CartaDePorteRegistradaServicioMonsantoDto { Id = 2, LaboratorioCuit = "222", LaboratorioRazonSocial = "RFS", RecorridoId = 2, TipoAnalisis = "Liquid" });
            srvRepositorio.Setup(x => x.ObtenerAnalisisDeCalidadPorCaladoId(It.IsAny<int>())).Returns(new AnalisisDeCalidadDto { CaracteristicasAnalizadas = new List<AnalisisPorCaracteristicaDto> { new AnalisisPorCaracteristicaDto { CaracteristicaCodigoSap = "23213" } } });
            srvRepositorio.Setup(x => x.ObtenerAnalisisPorVagones(It.IsAny<Guid>())).Returns(new List<AnalisisVagonDto> { new AnalisisVagonDto{Calado = calado,AnalisisDeCalidad = analisis,NumeroVagon = 1,PesoNeto = 100} });
            srvComando.Setup(x => x.Ejecutar(It.IsAny<CrearControlRecorrido>())).Returns(new Resultado());

            var resultado = host.TestActivity();

            Assert.That(resultado, Is.Not.Null);
            srvComando.Verify(x => x.Ejecutar(It.IsAny<CrearControlRecorrido>()), Times.Exactly(1));
            srvRepositorio.Verify(x => x.ObtenerCartaDePorteRegistradaServicioMonsanto(It.IsAny<Guid>(), It.IsAny<TipoVehiculo>()), Times.Exactly(1));
            srvRepositorio.Verify(x => x.ObtenerAnalisisDeCalidadPorCaladoId(It.IsAny<int>()), Times.Exactly(0));
            srvRepositorio.Verify(x => x.ObtenerAnalisisPorVagones(It.IsAny<Guid>()), Times.Exactly(1));

        }
    }
}
