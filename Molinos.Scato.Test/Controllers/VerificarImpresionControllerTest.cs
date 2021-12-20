using System;
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
    public class VerificarImpresionControllerTest
    {
        private VerificarImpresionController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IVerificarImpresionService>> actFactoryMock;
        private Mock<IVerificarImpresionService> contractMock;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IVerificarImpresionService>>();
            contractMock = new Mock<IVerificarImpresionService>();
            target = new VerificarImpresionController(new NullLogger(), actFactoryMock.Object, servRepositorioMock.Object);

            servRepositorioMock.Setup(x => x.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto()
            {
                Material = new MaterialDto { Descripcion = "Material" },
                TipoComercial = new TipoComercialDto() { Descripcion = "TC" },
                Workflow = new WorkflowDto { Codigo = "WF" },
                Patente = "PAT123",
                PesoBruto = 99,
            });
        }


        [Test]
        public void TestIndex()
        {
            var result = target.Index(new Guid(), new DatosUsuario { NombreUsuario = "Usuario" }) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void IndexPostOk()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.VerificarImpresion(It.IsAny<ControlRecorridoDto>(), It.IsAny<Guid>())).Returns(new Resultado());

            var dto = new ControlRecorridoDto();
            var result = target.Index(dto, "VerificarImpresion", 1) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
