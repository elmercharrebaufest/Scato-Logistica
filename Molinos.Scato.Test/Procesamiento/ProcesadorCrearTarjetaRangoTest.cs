using System;
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
    public class ProcesadorCrearTarjetaRangoTest
    {
        private ProcesadorCrearTarjetaRango target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TarjetaRangoDto dto;
        private List<TarjetaRango> tarjetasRango;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearTarjetaRango(repositorioMock.Object, conversor, new NullLogger());
            dto = new TarjetaRangoDto
                {
                    Id = 1,
                    Codigo = "4444",
                    RangoDesde = "10000",
                    RangoHasta = "30000",
                    ValidoDesde = new DateTime(2010, 1, 1),
                    ValidoHasta = new DateTime(2010, 1, 4),
                    CentroId = 1,
                };

            tarjetasRango = new List<TarjetaRango>
                {
                    new TarjetaRango
                        {
                            Id = 1,
                            Codigo = "22222",
                            RangoDesde = "10000",
                            RangoHasta = "30000",
                            ValidoDesde = new DateTime(2010,1,1),
                            ValidoHasta = new DateTime(2010,1,4),
                            Centro = new Centro{Id = 1},
                        }
                };
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(new Centro());
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearTarjetaRango {Dto = dto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TarjetaRango>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
