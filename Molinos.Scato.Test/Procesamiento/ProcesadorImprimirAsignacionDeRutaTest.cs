using System;
using System.Reflection;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Servicios.ServicioImpresion;
using Moq;
using Moq.Protected;
using Ninject.Extensions.Logging;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorImprimirAsignacionDeRutaTest
    {
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<ILogger> loggerMock;
        private Mock<IServicioImpresorFactory> servicioImpresorFactoryMock;
        private Mock<IServicioImpresion> servicioImpresorMock;
        private ImprimirAsignacionDeRuta comando;
        private ImpAsignacionDeRutaDto dto;

        [SetUp]
        public void SetUp()
        {
            this.repositorioMock = new Mock<IRepositorio>();
            this.conversorMock = new Mock<IConversor>();
            this.loggerMock = new Mock<ILogger>();
            this.servicioImpresorFactoryMock = new Mock<IServicioImpresorFactory>();
            this.servicioImpresorMock = new Mock<IServicioImpresion>();

            this.servicioImpresorFactoryMock.Setup(f => f.CrearServicio()).Returns(servicioImpresorMock.Object);

            dto = new ImpAsignacionDeRutaDto
            {
                Codigo = "TEST-001",
                Impresora = "Test Printer",
                MaterialId = 1,
                WorkflowId = Guid.NewGuid(),
                Material = "Material Test",
                CentroId = 1
            };

            comando = new ImprimirAsignacionDeRuta
            {
                Dto = dto,
                CantidadCopias = 1
            };
        }

        #region DebeImprimir

        private Mock<ProcesadorImprimirAsignacionDeRuta> SetupDebeImprimirMaterialPorWorkflow(MaterialPorWorkflow materialPorWorkflow)
        {
            // Mock de la clase bajo prueba, pero llamamos a los métodos reales excepto los protegidos que interceptemos
            var targetMock = new Mock<ProcesadorImprimirAsignacionDeRuta>(
                this.repositorioMock.Object,
                this.conversorMock.Object,
                this.loggerMock.Object,
                this.servicioImpresorFactoryMock.Object)
            {
                CallBase = true
            };

            // Aquí le decimos: “cuando alguien llame al protected ObtenerMaterialPorWorkflow(int, Guid, int),
            // devuélveme el materialPorWorkflow que le pase como parámetro”
            targetMock
              .Protected()
              .Setup<MaterialPorWorkflow>("ObtenerMaterialPorWorkflow", dto)
              .Returns(materialPorWorkflow);

            return targetMock;
        }

        [Test]
        public void DebeImprimir_CuandoConfiguracionEnMaterialPorWorkflowEsNull_DebeRetornarTrue()
        {
            // Arrange
            MaterialPorWorkflow materialPorWorkflow = null;
            var targetMock = this.SetupDebeImprimirMaterialPorWorkflow(materialPorWorkflow);

            // Act: invocamos el protected override DebeImprimir vía reflection
            var method = typeof(ProcesadorImprimirAsignacionDeRuta).GetMethod("DebeImprimir", BindingFlags.NonPublic | BindingFlags.Instance);
            bool resultado = (bool)method.Invoke(targetMock.Object, new object[] { comando });

            // Assert
            Assert.IsTrue(resultado);
        }

        [Test]
        public void DebeImprimir_CuandoConfiguracionEnMaterialPorWorkflowEsTrue_DebeRetornarTrue()
        {
            // Arrange
            MaterialPorWorkflow materialPorWorkflow = new MaterialPorWorkflow { ImprimirAsignacionRuta = true };
            var targetMock = this.SetupDebeImprimirMaterialPorWorkflow(materialPorWorkflow);

            // Act: invocamos el protected override DebeImprimir vía reflection
            var method = typeof(ProcesadorImprimirAsignacionDeRuta).GetMethod("DebeImprimir", BindingFlags.NonPublic | BindingFlags.Instance);
            bool resultado = (bool)method.Invoke(targetMock.Object, new object[] { comando });

            // Assert
            Assert.IsTrue(resultado);
        }

        [Test]
        public void DebeImprimir_CuandoConfiguracionEnMaterialPorWorkflowEsFalse_DebeRetornarFalse()
        {
            // Arrange
            MaterialPorWorkflow materialPorWorkflow = new MaterialPorWorkflow { ImprimirAsignacionRuta = false };
            var targetMock = this.SetupDebeImprimirMaterialPorWorkflow(materialPorWorkflow);

            // Act: invocamos el protected override DebeImprimir vía reflection
            var method = typeof(ProcesadorImprimirAsignacionDeRuta).GetMethod("DebeImprimir", BindingFlags.NonPublic | BindingFlags.Instance);
            bool resultado = (bool)method.Invoke(targetMock.Object, new object[] { comando });

            // Assert
            Assert.IsFalse(resultado);
        }

        #endregion
    }
}
