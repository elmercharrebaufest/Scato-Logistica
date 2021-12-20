using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProcesadorModificarNotificacionTest
    {
        private ProcesadorModificarNotificacion target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NotificacionDto tipoDto;
        private Notificacion tipo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarNotificacion(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new NotificacionDto
                {
                    Id = 1,
                    Mensaje = "Test"
                };
            tipo = new Notificacion
                {
                    Id = 1
                };
        }

        [Test]
        public void TestModificarEntidad()
        {
            var tiposExistentes = new List<Notificacion>
                {
                    new Notificacion
                {
                    Id = 1
                }
                };
            repositorioMock.Setup(s => s.Obtener<Notificacion>(It.IsAny<int>())).Returns(tipo);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Notificacion, bool>>>()))
                    .Returns<Expression<Func<Notificacion, bool>>>(q =>tiposExistentes.Any((q.Compile())));

            var comando = new ModificarNotificacion {Id = tipoDto.Id};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

       
        [Test]
        public void TestModificarEntidadValidoPorId()
        {
            var tiposExistentes = new List<Notificacion>
                {
                    new Notificacion
                        {
                            Id = 1
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Notificacion, bool>>>()))
                           .Returns<Expression<Func<Notificacion, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<Notificacion>(It.IsAny<int>())).Returns(tipo);

            var comando = new ModificarNotificacion { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}