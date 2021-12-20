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
    public class ProcesadorModificarUsuarioUltimoLoginTest
    {
        private ProcesadorModificarUsuarioUltimoLogin target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private UsuarioDto tipoDto;
        private List<Usuario> tiposExistentes;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarUsuarioUltimoLogin(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new UsuarioDto
            {
                Id = 1,
                UltimoLogin = DateTime.Now
            };
            tiposExistentes = new List<Usuario>
            {
                new Usuario
                {
                    Id = 1,
                    UltimoLogin = DateTime.Now
                },
                new Usuario
                {
                    Id = 2,
                    UltimoLogin = DateTime.Now
                }
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            var comando = new ModificarUsuarioUltimoLogin { Usuario = "User1" };
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .Returns(tiposExistentes[0]);
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
