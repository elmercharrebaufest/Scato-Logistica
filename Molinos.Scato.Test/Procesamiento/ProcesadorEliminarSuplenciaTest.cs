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
    public class ProcesadorEliminarSuplenciaTest
    {
        private ProcesadorEliminarSuplencia target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private SuplenciaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarSuplencia(repositorioMock.Object, conversor, new NullLogger());
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
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarSuplencia { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Suplencia>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
