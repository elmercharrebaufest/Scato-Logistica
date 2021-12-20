using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ObtenerCaracteristicasDeCalidadTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private ObtenerCaracteristicasDeCalidad target;
        private WorkflowInvokerTest host;
        
        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servRepositorio.Setup(s => s.ListarAnalisisYCaladoPorCaracteristica(It.IsAny<Guid>()))
                           .Returns(new AnalisisPorCaracteristicaDto[1]
                               {
                                   new AnalisisPorCaracteristicaDto
                                       {
                                           AnalisisDeCalidadId = 1,
                                           Caracteristica = "C",
                                           EsHumedad = true,
                                           DescuentoEnKg = 10,
                                           DescuentoEnPorcentaje = 10
                                       }
                               });

            target = new ObtenerCaracteristicasDeCalidad();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servRepositorio.Object);
        }

        [Test]
        public void ExecuteTest()
        {
            var resultado = host.TestActivity();

            Assert.NotNull(resultado);
            Assert.True(host.OutArguments["TieneDescuentos"]);
        }
    }

    
}
