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
    public class ProcesadorModificarMotivoQuiebreBarreraTest
    {
        private ProcesadorModificarMotivoQuiebreBarrera target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private MotivoQuiebreBarreraDto tipoDto;
        private MotivoQuiebreBarrera tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarMotivoQuiebreBarrera(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new MotivoQuiebreBarreraDto()
                {
                    Id = 1,
                    Fecha = DateTime.Now,
                    Hora = DateTime.Now.ToShortTimeString(),
                    Patente = "AAA111",
                    PuestoTrabajoId = 1,
                    TransportistaId = 1
                };
            tipo = new MotivoQuiebreBarrera()
            {
                Id = 1,
                Fecha = DateTime.Now,
                Patente = "AAA111",
                PuestoTrabajo = new PuestoDeTrabajo(){NombrePuesto = "PT1", Id = 1},
                Transportista = new Transportista(){Id = 1, RazonSocial = "Transportista1"}
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<MotivoQuiebreBarrera>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarMotivoQuiebreBarrera() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
