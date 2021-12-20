using System;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class DocumentoDeOrigenControllerTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private DocumentoDeOrigenController target;
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            log = new NullLogger();
            target = new DocumentoDeOrigenController(log,servRepositorio.Object);
        }

        [Test]
        public void Index()
        {
            var result =
                target.Index(
                    new DocumentoDeOrigenDto
                        {
                            Actividad = "DocumentoDeOrigen",
                            TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                            EsActividad = true,
                            InstanceId = Guid.NewGuid(),
                            RecorridoId = 1
                        }, "1234", "AAA111", true) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewBag.NroDocumento, "1234");
            Assert.AreEqual(result.ViewBag.Patente, "AAA111");
            Assert.AreEqual(result.ViewBag.EsActividad, true);
        }
    }
}
