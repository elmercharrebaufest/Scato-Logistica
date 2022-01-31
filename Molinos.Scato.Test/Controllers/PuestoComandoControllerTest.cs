using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Web.Seguridad;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class PuestoComandoControllerTest
    {
        private PuestoComandoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IListaDeWorkflows> workflowsMock;
        private Mock<IServicioActividadFactory<IPuestoComandoService>> actFactoryMock;
        private Mock<IServicioActividadFactory<IEjecutarService>> actFactoryEjMock;
        private Mock<IServicioActividadFactory<IPesadaService>> actFactoryPeMock;
        private DatosDeWorkflowsDto datosWorkflow;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            workflowsMock = new Mock<IListaDeWorkflows>();
            actFactoryMock = new Mock<IServicioActividadFactory<IPuestoComandoService>>();
            actFactoryEjMock = new Mock<IServicioActividadFactory<IEjecutarService>>();
            actFactoryPeMock = new Mock<IServicioActividadFactory<IPesadaService>>();

            target = new PuestoComandoController(new NullLogger(), workflowsMock.Object, servRepositorioMock.Object, 
                servComandosMock.Object, actFactoryMock.Object, actFactoryEjMock.Object, actFactoryPeMock.Object);

            var items = new List<InstanciaWorkflowDto>
                {
                    new InstanciaWorkflowDto{Id = new Guid("5573C344-BF0A-4D6E-AE5A-902FA6C32CF5")}
                };
            datosWorkflow = new DatosDeWorkflowsDto
            {
                Workflows = new ListaPaginada<InstanciaWorkflowDto>(items, 1, 1, 1),
                Calidades = new List<CalidadMaterialDto>(),
                WorkflowsCentro = new List<WorkflowDto>()
            };

            servRepositorioMock.Setup(s => s.ListarCalles(It.IsAny<int>())).Returns(new List<CalleDto>());
            servRepositorioMock.Setup(s => s.ListarHidraulicas(It.IsAny<int>(), It.IsAny<bool>())).Returns(new List<PuestosDeCargaDescargaDto>());
            servRepositorioMock.Setup(s => s.ListarBalanzasActivas(It.IsAny<int>(), TipoVehiculo.Camión)).Returns(new List<BalanzaDto>());
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorMaterialYCentro(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(new List<AlmacenDto>());
            servRepositorioMock.Setup(s => s.ListarTiposComercialesPorCentro(It.IsAny<int>())).Returns(new List<TipoComercialDto>());

            servRepositorioMock.Setup(s => s.ListarWorkFlows(It.IsAny<Paginacion>(), It.IsAny<FiltroListaDeWorkflowsDto>()))
                .Returns(datosWorkflow);
            servRepositorioMock.Setup(s => s.ListarCaracteristicasDeCalidadPorConfiguracion(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new List<CaracteristicaConfiguracionDeTablaDto>());
            workflowsMock.Setup(x => x.ObtenerWorkflowProximasAcciones(It.IsAny<string>(), It.IsAny<int>())).Returns(new List<string>());
        }

        [Test]
        public void TestIndexConMaterialId()
        {
            var filtro = new FiltroListaDeWorkflowsDto
            {
                MaterialId = 1
            };

            var result = target.Index(new DatosUsuario(), filtro) as ViewResult;
            IEnumerable<InstanciaWorkflowDto> results = target.ViewBag.Items;

            servRepositorioMock.Verify(p => p.ListarWorkFlows(It.IsAny<Paginacion>(), It.IsAny<FiltroListaDeWorkflowsDto>()), Times.Exactly(1));
            servRepositorioMock.Verify(p => p.ListarCaracteristicaConfiguracionDeTabla(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<Guid> { new Guid("5573C344-BF0A-4D6E-AE5A-902FA6C32CF5") }));
        }

        [Test]
        public void TestIndexSinMaterialId()
        {
            var filtro = new FiltroListaDeWorkflowsDto
            {
                MaterialId = null
            };

            var result = target.Index(new DatosUsuario(), filtro) as ViewResult;
            IEnumerable<InstanciaWorkflowDto> results = target.ViewBag.Items;

            servRepositorioMock.Verify(p => p.ListarWorkFlows(It.IsAny<Paginacion>(), It.IsAny<FiltroListaDeWorkflowsDto>()), Times.Exactly(1));
            servRepositorioMock.Verify(p => p.ListarCaracteristicasDeCalidadPorConfiguracion(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Exactly(0));
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<Guid> { new Guid("5573C344-BF0A-4D6E-AE5A-902FA6C32CF5") }));
        }

        [Test]
        public void TestAsignar()
        {
            var resultado = new ResultadoPuestoComando();
            resultado.Workflows = new List<DatosDeWorkflowDto>();
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ActualizarPuestocomando>())).Returns(resultado);
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ActualizarPuestoComandoCaladoEnPlanta>())).Returns(resultado);

            var asignacion = new AsignacionDto
            {
                AlmacenId = 1,
                BalanzaBrutoId = 2,
                BalanzaTaraId = 3,
                CalleId = 4,
                HidraulicasId = new int[] { 5 },
                InstanceIds = "5573C344-BF0A-4D6E-AE5A-902FA6C32CF5",
                MaterialId = 1
            };

            var result = target.Asignar(asignacion, true, new DatosUsuario()) as ContentResult;


            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestAsignarError()
        {
            var resultado = new ResultadoPuestoComando();
            resultado.Error("Error", "error");
            resultado.Workflows = new List<DatosDeWorkflowDto>();
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ActualizarPuestocomando>())).Returns(resultado);
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ActualizarPuestoComandoCaladoEnPlanta>())).Returns(resultado);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorMaterialYCentroSustentableMixto(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<AlmacenDto> { new AlmacenDto { Id = 1 } });
            servRepositorioMock.Setup(s => s.ListarHidraulicasPorCriterioSustentable(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(new List<PuestosDeCargaDescargaDto> { new PuestosDeCargaDescargaDto { Id = 1 } });

            var asignacion = new AsignacionDto
            {
                AlmacenId = 1,
                BalanzaBrutoId = 2,
                BalanzaTaraId = 3,
                CalleId = 4,
                HidraulicasId = new int[] { 5 },
                InstanceIds = "5573C344-BF0A-4D6E-AE5A-902FA6C32CF5",
                MaterialId = 1,
                SustentableMixto = true,
                SonSustentables = true
            };

            var result = target.Asignar(asignacion, true, new DatosUsuario()) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }
    }
}
