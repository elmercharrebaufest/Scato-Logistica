using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class RemitoACCPPTest
    {
        private RemitoACCPP target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new RemitoACCPP();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);           
        }

        [Test]
        public void RemitoACCPP()
        {
            var chofer = new ChoferDto
            {
                Id = 1,
                Cuil = "20-11435231-4",
                Apellido = "Garcia",
                Nombre = "Jose",
                NumeroDeDocumento = "11435231"
            };
            var remito = new RemitoDto
            {
                AcuerdoMarco = "11234",
                Chofer = chofer,
                MaterialId = 22,
                Remito = "453445671234",
                OrigenCodigoSap = "43512",
                KmRecorrer = 566,
                MaterialCodigoSap = "200134",
                ProcedenciaCodigoSap = "33",
                ProvinciaCodigoSap = "44",
                TipoComercialCodigoSap = "123",
                TransportistaCuit = "30-29093301-7",
                Cosecha = "15-16",
                CodEstab = "99210"
            };
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto{Id = 11,CodigoSAP = "44534"});
            host.InArguments.Remito = remito;
            host.InArguments.CentroId = 5;
            host.InArguments.FechaInicio = new DateTime(2015,8,11,11,23,54);
            host.InArguments.TipoVehiculo = TipoVehiculo.Camión;

            var resultado = host.TestActivity();

            var cartaPorte = host.OutArguments.CartaPorte as CartaPorteDto;
            Assert.That(resultado, Is.Not.Null);
            srvRepositorio.Verify(p => p.ObtenerCentro(It.IsAny<int>()), Times.Exactly(1));

            Assert.That(cartaPorte.AcuerdoMarco, Is.EqualTo(remito.AcuerdoMarco));
            Assert.That(cartaPorte.Chofer, Is.EqualTo(remito.Chofer));
            Assert.That(cartaPorte.MaterialId, Is.EqualTo(remito.MaterialId));
            Assert.That(cartaPorte.NroCartaPorte, Is.EqualTo(remito.Remito));
            Assert.That(cartaPorte.TitularCartaPorteCodigoSap, Is.EqualTo(remito.OrigenCodigoSap));
            Assert.That(cartaPorte.KmRecorrer, Is.EqualTo(remito.KmRecorrer));
            Assert.That(cartaPorte.MaterialCodigoSap, Is.EqualTo(remito.MaterialCodigoSap));
            Assert.That(cartaPorte.ProcedenciaCodigoSap, Is.EqualTo(remito.ProcedenciaCodigoSap));
            Assert.That(cartaPorte.ProvinciaCodigoSap, Is.EqualTo(remito.ProvinciaCodigoSap));
            Assert.That(cartaPorte.TipoComercialCodigoSap, Is.EqualTo(remito.TipoComercialCodigoSap));
            Assert.That(cartaPorte.TransportistaCUIT, Is.EqualTo(remito.TransportistaCuit));
            Assert.That(cartaPorte.Cosecha, Is.EqualTo(remito.Cosecha));
            Assert.That(cartaPorte.CodEstab, Is.EqualTo(remito.CodEstab));



        }
    }
}
