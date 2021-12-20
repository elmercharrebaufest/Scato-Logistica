using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
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
    public class ControlDePesoEsperadoControllerTest
    {
        private ControlDePesoEsperadoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IControlDePesoEsperadoService>> actFactoryMock;
        private Mock<IControlDePesoEsperadoService> contractMock;
        private ControlRecorridoDto dto;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IControlDePesoEsperadoService>>();
            contractMock = new Mock<IControlDePesoEsperadoService>();
            target = new ControlDePesoEsperadoController(new NullLogger(), actFactoryMock.Object, servRepositorioMock.Object);

            dto = new ControlRecorridoDto
            {
                Id = 1,
                WorkflowInstanceId = new Guid(),
                NombreUsuario = "Usuario",
                Decision = false,
                Mensaje = "Mensaje"
            };
        }


        [Test]
        public void TestIndex()
        {
            var recorridoDto = new RecorridoDto
            {
                Patente = "AAA111",
                TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                NumeroDocumentoIngreso = "123456789",
                Workflow = new WorkflowDto { Codigo = "Workflow 1", Descripcion = "Worfklow desc"},
                Material = new MaterialDto{Descripcion = "MaterialDesc 1"},
                TipoComercial = new TipoComercialDto{Descripcion = "Tipo Comercial 1", PesoEsperado = 100},
                Transportista = new TransportistaDto{RazonSocial = "Transportista SRL"}
            };

            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorridoDto);

            var result = target.Index(new Guid(), new DatosUsuario{NombreUsuario = "Usuario"}) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestControlDePesoEsperado()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.ControlDePesoEsperado(It.IsAny<ControlRecorridoDto>(), It.IsAny<Guid>())).Returns(new Resultado());

            var result = target.Index(dto, "EgresoMaterialNoProductivo", 1) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
