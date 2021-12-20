using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearArchivoDeMovimientosTest
    {
        private ProcesadorCrearArchivoDeMovimientos target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private FiltroArchivoDeMovimientosDto tipoDto;
        private Mock<IFirmaProvider> firma;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            firma = new Mock<IFirmaProvider>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearArchivoDeMovimientos(repositorioMock.Object, conversor, new NullLogger(), firma.Object);
            tipoDto = new FiltroArchivoDeMovimientosDto()
            {
                CentroId = 1,
                MaterialId = 1,
                Fecha = new DateTime(),
                TipoDeWorkflow = TipoDeWorkflow.Egreso,
                NombreUsuario = "a",
                CentroDesc = "b",
                Material = "cs"
            };
            firma.Setup(x => x.ObtenerFirmaSinLogo()).Returns(new FirmaDto{DescripcionCorta = "MOA"});
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearArchivoDeMovimientos() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.EjecutarComando(It.IsAny<IComando<int>>()), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
