using System;
using System.Linq.Expressions;
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
    public class ProcesadorCrearMuestraDeNirsTest
    {
        private ProcesadorCrearMuestraDeNirs target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private MuestraDeNirsDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearMuestraDeNirs(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new MuestraDeNirsDto
            {
                Id = 1,
                Fecha = DateTime.Today,
                WorkflowInstanceId = new Guid(),
                ValorFinal = 5,
                CentroId = 2,
                NirsId = 3,
            };
        }

        [Test]
        public void TestCrearEntidad()
        {

            var comando = new CrearMuestraDeNirs { Dto = tipoDto };
            repositorioMock.Setup(x => x.Obtener<Centro>(It.IsAny<int>()));
            repositorioMock.Setup(x => x.Obtener<Nirs>(It.IsAny<int>()));
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<MuestraDeNirs>(o => o.ValorFinal == tipoDto.ValorFinal)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }


    }
}
