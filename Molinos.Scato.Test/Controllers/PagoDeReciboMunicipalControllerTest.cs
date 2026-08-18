using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Firmware;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    class PagoDeReciboMunicipalControllerTest
    {
        private Mock<IServicioRepositorio> servicioRepo;
        private Mock<IServicioComandos> comando;
        private Mock<IServicioActividadFactory<IEjecutarService>> factory;
        private Mock<IListaDeWorkflows> workflows;
        private Mock<IEjecutarService> ejecutarMock;
        private Mock<IRecorridoWorkflow> recorridoWorkflow;
        private PagoDeReciboMunicipalController target;



        [SetUp]
        public void SetUp()
        {
            servicioRepo = new Mock<IServicioRepositorio>();
            comando = new Mock<IServicioComandos>();
            factory = new Mock<IServicioActividadFactory<IEjecutarService>>();
            workflows = new Mock<IListaDeWorkflows>();
            ejecutarMock = new Mock<IEjecutarService>();
            recorridoWorkflow = new Mock<IRecorridoWorkflow>();
            target = new PagoDeReciboMunicipalController(servicioRepo.Object, new NullLogger(), comando.Object, factory.Object, workflows.Object,recorridoWorkflow.Object);



            servicioRepo.Setup(r => r.ObtenerDatosRecorridoActivo(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new DatosRecorridoDto { InstanciaWorkflow = Guid.NewGuid() });

            workflows.Setup(w => w.ObtenerWorkflowProximaAccion(It.IsAny<Guid>()))
                .Returns(new ProximaAccionDto { Mensaje = "", ProximaAccion = "EnPlayaExterna" });

            ejecutarMock.Setup(x => x.Ejecutar(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>()))
                .Returns(new Resultado { });

            factory.Setup(f => f.CrearServicio(It.IsAny<int>())).Returns(ejecutarMock.Object);

            servicioRepo.Setup(s => s.ValidarProximaActividadPorPuestoSinPatente(It.IsAny<DatosRecorridoDto>(), It.IsAny<string>(), It.IsAny<IList<PuestoDeTrabajoDto>>()))
                .Returns(new ValidarProximaAccionDto { ProximaActividad = "", InstanceId = Guid.NewGuid(), PuestoDeTrabajoId = 1 });

        }

        [Test]
        public void Index()
        {
            servicioRepo.Setup(s => s.ListarPuestosDeTrabajoPorNombrePc(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, PidePatente = true, NombrePuesto= "p1" } });

            var result = target.Index(new DatosUsuario { CentroId = 1 }) as ViewResult;

            Assert.NotNull(result.ViewBag);
            Assert.NotNull(result.ViewData.Values);
            Assert.AreEqual(result.ViewData.Values.Count, 4);
            Assert.AreEqual(result.ViewData.Values.ElementAt(1).ToString(), "1");
            Assert.AreEqual(result.ViewData.Values.ElementAt(0).ToString(), "1");
        }

        //Revisar posible activacion
        //[Test]
        //public void PagarConMercadoPagoOk()
        //{
        //    var datosDePago = new ValoresPagarConMercadoPagoDto { Monto = 1, PuestoDeTrabajoId = 1, RecorridoId = 1, NumeroDeTarjeta = "1", Token = "1" };
        //    servicioRepo.Setup(s => s.ValidarProximaActividadPorPuestoSinPatente(It.IsAny<DatosRecorridoDto>(), It.IsAny<string>(), It.IsAny<IList<PuestoDeTrabajoDto>>()))
        //       .Returns(new ValidarProximaAccionDto { ProximaActividad = "", Valida = true, InstanceId = Guid.NewGuid(), PuestoDeTrabajoId = 1 });

        //    comando.Setup(c => c.Ejecutar(It.IsAny<Comando>()))
        //        .Returns(new ResultadoPagarMercadoPago { DetalleDePago = new EstadoPagoDto { } });

        //    var result = target.PagarConMercadoPago(datosDePago) as JsonResult;

        //    Assert.NotNull(result);
        //    Assert.NotNull(result.Data);
        //    comando.Verify(c => c.Ejecutar(It.IsAny<Comando>()), Times.Once());

        //}

        //Revisar posible activacion
        //[Test]
        //public void PagarConEfectivoOk()
        //{
        //    var datosDePago = new ValoresPagarConMercadoPagoDto { Monto = 1, PuestoDeTrabajoId = 1, RecorridoId = 1, NumeroDeTarjeta = "1", Token = "1" };

        //    servicioRepo.Setup(s => s.ObtenerPagoConMercadoPagoPorRecorridoId(It.IsAny<int>()))
        //        .Returns((PagoConMercadoPagoDto)null);

        //    var result = target.PagarConEfectivo(datosDePago) as JsonResult;

        //    Assert.NotNull(result);
        //    Assert.NotNull(result.Data);
        //}
        //[Test]
        //public void ObtenerDatosOk()
        //{
        //    servicioRepo.Setup(s => s.ObtenerRecorridoImpresionReciboMunicipalPorTarjeta(It.IsAny<string>()))
        //        .Returns(new ImpresionReciboMunicipalRecorridoDto { });

        //    var result = target.ObtenerDatos("") as JsonResult;

        //    Assert.NotNull(result);
        //    Assert.NotNull(result.Data);
        //    servicioRepo.Verify(s => s.ObtenerRecorridoImpresionReciboMunicipalPorTarjeta(It.IsAny<string>()), Times.Once());
        //}

        //Revisar posible activacion
        //[Test]
        //public void AutorizarMercadoPagoOk()
        //{
        //    comando.Setup(x => x.Ejecutar(It.IsAny<AutorizarMercadoPago>()))
        //        .Returns(new ResultadoCrear { Id = 1 });

        //    var result = target.AutorizarMercadoPago("CodigoTest") as ViewResult;

        //    Assert.Null(result);
        //    comando.Verify(x => x.Ejecutar(It.IsAny<AutorizarMercadoPago>()), Times.Once());
        //}

    }
}
