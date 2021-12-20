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
    public class VerificacionCamionRechazadoControllerTest
    {
        private VerificacionCamionRechazadoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IVerificacionCamionRechazadoService>> actFactoryMock;
        private Mock<IVerificacionCamionRechazadoService> contractMock;
        private ControlRecorridoDto dto;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IVerificacionCamionRechazadoService>>();
            contractMock = new Mock<IVerificacionCamionRechazadoService>();
            target = new VerificacionCamionRechazadoController(new NullLogger(), actFactoryMock.Object, servRepositorioMock.Object);

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
                PesoBruto = 50000,
                Centro = new CentroDto {},
                Workflow = new WorkflowDto { Codigo = "Workflow 1", Descripcion = "Worfklow desc" },
                BalanzaTaraId = 1
            };

            servRepositorioMock.Setup(s => s.ListarMotivos()).Returns(new List<MotivoDto>());
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorridoDto);
            servRepositorioMock.Setup(s => s.ObtenerPesoMaximo(It.IsAny<TipoVehiculo>(), It.IsAny<int>())).Returns(1);
            var result = target.Index(new Guid(), new DatosUsuario { NombreUsuario = "Usuario" }) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestVerificacionCamionRechazado()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.VerificacionCamionRechazado(It.IsAny<ControlRecorridoDto>(), It.IsAny<Guid>())).Returns(new Resultado());

            var result = target.Index(dto, "EgresoMaterialNoProductivo", 1) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
