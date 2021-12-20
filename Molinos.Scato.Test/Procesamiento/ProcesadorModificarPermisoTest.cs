using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarPermisoTest
    {
        private ProcesadorModificarPermiso target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private PermisoDto tipoDto;
        private Permiso tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarPermiso(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new PermisoDto
            {
                Id = 1,
                Descripcion = "Permiso 1",
                Codigo = PermisosScato.AbmAlmacen
            };
            tipo = new Permiso
            {
                Id = 1,
                Descripcion = "Permiso 1",
                Codigo = PermisosScato.AbmAlmacen
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Permiso>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarPermiso { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<PermisoDto>(), It.IsAny<Permiso>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<Permiso>
                {
                    new Permiso {Id = 5, Descripcion = "P1", Codigo = PermisosScato.AbmAlmacen},
                    new Permiso {Id = 6, Descripcion = "P2", Codigo = PermisosScato.AbmBalanza}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Permiso, bool>>>()))
                    .Returns<Expression<Func<Permiso, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "P1";
            var comando = new ModificarPermiso { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }
    }
}
