using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class MotivoQuiebreBarreraControllerTest
    {
        private MotivoQuiebreBarreraController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<MotivoQuiebreBarreraDto> motivoDto;
        private List<PuestoDeTrabajoDto> puestos;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new MotivoQuiebreBarreraController(null, servRepositorioMock.Object, servComandosMock.Object);

            motivoDto = new List<MotivoQuiebreBarreraDto>
                {
                    new MotivoQuiebreBarreraDto()
                        {
                            Id = 1,
                            Fecha = DateTime.Now,
                            Hora = DateTime.Now.ToShortTimeString(),
                            Patente = "AAA111",
                            PuestoTrabajoId = 1,
                            TransportistaId = 1
                        },
                    new MotivoQuiebreBarreraDto
                        {
                            Id = 2,
                            Fecha = DateTime.Now,
                            Hora = DateTime.Now.ToShortTimeString(),
                            Patente = "AAA111",
                            PuestoTrabajoId = 1,
                            TransportistaId = 1
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            puestos = new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, CentroId = 1, NombrePuesto = "Puesto1" } };
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(It.IsAny<int>())).Returns(puestos);
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var puesto = new PuestoDeTrabajoDto() { CentroId = 1, Id = 1, NombrePuesto = "Test"};
            servRepositorioMock.Setup(s => s.ListarPaginadoMotivoQuiebreBarrera(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<MotivoQuiebreBarreraDto>(motivoDto, 1, 2, 2));

            var result = target.Index(datosUsuario, puesto.Id, It.IsAny<int>()) as ViewResult;
            IEnumerable<MotivoQuiebreBarreraDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoMotivoQuiebreBarrera(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<MotivoQuiebreBarreraDto>(motivoDto, 1, 2, 2));

            var result = target.Listar(It.IsAny<int>(), new DatosUsuario{CentroId = 1}) as ViewResult;
            IEnumerable<MotivoQuiebreBarreraDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajo())
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto() { Id = 1, NombrePuesto = "PT1" } });

            servRepositorioMock.Setup(s => s.ObtenerMotivoQuiebreBarrera(It.IsAny<int>()))
                .Returns(new MotivoQuiebreBarreraDto()
                    {
                        Id = 1,
                        Fecha = DateTime.Now,
                        Hora = DateTime.Now.ToShortTimeString(),
                        Patente = "AAA111",
                        PuestoTrabajoId = 1,
                        TransportistaId = 1
                    });

            var result = target.Modificar(1) as ViewResult;

            Assert.NotNull(result.Model);
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarMotivoQuiebreBarrera>()))
                .Returns(new Resultado());

            var tipoDto = new MotivoQuiebreBarreraDto()
                {
                    Id = 1,
                    Fecha = DateTime.Now,
                    Hora = DateTime.Now.ToShortTimeString(),
                    Patente = "AAA111",
                    PuestoTrabajoId = 1,
                    TransportistaId = 1
                };

            var result = target.Modificar(tipoDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajo())
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto() { Id = 1, NombrePuesto = "PT1" } });

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarMotivoQuiebreBarrera>())).Returns(resultado);

            var tipoDto = new MotivoQuiebreBarreraDto()
            {
                Id = 1,
                Fecha = DateTime.Now,
                Hora = DateTime.Now.ToShortTimeString(),
                Patente = "AAA111",
                PuestoTrabajoId = 1,
                TransportistaId = 1
            };

            var result = target.Modificar(tipoDto) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.AreEqual(target.ModelState.IsValid, false);
        }
    }
}
