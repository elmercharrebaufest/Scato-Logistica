using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarFechaEgresoTest
    {
        private ProcesadorModificarFechaEgreso target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;
        private CartaPorte cartaPorte;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            log = new NullLogger();
            target = new ProcesadorModificarFechaEgreso(repositorioMock.Object, conversorMock.Object, log);

            cartaPorte = new CartaPorte
            {
                Id = 1,
                Chofer = new Chofer { Id = 1, Nombre = "Chofer" },
                Material = new Material { Id = 1, Descripcion = "MaterialDesc 1" },
                TipoComercial = new TipoComercial { Id = 1, Descripcion = "Tipo 1" },
            };


        }

        [Test]
        public void TestModificarCartaPorteFechaEgreso()
        {
            var fecha = new DateTime(2012, 1, 1);
            var recorrido = new Recorrido();
            var comando = new ModificarFechaEgreso { WorkflowInstanciaId = new Guid(), Fecha = fecha };
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(recorrido);
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(recorrido.FechaEgreso, Is.EqualTo(fecha));
        }

        [Test]
        public void TestModificarError()
        {
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<int>())).Throws(new Exception("error"));
            var resultado = target.Ejecutar(new ModificarFechaEgreso { WorkflowInstanciaId = new Guid(), Fecha = DateTime.Now });
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Never());
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Values.Contains(Textos.Error_Generico), Is.EqualTo(true));
        }

    }
}