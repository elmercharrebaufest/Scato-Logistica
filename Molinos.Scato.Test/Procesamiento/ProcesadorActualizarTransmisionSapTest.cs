using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorActualizarTransmisionSapTest
    {
        private ProcesadorActualizarTransmisionSap target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TransmisionSapAModificarDto tipoDto;
        private TransmisionASap transmision;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorActualizarTransmisionSap(repositorioMock.Object, conversor, new NullLogger());
            transmision = new LlegadaAdestinosEnRedespachosTransmisionASap
                {   Id = 1,
                    Estado = EstadoTransmisionASap.Pendiente,
                    FuncionSap = FuncionSAP.LlegadaADestinosEnRedespachos,
                    InstanciaWorkflow = new Guid(),
                    Documento = "22000333",
                    Ejercicio = "ejercicio1",
                    DocLegal = "3344",
                    FechaContab = "2015-06-22",
                };
            var campos = new Dictionary<string, string> { { "Documento", "valor1" }, { "DocLegal", "valor2" } };
            tipoDto = new TransmisionSapAModificarDto
            {
                Id = 1,
                Campos = campos
            };
        }

        [Test]
        public void ActualizarEntidad()
        {
            var comando = new ActualizarTransmision { Dto = tipoDto };
            repositorioMock.Setup(v => v.ObtenerConsultaEscalar<TransmisionASap>(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void ActualizarEntidadError()
        {
            var comando = new ActualizarTransmision { Dto = tipoDto };
            repositorioMock.Setup(v => v.ObtenerConsultaEscalar<TransmisionASap>(It.IsAny<ObtenerTransmisionASap>())).Returns((TransmisionASap)null);
            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }


    }
}
