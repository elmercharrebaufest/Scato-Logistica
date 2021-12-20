using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearAjusteStockBinesTest
    {
        private ProcesadorCrearAjusteStockBines target;
        private Mock<IRepositorio> servRepositorio;
        private IConversor conversor;
        private AjusteStockBinesDto dto;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearAjusteStockBines(servRepositorio.Object, conversor, new NullLogger());
            dto = new AjusteStockBinesDto
            {
                Id = 1,
                CentroId = 1,
                CentroDescripcion = "C",
                Stock = 2,
                Fecha = new DateTime(2015, 5, 5),
                MaterialId = 1,
                Observaciones = "Obs",
                ProveedorId =  0
            };

            servRepositorio.Setup(s => s.Obtener<Centro>(It.Is<int>(i => i == dto.CentroId))).Returns(new Centro { Id = (int)(dto.CentroId) });
            servRepositorio.Setup(s => s.Obtener<Proveedor>(It.Is<int>(i => i == dto.ProveedorId)))
                           .Returns(new Proveedor{Id = 0});
            servRepositorio.Setup(s => s.Obtener<Material>(It.Is<int>(i => i == dto.MaterialId))).Returns(new Material{ Id = (dto.MaterialId) });
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearAjusteStockBines { Dto = dto };
            var resultado = target.Ejecutar(comando);
            servRepositorio.Verify(s => s.Agregar(It.Is<AjusteStockBines>(p=> p.Centro.Id == dto.CentroId)),
                Times.Exactly(1));
            servRepositorio.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
