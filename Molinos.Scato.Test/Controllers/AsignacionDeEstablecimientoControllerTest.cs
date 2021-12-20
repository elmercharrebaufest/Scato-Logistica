using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class AsignacionDeEstablecimientoControllerTest
    {
        private AsignacionDeEstablecimientoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IAsignacionDeEstablecimientoService>> actFactoryMock;
        private Mock<IAsignacionDeEstablecimientoService> contractMock;
        private Mock<IListaDeWorkflows> listaMock;
        private NullLogger logger;
        private DatosUsuario datos;
        private AsignacionDeEstablecimientoDto model;
        private RecorridoDto recorrido;
        private Mock<IServicioComandos> comandosMock;
        
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IAsignacionDeEstablecimientoService>>();
            contractMock = new Mock<IAsignacionDeEstablecimientoService>();
            logger = new NullLogger();
            listaMock = new Mock<IListaDeWorkflows>();
            comandosMock = new Mock<IServicioComandos>();
            target = new AsignacionDeEstablecimientoController(logger, actFactoryMock.Object, servRepositorioMock.Object, listaMock.Object, comandosMock.Object);
            datos = new DatosUsuario { CentroDescripcion = "centro 1" };

            recorrido = new RecorridoDto
            {
                PesoBruto = 555,
                PesoTara = 222,
                PesoBrutoFecha = new DateTime(2010, 2, 2),
                PesoTaraFecha = new DateTime(2010, 1, 1),
                Vehiculo = new VehiculoDto { Patente = "AAA111" },
                FechaEgreso = new DateTime(2012, 2, 2),
                Terminado = true,
                WorkflowDefinicionId = 5,
                Workflow = new WorkflowDto()
            };


            model = new AsignacionDeEstablecimientoDto
            {
                EstablecimientoId = 1,
                Workflow = "CodigoBaufest",
                WorkflowDefinicionId = 5
            };
        }


        [Test]
        public void TestIndexPost()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);

            listaMock.Setup(s => s.ObtenerWorkflowPorGuid(It.IsAny<Guid>())).Returns((InstanciaWorkflowDto)null);
            contractMock.Setup(s => s.AsignacionDeEstablecimiento(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<ControlRecorridoDto>(),It.IsAny<bool>())).Returns(new Resultado());
            var result = target.Index(model, datos) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
            contractMock.Verify(v => v.AsignacionDeEstablecimiento(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<bool>()), Times.Once());
        }

        [Test]
        public void TestProcedenciaCartaPorteIgualALocalidadEstablecimiento()
        {
            servRepositorioMock.Setup(s => s.ObtenerCartaPortePorInstanceId(It.IsAny<Guid>()))
                               .Returns(new CartaPorteDto{ProcedenciaId = 1});
            servRepositorioMock.Setup(s => s.ObtenerEstablecimiento(It.IsAny<int>()))
                               .Returns(new EstablecimientoDto {LocalidadId = 1});

            
            var resultado = target.ProcedenciaCartaPorteIgualALocalidadEstablecimiento(Guid.NewGuid().ToString(), "1") as JsonResult;
            Assert.NotNull(resultado);
            Assert.True((bool)resultado.Data);
        }

        [Test]
        public void RechazarTest()
        {
            servRepositorioMock.Setup(s => s.ListarMotivos())
                               .Returns(new List<MotivoDto> {new MotivoDto {Descripcion = "Motivo", Id = 1}});

            datos = new DatosUsuario {NombreUsuario = "w", PuestoDeTrabajoId = 1};

            var result = target.Rechazar("w", 1, Guid.NewGuid(), datos) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "_TransportistaRechazado");
        }

        [Test]
        public void TransportistaRechazadoTest()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.AsignacionDeEstablecimiento(It.IsAny<Guid>(), It.IsAny<int?>(), It.IsAny<ControlRecorridoDto>(),It.IsAny<bool>())).Returns(new Resultado());
            var controlRecorrido = new ControlRecorridoDto{WorkflowInstanceId = Guid.NewGuid()};
            var result = target.TransportistaRechazado("w", 1, controlRecorrido.WorkflowInstanceId, controlRecorrido) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void TransportistaRechazadoErrorTest()
        {

            var resultado = new Resultado();
            resultado.Error("Error","Error");
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.AsignacionDeEstablecimiento(It.IsAny<Guid>(), It.IsAny<int?>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<bool>())).Returns(resultado);
            var controlRecorrido = new ControlRecorridoDto { WorkflowInstanceId = Guid.NewGuid() };
            var result = target.TransportistaRechazado("w", 1, controlRecorrido.WorkflowInstanceId, controlRecorrido) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["id"], controlRecorrido.WorkflowInstanceId);
        }

    }
}
