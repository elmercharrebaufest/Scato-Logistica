using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
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
    public class ProcesadorCrearVinedoPropioTest
    {
        private ProcesadorCrearVinedoPropio target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private VinedoPropioDto tipoDto;
        private List<CuartelDto> cuarteles;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearVinedoPropio(repositorioMock.Object, conversorMock, new NullLogger());
            cuarteles = new List<CuartelDto>
                {
                    new CuartelDto
                        {
                           Id = 1,
                           Codigo = "5000"
                        },
                    new CuartelDto
                        {
                           Id = 2,
                           Codigo = "5001"
                        }
                };
            tipoDto = new VinedoPropioDto
            {
                Id = 1,
                NumeroINV = "1000",
                Cuarteles = cuarteles,
                Descripcion = "Viñedo1",
                CuartelesJson = "[{\"Id\":1,\"Codigo\":\"5000\"},{\"Id\":2,\"Codigo\":\"5001\"}]",
                IngresosBrutos = "7000"
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearVinedoPropio { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<VinedoPropio>(o => o.Descripcion == comando.Dto.Descripcion)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}