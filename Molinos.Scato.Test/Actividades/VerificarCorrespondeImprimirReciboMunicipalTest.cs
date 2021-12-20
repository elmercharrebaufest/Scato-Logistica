using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarCorrespondeImprimirReciboMunicipalTest
    {
        private VerificarCorrespondeImprimirReciboMunicipal target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new VerificarCorrespondeImprimirReciboMunicipal();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestVerificarCorrespondeImprimirReciboMunicipal()
        {
            srvRepositorio.Setup(s => s.MaterialImprimeReciboMunicipal(It.IsAny<Guid>())).Returns(true);
            srvRepositorio.Setup(s => s.BuscarMaterialesPorCentro(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(new List<MaterialPorCentroDto> { new MaterialPorCentroDto() { Id = 1, CentroId = 1, MaterialId = 1 } });
            host.InArguments.CentroId = 1;
            host.InArguments.MaterialId = 1;
            host.InArguments.WorkflowId = System.Guid.Empty;
            var resultado = host.TestActivity();

            var corresponde = resultado.First(f => f.Key == "CorrespondeImprimirReciboMunicipal").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(corresponde, Is.EqualTo(true));
        }

        [Test]
        public void TestVerificarCorrespondeImprimirReciboMunicipalFalso()
        {
            srvRepositorio.Setup(s => s.MaterialImprimeReciboMunicipal(It.IsAny<Guid>())).Returns(false);
            srvRepositorio.Setup(s => s.BuscarMaterialesPorCentro(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(new List<MaterialPorCentroDto> { new MaterialPorCentroDto() { Id = 1, CentroId = 1, MaterialId = 1 } });
            host.InArguments.CentroId = 1;
            host.InArguments.MaterialId = 1;
            host.InArguments.WorkflowId = System.Guid.Empty;    

            var resultado = host.TestActivity();

            var corresponde = resultado.First(f => f.Key == "CorrespondeImprimirReciboMunicipal").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(corresponde, Is.EqualTo(false));
        }
    }
}
