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
    public class ConfirmacionDeCargaDescargaControllerTest
    {
        private ConfirmacionDeCargaDescargaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IEjecutarService>> actFactoryMock;
        private Mock<IEjecutarService> contractMock;
        private Mock<IServicioComandos> servComandosMock;
        private ControlRecorridoDto dto;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IEjecutarService>>();
            contractMock = new Mock<IEjecutarService>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ConfirmacionDeCargaDescargaController(new NullLogger(), actFactoryMock.Object, servRepositorioMock.Object, servComandosMock.Object);

            dto = new ControlRecorridoDto
            {
                Id = 1,
                WorkflowInstanceId = new Guid(),
                NombreUsuario = "Usuario",
                Decision = false,
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
                Workflow = new WorkflowDto { Codigo = "Workflow 1"},
                Material = new MaterialDto { Descripcion = "MaterialDesc 1" },
                TipoComercial = new TipoComercialDto { Descripcion = "Tipo Comercial 1", PesoEsperado = 100 },
                Almacen = new AlmacenDto{Descripcion = "Almacen 1"}
            };

            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorridoDto);

            var result = target.Index(new Guid(), new DatosUsuario { NombreUsuario = "Usuario" }) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestConfirmacionDeCargaDescarga()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.Ejecutar(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>())).Returns(new Resultado());

            var result = target.Index(dto, "EgresoPorRedespachoDeGranos",1) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
