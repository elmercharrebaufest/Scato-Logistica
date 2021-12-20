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
    public class ProcesadorEliminarFormatoDeImpresionTest
    {
        private ProcesadorEliminarFormatoDeImpresion target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private FormatoDeImpresionDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarFormatoDeImpresion(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new FormatoDeImpresionDto
            {
                Id = 1,
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var formato = new FormatoDeImpresion
                {
                    Id = 1,
                    FormatosDeCampo = new List<FormatoDeCampo>
                        {
                            new FormatoDeCampo {Id = 2},
                            new FormatoDeCampo {Id = 3}
                        }
                };
            repositorioMock.Setup(r => r.Obtener<FormatoDeImpresion>(1)).Returns(formato);
            var comando = new EliminarFormatoDeImpresion { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            

            repositorioMock.Verify(s => s.Remover(It.Is<FormatoDeImpresion>(x => x.Id == 1)),Times.Once());
            repositorioMock.Verify(s => s.Remover(It.Is<FormatoDeCampo>(x => x.Id == 2)), Times.Once());
            repositorioMock.Verify(s => s.Remover(It.Is<FormatoDeCampo>(x => x.Id == 3)), Times.Once());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
