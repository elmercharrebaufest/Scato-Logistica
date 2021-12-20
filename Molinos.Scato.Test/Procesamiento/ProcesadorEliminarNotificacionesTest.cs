using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
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
    public class ProcesadorEliminarNotificacionesTest
    {
        private ProcesadorEliminarNotificaciones target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NotificacionesDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarNotificaciones(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new NotificacionesDto()
            {
                
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            repositorioMock.Setup(x => x.Listar<Notificacion>(It.IsAny<Expression<Func<Notificacion,bool>>>())).Returns(new List<Notificacion>{ new Notificacion()});

            var comando = new EliminarNotificaciones() {Grupos = ""};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover(It.IsAny<Notificacion>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
