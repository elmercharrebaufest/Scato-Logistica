using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.WebMobile.Controllers;
using Moq;
using Ninject.Extensions.Logging;
using NUnit.Framework;

namespace Molinos.Scato.Test.ControllersMobile
{
    [TestFixture]
    public class CamionesRechazadosControllerTest
    {
        private Mock<IServicioRepositorio> servicio;
        private ILogger log;
        private CamionesRechazadosController controller;
        [SetUp]
        public void SetUp()
        {
            servicio = new Mock<IServicioRepositorio>();
            log = new NullLogger();
            controller = new CamionesRechazadosController(log, servicio.Object);
        }

        [Test]
        public void IndexTest()
        {
            var claimCollection = new List<Claim>
            {
                new Claim("CentroId", "5")
            };
            var identity = new GenericIdentity("TestRun");
            identity.AddClaims(claimCollection);
            var principal = new GenericPrincipal(identity, null);
            Thread.CurrentPrincipal = principal;
            
            var items = new List<InstanciaWorkflowDto>(){new InstanciaWorkflowDto()};
            servicio.Setup(x => x.ListarRecorridosRechazados(It.Is<FiltroListaDeWorkflowsDto>(y => y.CentroId == 5), It.IsAny<Paginacion>())).Returns(new ListaPaginada<InstanciaWorkflowDto>(items,1,1000,1));

            var resultado = controller.Index() as ViewResult;

            Assert.AreEqual(((IList<InstanciaWorkflowDto>)resultado.ViewBag.Items).Count, 1);
        }

        [Test]
        public void FotosCamiones()
        {
            var id = Guid.NewGuid();
            servicio.Setup(x => x.ListarFotosCamion(It.Is<Guid>(y => y == id)  ,It.IsAny<String>())).Returns(new FotosDto(){Patente = "AAA1111"});

            var resultado = controller.FotosCamiones(id) as ViewResult;

            Assert.AreEqual((resultado.Model as FotosDto).Patente, "AAA1111");
        }
    }
}
