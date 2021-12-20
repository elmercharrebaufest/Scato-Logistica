using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorSincronizarProveedoresTest
    {
        private ProcesadorSincronizarProveedores target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<ZSDWS_SCATO> servicioSapMock;
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            servicioSapMock = new Mock<ZSDWS_SCATO>();
            log = new NullLogger();
            target = new ProcesadorSincronizarProveedores(repositorioMock.Object, conversorMock.Object, servicioSapMock.Object, log);

            repositorioMock.Setup(
                s =>
                s.ObtenerMasReciente(It.IsAny<Expression<Func<LogSincronizacion, bool>>>(),
                                     It.IsAny<Expression<Func<LogSincronizacion, DateTime>>>()))
                           .Returns(new LogSincronizacion {FechaEjecucion = new DateTime(2014, 1, 1)});

        }

        [Test]
        public void TestSincronizarCompleta()
        {
            servicioSapMock.Setup(s => s.DatosProveedores(It.IsAny<DatosProveedoresRequest>()))
                .Returns<DatosProveedoresRequest>(req => new DatosProveedoresResponse1(new DatosProveedoresResponse
                {
                    Proveedores = new[] {
                        new ZSDES3133 {NAME1 = "Proveedor 1", LIFNR = "001", STCD1 = "20123456789", ACTIVO = "X"},
                        new ZSDES3133 {NAME1 = "Proveedor 2", LIFNR = "002", STCD1 = "27234567890", ACTIVO = "X"}
                        }
                }));
            LogSincronizacion logS = null;
            repositorioMock.Setup(s => s.Agregar(It.IsAny<LogSincronizacion>()))
                           .Returns<LogSincronizacion>(x => logS = x);

            var resultado = target.Ejecutar(new SincronizarProveedores { RetornarResultado = false, Cuit = null}) as ResultadoSincronizarProveedores;

            repositorioMock.Verify(v => v.Agregar(It.IsAny<Proveedor>()), Times.Exactly(2));

            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.Cantidad, Is.EqualTo(2));
            Assert.That(resultado.Items.Count, Is.EqualTo(0));
            Assert.That(logS.NombreInterface, Is.EqualTo("Proveedores"));
            Assert.That(logS.Completa, Is.True);
            Assert.That(logS.Correcta, Is.True);
        }

        [Test]
        public void TestSincronizarParcial()
        {
            servicioSapMock.Setup(s => s.DatosProveedores(It.IsAny<DatosProveedoresRequest>()))
                .Returns<DatosProveedoresRequest>(req => new DatosProveedoresResponse1(new DatosProveedoresResponse
                {
                    Proveedores = new[] {
                        new ZSDES3133 {NAME1 = "Proveedor 1", LIFNR = "001", STCD1 = "20123456789", ACTIVO = "X"},
                        new ZSDES3133 {NAME1 = "Proveedor 2", LIFNR = "002", STCD1 = "27234567890", ACTIVO = "X"}
                        }
                }));
            LogSincronizacion logS = null;
            repositorioMock.Setup(s => s.Agregar(It.IsAny<LogSincronizacion>()))
                           .Returns<LogSincronizacion>(x => logS = x);

            var resultado = target.Ejecutar(new SincronizarProveedores { RetornarResultado = true, Cuit = "2345" }) as ResultadoSincronizarProveedores;

            repositorioMock.Verify(v => v.Agregar(It.IsAny<Proveedor>()), Times.Exactly(2));
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.Cantidad, Is.EqualTo(2));
            Assert.That(resultado.Items.Count, Is.EqualTo(2));
            Assert.That(logS.NombreInterface, Is.EqualTo("Proveedores"));
            Assert.That(logS.Completa, Is.False);
            Assert.That(logS.Correcta, Is.True);
        }

        [Test]
        public void TestSincronizarError()
        {
            const string mensajeError = "Error test";
            servicioSapMock.Setup(s => s.DatosProveedores(It.IsAny<DatosProveedoresRequest>()))
                           .Throws(new Exception(mensajeError));
            LogSincronizacion logS = null;
            repositorioMock.Setup(s => s.Agregar(It.IsAny<LogSincronizacion>()))
                           .Returns<LogSincronizacion>(x => logS = x);

            var resultado = target.Ejecutar(new SincronizarProveedores { RetornarResultado = false, Cuit = null }) as ResultadoSincronizarProveedores;

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(logS.Correcta, Is.False);
            Assert.That(logS.NombreInterface, Is.EqualTo("Proveedores"));
            Assert.That(logS.Mensaje, Is.EqualTo(mensajeError));
        }

    }
}