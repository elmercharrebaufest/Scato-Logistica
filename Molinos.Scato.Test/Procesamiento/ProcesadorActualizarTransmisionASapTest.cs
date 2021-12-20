using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorActualizarTransmisionASapTest
    {
        private ProcesadorActualizarTransmisionASap target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private TransmisionASapDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorActualizarTransmisionASap(repositorioMock.Object, conversorMock,
                                                             new NullLogger());
            tipoDto = new TransmisionASapDto
                {
                    Id = 1,
                    Estado = EstadoTransmisionASap.Correcto,
                    Fecha = DateTime.Now,
                    FuncionSap = FuncionSAP.AjusteDeDiferencias,
                    InstanciaWorkflow = Guid.NewGuid(),
                    MensajeError = "",
                    NumeroDocumento = "1234",
                    Patente = "ABC123",
                    TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<TransmisionASap, bool>>>()))
                           .Returns<TransmisionASap>(null);

            var comando = new ActualizarTransmisionASap {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(
                s => s.Agregar(It.Is<TransmisionASap>(t => t.InstanciaWorkflow == tipoDto.InstanciaWorkflow)),
                Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestActualizarEntidad()
        {
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>()))
                           .Returns(new AjusteDeDiferenciasEnRedespachosTransmisionASap
                                   {
                                       Estado = EstadoTransmisionASap.Pendiente,
                                       InstanciaWorkflow = Guid.NewGuid(),
                                       Fecha = DateTime.Now,
                                       FuncionSap = FuncionSAP.AjusteDeDiferencias,
                                       Id = 2
                                   });

            var comando = new ActualizarTransmisionASap { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(
                s => s.Agregar(It.IsAny<TransmisionASap>()),
                Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
