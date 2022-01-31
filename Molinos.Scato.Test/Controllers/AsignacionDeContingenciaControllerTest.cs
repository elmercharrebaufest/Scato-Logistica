using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class AsignacionDeContingenciaControllerTest
    {
        private AsignacionDeContingenciaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private readonly NullLogger logger;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            target = new AsignacionDeContingenciaController(logger, servRepositorioMock.Object, servComandosMock.Object, orquestadorMock.Object);

            servRepositorioMock.Setup(x => x.ObtenerUsuariosContingencia()).Returns(new List<string>());
        }

        [Test]
        public void TestIndex()
        {
            var lista = new ListaPaginada<PuestoDeTrabajoContingenciaDto>(new List<PuestoDeTrabajoContingenciaDto>() { new PuestoDeTrabajoContingenciaDto { 
            Id=1
            } }, 1, 1, 1);
            servRepositorioMock.Setup(s => s.ListarPaginadoPuestosDeTrabajoContingencia(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(lista);
            servRepositorioMock.Setup(s => s.ListarNirsPorCentro(It.IsAny<int>()))
                .Returns(new List<NirsDto>());
            servRepositorioMock.Setup(s => s.ListarHumedimetrosPorCentro(It.IsAny<int>()))
               .Returns(new List<HumedimetroDto>());
            servRepositorioMock.Setup(s => s.ListarTodasLasBalanzasActivas(It.IsAny<int>()))
               .Returns(new List<BalanzaDto>());
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
               .Returns(new CentroDto());
            servRepositorioMock.Setup(s => s.ImprimeTicketSalida(It.IsAny<int>()))
               .Returns(true);
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(It.IsAny<int>()))
               .Returns(new List<PuestoDeTrabajoDto>() { new PuestoDeTrabajoDto {Id = 1, NombrePuesto = "Ingreso Planta" },
               new PuestoDeTrabajoDto {Id = 2, NombrePuesto = "Ingreso Planta No Granos" } });
            servRepositorioMock.Setup(s => s.EsPuestoEnContingencia(It.IsAny<int>(), false))
               .Returns(true);
            servRepositorioMock.Setup(s => s.EsPuestoEnContingencia(It.IsAny<int>(), true))
               .Returns(false);

            var result = target.Index(new DatosUsuario(), "") as ViewResult;
            Assert.IsNullOrEmpty(result.ViewName);
            Assert.AreEqual(((ListaPaginada<PuestoDeTrabajoContingenciaDto>)target.ViewBag.Items).ItemsPorPagina, lista.ItemsPorPagina);
            Assert.AreEqual(((List<NirsDto>)target.ViewBag.Nirs).Count, 0);
            Assert.AreEqual(((List<HumedimetroDto>)target.ViewBag.Humedimetros).Count, 0);
            Assert.AreEqual(((List<BalanzaDto>)target.ViewBag.Balanzas).Count, 0);
            Assert.AreEqual(((CentroDto)target.ViewBag.Centro).Id, 0);
            Assert.AreEqual(((PuestoDeTrabajoDto)target.ViewBag.PuestoGranos).Id,1);
            Assert.AreEqual(((PuestoDeTrabajoDto)target.ViewBag.PuestoNoGranos).Id,2);
            Assert.AreEqual(target.ViewBag.ContingenciaGranos, false);
            Assert.AreEqual(target.ViewBag.ContingenciaNoGranos, true);
        }

        [Test]
        public void TestModificarModalidad()
        {
            var puesto = new PuestoDeTrabajoDto() { PidePatente = false, ImprimeTarjetaDeAcceso = true };
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>()))
                .Returns(puesto);

            var result = target.ModificarModalidad(new DatosUsuario(),1, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(puesto.PidePatente, true);
            Assert.AreEqual(puesto.ImprimeTarjetaDeAcceso, false);
     
        }
        [Test]
        public void TestModificarModalidadManual()
        {
            var puesto = new PuestoDeTrabajoDto() { PidePatente = true, ImprimeTarjetaDeAcceso = false };
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>()))
                .Returns(puesto);

            var result = target.ModificarModalidad(new DatosUsuario(), 1, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(puesto.PidePatente, false);
            Assert.AreEqual(puesto.ImprimeTarjetaDeAcceso, true);

        }
        [Test]
        public void TestSinAfip()
        {
            var puesto = new PuestoDeTrabajoDto() { SinAfip = true };
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>()))
                .Returns(puesto);

            var result = target.SinAfip(new DatosUsuario(), 1, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(puesto.SinAfip, false);
        }
        [Test]
        public void TestSinCupo()
        {
            var puesto = new PuestoDeTrabajoDto() { SinCupo = true };
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>()))
                .Returns(puesto);

            var result = target.SinCupo(new DatosUsuario(), 1, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(puesto.SinCupo, false);
        }
        [Test]
        public void TestSinFotoCartaPorte()
        {
            var puesto = new PuestoDeTrabajoDto() { SinFotoCartaPorte = true };
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>()))
                .Returns(puesto);

            var result = target.SinFotoCartaPorte(new DatosUsuario(), 1, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(puesto.SinFotoCartaPorte, false);
        }
        [Test]
        public void TestImprimeCartaPorte()
        {
            var puesto = new PuestoDeTrabajoDto() { ImprimeCartaPorte = true };
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>()))
                .Returns(puesto);

            var result = target.ImprimeCartaPorte(new DatosUsuario(), 1, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(puesto.ImprimeCartaPorte, false);
        }
        [Test]
        public void TestImprimeTarjetaDeAcceso()
        {
            var puesto = new PuestoDeTrabajoDto() { ImprimeTarjetaDeAcceso = true };
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>()))
                .Returns(puesto);

            var result = target.ImprimeTarjetaDeAcceso(new DatosUsuario(), 1, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(puesto.ImprimeTarjetaDeAcceso, false);
        }
        [Test]
        public void TestTomarFotoCartaDePorteEnCentro()
        {
            var centro = new CentroDto() { TomarFotoEnMesa = true };
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                .Returns(centro);

            var result = target.TomarFotoCartaDePorteEnCentro(new DatosUsuario(),  "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(centro.TomarFotoEnMesa, false);
        }
        [Test]
        public void TestDescargaCartaPortePorCtg()
        {
            var centro = new CentroDto() { DescargaCartaPortePorCtg = true };
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                .Returns(centro);

            var result = target.DescargaCartaPortePorCtg(new DatosUsuario(), "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(centro.DescargaCartaPortePorCtg, false);
        }
        [Test]
        public void TestSolicitaConfirmarCTG()
        {
            var centro = new CentroDto() { SolicitaConfirmarCTG = true };
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                .Returns(centro);

            var result = target.SolicitaConfirmarCTG(new DatosUsuario(), "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(centro.SolicitaConfirmarCTG, false);
        }
        [Test]
        public void TestValidarCupo()
        {
            var centro = new CentroDto() { ValidarCupo = true };
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                .Returns(centro);

            var result = target.ValidarCupo(new DatosUsuario(), "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(centro.ValidarCupo, false);
        }
        [Test]
        public void TestEncolaBajaCtgAutomatico()
        {
            var centro = new CentroDto() { EncolaBajaCtgAutomatico = true };
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                .Returns(centro);

            var result = target.EncolaBajaCtgAutomatico(new DatosUsuario(), "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(centro.EncolaBajaCtgAutomatico, false);
        }
        [Test]
        public void TestNirsManual()
        {
            var nirs = new NirsDto() { Modalidad = Modalidad.Automática };
            servRepositorioMock.Setup(s => s.ObtenerNirs(It.IsAny<int>()))
                .Returns(nirs);

            var result = target.NirsManual(new DatosUsuario(),1, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
        }
        [Test]
        public void TestHumedimetroManual()
        {
            var humedimetro = new HumedimetroDto() { Modalidad = Modalidad.Automática };
            servRepositorioMock.Setup(s => s.ObtenerHumedimetro(It.IsAny<int>()))
                .Returns(humedimetro);

            var result = target.HumedimetroManual(new DatosUsuario(), 1, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
        }
        [Test]
        public void TestBalanzaManual()
        {
            var balanza = new BalanzaDto() { Modalidad = Modalidad.Automática };
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>()))
                .Returns(balanza);

            var result = target.BalanzaManual(new DatosUsuario(), 1, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
        }
        [Test]
        public void TestImpresionTicketDeSalida()
        {
            servRepositorioMock.Setup(s => s.ImprimeTicketSalida(It.IsAny<int>()))
                .Returns(true);

            var result = target.ImpresionTicketDeSalida(new DatosUsuario(), "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
        }
        [Test]
        public void TestNoAsignaCalleEnGaritaEntrada()
        {
            var puesto = new PuestoDeTrabajoDto() { NoAsignaCalleEnGaritaEntrada= true };
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>()))
                .Returns(puesto);

            var result = target.NoAsignaCalleEnGaritaEntrada(new DatosUsuario(), 1, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);
            Assert.AreEqual(puesto.NoAsignaCalleEnGaritaEntrada, false);
        }
        [Test]
        public void TestActivarContingenciaGranos()
        {
            var result = target.ContingenciaGranos(new DatosUsuario(), 1, true, true, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);

            servComandosMock.Verify(x => x.Ejecutar(It.IsAny<ActivarContingenciaIngresoPlanta>()), Times.Once());
        }
        [Test]
        public void TestDesactivarContingenciaGranos()
        {
            var result = target.ContingenciaGranos(new DatosUsuario(), 1, false, true, "") as JsonResult;
            Assert.IsNullOrEmpty((string)result.Data);

            servComandosMock.Verify(x => x.Ejecutar(It.IsAny<DesactivarContingenciaIngresoPlanta>()), Times.Once());
        }
    }
}
