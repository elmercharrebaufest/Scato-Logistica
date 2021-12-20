using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class EgresosMaterialNoProductivoTransmisionASapAsincronicoTest
    {
        private EgresosMaterialNoProductivoTransmisionASapAsincronico target;
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
            target = new EgresosMaterialNoProductivoTransmisionASapAsincronico();
            host = WorkflowInvokerTest.Create(target);

            host.Extensions.Add(servRepositorio);
            host.Extensions.Add(servcomando);
            host.Extensions.Add(servSap.Object);
        }

        [Test]
        public void TestEjecutar()
        {
            host.InArguments.InstanceId = Guid.NewGuid();
            host.InArguments.Request = new EgresosNoProductivosRequest() { EgresosNoProductivos = new EgresosNoProductivos { Cliente = "C", PatCamion = "AAA111" } };

            var result = host.TestActivity();
            Assert.NotNull(result);
            Assert.AreEqual(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.Count, 0);
        }

        [Test]
        public void TestEjecutarException()
        {
            host.InArguments.InstanceId = Guid.NewGuid();
            host.InArguments.Request = new EgresosNoProductivosRequest() { EgresosNoProductivos = new EgresosNoProductivos { Cliente = "C", PatCamion = "AAA111" } };
            servSap.Setup(s => s.EgresosMaterialNoProductivo(It.IsAny<Guid>(), It.IsAny<EgresosNoProductivos>()))
                   .Throws(new Exception("Error"));

            var result = host.TestActivity();
            Assert.NotNull(result);
            Assert.AreEqual(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.Count, 1);
            Assert.AreEqual(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.ElementAt(0).Value, "Error en el procedimiento del servicio SAP: Error");
        }
    }
}
