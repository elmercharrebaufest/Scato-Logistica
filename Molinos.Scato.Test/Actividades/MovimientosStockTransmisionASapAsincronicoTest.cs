using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class MovimientosStockTransmisionASapAsincronicoTest
    {
        private MovimientosStockTransmisionASapAsincronico target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IRepositorio> repositorio;
        private Mock<IServicioSapAsincronico> srvSap;

        [SetUp]
        public void SetUp()
        {
            target = new MovimientosStockTransmisionASapAsincronico();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            srvSap = new Mock<IServicioSapAsincronico>();
            host.Extensions.Add(srvSap.Object);
        }

        [Test]
        public void TestGenerarRequest()
        {
            host.InArguments.Request = new MovAjuste() { Almacen = "1", Cantidad = "1000", Centro = "1", Material = "1", NroDocumento = "1010", Patente = "ABC123", FechaContab = "2015-03-20" };
            host.InArguments.InstanceId = new Guid();

            var resultado = host.TestActivity();
            var request = resultado.First(f => f.Key == "Result").Value as Resultado;
            
            Assert.That(host.OutArguments.FuncionaServicio, Is.True);
            Assert.That(request.HayErrores, Is.False);

        }
    }
}
