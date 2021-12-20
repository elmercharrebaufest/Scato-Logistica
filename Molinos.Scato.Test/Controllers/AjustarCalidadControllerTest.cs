using System.Collections.Generic;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
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
    public class AjustarCalidadControllerTest
    {
        private AjustarCalidadController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private IList<CaladoPorCaracteristicaDto> dtos;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new AjustarCalidadController(new NullLogger(), servRepositorioMock.Object, servComandosMock.Object);

            dtos = new List<CaladoPorCaracteristicaDto>
            {
                new CaladoPorCaracteristicaDto{Id = 1, ValorCalado = 10, CaracteristicaId = 1, Caracteristica = "Caracteristica 1"},
                new CaladoPorCaracteristicaDto{Id = 2, ValorCalado = 5, CaracteristicaId = 2, Caracteristica = "Caracteristica 2"}
            };
        }

        [Test]
        public void TestIndex()
        {
            var result = target.Index(new DatosUsuario(), new FiltroRecorridoModel()) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestModificar()
        {
            const string json = "[]";

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarValorCalado>()))
                .Returns(new Resultado());

            var result = target.Modificar(json, "CartaPorte", "1", 1,new DatosUsuario{NombreUsuario = "usuario"}) as JsonResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
        }

        [Test]
        public void TestCaladoInvalido()
        {
            const string json = "[]";
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("Key", "Error"));

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarValorCalado>()))
                .Returns(resultado);

            var result = target.Modificar(json, "CartaPorte", "1",1, new DatosUsuario { NombreUsuario = "usuario" }) as JsonResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
        }
    }
}
