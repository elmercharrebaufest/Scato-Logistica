using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorEliminarVinedoPropioTest
    {
        private ProcesadorEliminarVinedoPropio target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private VinedoPropio tipoDto;
        private List<Cuartel> cuarteles;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarVinedoPropio(repositorioMock.Object, conversorMock, new NullLogger());
            cuarteles = new List<Cuartel>
                {
                    new Cuartel
                        {
                           Id = 1,
                           Codigo = "5000"
                        },
                    new Cuartel
                        {
                           Id = 2,
                           Codigo = "5001"
                        }
                };
            tipoDto = new VinedoPropio
            {
                Id = 1,
                NumeroINV = "1000",
                Cuarteles = cuarteles,
                Descripcion = "Viñedo1",
                IngresosBrutos = "7000"
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<VinedoPropio>(It.IsAny<int>())).Returns(tipoDto);
            var comando = new EliminarVinedoPropio { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover(It.Is<VinedoPropio>(c => c.Id == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
