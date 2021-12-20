using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorEliminarPermisoTest
    {
        private ProcesadorEliminarPermiso target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private PermisoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarPermiso(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new PermisoDto
            {
                Id = 1,
                Descripcion = "Permiso 1",
                Codigo = PermisosScato.AbmAlmacen
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarPermiso { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Permiso>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
