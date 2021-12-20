using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
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
    public class ProcesadorActualizarUltimoAnalisisdePuestodeTrabajoTest
    {
        private ProcesadorActualizarUltimoAnalisisDePuestoDeTrabajo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            log = new NullLogger();
            target = new ProcesadorActualizarUltimoAnalisisDePuestoDeTrabajo(repositorioMock.Object, conversorMock.Object, log);
        }

        [Test]
        public void TestModificar()
        {
            repositorioMock.Setup(s => s.Obtener<PuestoDeTrabajo>(It.IsAny<int>())).Returns(new PuestoDeTrabajo());

            var resultado = target.Ejecutar(new ActualizarUltimoAnalisisDePuestoDeTrabajo { IdPuesto = 6, UltimoAnalisis = DateTime.Now, Usuario = "Marcos P", InstanceId = new Guid("25892e17-80f6-415f-9c65-7395632f0223") });

            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.False);
        }
    }
}