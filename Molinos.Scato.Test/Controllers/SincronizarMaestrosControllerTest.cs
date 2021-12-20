using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class SincronizarMaestrosControllerTest
    {
        private NullLogger log;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servComandos;
        private SincronizarMaestrosController target;


        [SetUp]
        public void SetUp()
        {
            log = new NullLogger();
            servRepositorio = new Mock<IServicioRepositorio>();
            servComandos = new Mock<IServicioComandos>();
            target = new SincronizarMaestrosController(servComandos.Object, servRepositorio.Object, log);


        }

        [Test]
        public void ProveedoresTest()
        {
            servComandos.Setup(s => s.Ejecutar(It.IsAny<SincronizarProveedores>()))
                        .Returns(new ResultadoSincronizarProveedores(){Cantidad = 1,Items = new List<ProveedorDto>{new ProveedorDto{Descripcion = "Proveedor1"}}});
            var result = target.Proveedores() as ViewResult;
            
            Assert.NotNull(result);
            Assert.AreEqual(((ResultadoSincronizarProveedores)(result.Model)).Cantidad,1);
            Assert.AreEqual(
                ((ResultadoSincronizarProveedores) (result.Model)).Items.FirstOrDefault().Descripcion,
                "Proveedor1");
        }

        [Test]
        public void ProveedoresFalseTest()
        {
            servComandos.Setup(s => s.Ejecutar(It.IsAny<SincronizarProveedores>()))
                        .Returns(new ResultadoSincronizarProveedores() { Cantidad = 1, Items = new List<ProveedorDto> { new ProveedorDto { Descripcion = "Proveedor1" } } });
            var result = target.Proveedores(false) as ContentResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Content, string.Empty);
        }

        [Test]
        public void ClientesTest()
        {
            servComandos.Setup(s => s.Ejecutar(It.IsAny<SincronizarClientes>()))
                        .Returns(new ResultadoSincronizarClientes() { Cantidad = 1, Items = new List<ClienteDto> { new ClienteDto { Descripcion = "Cliente1" } } });
            var result = target.Clientes() as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(((ResultadoSincronizarClientes)(result.Model)).Cantidad, 1);
            Assert.AreEqual(
                ((ResultadoSincronizarClientes)(result.Model)).Items.FirstOrDefault().Descripcion,
                "Cliente1");
        }

        [Test]
        public void ClientesFalseTest()
        {
            servComandos.Setup(s => s.Ejecutar(It.IsAny<SincronizarClientes>()))
                        .Returns(new ResultadoSincronizarClientes() { Cantidad = 1, Items = new List<ClienteDto> { new ClienteDto() { Descripcion = "Cliente1" } } });
            var result = target.Clientes(false) as ContentResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Content, string.Empty);
        }

        [Test]
        public void MaterialesTest()
        {
            servComandos.Setup(s => s.Ejecutar(It.IsAny<SincronizarMateriales>()))
                        .Returns(new ResultadoSincronizarMateriales() { Cantidad = 1, Items = new List<MaterialDto> { new MaterialDto() { Descripcion = "Material1" } } });
            var result = target.Materiales() as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(((ResultadoSincronizarMateriales)(result.Model)).Cantidad, 1);
            Assert.AreEqual(
                ((ResultadoSincronizarMateriales)(result.Model)).Items.FirstOrDefault().Descripcion,
                "Material1");
        }

        [Test]
        public void MaterialesFalseTest()
        {
            servComandos.Setup(s => s.Ejecutar(It.IsAny<SincronizarMateriales>()))
                        .Returns(new ResultadoSincronizarMateriales() { Cantidad = 1, Items = new List<MaterialDto> { new MaterialDto { Descripcion = "Material1" } } });
            var result = target.Materiales(false) as ContentResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Content, string.Empty);
        }

    }
}
