using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearDistribucionDeAlmacenesTest
    {
        private ProcesadorCrearDistribucionDeAlmacenes target;
        private Mock<IRepositorio> repositorioMock;

        private DistribucionDeAlmacenesDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();

            target = new ProcesadorCrearDistribucionDeAlmacenes(repositorioMock.Object, new ConversorAutoMapper(), new NullLogger());
            

        }

        [Test]
        public void TestCrearEntidad()
        {
            tipoDto = new DistribucionDeAlmacenesDto
            {
                NombreDeUsuario = "nombre",
                DistribucionesDeAlmacenes = new List<DistribucionDeAlmacenDto> { new DistribucionDeAlmacenDto { AlmacenId = 1 ,Litros = 50}, new DistribucionDeAlmacenDto { AlmacenId = 2 ,Litros = 25} },
                PesoNetoBodegaEnLitros = 75,
                InstanceId = new Guid()
            };

            repositorioMock.Setup(x => x.Existe<Almacen>(It.IsAny<Expression<Func<Almacen,bool>>>())).Returns(true);
            repositorioMock.Setup(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            var comando = new CrearDistribucionDeAlmacenes {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<DistribucionDeAlmacenes>(o => o.NombreDeUsuario == "nombre")), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Almacen>(It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDocumento()
        {
            tipoDto = new DistribucionDeAlmacenesDto
            {
                NombreDeUsuario = "nombre",
                DistribucionesDeAlmacenes = new List<DistribucionDeAlmacenDto> { new DistribucionDeAlmacenDto { AlmacenId = 1, Litros = 49 }, new DistribucionDeAlmacenDto { AlmacenId = 2, Litros = 25 } },
                PesoNetoBodegaEnLitros = 75,
                InstanceId = new Guid()
            };

            repositorioMock.Setup(x => x.Existe<Almacen>(It.IsAny<Expression<Func<Almacen, bool>>>())).Returns(true);
            repositorioMock.Setup(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            var comando = new CrearDistribucionDeAlmacenes { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.Agregar(It.IsAny<DistribucionDeAlmacenes>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("DistribucionesDeAlmacenes"));
        }
    }
}