using System;
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
    public class ProcesadorEliminarAjusteDeStockTest
    {
        private ProcesadorEliminarAjusteDeStock target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private AjusteDeStockDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarAjusteDeStock(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new AjusteDeStockDto()
            {
                Id = 1,
                CentroId = 1,
                Fecha = DateTime.Now,
                MaterialId = 1,
                PesoBrutoIngreso = 0,
                PesoNetoEgreso = 0,
                PesoNetoIngreso = 1,
                TipoComprobanteOnccaId = 1,
                NumeroDocumentoIngreso = "1234-12345678"
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            repositorioMock.Setup(x => x.Obtener<AjusteDeStock>(It.IsAny<int>())).Returns(new AjusteDeStock());

            var comando = new EliminarAjusteDeStock() { Id = tipoDto.Id, Usuario = "User1"};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<AjusteDeStock>(It.IsAny<AjusteDeStock>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
