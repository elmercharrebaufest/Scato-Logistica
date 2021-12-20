using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class AsignarAutomaticamentePuestoComandoTest
    {
        private AsignarAutomaticamentePuestoComando target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new AsignarAutomaticamentePuestoComando();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestSinAsignaciones()
        {
            srvRepositorio.Setup(s => s.BuscarAsignacionDeRecorridoPorInstanceId(It.IsAny<Guid>())).Returns((AsignacionDeRecorridoDto)null);

            host.InArguments.InstanceId = new Guid();

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "EstaAsignado").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void TestConAsignacion()
        {
            srvRepositorio.Setup(s => s.BuscarAsignacionDeRecorridoPorInstanceId(It.IsAny<Guid>()))
                          .Returns(new AsignacionDeRecorridoDto
                              {
                                  Id = 1,
                                  AlmacenDestinoId = 11,
                                  BalanzaBrutoId = 21,
                                  BalanzaTaraId = 22,
                                  CalleId = 31,
                                  HidraulicasId = new[] {41, 42}
                              });

            srvRepositorio.Setup(s => s.ObtenerMaterialPorCentroPorInstanceId(It.IsAny<Guid>()))
                          .Returns(new MaterialPorCentroDto {Id = 1, MaterialId = 51});

            srvComandos.Setup(s => s.Ejecutar(It.IsAny<ActualizarPuestocomando>()))
                       .Returns(new ResultadoPuestoComando());

            host.InArguments.InstanceId = new Guid();

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "EstaAsignado").Value;

            srvComandos.Verify(s => s.Ejecutar(It.IsAny<ActualizarPuestocomando>()), Times.Exactly(1));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(true));

        }
    }
}
