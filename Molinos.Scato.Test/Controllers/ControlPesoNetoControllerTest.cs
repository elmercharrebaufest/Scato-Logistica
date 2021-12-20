using System;
using System.Collections.Generic;
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
    public class ControlPesoNetoControllerTest
    {
        private ControlPesoNetoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IControlPesoNetoService>> actFactoryMock;
        private Mock<IControlPesoNetoService> contractMock;
        private ControlRecorridoDto dto;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IControlPesoNetoService>>();
            contractMock = new Mock<IControlPesoNetoService>();
            target = new ControlPesoNetoController(new NullLogger(), actFactoryMock.Object, servRepositorioMock.Object);

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
                TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                NumeroDocumentoIngreso = "123456789",
                Workflow = new WorkflowDto { Codigo = "Workflow 1", Descripcion = "Worfklow desc" },
                Material = new MaterialDto { Descripcion = "Material 1" },
                TipoComercial = new TipoComercialDto { Descripcion = "Tipo Comercial 1", PesoEsperado = 100 },
                PesoBruto = 40500,
                PesoBrutoOrigen = 45000
            };

            servRepositorioMock.Setup(s => s.ListarMotivos()).Returns(new List<MotivoDto>());
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorridoDto);
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto { Id = 10 });

            var result = target.Index(new Guid(), new DatosUsuario { NombreUsuario = "Usuario" }) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestControlPesoNetoRomaneo()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.ControlPesoNeto(It.IsAny<ControlRecorridoDto>(), It.IsAny<Guid>())).Returns(new Resultado());

            var result = target.Index(dto, "EgresoMaterialNoProductivo", 1) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
