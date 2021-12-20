using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
    public class AsignarTicketMunicipalControllerTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servComandos;
        private NullLogger log;
        private AsignarTicketMunicipalController target;
        private DatosUsuario usuario;
        private LogExceptuadosTicketMunicipalDto dto;
        private Guid instanceId;

        [SetUp]
        public void SetUp()
        {
            servComandos = new Mock<IServicioComandos>();
            servRepositorio = new Mock<IServicioRepositorio>();
            servRepositorio.Setup(
                s =>
                s.ListarRecorridosEnPlayaExternaPorCentro(It.IsAny<FiltroListaDeWorkflowsDto>(), It.IsAny<Paginacion>()))
                           .Returns(new DatosDeWorkflowsDto
                               {
                                   Workflows =
                                       new ListaPaginada<InstanciaWorkflowDto>(
                                        new List<InstanciaWorkflowDto>
                                            {
                                                new InstanciaWorkflowDto
                                                    {
                                                        Id = instanceId,
                                                        CentroId = 1,
                                                        ChoferNombre = "A",
                                                        MaterialId = 1
                                                    }
                                            }, 1, 1, 1),
                                   WorkflowsCentro =
                                       new List<WorkflowDto>
                                           {
                                               new WorkflowDto
                                                   {
                                                       CentroId = 1,
                                                       Activo = true,
                                                       Descripcion = "Workflow",
                                                       Codigo = "W"
                                                   }
                                           }
                               });

            servRepositorio.Setup(s => s.ListarTiposComercialesPorCentro(It.IsAny<int>()))
                           .Returns(new List<TipoComercialDto>
                               {
                                   new TipoComercialDto {Descripcion = "TipoComercial", Id = 1}
                               });

            servComandos.Setup(s => s.Ejecutar(It.IsAny<ModificarRecorridoPagaTicketMunicipal>()))
                        .Returns(new Resultado());
            log = new NullLogger();
            target = new AsignarTicketMunicipalController(servRepositorio.Object, log, servComandos.Object);
            usuario = new DatosUsuario {NombreUsuario = "w"};
            instanceId = Guid.NewGuid();
            dto = new LogExceptuadosTicketMunicipalDto {InstanceId = instanceId};
        }

        [Test]
        public void Index()
        {
            var result = target.Index(usuario, new FiltroListaDeWorkflowsDto()) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(((FiltroListaDeWorkflowsDto) result.Model).NombreUsuario, "w");
        }

        [Test]
        public void Listar()
        {
            var result = target.Listar(usuario, new FiltroListaDeWorkflowsDto {Patente = "aaa111"}) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(((FiltroListaDeWorkflowsDto) result.Model).Patente, "AAA111");
        }

        [Test]
        public void ModificarGet()
        {
            var result = target.Modificar(instanceId, true) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName,"_CrearModificar");
        }

        [Test]
        public void ModificarPostValidModel()
        {
            var result = target.Modificar(dto, usuario) as ContentResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Content, "ajax-edit-success");

        }

        [Test]
        public void ModificarPostNotValidModel()
        {
            var error = new Resultado();
            error.Error("Error","error");
            servComandos.Setup(s => s.Ejecutar(It.IsAny<ModificarRecorridoPagaTicketMunicipal>())).Returns(error);
            var result = target.Modificar(dto, usuario) as ViewResult;

            Assert.NotNull(result);
            ModelState errorModel;
            target.ModelState.TryGetValue("Error", out errorModel);
            Assert.NotNull(errorModel);
            Assert.AreEqual(errorModel.Errors.FirstOrDefault().ErrorMessage, "error");
            Assert.AreEqual(result.ViewName,"");

        }
    }
}
