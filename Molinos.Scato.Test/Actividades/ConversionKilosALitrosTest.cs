using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ConversionKilosALitrosTest
    {
        private ConversionKilosALitros target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new ConversionKilosALitros();
            srvRepositorio = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestConversionKilosALitros()
        {
            srvRepositorio.Setup(s => s.Ejecutar(It.IsAny<ModificarRecorridoPesoNetoBodegaEnLitros>())).Returns(new ResultadoModificarRecorridoPesoNetoBodegaEnLitros(){PesoNetoBodegaEnLitros = 3});
            host.InArguments.WorkflowId = new Guid();
            host.InArguments.PesoNetoBodega = 2;
            host.InArguments.MaterialId = 1;
            host.InArguments.PuestoDeTrabajoId = 1;

            var resultado = host.TestActivity();

            var peso = resultado.First(f => f.Key == "PesoNetoBodegaEnLitros").Value;



            Assert.That(resultado, Is.Not.Null);
            Assert.That(peso, Is.EqualTo(3));
        }

    }
}
