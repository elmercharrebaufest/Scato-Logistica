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
    public class ProcesadorModificarTarjetaRangoTest
    {
        private ProcesadorModificarTarjetaRango target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TarjetaRangoDto tipoDto;
        private TarjetaRango tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarTarjetaRango(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TarjetaRangoDto
                {
                    Id = 1,
                    Codigo = "4444",
                    RangoDesde = "10000",
                    RangoHasta = "30000",
                    ValidoDesde = new DateTime(2010, 1, 1),
                    ValidoHasta = new DateTime(2010, 1, 4),
                    CentroId = 1,
                };
            tipo = new TarjetaRango
                {
                    Id = 1,
                    Codigo = "4444",
                    RangoDesde = "10000",
                    RangoHasta = "30000",
                    ValidoDesde = new DateTime(2010, 1, 1),
                    ValidoHasta = new DateTime(2010, 1, 4),
                    Centro = new Centro(),
                };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<TarjetaRango>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarTarjetaRango {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));

        }
    }
}
