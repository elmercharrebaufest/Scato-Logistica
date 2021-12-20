using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
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
    public class AsignacionTarjetaDeAccesoControllerTest
    {
        private AsignacionTarjetaDeAccesoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IAsignacionTarjetaDeAccesoService>> actFactoryMock;
        private Mock<IAsignacionTarjetaDeAccesoService> contractMock;
        private Mock<IListaDeWorkflows> listaMock;
        private NullLogger logger;
        private DatosUsuario datos;
        private AsignacionTarjetaDeAccessoModel model;
        private RecorridoDto recorrido;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IAsignacionTarjetaDeAccesoService>>();
            contractMock = new Mock<IAsignacionTarjetaDeAccesoService>();
            logger = new NullLogger();
            listaMock = new Mock<IListaDeWorkflows>();
            target = new AsignacionTarjetaDeAccesoController(logger, actFactoryMock.Object, servRepositorioMock.Object, listaMock.Object);
            datos = new DatosUsuario { CentroDescripcion = "centro 1", NombrePc = "a" , CentroId = 5 };
            


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


            model = new AsignacionTarjetaDeAccessoModel
            {
                Numero = "100000",
                Workflow = "CodigoBaufest",
                WorkflowDefinicionId = 5
            };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>())).Returns(new DatosDeInstanciaDto{WorkflowDefinicionId = 5});
            servRepositorioMock.Setup(x => x.ListarPuestosDeTrabajoPorNombrePc("a", 5)).Returns(new List<PuestoDeTrabajoDto>());
            servRepositorioMock.Setup(x => x.ObtenerFotoCPDeCartaDePortePorrecorrido(It.IsAny<Guid>())).Returns(new FotosDto { Fotos = new List<FotoDto>() });
            var result = target.Index(It.IsAny<Guid>(), new DatosUsuario { NombrePc = "a", CentroId = 5 }) as ViewResult;
            var model = (AsignacionTarjetaDeAccessoModel)result.Model;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(model.WorkflowDefinicionId, Is.EqualTo(5));
        }

        [Test]
        public void TestIndexPost()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);

            servRepositorioMock.Setup(s => s.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), It.IsAny<int>())).Returns(new Guid());

            listaMock.Setup(s => s.ObtenerWorkflowPorGuid(It.IsAny<Guid>())).Returns((InstanciaWorkflowDto)null);
            contractMock.Setup(s => s.AsignacionTarjetaDeAcceso(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>())).Returns(new Resultado());
            var result = target.Index(model, datos) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
            contractMock.Verify(v => v.AsignacionTarjetaDeAcceso(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>()), Times.Once());
        }

        [Test]
        public void TestIndexTarjetaBloqueada()
        {
            servRepositorioMock.Setup(x => x.ObtenerFotoCPDeCartaDePortePorrecorrido(It.IsAny<Guid>())).Returns(new FotosDto { Fotos = new List<FotoDto>() });
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(x => x.ListarPuestosDeTrabajoPorNombrePc("a", 5)).Returns(new List<PuestoDeTrabajoDto>());
            var result = target.Index(model, datos) as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.AsignacionTarjetaDeAcceso_TarjetaBloqueada));
        }
        [Test]
        public void TestIndexTarjetaRangoInvalido()
        {
            servRepositorioMock.Setup(x => x.ObtenerFotoCPDeCartaDePortePorrecorrido(It.IsAny<Guid>())).Returns(new FotosDto { Fotos = new List<FotoDto>() });
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(x => x.ListarPuestosDeTrabajoPorNombrePc("a", 5)).Returns(new List<PuestoDeTrabajoDto>());
            var result = target.Index(model, datos) as ViewResult;
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.AsignacionTarjetaDeAcceso_TarjetaSinRango));
        }
        [Test]
        public void TestIndexTarjetaEnUso()
        {
            servRepositorioMock.Setup(x => x.ObtenerFotoCPDeCartaDePortePorrecorrido(It.IsAny<Guid>())).Returns(new FotosDto { Fotos = new List<FotoDto>() });
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(x => x.ListarPuestosDeTrabajoPorNombrePc("a", 5)).Returns(new List<PuestoDeTrabajoDto>());
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            listaMock.Setup(s => s.VerificarExistenciaDeWorkflowPorGuid(It.IsAny<Guid>())).Returns(true);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), It.IsAny<int>())).Returns(new Guid());
            var result = target.Index(model, datos) as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.ImpresionTarjetaDeAcceso_EnUso));
        }
    }
}
