using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class FletesDobleTramoTransmisionASapAsincronicoTest
    {
        private FletesDobleTramoTransmisionASapAsincronico target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> servcomando;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioSapAsincronico> servSap;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            servSap = new Mock<IServicioSapAsincronico>();
            target = new FletesDobleTramoTransmisionASapAsincronico();
            host = WorkflowInvokerTest.Create(target);

            host.Extensions.Add(servRepositorio);
            host.Extensions.Add(servcomando);
            
        }

        [Test]
        public void TestEjecutar()
        {
            host.Extensions.Add(servSap.Object);
            host.InArguments.InstanceId = Guid.NewGuid();
            host.InArguments.Request = new FletesDobleTramoRequest(){FletesDobleTramo = new FletesDobleTramo{Almacen = "A", Centro = "C"}};

            var result = host.TestActivity();
            Assert.NotNull(result);
            Assert.AreEqual(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.Count,0);
        }
        
        [Test]
        public void TestEjecutarException()
        {
            host.InArguments.InstanceId = Guid.NewGuid();
            host.InArguments.Request = new FletesDobleTramoRequest(){FletesDobleTramo = new FletesDobleTramo{Almacen = "A", Centro = "C"}};
            
            var result = host.TestActivity();
            Assert.NotNull(result);
            Assert.AreEqual(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.Count, 1);
            Assert.AreEqual(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.ElementAt(0).Key, "InstanciaWorkflow");
        }
    }
}
