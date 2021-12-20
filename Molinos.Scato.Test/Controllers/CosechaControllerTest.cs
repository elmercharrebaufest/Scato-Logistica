using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CosechaControllerTest
    {
        private CosechaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock; 
        private CosechaDto dto;
        private List<CosechaDto> lista;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new CosechaController(new NullLogger(), servRepositorioMock.Object, servComandosMock.Object);
            dto = new CosechaDto()
            {
                Id = 1,
                Descripcion = "11-12",
                EpaPesoDescontado = false
            };

            lista = new List<CosechaDto>
            {
                new CosechaDto()
                {
                    Id = 1,
                    Descripcion = "11-12",
                    EpaPesoDescontado = true
                },
                new CosechaDto()
                {
                    Id = 2,
                    Descripcion = "12-13",
                    EpaPesoDescontado = false
                }
            };
        }
      
        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(x => x.ListarPaginadoCosechas(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<CosechaDto>(lista, 1, 1, 10));

            var result = target.Index("Filtro") as ViewResult;
            IEnumerable<CosechaDto> results = target.ViewBag.Items;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("11-12"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoCosechas(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<CosechaDto>(lista, 1, 2, 2));

            var result = target.Listar("Filtro") as ViewResult;
            IEnumerable<CosechaDto> results = target.ViewBag.Items;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("11-12"));
        }

        [Test]
        public void CrearTest()
        {
            var resultado = target.Crear() as ViewResult;

            Assert.NotNull(resultado);
            Assert.That(resultado.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void CrearPostTest()
        {
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            var resultado = target.Crear(dto, new DatosUsuario()) as AjaxEditSuccessResult;

            servComandosMock.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Once());
            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.Content, "ajax-edit-success");
        }

        [Test]
        public void CrearConModelInvalidPostTest()
        {
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());

            target.ModelState.AddModelError("Id", "");

            var resultado = target.Crear(dto, new DatosUsuario()) as ViewResult;

            servComandosMock.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Never());
            Assert.NotNull(resultado);
            Assert.That(resultado.ViewName, Is.Null.Or.Empty);
            Assert.That(resultado.Model, Is.TypeOf<CosechaDto>());
            Assert.That(target.ModelState.IsValid, Is.False);
        }

        [Test]
        public void CrearConErrorPostTest()
        {
            var resultadoError = new Resultado();
            resultadoError.Errores.Add("Error", "Error");
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(resultadoError);

            var resultado = target.Crear(dto, new DatosUsuario()) as ViewResult;

            servComandosMock.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Once());
            Assert.NotNull(resultado);
            Assert.That(resultado.ViewName, Is.Null.Or.Empty);
            Assert.That(resultado.Model, Is.TypeOf<CosechaDto>());
            Assert.That(target.ModelState.IsValid, Is.False);
        }

        [Test]
        public void ModificarTest()
        {
            servRepositorioMock.Setup(x => x.ObtenerCosecha(It.IsAny<int>())).Returns(dto);

            var resultado = target.Modificar(1) as ViewResult;

            servRepositorioMock.Verify(x => x.ObtenerCosecha(It.IsAny<int>()), Times.Once());
            Assert.NotNull(resultado);
            Assert.That(resultado.ViewName, Is.Null.Or.Empty);
            Assert.That(resultado.Model, Is.TypeOf<CosechaDto>());
        }

        [Test]
        public void ModificarPostTest()
        {
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            var resultado = target.Modificar(dto, new DatosUsuario()) as AjaxEditSuccessResult;

            servComandosMock.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Once());
            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.Content, "ajax-edit-success");
        }

        [Test]
        public void ModificarConModelInvalidPostTest()
        {
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            
            target.ModelState.AddModelError("Id", "");

            var resultado = target.Modificar(dto, new DatosUsuario()) as ViewResult;

            servComandosMock.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Never());
            Assert.NotNull(resultado);
            Assert.That(resultado.ViewName, Is.Null.Or.Empty);
            Assert.That(resultado.Model, Is.TypeOf<CosechaDto>());
            Assert.That(target.ModelState.IsValid, Is.False);
        }

        [Test]
        public void ModificarConErrorPostTest()
        {
            var resultadoError = new Resultado();
            resultadoError.Errores.Add("Error", "Error");
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(resultadoError);

            var resultado = target.Modificar(dto, new DatosUsuario()) as ViewResult;

            servComandosMock.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Once());
            Assert.NotNull(resultado);
            Assert.That(resultado.ViewName, Is.Null.Or.Empty);
            Assert.That(resultado.Model, Is.TypeOf<CosechaDto>());
            Assert.That(target.ModelState.IsValid, Is.False);
        }
    }
}
