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
    public class ProcesadorEliminarVinedoTercerosTest
    {
        private ProcesadorEliminarVinedoTerceros target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private VinedoTercerosDto tipoDto;
        private Proveedor proveedor;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarVinedoTerceros(repositorioMock.Object, conversorMock, new NullLogger());
            proveedor = new Proveedor
            {
                Id = 1,
                PR = true
            };

            tipoDto = new VinedoTercerosDto
            {
                Id = 10,
                NumeroINV = "1000",
                Descripcion = "Viñedo1",
                IngresosBrutos = "7000",
                ProveedorId = 1,
                esProveedorPR = proveedor.PR
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarVinedoTerceros { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<VinedoTerceros>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
