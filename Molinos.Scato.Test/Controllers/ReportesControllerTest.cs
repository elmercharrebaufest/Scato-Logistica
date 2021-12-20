using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class ReportesControllerTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private ReportesController target;
        private Mock<IConfiguracionProvider> configuracion;

        [SetUp]
        public void SetUp()
        {
            configuracion = new Mock<IConfiguracionProvider>();
            servRepositorio = new Mock<IServicioRepositorio>();

            target = new ReportesController(servRepositorio.Object,configuracion.Object);
        }

        [Test]
        public void Index()
        {
            var result = target.Index(new DatosUsuario {CentroId = 1}, "Reporte1") as ViewResult;

            Assert.NotNull(result);
        }
    }
}
