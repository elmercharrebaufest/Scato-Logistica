using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class GuardarPesoNetoTransileTest
    {
        private GuardarPesoNetoTransile target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> servComandosMock;

        [SetUp]
        public void SetUp()
        {
            target = new GuardarPesoNetoTransile();
            srvRepositorio = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(servComandosMock.Object);
        }

        [Test]
        public void GuardarPesoNetoTransile()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            host.InArguments.Peso = 30000;
            host.InArguments.Tolerancia = 1000;
            host.InArguments.InstanceId = new Guid();
            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "FinTransile").Value;


            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(false));
        }
    }
}
