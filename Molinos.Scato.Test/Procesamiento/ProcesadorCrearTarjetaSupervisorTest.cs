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
    public class ProcesadorCrearTarjetaSupervisorTest
    {
        private ProcesadorCrearTarjetaSupervisor target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TarjetaSupervisorDto dto;
        private List<TarjetaSupervisor> tarjetasSupervisor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearTarjetaSupervisor(repositorioMock.Object, conversor, new NullLogger());
            dto = new TarjetaSupervisorDto
            {
                Id = 1,
                Descripcion = "Tarjeta 1",
                Salida = 1,
                CentroId = 1,
            };

            tarjetasSupervisor = new List<TarjetaSupervisor>
                {
                    new TarjetaSupervisor
                        {
                            Id = 1,
                            Descripcion = "Tarjeta 1",
                            Salida = 1,
                            Centro = new Centro{Id = 1},
                        }
                };
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(new Centro());
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearTarjetaSupervisor { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TarjetaSupervisor>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}
