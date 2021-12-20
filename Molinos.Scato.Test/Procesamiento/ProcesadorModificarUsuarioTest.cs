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
    public class ProcesadorModificarUsuarioTest
    {
        private ProcesadorModificarUsuario target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private UsuarioDto tipoDto;
        private Usuario tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarUsuario(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new UsuarioDto
            {
                Id = 1,
                NombreUsuario = "Usuario 1",
                RolesAsociados = new List<RolDto>(),
                CentrosAsociados = new List<CentroDto>()
            };
            tipo = new Usuario
            {
                Id = 1,
                NombreUsuario = "Usuario 1",
                RolesAsociados = new List<Rol>(),
                CentrosAsociados = new List<Centro>()
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Usuario>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarUsuario { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<UsuarioDto>(), It.IsAny<Usuario>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorNombreUsuario()
        {
            var tiposExistentes = new List<Usuario>
                {
                    new Usuario {Id = 5, NombreUsuario = "U1", },
                    new Usuario {Id = 6, NombreUsuario = "U2", }
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .Returns<Expression<Func<Usuario, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.NombreUsuario = "U1";
            var comando = new ModificarUsuario { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("NombreUsuario"));
        }
    }
}
