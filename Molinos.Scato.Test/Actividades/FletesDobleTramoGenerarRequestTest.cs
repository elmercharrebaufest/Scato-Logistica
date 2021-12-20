using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class FletesDobleTramoGenerarRequestTest
    {
        private FletesDobleTramoGenerarRequest target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> srvComando;

        [SetUp]
        public void SetUp()
        {
            target = new FletesDobleTramoGenerarRequest();
            srvRepositorio = new Mock<IServicioRepositorio>();
            srvComando = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(srvComando.Object);
        }

        [Test]
        public void TestGenerarRequest()
        {
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto { Id = 1, Descripcion = "Chivilcoy", CodigoSAP = "1111"});

            host.InArguments.CartaPorte = new CartaPorteDto
                {
                    TitularCartaPorteCodigoSap ="123", 
                    Chofer = new ChoferDto { Nombre = "Raul", Apellido = "Perez" },
                    TipoComercialSentido = "1",
                    FechaEmision = DateTime.Now,
                    KmRecorrer = 100,
                    MaterialCodigoSap = "3333333",
                    NroCartaPorte = "000512345678",
                    ProcedenciaCodigoSap = "444",
                    ProvinciaCodigoSap = "EF21",
                    TransportistaCUIT = "30-60346323-4"
                };
            host.InArguments.Vehiculo = new VehiculoDto { Patente = "LAS123", NumeroVehiculo = 1 };
            host.InArguments.AlmacenId = 1;
            host.InArguments.FechaEgreso = DateTime.Now;
            host.InArguments.PesoNeto = 15000;
            host.InArguments.CentroId = 1;

            var resultado = host.TestActivity();

            var resultadoServicio = resultado.First(f => f.Key == "Resultado").Value as Resultado;
            var requestServicio = resultado.First(f => f.Key == "Request").Value as FletesDobleTramoRequest;

            Assert.IsNotNull(resultadoServicio);
            Assert.IsNotNull(requestServicio);
            Assert.IsFalse(resultadoServicio.HayErrores);
        }
    }
}
