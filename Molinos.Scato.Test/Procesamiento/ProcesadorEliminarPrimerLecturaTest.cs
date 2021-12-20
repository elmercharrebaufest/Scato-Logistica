using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorEliminarPrimerLecturaTest
    {
        private ProcesadorEliminarPrimerLectura target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarPrimerLectura(repositorioMock.Object, conversorMock.Object, new NullLogger());
  
        }

        [Test]
        public void TestEliminarEntidad()
        {
            repositorioMock.Setup(
                x =>
                x.ObtenerMenor(It.IsAny<Expression<Func<LecturaDeTarjeta, bool>>>(),
                               It.IsAny<Expression<Func<LecturaDeTarjeta, int>>>())).Returns(new LecturaDeTarjeta());
            var comando = new EliminarPrimerLectura {PuestoDeTrabajoId = 1};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<LecturaDeTarjeta>(It.IsAny<LecturaDeTarjeta>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
