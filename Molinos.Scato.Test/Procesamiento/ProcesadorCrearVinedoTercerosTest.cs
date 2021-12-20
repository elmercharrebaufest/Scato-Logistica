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
    public class ProcesadorCrearVinedoTercerosTest
    {
        private ProcesadorCrearVinedoTerceros target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private VinedoTercerosDto tipoDto;
        private Proveedor proveedor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearVinedoTerceros(repositorioMock.Object, conversorMock, new NullLogger());

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
                ProveedorId = proveedor.Id,
                esProveedorPR = proveedor.PR

            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearVinedoTerceros {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<VinedoTerceros>(o => o.Descripcion == comando.Dto.Descripcion)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}