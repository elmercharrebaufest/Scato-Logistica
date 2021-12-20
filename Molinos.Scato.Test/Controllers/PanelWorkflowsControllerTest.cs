using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.ApplicationServer.StoreManagement.Query;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class PanelWorkflowsControllerTest
    {
        private PanelWorkflowsController target;
        private Mock<IListaDeWorkflows> listaWf;
        private Mock<IServicioRepositorio> servRepositorio;
        private NullLogger log;
        private Guid guid;
        private DatosUsuario usuario;
        private FiltroListaDeWorkflowsDto filtro;
        private InstanciaWorkflowDto instanciaDto;
        [SetUp]
        public void SetUp()
        {
            listaWf = new Mock<IListaDeWorkflows>();
            servRepositorio = new Mock<IServicioRepositorio>();
            log = new NullLogger();
            usuario = new DatosUsuario {NombreUsuario = "w", NombrePc = "a", CentroId = 1};
            guid = Guid.NewGuid();
            filtro = new FiltroListaDeWorkflowsDto {NombreUsuario = "w", CentroId = 1, Patente = "aaa111"};
            instanciaDto = new InstanciaWorkflowDto {Id = guid, CentroId = 1, Condicion = InstanceCondition.Idle,Estado = InstanceStatus.Suspended};
            listaWf.Setup(s => s.ListarTotalWorkFlows(It.IsAny<Paginacion>(), It.IsAny<FiltroListaDeWorkflowsDto>()))
                   .Returns(new ListarWorkFlowsDto
                       {
                           InstanciasWorkflowDto =
                               new ListaPaginada<InstanciaWorkflowDto>(
                                new [] {instanciaDto}, 1, 10,
                                1)
                       });
            listaWf.Setup(s => s.ObtenerWorkflow(It.IsAny<Guid>())).Returns(instanciaDto);
            listaWf.Setup(s => s.EliminarInstanciaWorkflow(It.IsAny<Guid>())).Returns(new Resultado());
            listaWf.Setup(s => s.ResumirInstanciaWorkflow(It.IsAny<Guid>())).Returns(new Resultado());
            target = new PanelWorkflowsController(servRepositorio.Object, listaWf.Object,log);


        }

        [Test]
        public void Index()
        {
            var resultado = target.Index(filtro, usuario) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(((FiltroListaDeWorkflowsDto)(resultado.Model)).CentroId,1);
            Assert.AreEqual(((ListaPaginada < InstanciaWorkflowDto > )resultado.ViewBag.Items).Items.FirstOrDefault().Id, guid);
        }

        [Test]
        public void Listar()
        {
            var resultado = target.Listar(usuario,filtro) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(((FiltroListaDeWorkflowsDto)(resultado.Model)).CentroId, 1);
            Assert.AreEqual(((FiltroListaDeWorkflowsDto)(resultado.Model)).Patente, "AAA111");
            Assert.AreEqual(((ListaPaginada<InstanciaWorkflowDto>)resultado.ViewBag.Items).Items.FirstOrDefault().Id, guid);
        }

        [Test]
        public void EliminarOk()
        {
            var resultado = target.EliminarWorkflow(guid, usuario) as ContentResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.Content, "true");
        }

        [Test]
        public void EliminarResultadoError()
        {
            var errorResult = new Resultado();
            errorResult.Error("E","error");

            listaWf.Setup(s => s.EliminarInstanciaWorkflow(It.IsAny<Guid>())).Returns(errorResult);
            var resultado = target.EliminarWorkflow(guid, usuario) as ContentResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.Content, "error");
        }

        [Test]
        public void ResumirOk()
        {
            var resultado = target.ResumirWorkflow(guid, usuario);

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.Data, "El comando para resumir el workflow fue encolado");
        }

        [Test]
        public void ResumirResultadoError()
        {
            var errorResult = new Resultado();
            errorResult.Error("E", "error");

            listaWf.Setup(s => s.ResumirInstanciaWorkflow(It.IsAny<Guid>())).Returns(errorResult);
            var resultado = target.ResumirWorkflow(guid, usuario);

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.Data, "error");
        }

        [Test]
        public void ResumirEstadoIncorrecto()
        {
            instanciaDto.Estado = InstanceStatus.Running;
            
            var resultado = target.ResumirWorkflow(guid, usuario);

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.Data, "El estado actual del workflow no permite que sea resumido");
        }

        [Test]
        public void ListarSeguimientoDeWorkflows()
        {
            servRepositorio.Setup(s => s.ConsultaControlRecorridoLogActividad(It.IsAny<Guid>()))
                           .Returns(new List<ControlRecorridoLogActividadConsultaDto>
                               {
                                   new ControlRecorridoLogActividadConsultaDto
                                       {
                                           Actividad = "A",
                                           Comentario = "C",
                                           Fecha = new DateTime(2015, 6, 6),
                                           Tabla = "ControlRecorrido"
                                       }
                               });

            var result = target.ListarSeguimientoWorkflow(guid) as PartialViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "ListarLogActividadControlRecorrido");
            Assert.AreEqual(((List<ControlRecorridoLogActividadConsultaDto>)(result.ViewBag.LogItems)).FirstOrDefault().Actividad,"A");
        }

    }
}
