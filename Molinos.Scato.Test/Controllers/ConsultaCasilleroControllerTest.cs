using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ConsultaCasilleroControllerTest
    {
        private ConsultaCasilleroController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<ConsultaCasilleroAntiguedadDto> consultaCasillerosAntiguedad;
        private List<ConsultaCasilleroDto> consultaCasilleros;
        private List<ConsultaCasilleroMuestraDto> consultaCasillerosMuestra;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ConsultaCasilleroController(
                null, servRepositorioMock.Object);

            consultaCasillerosAntiguedad = new List<ConsultaCasilleroAntiguedadDto>
                {
                    new ConsultaCasilleroAntiguedadDto
                        {
                            Centro = "Chivilcoy",
                            OcupadosMas = "1(33%)",
                            OcupadosMenos = "1(33%)",
                            Libres = "1"
                        },
                    new ConsultaCasilleroAntiguedadDto
                        {
                            Centro = "Luccheti",
                            OcupadosMas = "2(100%)",
                            OcupadosMenos = "0",
                            Libres = "0"
                        }
                };

            consultaCasilleros = new List<ConsultaCasilleroDto>
                {
                    new ConsultaCasilleroDto
                        {
                            NDeCasillero = "1111-000001",
                            CantMuestra = "2",
                            TipoDocumento = TipoDocumentoIngreso.CartaPorte.ToString(),
                            NumeroDocumento = "412412412412",
                            Patente = "JRK412"
                        },
                    new ConsultaCasilleroDto
                        {
                            NDeCasillero = "1111-000002",
                            CantMuestra = "2",
                            TipoDocumento = TipoDocumentoIngreso.Remito.ToString(),
                            NumeroDocumento = "122412412412",
                            Patente = "LKF450"
                        }
                };

            consultaCasillerosMuestra = new List<ConsultaCasilleroMuestraDto>
                {
                    new ConsultaCasilleroMuestraDto
                        {
                            NumeroDocumento = "123456789123",
                            Patente = "PHJ890",
                            NDeCasillero = "1111-000001",
                            CantMuestra = "2"
                        },
                    new ConsultaCasilleroMuestraDto
                        {
                            NumeroDocumento = "765456789123",
                            Patente = "JRK412",
                            NDeCasillero = "1111-000001",
                            CantMuestra = "2"
                        }
                };
        }

        [Test]
        public void TestIndexAntiguedad()
        {
            var result = target.Antiguedad() as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestListarAntiguedad()
        {
            servRepositorioMock.Setup(s => s.ListarConsultaCasillerosPorAntiguedad(It.IsAny<ConsultaCasilleroAntiguedadDto>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ConsultaCasilleroAntiguedadDto>(consultaCasillerosAntiguedad, 1, 2, 2));

            var result = target.ListarAntiguedad(new ConsultaCasilleroAntiguedadDto{DiasDeAntiguedad = "1"}) as ViewResult;
            IEnumerable<ConsultaCasilleroAntiguedadDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("ListarAntiguedad"));
            Assert.That(results.Select(x => x.Centro), Is.EquivalentTo(new List<string> { "Chivilcoy", "Luccheti" }));
            Assert.That((object)target.ViewBag.Items.Items[0].Centro, Is.EqualTo("Chivilcoy"));
        }

        [Test]
        public void TestIndexCasillero()
        {
            var result = target.Casillero() as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestListarCasillero()
        {
            servRepositorioMock.Setup(s => s.ListarConsultaCasillerosPorCasillero(It.IsAny<ConsultaCasilleroDto>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ConsultaCasilleroDto>(consultaCasilleros, 1, 2, 2));

            var result = target.ListarCasillero(new DatosUsuario(), new ConsultaCasilleroDto()) as ViewResult;
            IEnumerable<ConsultaCasilleroDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("ListarCasillero"));
            Assert.That(results.Select(x => x.NDeCasillero), Is.EquivalentTo(new List<string> { "1111-000001", "1111-000002" }));
            Assert.That((object)target.ViewBag.Items.Items[0].NDeCasillero, Is.EqualTo("1111-000001"));
        }

        [Test]
        public void TestIndexMuestra()
        {
            var result = target.Muestra() as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestListarMuestra()
        {
            servRepositorioMock.Setup(s => s.ListarConsultaCasillerosPorMuestra(It.IsAny<ConsultaCasilleroMuestraDto>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ConsultaCasilleroMuestraDto>(consultaCasillerosMuestra, 1, 2, 2));

            var result = target.ListarMuestra(It.IsAny<ConsultaCasilleroMuestraDto>()) as ViewResult;
            IEnumerable<ConsultaCasilleroMuestraDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("ListarMuestra"));
            Assert.That(results.Select(x => x.NumeroDocumento), Is.EquivalentTo(new List<string> { "123456789123", "765456789123" }));
            Assert.That((object)target.ViewBag.Items.Items[0].NumeroDocumento, Is.EqualTo("123456789123"));
        }
    }
}
