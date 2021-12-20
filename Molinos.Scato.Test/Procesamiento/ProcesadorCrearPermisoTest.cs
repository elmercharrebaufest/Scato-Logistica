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
    public class ProcesadorCrearPermisoTest
    {
        private ProcesadorCrearPermiso target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private PermisoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearPermiso(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new PermisoDto
            {
                Id = 1,
                Descripcion = "Permiso 1",
                Codigo = PermisosScato.AbmAlmacen
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearPermiso { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Permiso>(o => o.Descripcion == tipoDto.Descripcion)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<Permiso>
                {
                    new Permiso {Id = 5, Descripcion = "P1", Codigo = PermisosScato.AbmAlmacen},
                    new Permiso {Id = 6, Descripcion = "P2", Codigo = PermisosScato.AbmBalanza}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Permiso, bool>>>()))
                    .Returns<Expression<Func<Permiso, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "P1";
            var comando = new CrearPermiso { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Permiso>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }
    }
}
