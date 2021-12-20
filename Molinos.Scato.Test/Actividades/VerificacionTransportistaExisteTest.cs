using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificacionTransportistaExisteTest
    {
        private VerificacionTransportistaExiste target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servComando;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servComando = new Mock<IServicioComandos>();
            target = new VerificacionTransportistaExiste();
            host = WorkflowInvokerTest.Create(target);

            servRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>())).Returns(new TransportistaDto());

            host.InArguments.WorkflowId = Guid.NewGuid();
            host.InArguments.TransportistaId = 1;
            host.InArguments.PuestoDeTrabajoId = 1;

            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servComando.Object);
        }


        [Test]
        public void ExecuteTest()
        {
            var result = host.TestActivity();
            Assert.NotNull(result);
            Assert.True((Boolean)result.FirstOrDefault().Value);
        }

        [Test]
        public void TranspotistaIdNull()
        {
            host.InArguments.TransportistaId = null;

            var result = host.TestActivity();
            Assert.NotNull(result);
            Assert.False((Boolean)result.FirstOrDefault().Value);
        }

        [Test]
        public void NoExisteElTransportista()
        {
            servRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>())).Returns((TransportistaDto)null);

            var result = host.TestActivity();
            Assert.NotNull(result);
            Assert.False((Boolean)result.FirstOrDefault().Value);
        }

        [Test]
        public void ExceptionsTest()
        {
            servComando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception());
            servComando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception());

            var result = host.TestActivity();
            Assert.NotNull(result);
            Assert.True((Boolean)result.FirstOrDefault().Value);
        }
    }
}
