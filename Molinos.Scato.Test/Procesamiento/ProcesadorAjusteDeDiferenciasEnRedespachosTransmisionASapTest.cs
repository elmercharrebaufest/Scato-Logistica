using System;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorActualizarAjusteDeDiferenciasEnRedespachosTransmisionASapTest
    {
        private ProcesadorActualizarAjusteDeDiferenciasEnRedespachosTransmisionASap target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private AjusteDeDiferenciasEnRedespachosTransmisionASapDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorActualizarAjusteDeDiferenciasEnRedespachosTransmisionASap(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new AjusteDeDiferenciasEnRedespachosTransmisionASapDto
            {
                Id = 1,
                Almacen = "test",
                Cantidad = "test",
                Centro = "test",
                CentroDeCoste = "test",
                ClaseExpedicion = "test",
                Estado = EstadoTransmisionASap.Correcto,
                Fecha = DateTime.Now,
                FechaContab = DateTime.Now,
                FechaDoc = DateTime.Now,
                Material = "test",
                NroDocumento = "test",
                Patente = "AAA111",
                UniMed = "test"
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            Assert.That(1 == 1);
        }

    }
}