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
    public class ProcesadorCrearAjusteDeStockTest
    {
        private ProcesadorCrearAjusteDeStock target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private AjusteDeStockDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearAjusteDeStock(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new AjusteDeStockDto
                {
                    Id = 1,
                    Fecha = DateTime.Today,
                    TipoComprobanteOnccaId = 1,
                    NumeroDocumentoIngreso = "111111111111",
                    MaterialId = 1,
                    MaterialDesc = "Material",
                    Observaciones = "Observaciones",
                    PesoBrutoIngreso = 40000,
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearAjusteDeStock {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(
                s => s.Agregar(It.Is<AjusteDeStock>(o => o.PesoBrutoIngreso == tipoDto.PesoBrutoIngreso)),
                Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
