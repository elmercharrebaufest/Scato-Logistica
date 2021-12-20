using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;
using TipoAnalisis = Molinos.Scato.Servicios.GestionarCartasDePortePE.TipoAnalisis;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class PanelDeControlTransaccionesSapControllerTest
    {
        private PanelDeControlTransaccionesSapController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<TransmisionASapDto> transmisionesASapDto;
        private Mock<ZSDWS_SCATO> servicioSapMock;
        private Mock<IServicioSapAsincronico> servicioSapAsincMock;
        private Mock<WaybillManagementPODv2> servicioMonsantoMock;
        private TransmisionSapAModificarDto transmisionesAModificar;
        private IList<TransmisionesCupoEnErrorDto> transmisionesCuposDto;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            servicioSapMock = new Mock<ZSDWS_SCATO>();
            servicioSapAsincMock = new Mock<IServicioSapAsincronico>();
            servicioMonsantoMock = new Mock<WaybillManagementPODv2>();
            target = new PanelDeControlTransaccionesSapController(new NullLogger(), servRepositorioMock.Object, servComandosMock.Object, servicioSapMock.Object, servicioMonsantoMock.Object, servicioSapAsincMock.Object);

            transmisionesASapDto = new List<TransmisionASapDto>
                {
                    new TransmisionASapDto
                        {
                            Id = 1,
                            Estado = EstadoTransmisionASap.Correcto,
                            InstanciaWorkflow = new Guid(),
                            FuncionSap = FuncionSAP.AjusteDeDiferencias,
                            Fecha = new DateTime(2010,1,1),
                            Patente = "AAA222",
                            TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                            NumeroDocumento = "1234"
                        },
                    new TransmisionASapDto
                        {
                            Id = 2,
                            Estado = EstadoTransmisionASap.Correcto,
                            InstanciaWorkflow = new Guid(),
                            FuncionSap = FuncionSAP.AjusteDeDiferencias,
                            Fecha = new DateTime(2010,1,1),
                            Patente = "AAA222",
                            TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                            NumeroDocumento = "1234"
                        }
                };
            transmisionesCuposDto = new List<TransmisionesCupoEnErrorDto>
                {
                    new TransmisionesCupoEnErrorDto
                        {
                            InstanciaWorkflow = new Guid(),
                            Z2200 = new Z_SDMF_Z2200N()
                        },
                    new TransmisionesCupoEnErrorDto
                        {
                            InstanciaWorkflow = new Guid(),
                            Z2200 = new Z_SDMF_Z2200N()
                        }
                };
            transmisionesAModificar = new TransmisionSapAModificarDto
                {
                    Id = 1,
                    Campos = new Dictionary<string, string> { { "Documento", "valor1" }, { "DocLegal", "valor2" } },
                    EstadoTransmision = EstadoTransmisionASap.Error                  
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarTransmisionesASap(It.IsAny<FiltroPanelDeTransaccionesSapDto>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<TransmisionASapDto>(transmisionesASapDto, 1, 2, 2));

            var result = target.Index(new DatosUsuario()) as ViewResult;
            IEnumerable<TransmisionASapDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].FuncionSap, Is.EqualTo(FuncionSAP.AjusteDeDiferencias));
            servRepositorioMock.Verify(s => s.ListarTransmisionesASap(It.IsAny<FiltroPanelDeTransaccionesSapDto>(), It.IsAny<Paginacion>()),Times.Exactly(1));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarTransmisionesASap(It.IsAny<FiltroPanelDeTransaccionesSapDto>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<TransmisionASapDto>(transmisionesASapDto, 1, 2, 2));
            servRepositorioMock.Setup(s => s.ObtenerTransmisionesCuposIdsConError()).Returns(new List<int>{1,2,3});
            
            var result = target.Listar(new DatosUsuario(), new FiltroPanelDeTransaccionesSapDto{Patente = "aaa111"}) as ViewResult;
            IEnumerable<TransmisionASapDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].FuncionSap, Is.EqualTo(FuncionSAP.AjusteDeDiferencias));
        }

        [Test]
        public void TestListarModeloInvalido()
        {
            target.ModelState.AddModelError("E","error");
            servRepositorioMock.Setup(s => s.ListarTransmisionesASap(It.IsAny<FiltroPanelDeTransaccionesSapDto>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<TransmisionASapDto>(transmisionesASapDto, 1, 2, 2));

            var result = target.Listar(new DatosUsuario(), new FiltroPanelDeTransaccionesSapDto { Patente = "aaa111" }) as ViewResult;
            IEnumerable<TransmisionASapDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.Empty);
        }

        [Test]
        public void TestIndexError()
        {
            servRepositorioMock.Setup(s => s.ListarTransmisionesASap(It.IsAny<FiltroPanelDeTransaccionesSapDto>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<TransmisionASapDto>(transmisionesASapDto, 1, 2, 2));
            target.ModelState.AddModelError("error", "error");
            var result = target.Index(new DatosUsuario()) as ViewResult;
            IEnumerable<TransmisionASapDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(results.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Modificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerTransmisionASapPorIdyFuncionSap(It.IsAny<int>())).Returns(transmisionesAModificar);

            var result = target.Modificar(1,new DatosUsuario()) as ViewResult;

            servRepositorioMock.Verify(s => s.ObtenerTransmisionASapPorIdyFuncionSap(It.IsAny<int>()),Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void ModificarError()
        {
            transmisionesAModificar.EstadoTransmision = EstadoTransmisionASap.Correcto;
            servRepositorioMock.Setup(s => s.ObtenerTransmisionASapPorIdyFuncionSap(It.IsAny<int>())).Returns(transmisionesAModificar);

            var result = target.Modificar(1, new DatosUsuario()) as RedirectToRouteResult;

            servRepositorioMock.Verify(s => s.ObtenerTransmisionASapPorIdyFuncionSap(It.IsAny<int>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(target.TempData["Alerta"], Textos.ModificarTransmisionSap_Error);
            Assert.AreEqual(target.TempData["TipoAlerta"], TipoAlerta.Error);
        }

        [Test]
        public void NoTransmitir()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ActualizarTransmisionASap>())).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ObtenerTransmisionASap(It.IsAny<int>())).Returns(new TransmisionASapDto ());
            var result = target.NoTransmitir("1|2|3|4") as ContentResult;
            
            servComandosMock.Verify(s => s.Ejecutar(It.IsAny<ActualizarTransmisionASap>()), Times.Exactly(4));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, "OK");
        }

        [Test]
        public void ModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ActualizarTransmision>())).Returns(new Resultado());

            var result = target.Modificar(new DatosUsuario(),transmisionesAModificar) as ContentResult;

            servComandosMock.Verify(s => s.Ejecutar(It.IsAny<ActualizarTransmision>()), Times.Exactly(1));
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void ModificarPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ActualizarTransmision>())).Returns(new Resultado());
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ActualizarTransmision>())).Returns(resultado);

            var result = target.Modificar(new DatosUsuario(),transmisionesAModificar) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ActualizarTransmision>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void RetransmitirTodas()
        {
            servRepositorioMock.Setup(s => s.ListarCuposEnErrorASapPorFiltro(It.IsAny<FiltroPanelDeTransaccionesSapDto>())).Returns(transmisionesCuposDto);
            servicioSapAsincMock.Setup(s => s.InformarCupo(It.IsAny<Guid>(), It.IsAny<Z_SDMF_Z2200N>()));

            var result = target.RetransmitirTodas("") as ContentResult;

            servRepositorioMock.Verify(s => s.ListarCuposEnErrorASapPorFiltro(It.IsAny<FiltroPanelDeTransaccionesSapDto>()), Times.Exactly(1));
            servicioSapAsincMock.Verify(s => s.InformarCupo(It.IsAny<Guid>(), It.IsAny<Z_SDMF_Z2200N>()), Times.Exactly(2));
            var expectedResult = new ContentResult { Content = "OK" };
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }
        
      
    }
}
