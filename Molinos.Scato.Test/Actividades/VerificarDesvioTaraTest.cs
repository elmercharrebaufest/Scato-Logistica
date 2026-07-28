using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;
using System;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarDesvioTaraTest
    {
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> servicioRepositorioMock;
        private VerificarDesvioTara target;

        [SetUp]
        public void SetUp()
        {
            target = new VerificarDesvioTara();
            servicioRepositorioMock = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servicioRepositorioMock.Object);

            servicioRepositorioMock.Setup(s => s.ObtenerConfiguracionGeneral(
                    Constantes.ConfiguracionGeneral.Pantalla.DiferenciaPesoTaraWFE,
                    Constantes.ConfiguracionGeneral.DiferenciaPesoTaraWFE.DiferenciaTolerancia,
                    It.IsAny<int?>()))
                .Returns(new ConfiguracionGeneralDto { Valor = "1000" });

            servicioRepositorioMock.Setup(s => s.ObtenerConfiguracionGeneral(
                    Constantes.ConfiguracionGeneral.Pantalla.DiferenciaPesoTaraWFE,
                    Constantes.ConfiguracionGeneral.DiferenciaPesoTaraWFE.ListaDistribucion,
                    It.IsAny<int?>()))
                .Returns(new ConfiguracionGeneralDto { Valor = "a@molinos.com.ar;b@molinos.com.ar" });

            servicioRepositorioMock.Setup(s => s.ObtenerPromedioTaraPorVehiculo(It.IsAny<int>()))
                .Returns(new PromedioTaraVehiculoDto
                {
                    PatenteCamion = "AAA111",
                    PatenteAcoplado = "ACO123",
                    PromedioTara = 9000,
                    ChoferApellido = "PEREZ",
                    ChoferNombre = "JUAN",
                    ChoferNumeroDocumento = "30111222"
                });
        }

        [Test]
        public void CuandoDesvioSuperaUmbral_EntoncesCorrespondeYGeneraBodyYDestino()
        {
            servicioRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>()))
                .Returns(new RecorridoDto
                {
                    Id = 1,
                    Patente = "AAA111",
                    NumeroDocumentoIngreso = "DOC-1",
                    PesoTara = 12000,
                    Vehiculo = new VehiculoDto { PatenteAcoplado = "ACO123" }
                });

            host.TestActivity();

            Assert.That(host.OutArguments.Corresponde, Is.True);
            Assert.That(host.OutArguments.Destino, Is.EqualTo("a@molinos.com.ar;b@molinos.com.ar"));
            Assert.That(host.OutArguments.Asunto, Is.EqualTo("Desvio tara - AAA111"));
            //Assert.That(host.OutArguments.Body, Does.Contain("Patente: <b>AAA111</b>"));
            //Assert.That(host.OutArguments.Body, Does.Contain("Acoplado: <b>ACO123</b>"));
            //Assert.That(host.OutArguments.Body, Does.Contain("CPE: <b>0001000123</b>"));
            //Assert.That(host.OutArguments.Body, Does.Contain("Chofer: <b>PEREZ, JUAN</b>"));
            //Assert.That(host.OutArguments.Body, Does.Contain("Documento chofer: <b>30111222</b>"));
            Assert.That(host.OutArguments.DesvioKg, Is.EqualTo(3000m));
        }

        [Test]
        public void CuandoDesvioNoSuperaUmbral_EntoncesNoCorresponde()
        {
            servicioRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>()))
                .Returns(new RecorridoDto
                {
                    Id = 2,
                    Patente = "BBB222",
                    NumeroDocumentoIngreso = "DOC-2",
                    PesoTara = 9800
                });
            servicioRepositorioMock.Setup(s => s.ObtenerPromedioTaraPorVehiculo(2))
                .Returns(new PromedioTaraVehiculoDto
                {
                    PatenteCamion = "BBB222",
                    PatenteAcoplado = "ACO222",
                    PromedioTara = 9000,
                    ChoferApellido = "LOPEZ",
                    ChoferNombre = "ANA",
                    ChoferNumeroDocumento = "28999111"
                });

            host.TestActivity();

            Assert.That(host.OutArguments.Corresponde, Is.False);
            Assert.That(host.OutArguments.DesvioKg, Is.EqualTo(800m));
        }

        [Test]
        public void CuandoPromedioEsCero_EntoncesNoCorresponde()
        {
            servicioRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>()))
                .Returns(new RecorridoDto
                {
                    Id = 3,
                    Patente = "CCC333",
                    NumeroDocumentoIngreso = "DOC-3",
                    PesoTara = 10000
                });
            servicioRepositorioMock.Setup(s => s.ObtenerPromedioTaraPorVehiculo(3))
                .Returns(new PromedioTaraVehiculoDto
                {
                    PatenteCamion = "CCC333",
                    PatenteAcoplado = "ACO333",
                    PromedioTara = 0,
                    ChoferApellido = "GOMEZ",
                    ChoferNombre = "LUIS",
                    ChoferNumeroDocumento = "25555444"
                });

            host.TestActivity();

            Assert.That(host.OutArguments.Corresponde, Is.False);
            Assert.That(host.OutArguments.DesvioKg, Is.EqualTo(10000m));
        }
    }
}
