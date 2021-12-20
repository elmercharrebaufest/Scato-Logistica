using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ConsultarDatosMailStockTest
    {
        private ConsultarDatosMailStock target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> repositorioMock;
        private MailAvisoStockDto datosMail;

        [SetUp]
        public void SetUp()
        {
            target = new ConsultarDatosMailStock();
            repositorioMock = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(repositorioMock.Object);
        }

        [Test]
        public void ConsultarDatos()
        {
            datosMail = new MailAvisoStockDto()
            {
                CodigoDeEstablecimiento = "14",
                Cosecha = "11-12",
                Error = null,
                Id = 1,
                Localidad = "Labordeboy",
                NombreDeEstablecimiento = "Establecimiento S.A.",
                Provincia = "Santa Fe",
                RazonesSociales = new List<string>() { "Razon1 SA", "Razon2 SA" },
                SeEnviaMail = true,
                StockDeclarado = 50000,
                StockUtilizado = 20000
            };
            repositorioMock.Setup(x => x.ObtenerDatosMailAvisoStock(It.IsAny<Guid>())).Returns(datosMail);

            var resultado = host.TestActivity();
            
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.Values.Contains("Establecimiento S.A."));
            Assert.That(resultado.Values.Contains("11-12"));
            Assert.That(resultado.Values.Contains("Labordeboy"));
            Assert.That(resultado.Values.Contains("Santa Fe"));
            Assert.That(resultado.Values.Contains("50000"));
            Assert.That(resultado.Values.Contains("20000"));
        }
    }
}
