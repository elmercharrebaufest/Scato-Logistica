using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
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
    public class LiberacionDeCasillerosControllerTest
    {
        private LiberacionDeCasillerosController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<LiberacionDeCasillerosDto> liberacionDeCasilleros;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new LiberacionDeCasillerosController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            liberacionDeCasilleros = new List<LiberacionDeCasillerosDto>
                {
                    new LiberacionDeCasillerosDto
                        {
                            Id = 1,
                            NDeCasillero = "1111-000001",
                            TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte.ToString(),
                            NumeroDocumento = "122412412412",
                            Patente = "LKF450"
                        },
                    new LiberacionDeCasillerosDto
                        {
                            Id = 2,
                            NDeCasillero = "1111-000002",
                            TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte.ToString(),
                            NumeroDocumento = "567892412412",
                            Patente = "FGH321"
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            var result = target.Index(new DatosUsuario()) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarLiberacionDeCasilleros(It.IsAny<LiberacionDeCasillerosDto>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<LiberacionDeCasillerosDto>(liberacionDeCasilleros, 1, 2, 2));

            var result = target.Listar(new LiberacionDeCasillerosDto { DiasDeAntiguedad = "1" }) as ViewResult;
            IEnumerable<LiberacionDeCasillerosDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].NDeCasillero, Is.EqualTo("1111-000001"));
        }

        [Test]
        public void TestLiberacionDeCasilleros()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarMicroMuestrasPorCasillero>())).Returns(new Resultado());

            var actual = target.LiberarCasilleros("1") as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);
        }
    }
}
