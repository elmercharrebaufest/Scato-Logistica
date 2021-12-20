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
    public class ProcesadorModificarMaterialTest
    {
        private ProcesadorModificarMaterial target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private MaterialDto tipoDto;
        private Material tipo;
        private MaterialPorCentroDto materialPorCentroDto;
        private MaterialPorCentro materialPorCentro;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarMaterial(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new MaterialDto
            {
                Id = 1,
                Descripcion = "MaterialDesc 1",
                DescripcionCorta = "mat 1"
            };
            tipo = new Material
            {
                Id = 1,
                Descripcion = "MaterialDesc Dto 1",
                DescripcionCorta = "mat dto 1"
            };
            materialPorCentroDto = new MaterialPorCentroDto
            {
                Id = 1,
                CentroId = 1, 
                MaterialId = 1, 
                AlmacenPredId = 1, 
                AnalisisInterno = 50
            };
            materialPorCentro = new MaterialPorCentro
            {
                Id = 1,
                Centro = new Centro{Id = 1},
                Material = tipo,
                AlmacenPredeterminado = new Almacen{Id = 1},
                AnalisisInterno = 50
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(tipo);
            repositorioMock.Setup(s => s.Obtener<MaterialPorCentro>(It.IsAny<int>())).Returns(materialPorCentro);
            repositorioMock.Setup(s => s.Obtener<Almacen>(It.IsAny<int>())).Returns(new Almacen{Id = 1});
            var comando = new ModificarMaterial { Dto = tipoDto, MaterialPorCentroDto = materialPorCentroDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
