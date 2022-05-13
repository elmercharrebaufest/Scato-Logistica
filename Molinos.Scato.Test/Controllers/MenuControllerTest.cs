using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class MenuControllerTest
    {
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioOrquestador> servOrquestador;
        private Mock<IServicioEstadoPuesto> servEstadoPuesto;
        private MenuController target;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            servOrquestador = new Mock<IServicioOrquestador>();
            servEstadoPuesto = new Mock<IServicioEstadoPuesto>();
            target = new MenuController(new NullLogger(), servRepositorioMock.Object, servComandosMock.Object, servOrquestador.Object, servEstadoPuesto.Object);
        }


        [Test]
        public void MenuTest()
        {
            var workflows = new List<WorkflowInfoDto>
                {
                    new WorkflowInfoDto {Codigo = "C1", Descripcion = "D1", ActividadInicial = "A1"},
                    new WorkflowInfoDto {Codigo = "C2", Descripcion = "D2", ActividadInicial = "A2"}
                };
            servRepositorioMock.Setup(s => s.ListarWorkflowsPorUsuarioYCentro(It.IsAny<string>(), It.IsAny<int>())).Returns(workflows);
            servRepositorioMock.Setup(s => s.ObtenerPuestosIdPorPC(It.IsAny<string>())).Returns(new List<int> {1});
            var datosUsuario = new DatosUsuario();
            var result = target.Menu(datosUsuario) as PartialViewResult;
            IEnumerable<WorkflowInfoDto> results = target.ViewBag.Workflows;
            Assert.That(result.ViewName, Is.EqualTo("_Menu"));
            Assert.That(results.Select(x => x.ActividadInicial), Is.EquivalentTo(new List<string> { "A1", "A2" }));
            Assert.That((bool)target.ViewBag.Mantenimiento, Is.False);
        }

    }
}
