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
    public class ProcesadorModificarProveedorTest
    {
        private ProcesadorModificarProveedor target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Proveedor proveedor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarProveedor(repositorioMock.Object, conversorMock.Object, new NullLogger());
            proveedor = new Proveedor
            {
                Id = 1,
                EsDestinatario = true,
                CodigoSap = "1",
                Descripcion = "Codigo Cero",
                Cuil = "20-30591238-8",
                Domicilio = "Domicilio",
                RazonSocial = "Emilio",
            };

            repositorioMock.Setup(s => s.Obtener<Proveedor>(It.IsAny<int>())).Returns(proveedor);
        }

        [Test]
        public void TestModificarEntidad()
        {

            var entregadorDto = new ProveedorDto
            {
                Id = 1,
                EsDestinatario = true,
                CodigoSap = "1",
                Descripcion = "Codigo Cero",
                Cuil = "20-30591238-8",
                Domicilio = "Domicilio",
                RazonSocial = "Emilio",
            };


            var comando = new ModificarProveedor { Dto = entregadorDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
