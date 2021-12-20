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
    public class ProcesadorCrearUsuarioTest
    {
        private ProcesadorCrearUsuario target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private UsuarioDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearUsuario(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new UsuarioDto
            {
                Id = 1,
                NombreUsuario = "Usuario 1",
                RolesAsociados = new List<RolDto> { new RolDto { Id = 1, Descripcion = "Rol 1" }, new RolDto { Id = 2, Descripcion = "Rol 2" } },
                CentrosAsociados = new List<CentroDto> { new CentroDto { Id = 1, Descripcion = "Centro 1" }, new CentroDto { Id = 2, Descripcion = "Centro 2" } }
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearUsuario { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Usuario>(o => o.NombreUsuario == tipoDto.NombreUsuario)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorNombreUsuario()
        {
            var tiposExistentes = new List<Usuario>
                {
                    new Usuario {Id = 5, NombreUsuario = "U1"},
                    new Usuario {Id = 6, NombreUsuario = "U2"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .Returns<Expression<Func<Usuario, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.NombreUsuario = "U1";
            var comando = new CrearUsuario { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Usuario>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("NombreUsuario"));
        }
    }
}
