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
    public class ProcesadorModificarRolTest
    {
        private ProcesadorModificarRol target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private RolDto tipoDto;
        private Rol tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarRol(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new RolDto
            {
                Id = 1,
                Descripcion = "Rol 1",
                PermisosAsociados = new List<PermisoDto>()
            };
            tipo = new Rol
            {
                Id = 1,
                Descripcion = "Rol 1",
                PermisosAsociados = new List<Permiso>()
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Rol>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarRol { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<RolDto>(), It.IsAny<Rol>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<Rol>
                {
                    new Rol {Id = 5, Descripcion = "R1", },
                    new Rol {Id = 6, Descripcion = "R2", }
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Rol, bool>>>()))
                    .Returns<Expression<Func<Rol, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "R1";
            var comando = new ModificarRol { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }
    }
}
