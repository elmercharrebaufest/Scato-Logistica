using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarRecorridoAlmacenTest
    {
        private ProcesadorModificarRecorridoAlmacen target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;
        private Recorrido recorrido;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            log = new NullLogger();
            target = new ProcesadorModificarRecorridoAlmacen(repositorioMock.Object, conversorMock.Object, log);

            recorrido = new Recorrido
            {
                Id = 1,
                Almacen = new Almacen { Id = 1, Centro = new Centro { Id = 1 }, Descripcion = "Almacen 1" },
                Centro = new Centro { Id = 1 },
                Chofer = new Chofer { Id = 1, Nombre = "Chofer" },
                DatosProximaActividad = "Tara",
                InstanciaWorkflow = new Guid("25892e17-80f6-415f-9c65-7395632f0223"),
                Material = new Material { Id = 1, Descripcion = "MaterialDesc 1" },
                NumeroDocumentoIngreso = "1111",
                Patente = "AAA111",
                TipoComercial = new TipoComercial { Id = 1, Descripcion = "Tipo 1" },
                TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                Workflow = new Workflow { Id = 1, Descripcion = "EgresoMaterialNoProductivo" }
            };
        }

        [Test]
        public void TestModificar()
        {
            recorrido.Almacen = null;
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(recorrido);
            repositorioMock.Setup(s => s.Obtener<Almacen>(It.IsAny<int>())).Returns(new Almacen());
            var resultado = target.Ejecutar(new ModificarRecorridoAlmacen { InstanceId = new Guid() , AlmacenId = 1 });
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            repositorioMock.Verify(v => v.Obtener<Almacen>(It.IsAny<int>()), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(recorrido.Almacen, Is.Not.EqualTo(null));
        }

        [Test]
        public void TestModificarError()
        {
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Throws(new Exception("error"));
            var resultado = target.Ejecutar(new ModificarRecorridoAlmacen { InstanceId = new Guid(), AlmacenId = 1 });
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Never());
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Values.Contains("error"), Is.EqualTo(true));
        }
    }
}
