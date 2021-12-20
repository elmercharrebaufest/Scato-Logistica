using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearLoteTest
    {
        private ProcesadorCrearLote target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private LoteDto dto;
        private List<MuestraEnvioACamaraDto> muestrasDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearLote(repositorioMock.Object, conversor, new NullLogger());
            muestrasDto = new List<MuestraEnvioACamaraDto> { new MuestraEnvioACamaraDto {Id = 1, EstadoMuestra = EstadoMuestra.Pendiente}, new MuestraEnvioACamaraDto{Id = 2, EstadoMuestra = EstadoMuestra.Pendiente} };
            dto = new LoteDto
                {
                    CamaraId = 1,
                    Fecha = new DateTime(2010,2,2),
                    NumeroDeLote = "1111",
                    Muestras = muestrasDto,
                };
        }

        [Test]
        public void TestCrear()
        {
            var comando = new CrearLote {Dto = dto};

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Lote, bool>>>())).Returns(false);
            repositorioMock.Setup(s => s.Obtener<MuestraEnvioACamara>(It.IsAny<int>())).Returns(new MuestraEnvioACamara());

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearYaExiste()
        {
            var comando = new CrearLote { Dto = dto };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Lote, bool>>>())).Returns(true);

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}
