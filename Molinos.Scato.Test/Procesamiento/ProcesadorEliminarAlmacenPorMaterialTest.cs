using System.Collections.Generic;
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
    public class ProcesadorEliminarAlmacenPorMaterialTest
    {
        private ProcesadorEliminarAlmacenPorMaterial target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarAlmacenPorMaterial(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var almacen = new Almacen {Materiales = new List<Material>()};
            var almacenes = new List<Almacen>();
            almacenes.Add(almacen);
            repositorioMock.Setup(s => s.Obtener<Almacen>(It.IsAny<int>())).Returns(almacen);
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(new Material{Almacenes = almacenes});
            var comando = new EliminarAlmacenPorMaterial { Dto = new AlmacenPorMaterial {AlmacenId = 1,MaterialId = 2}};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
