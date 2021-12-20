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
    public class ProcesadorCrearRolTest
    {
        private ProcesadorCrearRol target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private RolDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearRol(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new RolDto
            {
                Id = 1,
                Descripcion = "Rol 1",
                PermisosAsociados = new List<PermisoDto> { new PermisoDto { Id = 1, Descripcion = "Permiso 1" }, new PermisoDto { Id = 2, Descripcion = "Permiso 2" } }
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearRol { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Rol>(o => o.Descripcion == tipoDto.Descripcion)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<Rol>
                {
                    new Rol {Id = 5, Descripcion = "R1"},
                    new Rol {Id = 6, Descripcion = "R2"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Rol, bool>>>()))
                    .Returns<Expression<Func<Rol, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "R1";
            var comando = new CrearRol { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Rol>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }
    }
}
