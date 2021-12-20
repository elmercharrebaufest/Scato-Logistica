using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class VisteoControllerTest
    {
        private VisteoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IVisteoService>> actFactoryMock;
        private Mock<IVisteoService> contractMock;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IVisteoService>>();
            contractMock = new Mock<IVisteoService>();
            target = new VisteoController(new NullLogger(), actFactoryMock.Object, servRepositorioMock.Object);

            servRepositorioMock.Setup(x => x.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto()
            {
                Material = new MaterialDto { Descripcion = "Material" },
                TipoComercial = new TipoComercialDto() { Descripcion = "TC" },
                Workflow = new WorkflowDto { Codigo = "WF" },
                Patente = "PAT123",
                PesoBruto = 99,
            });
            servRepositorioMock.Setup(x => x.ListarMotivos()).Returns(new List<MotivoDto>());
        }


        [Test]
        public void TestIndex()
        {
            var result = target.Index(new Guid(), new DatosUsuario { NombreUsuario = "Usuario" }) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestVisteo()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.Visteo(It.IsAny<ControlRecorridoDto>(), It.IsAny<Guid>())).Returns(new Resultado());

            var dto = new ControlRecorridoDto();
            var result = target.Index(dto, "EgresoMaterialNoProductivo", 1) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
