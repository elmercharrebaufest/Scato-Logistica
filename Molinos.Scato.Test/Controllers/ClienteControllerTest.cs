using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
    public class ClienteControllerTest
    {
        private ClienteController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<ClienteDto> clientes;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ClienteController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            clientes = new List<ClienteDto>
                {
                    new ClienteDto
                        {
                            Id = 1,
                           Descripcion ="Damian Sanchez Bueno",
                           Activo =true,
                           Bloqueado = false,
                           Cuit = "20-20202020-8",
                           CodigoSap ="202020"
                        },
                    new ClienteDto
                        {
                            Id = 2,
                           Descripcion ="Damian Sanchez Malo",
                           Activo =true,
                           Bloqueado = true,
                           Cuit = "20-20202020-8",
                           CodigoSap ="202020"
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarClientes(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ClienteDto>(clientes, 1, 2, 2));

            const string filter = "";
            var result = target.Index(filter) as ViewResult;
            IEnumerable<ClienteDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("Damian Sanchez Bueno"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarClientes(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ClienteDto>(clientes, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<ClienteDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("Damian Sanchez Bueno"));
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerCliente(It.IsAny<int>()))
             .Returns(new ClienteDto
             {
                 Id = 1,
                 Descripcion = "Damian Sanchez Bueno",
                 Activo = true,
                 Bloqueado = false,
                 Cuit = "20-20202020-8",
                 CodigoSap = "202020"
             });

            var result = target.Modificar(1) as ViewResult;
            Assert.NotNull(result.Model);
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarCliente>()))
                .Returns(new Resultado());

            var clienteDto = new ClienteDto
            {
                Id = 1,
                Descripcion = "Damian Sanchez Bueno",
                Activo = true,
                Bloqueado = false,
                Cuit = "20-20202020-8",
                CodigoSap = "202020"
            };

            var result = target.Modificar(clienteDto, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

    }
}
