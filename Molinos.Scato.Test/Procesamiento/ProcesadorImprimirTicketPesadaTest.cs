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
    public class ProcesadorImprimirTicketPesadaTest
    {
        private ProcesadorImprimirTicketPesada target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<ILogger> loggerMock;
        private Mock<IFirmaProvider> firmaProviderMock;
        private Mock<IServicioImpresorFactory> servicioImpresorFactoryMock;
        private Mock<IServicioImpresion> servicioImpresorMock;
        private ImprimirTicketPesada comando;
        private ImpTicketPesadaDto dto;
        private FirmaDto firma;
        private ImpTicketPesada entidad;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            loggerMock = new Mock<ILogger>();
            firmaProviderMock = new Mock<IFirmaProvider>();
            servicioImpresorFactoryMock = new Mock<IServicioImpresorFactory>();
            servicioImpresorMock = new Mock<IServicioImpresion>();

            servicioImpresorFactoryMock.Setup(f => f.CrearServicio()).Returns(servicioImpresorMock.Object);

            dto = new ImpTicketPesadaDto
            {
                Codigo = "TEST-001",
                Impresora = "Test Printer",
                MaterialId = 1,
                WorkflowId = Guid.NewGuid(),
                Material = "Material Test",
                CentroId = 1
            };

            comando = new ImprimirTicketPesada
            {
                Dto = dto,
                CantidadCopias = 1
            };

            firma = new FirmaDto
            {
                Descripcion = "Test Firma",
                RazonSocial = "Test Razon Social"
            };

            entidad = new ImpTicketPesada
            {
                Id = 1,
                Codigo = "TEST-001"
            };

            conversorMock.Setup(c => c.Convertir<ImpTicketPesadaDto, ImpTicketPesada>(dto))
                .Returns(entidad);

            firmaProviderMock.Setup(f => f.ObtenerFirmaSinLogo())
                .Returns(firma);


            target = new ProcesadorImprimirTicketPesada(
                repositorioMock.Object,
                conversorMock.Object,
                loggerMock.Object,
                firmaProviderMock.Object,
                servicioImpresorFactoryMock.Object);
        }

        [Test]
        public void EjecutarAsync_CuandoLaValidacionEsExitosa_DebeImprimir()
        {
            var method = typeof(ProcesadorImprimirTicketPesada).GetMethod("EjecutarAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(target, new object[] { comando, servicioImpresorMock.Object });

            firmaProviderMock.Verify(f => f.ObtenerFirmaSinLogo(), Times.Once());
            servicioImpresorMock.Verify(s => s.Ejecutar(It.Is<ImprimirTicketPesada>(c => c.Firma == firma)), Times.Once());
        }

        [Test]
        public void EjecutarSync_CuandoSeEjecutaCorrectamente_DebeGuardarEntidad()
        {
            repositorioMock.Setup(r => r.Agregar(entidad)).Returns(entidad);
            var method = typeof(ProcesadorImprimirTicketPesada).GetMethod("EjecutarSync", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(target, new object[] { comando });

            conversorMock.Verify(c => c.Convertir<ImpTicketPesadaDto, ImpTicketPesada>(dto), Times.Once());
            repositorioMock.Verify(r => r.Agregar(It.Is<ImpTicketPesada>(
                e => e.TipoImpresion == Molinos.Scato.Dominio.Enums.TipoImpresion.TicketPesada)), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
            Assert.AreEqual(entidad.Id, result);
        }

        [Test]
        public void EjecutarSync_CuandoOcurreError_DebeRegistrarYLanzarExcepcion()
        {
            var exception = new Exception("Test Exception");
            repositorioMock.Setup(r => r.Agregar(It.IsAny<ImpTicketPesada>()))
                .Throws(exception);
            var method = typeof(ProcesadorImprimirTicketPesada).GetMethod("EjecutarSync", BindingFlags.NonPublic | BindingFlags.Instance);

            try
            {
                method.Invoke(target, new object[] { comando });
                Assert.Fail("Se esperaba que ocurriera una excepción");
            }
            catch (TargetInvocationException ex)
            {
                // Verificar que la excepción interna es la esperada
                Assert.IsInstanceOf<Exception>(ex.InnerException);
                Assert.AreEqual("Test Exception", ex.InnerException.Message);

                // Verificar que se registró el error
                loggerMock.Verify(l => l.Error(exception, It.IsAny<string>()), Times.Once());
            }
        }

        [Test]
        public void EjecutarAsync_CuandoOcurreError_DebeRegistrarYLanzarExcepcion()
        {
            var exception = new Exception("Test Exception");
            servicioImpresorMock.Setup(r => r.Ejecutar(It.IsAny<Comando>())).Throws(exception);
            var method = typeof(ProcesadorImprimirTicketPesada).GetMethod("EjecutarAsync", BindingFlags.NonPublic | BindingFlags.Instance);

            try
            {
                method.Invoke(target, new object[] { comando, servicioImpresorMock.Object });
                Assert.Fail("Se esperaba que ocurriera una excepción");
            }
            catch (TargetInvocationException ex)
            {
                // Verificar que la excepción interna es la esperada
                Assert.IsInstanceOf<Exception>(ex.InnerException);
                Assert.AreEqual("Test Exception", ex.InnerException.Message);

                // Verificar que se registró el error
                loggerMock.Verify(l => l.Error(exception, It.IsAny<string>()), Times.Once());
            }
        }

        #region DebeImprimir

        private Mock<ProcesadorImprimirTicketPesada> SetupDebeImprimirMaterialPorWorkflow(MaterialPorWorkflow materialPorWorkflow)
        {
            // Mock de la clase bajo prueba, pero llamamos a los métodos reales excepto los protegidos que interceptemos
            var targetMock = new Mock<ProcesadorImprimirTicketPesada>(
                this.repositorioMock.Object,
                this.conversorMock.Object,
                this.loggerMock.Object,
                this.firmaProviderMock.Object,
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

        //private void SetupDebeImprimirRepositorio(MaterialPorWorkflow materialPorWorkflow, Recorrido recorrido)
        //{
        //    // Configurar el comportamiento del mock para el método Obtener
        //    this.repositorioMock
        //        .Setup(r => r.Obtener(
        //            It.IsAny<List<Expression<Func<Recorrido, object>>>>(),
        //            It.IsAny<Expression<Func<Recorrido, bool>>>()))
        //        .Returns(recorrido);

        //    this.repositorioMock
        //        .Setup(r => r.Obtener<MaterialPorWorkflow>(
        //            It.IsAny<Expression<Func<MaterialPorWorkflow, bool>>>()))
        //        .Returns(materialPorWorkflow);
        //}

        #region DebeImprimir_CuandoConfiguracionEnMaterialPorWorkflowEsNull_DebeRetornarTrue

        [Test]
        public void DebeImprimir_CuandoConfiguracionEnMaterialPorWorkflowEsNull_DebeRetornarTrue()
        {
            // Arrange
            MaterialPorWorkflow materialPorWorkflow = null;
            var targetMock = this.SetupDebeImprimirMaterialPorWorkflow(materialPorWorkflow);           

            // Act: invocamos el protected override DebeImprimir vía reflection
            var method = typeof(ProcesadorImprimirTicketPesada).GetMethod("DebeImprimir", BindingFlags.NonPublic | BindingFlags.Instance);
            bool resultado = (bool)method.Invoke(targetMock.Object, new object[] { comando });

            // Assert
            Assert.IsTrue(resultado);
        }

        //[Test]
        //public void DebeImprimir_CuandoMaterialPorWorkflowEsNull_DebeRetornarTrue()
        //{
        //    // Arrange
        //    var recorrido = new Recorrido { InstanciaWorkflow = Guid.NewGuid(), Workflow = new Workflow() { Id = 1 } };
        //    MaterialPorWorkflow materialPorWorkflow = null;
        //    this.SetupDebeImprimirRepositorio(materialPorWorkflow, recorrido);

        //    // Act
        //    var method = typeof(ProcesadorImprimirTicketPesada).GetMethod("DebeImprimir", BindingFlags.NonPublic | BindingFlags.Instance);
        //    var result = (bool)method.Invoke(target, new object[] { comando });

        //    // Assert
        //    Assert.IsTrue(result);
        //}

        #endregion

        #region DebeImprimir_CuandoConfiguracionEnMaterialPorWorkflowEsTrue_DebeRetornarTrue

        [Test]
        public void DebeImprimir_CuandoConfiguracionEnMaterialPorWorkflowEsTrue_DebeRetornarTrue()
        {
            // Arrange
            MaterialPorWorkflow materialPorWorkflow = new MaterialPorWorkflow { ImprimirTicketPesada = true };
            var targetMock = this.SetupDebeImprimirMaterialPorWorkflow(materialPorWorkflow);

            // Act: invocamos el protected override DebeImprimir vía reflection
            var method = typeof(ProcesadorImprimirTicketPesada).GetMethod("DebeImprimir", BindingFlags.NonPublic | BindingFlags.Instance);
            bool resultado = (bool)method.Invoke(targetMock.Object, new object[] { comando });

            // Assert
            Assert.IsTrue(resultado);
        }

        //[Test]
        //public void DebeImprimir_CuandoMaterialPorWorkflowImprimirTicketPesadaEsTrue_DebeRetornarTrue()
        //{
        //    // Configurar el comportamiento del mock para el método Obtener
        //    var recorrido = new Recorrido { InstanciaWorkflow = Guid.NewGuid(), Workflow = new Workflow() { Id = 1 } };
        //    var materialPorWorkflow = new MaterialPorWorkflow() { ImprimirTicketPesada = true };
        //    this.SetupDebeImprimirRepositorio(materialPorWorkflow, recorrido);

        //    var method = typeof(ProcesadorImprimirTicketPesada).GetMethod("DebeImprimir", BindingFlags.NonPublic | BindingFlags.Instance);
        //    var result = (bool)method.Invoke(target, new object[] { comando });

        //    Assert.IsTrue(result);
        //}

        #endregion

        #region DebeImprimir_CuandoConfiguracionEnMaterialPorWorkflowEsFalse_DebeRetornarFalse

        [Test]
        public void DebeImprimir_CuandoConfiguracionEnMaterialPorWorkflowEsFalse_DebeRetornarFalse()
        {
            // Arrange
            MaterialPorWorkflow materialPorWorkflow = new MaterialPorWorkflow { ImprimirTicketPesada = false };
            var targetMock = this.SetupDebeImprimirMaterialPorWorkflow(materialPorWorkflow);

            // Act: invocamos el protected override DebeImprimir vía reflection
            var method = typeof(ProcesadorImprimirTicketPesada).GetMethod("DebeImprimir", BindingFlags.NonPublic | BindingFlags.Instance);
            bool resultado = (bool)method.Invoke(targetMock.Object, new object[] { comando });

            // Assert
            Assert.IsFalse(resultado);
        }

        //[Test]
        //public void DebeImprimir_CuandoMaterialPorWorkflowImprimirTicketPesadaEsFalse_DebeRetornarFalse()
        //{
        //    // Arrange
        //    // Configurar el comportamiento del mock para el método Obtener
        //    var recorrido = new Recorrido { InstanciaWorkflow = Guid.NewGuid(), Workflow = new Workflow() { Id = 1 } };
        //    var materialPorWorkflow = new MaterialPorWorkflow { ImprimirTicketPesada = false };
        //    this.SetupDebeImprimirRepositorio(materialPorWorkflow, recorrido);

        //    // Act
        //    var method = typeof(ProcesadorImprimirTicketPesada).GetMethod("DebeImprimir", BindingFlags.NonPublic | BindingFlags.Instance);
        //    var result = (bool)method.Invoke(target, new object[] { comando });

        //    // Assert
        //    Assert.IsFalse(result);
        //}

        #endregion

        #endregion
    }
}