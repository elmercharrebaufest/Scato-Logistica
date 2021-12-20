using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
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
    public class ImpresoraControllerTest
    {
        private ImpresoraController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandosFactory> servComandosFactoryMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IConfiguracionProvider> servConfiguracionMock;
        private List<ImpresoraDto> impresoras;
        private Mock<IServicioRepositorioFactory> servRepositorioFactoryMock;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            servComandosFactoryMock = new Mock<IServicioComandosFactory>();
            servConfiguracionMock = new Mock<IConfiguracionProvider>();
            servRepositorioFactoryMock = new Mock<IServicioRepositorioFactory>();
            target = new ImpresoraController(new NullLogger(), servRepositorioMock.Object, servRepositorioFactoryMock.Object, servComandosFactoryMock.Object, servComandosMock.Object, servConfiguracionMock.Object);

            datosUsuario = new DatosUsuario
            {
                NombreUsuario = "Usuario 1"
            };
            impresoras = new List<ImpresoraDto>
                {
                    new ImpresoraDto
                        {
                            Id = 1,
                            Descripcion = "a",
                        },
                    new ImpresoraDto
                        {
                            Id = 1,
                            Descripcion = "b",
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoImpresoras(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ImpresoraDto>(impresoras, 1, 2, 2));

            servConfiguracionMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "https://localhost/hostlocal/host.c" } });
            const string filter = "";


            var result = target.Index(new DatosUsuario(), filter) as ViewResult;


            IEnumerable<ImpresoraDto> results = target.ViewBag.Items;

            IList<SelectListItem> servers = target.ViewBag.Servers;

            Assert.That(servers, Is.Not.Null);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,1 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("a"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoImpresoras(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ImpresoraDto>(impresoras, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(new DatosUsuario(), filter) as ViewResult;
            IEnumerable<ImpresoraDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 1 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("a"));
        }

        [Test]
        public void TestCrear()
        {
            var result = target.Crear() as ViewResult;

            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosFactoryMock.Setup(x => x.CrearServicio(It.IsAny<string>())).Returns(servComandosMock.Object);
            servConfiguracionMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "servurl" } });


            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<InstalarImpresora>()))
                .Returns(new Resultado());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearImpresora>()))
                .Returns(new Resultado());
            
            var impresoraDto = new ImpresoraDto
            {
                Id = 1,
                Descripcion = "c",
                Direccion = "d"
            };

            var result = target.Crear(new DatosUsuario(), impresoraDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(2));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalidoCrear()
        {
            servComandosFactoryMock.Setup(x => x.CrearServicio(It.IsAny<string>())).Returns(servComandosMock.Object);
            servConfiguracionMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "servurl" } });

            var resultado = new Resultado();
            resultado.Error("Error", "error");

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<InstalarImpresora>()))
                .Returns(new Resultado());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearImpresora>()))
                .Returns(resultado);

            var impresoraDto = new ImpresoraDto
            {
                Id = 1,
                Descripcion = "c",
                Direccion = "d"
            };


            var result = target.Crear(new DatosUsuario(), impresoraDto) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(2));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestCrearPostInvalidoInstalar()
        {
            servComandosFactoryMock.Setup(x => x.CrearServicio(It.IsAny<string>())).Returns(servComandosMock.Object);
            servConfiguracionMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "servurl" } });

            var resultado = new Resultado();
            resultado.Error("Error", "error");

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<InstalarImpresora>()))
                .Returns(resultado);
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearImpresora>()))
                .Returns(new Resultado());

            var impresoraDto = new ImpresoraDto
            {
                Id = 1,
                Descripcion = "c",
                Direccion = "d"
            };


            var result = target.Crear(new DatosUsuario(), impresoraDto) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestImprimir()
        {
            var resultado = new Resultado();
            servRepositorioMock.Setup(x => x.ObtenerImpresora(It.IsAny<int>())).Returns(impresoras[0]);
            servConfiguracionMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "https://localhost/hostlocal/host.c" } });
            servComandosFactoryMock.Setup(x => x.CrearServicio(It.IsAny<string>())).Returns(servComandosMock.Object);
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(resultado);
            var result = target.Imprimir("localhost", impresoras[0].Id, datosUsuario);
            Assert.NotNull(result);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            servRepositorioMock.Verify(p => p.ObtenerImpresora(It.IsAny<int>()), Times.Exactly(1));
            servConfiguracionMock.Verify(p => p.AppSettings, Times.Exactly(1));
            servComandosFactoryMock.Verify(p => p.CrearServicio(It.IsAny<string>()), Times.Exactly(1));
            servComandosMock.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));

        }

        [Test]
        public void TestImprimirImpresoraNull()
        {
            var resultado = new Resultado();
            servRepositorioMock.Setup(x => x.ObtenerImpresora(It.IsAny<int>())).Returns((Func<ImpresoraDto>) null);
            servConfiguracionMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "https://localhost/hostlocal/host.c" } });
            servComandosFactoryMock.Setup(x => x.CrearServicio(It.IsAny<string>())).Returns(servComandosMock.Object);
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(resultado);
            var result = target.Imprimir("localhost", impresoras[0].Id, datosUsuario);
            
            Assert.NotNull(result);
           
            resultado.Errores.Add("ImpresoraIsNull", "True");

            Assert.That(resultado.HayErrores, Is.EqualTo(true));

            servRepositorioMock.Verify(p => p.ObtenerImpresora(It.IsAny<int>()), Times.Exactly(1));
            servConfiguracionMock.Verify(p => p.AppSettings, Times.Exactly(0));
            servComandosFactoryMock.Verify(p => p.CrearServicio(It.IsAny<string>()), Times.Exactly(0));
            servComandosMock.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));

        }

        [Test]
        public void TestImprimirExcepcion()
        {
            var resultado = new Resultado();
            servRepositorioMock.Setup(x => x.ObtenerImpresora(It.IsAny<int>())).Returns(impresoras[0]);
            servConfiguracionMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "https://localhost/hostlocal/host.c" } });
            servComandosFactoryMock.Setup(x => x.CrearServicio(It.IsAny<string>())).Throws(new Exception());
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(resultado);
            var result = target.Imprimir("localhost", impresoras[0].Id, datosUsuario);

            Assert.NotNull(result);

            Assert.That(target.ModelState.Count, Is.EqualTo(1));
           
            servRepositorioMock.Verify(p => p.ObtenerImpresora(It.IsAny<int>()), Times.Exactly(1));
            servConfiguracionMock.Verify(p => p.AppSettings, Times.Exactly(1));
            servComandosFactoryMock.Verify(p => p.CrearServicio(It.IsAny<string>()), Times.Exactly(1));
            servComandosMock.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));

        }

        [Test]
        public void TestModificar()
        {
            var result = target.Modificar(1) as ViewResult;

            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosFactoryMock.Setup(x => x.CrearServicio(It.IsAny<string>())).Returns(servComandosMock.Object);
            servConfiguracionMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "servurl" } });


            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<InstalarImpresora>()))
                .Returns(new Resultado());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarImpresora>()))
                .Returns(new Resultado());

            var impresoraDto = new ImpresoraDto
            {
                Id = 1,
                Descripcion = "c",
                Direccion = "d"
            };


            var result = target.Modificar(new DatosUsuario(),impresoraDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(2));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servComandosFactoryMock.Setup(x => x.CrearServicio(It.IsAny<string>())).Returns(servComandosMock.Object);
            servConfiguracionMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "servurl" } });

            var resultado = new Resultado();
            resultado.Error("Error", "error");

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<InstalarImpresora>()))
                .Returns(resultado);
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearImpresora>()))
                .Returns(new Resultado());

            var impresoraDto = new ImpresoraDto
            {
                Id = 1,
                Descripcion = "c",
                Direccion = "d"
            };


            var result = target.Modificar(new DatosUsuario(), impresoraDto) as ViewResult;


            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestEliminar()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarImpresora>())).Returns(new Resultado());

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);
            
        }

        [Test]
        public void TestEliminarInvalido()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarImpresora>())).Returns(resultado);

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }

        [Test]
        public void VerCola()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);

            servRepositorioFactoryMock.Setup(x => x.CrearServicio(It.IsAny<string>())).Returns(servRepositorioMock.Object);
            servConfiguracionMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "servurl" } });
            servRepositorioMock.Setup(s => s.ConsultarColaImpresion(It.IsAny<int>(), It.IsAny<string>()))
                               .Returns(new List<ColaImpresionDto>
                                   {
                                       new ColaImpresionDto
                                           {
                                               Documento = "doc1",
                                               Estado = "estado1",
                                               Fecha = new DateTime(),
                                               ImpresoraId = 1,
                                               JobId = 1,
                                               Paginas = 2,
                                               Servidor = "servidro1",
                                               Tamanio = 150
                                           }
                                   });

            var actual = target.VerCola(0) as PartialViewResult;

            servRepositorioMock.Verify(p => p.ConsultarColaImpresion(It.IsAny<int>(),It.IsAny<string>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

        [Test]
        public void AccionImpresionCola()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);

            servRepositorioFactoryMock.Setup(x => x.CrearServicio(It.IsAny<string>())).Returns(servRepositorioMock.Object);
            servConfiguracionMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "servurl" } });
            servRepositorioMock.Setup(s => s.AccionJobImpresion(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<AccionColaImpresion>()));
            servRepositorioMock.Setup(s => s.ConsultarColaImpresion(It.IsAny<int>(), It.IsAny<string>()))
                               .Returns(new List<ColaImpresionDto>
                                   {
                                       new ColaImpresionDto
                                           {
                                               Documento = "doc1",
                                               Estado = "estado1",
                                               Fecha = new DateTime(),
                                               ImpresoraId = 1,
                                               JobId = 1,
                                               Paginas = 2,
                                               Servidor = "servidro1",
                                               Tamanio = 150
                                           }
                                   });
            var actual = target.AccionImpresionCola(1,1,"serv1",AccionColaImpresion.Pausar) as PartialViewResult;

            servRepositorioMock.Verify(p => p.ConsultarColaImpresion(It.IsAny<int>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }
    }
}
