using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class AutorizarDescuentosEntregadorControllerTest
    {
        private AutorizarDescuentosEntregadorController target;
        private Mock<IServicioActividadFactory<IAutorizarDescuentosEntregadorService>> factory;
        private Mock<IAutorizarDescuentosEntregadorService> contract;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomandos;
        private Guid instanceId;
        private NullLogger log;
        private Mock<IListaDeWorkflows> servworkflows;

        [SetUp]
        public void SetUp()
        {
            factory = new Mock<IServicioActividadFactory<IAutorizarDescuentosEntregadorService>>();
            contract = new Mock<IAutorizarDescuentosEntregadorService>();
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomandos = new Mock<IServicioComandos>();
            log = new NullLogger();
            instanceId = Guid.NewGuid();
            servworkflows = new Mock<IListaDeWorkflows>();
            target = new AutorizarDescuentosEntregadorController(log,servRepositorio.Object, factory.Object, servworkflows.Object);

            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto{InstanciaWorkflow = instanceId});
            servRepositorio.Setup(
                s => s.ListarPaginadoAnalisisYCaladoPorCaracteristica(It.IsAny<Guid>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<AnalisisPorCaracteristicaDto>(
                                   new List<AnalisisPorCaracteristicaDto>
                                       {
                                           new AnalisisPorCaracteristicaDto
                                               {
                                                   AnalisisDeCalidadId = 1,
                                                   Caracteristica = "Car"
                                               }
                                       }, 1, 10, 1));
            servRepositorio.Setup(s => s.ListarMotivos()).Returns(new List<MotivoDto>
                {
                    new MotivoDto
                        {
                            Descripcion =
                                "Motivo"
                        }
                });

            factory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contract.Object);
            contract.Setup(s => s.AutorizarDescuentosEntregador(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(new Resultado());

        }

        [Test]
        public void Index()
        {
            var result = target.Index(instanceId) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(((ListaPaginada<AnalisisPorCaracteristicaDto>)result.ViewBag.Caracteristicas).FirstOrDefault().Caracteristica, "Car");
        }

        [Test]
        public void Listar()
        {
            var result = target.Listar(instanceId) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(((ListaPaginada<AnalisisPorCaracteristicaDto>)result.ViewBag.Caracteristicas).FirstOrDefault().Caracteristica, "Car");
            Assert.AreEqual(result.ViewName, "Listar");
            
        }

        [Test]
        public void AceptarSinErrores()
        {
            var result = target.Aceptar("W", 1,instanceId,new DatosUsuario{NombreUsuario = "w"}) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void AceptarConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error","error");
            contract.Setup(s => s.AutorizarDescuentosEntregador(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(resultado);
                    
            var result = target.Aceptar("W", 1, instanceId, new DatosUsuario {NombreUsuario = "W"}) as RedirectToRouteResult;
            
            Assert.NotNull(result);
            Assert.AreEqual(target.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage, "error");
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["id"], instanceId);
        }

        [Test]
        public void Rechazar()
        {
            var result = target.Rechazar("W", 1, instanceId, new DatosUsuario {NombreUsuario = "w"}) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "_TransportistaRechazado");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
            Assert.AreEqual(result.ViewBag.Workflow, "W");
        }

        [Test]
        public void TransportistaRechazadoSinErrores()
        {
            var result = target.TransportistaRechazado(new ControlRecorridoDto(), "w", 1) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void TransportistaRechazadoConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            contract.Setup(s => s.AutorizarDescuentosEntregador(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(resultado);
            var result = target.TransportistaRechazado(new ControlRecorridoDto{WorkflowInstanceId = instanceId}, "w", 1) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(target.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage, "error");
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["id"], instanceId);

        }

    }
}
