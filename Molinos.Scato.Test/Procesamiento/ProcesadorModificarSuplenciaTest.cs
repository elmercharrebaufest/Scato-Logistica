using System;
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
    public class ProcesadorModificarSuplenciaTest
    {
        private ProcesadorModificarSuplencia target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private SuplenciaDto tipoDto;
        private Suplencia tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarSuplencia(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new SuplenciaDto
            {
                Id = 1,
                UsuarioASuplantarId = 1,
                UsuarioASuplantarNombreUsuario = "Usuario a Suplantar 1",
                UsuarioSuplenteId = 2,
                UsuarioSuplenteNombreUsuario = "Usuario Suplente 1",
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime()
            };
            tipo = new Suplencia
            {
                Id = 1,
                UsuarioASuplantar = new Usuario{Id = 1, Nombre = "Usuario a Suplantar 1"},
                UsuarioSuplente = new Usuario{Id = 2, Nombre = "Usuario Suplente 1"},
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime()
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Suplencia>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarSuplencia { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<SuplenciaDto>(), It.IsAny<Suplencia>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
