using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.WebMobile.Controllers;
using Molinos.Scato.WebMobile.Helpers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.ControllersMobile
{
    [TestFixture]
    public class CupoControllerTest
    {
        private CupoController target;
        private Mock<IServicioRepositorio> servicio;
        private Mock<IServicioComandos> servicioComandos;
        private Mock<IConfiguracionProvider> configuracion;
        private  NullLogger log;

        [SetUp]
        public void SetUp()
        {
            servicio = new Mock<IServicioRepositorio>();
            servicioComandos = new Mock<IServicioComandos>();
            configuracion = new Mock<IConfiguracionProvider>();
            log = new NullLogger();
            target = new CupoController(log, servicio.Object, servicioComandos.Object, configuracion.Object);
            configuracion.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "TiemposCuposOtorgados", "5" } });

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
            
            servicio.Setup(x => x.ListarEstadoCupos(5)).Returns(new List<CupoMobileDto>
            {
                new CupoMobileDto { ArribadosDia = 1 }
            });
            var resultado = target.Index() as ViewResult;
            var cupoModel = (resultado.Model as IList<CupoMobileDto>)[0].ArribadosDia;

            Assert.AreEqual(((IList<CupoMobileDto>)resultado.Model).Count, 1);
            Assert.AreEqual(cupoModel, 1);
        }


    }
}
