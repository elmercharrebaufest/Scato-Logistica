using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;
using Comando = Molinos.Scato.Dominio.Comandos.Comando;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class PuestoDeTrabajoControllerTest
    {
        private PuestoDeTrabajoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private Mock<IServicioEstadoPuesto> estado;
        private Mock<IFirmwareFactory> firmwareFactoryMock;

        private List<PuestoDeTrabajoDto> puestos;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            estado = new Mock<IServicioEstadoPuesto>();
            firmwareFactoryMock = new Mock<IFirmwareFactory>();

            // Mock firmwares disponibles
            firmwareFactoryMock.Setup(f => f.FirmwareDisponibles())
                .Returns(new Dictionary<string, string> { { "FirmwareTest", "TestValue" } });

            target = new PuestoDeTrabajoController(
                null, servRepositorioMock.Object, servComandosMock.Object, orquestadorMock.Object, firmwareFactoryMock.Object, estado.Object);

            puestos = new List<PuestoDeTrabajoDto>
                {
                    new PuestoDeTrabajoDto
                        {
                            Id = 1,
                            CentroId = 1,
                            NombrePuesto = "Puesto 1",
                            NombrePc = "PC 1",
                            Entrada = "Barrera 1",
                            Lector = "Lector 1",
                        },
                    new PuestoDeTrabajoDto
                        {
                            Id = 2,
                            CentroId = 2,
                            NombrePuesto = "Puesto 2",
                            NombrePc = "PC 2",
                            Entrada = "Barrera 2",
                            Lector = "Lector 2",
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoPuestosDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<PuestoDeTrabajoDto>(puestos, 1, 2, 2));

            const string filter = "";
            var datosUsuario = new DatosUsuario {CentroId = 1};
            var result = target.Index(datosUsuario, filter) as ViewResult;
            IEnumerable<PuestoDeTrabajoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].NombrePuesto, Is.EqualTo("Puesto 1"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoPuestosDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<PuestoDeTrabajoDto>(puestos, 1, 2, 2));

            const string filter = "";
            var datosUsuario = new DatosUsuario {CentroId = 1};
            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<PuestoDeTrabajoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].NombrePuesto, Is.EqualTo("Puesto 1"));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoPuestosDeTrabajo("2", It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<PuestoDeTrabajoDto>(new List<PuestoDeTrabajoDto> { puestos[1] }, 1, 1, 1));

            const string filter = "2";
            var datosUsuario = new DatosUsuario {CentroId = 1};
            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<PuestoDeTrabajoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].NombrePuesto, Is.EqualTo("Puesto 2"));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearPuestoDeTrabajo>()))
                .Returns(new Resultado());

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, "[{\"Codigo\":\"Barem01\",\"Descripcion\":\"Bar1\"}]", "[{\"Codigo\":\"Barem01\",\"Descripcion\":\"Bar1\"}]", "[]", "[]", puestos[0],"[]") as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearPuestoDeTrabajo>())).Returns(resultado);
            ConfigurarMocksSetearVista();

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, "[{\"Codigo\":\"Barem01\",\"Descripcion\":\"Bar1\"}]", "[{\"Codigo\":\"Barem01\",\"Descripcion\":\"Bar1\"}]", "[]", "[]", puestos[0],"[]") as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestCrearPostBarrerasVacio()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearPuestoDeTrabajo>()))
                .Returns(new Resultado());
            ConfigurarMocksSetearVista();
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, "", "", "","", puestos[0],"") as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.Null(result);
            Assert.False(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(1))
                .Returns(puestos[0]);
            ConfigurarMocksSetearVista();

            var result = target.Modificar(1,new DatosUsuario()) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarPuestoDeTrabajo>()))
                .Returns(new Resultado());

            var camaraDto = new PuestoDeTrabajoDto
            {
                Id = 1,
                NombrePuesto = "Nombre modificado",
            };

            var result = target.Modificar(camaraDto, "[{\"Codigo\":\"Barem01\",\"Descripcion\":\"Bar1\"}]", "[{\"Codigo\":\"Barem01\",\"Descripcion\":\"Bar1\"}]", "[]", "[]", new DatosUsuario(),"[]") as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }
        [Test]
        public void TestModificarPostBarrerasVacio()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarPuestoDeTrabajo>()))
                .Returns(new Resultado());
            ConfigurarMocksSetearVista();

            var camaraDto = new PuestoDeTrabajoDto
            {
                Id = 1,
                NombrePuesto = "Nombre modificado",
            };

            var result = target.Modificar(camaraDto, "", "", "", "", new DatosUsuario(),"") as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.Null(result);
            Assert.False(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarPuestoDeTrabajo>()))
                .Returns(new Resultado());
            ConfigurarMocksSetearVista();

            var camaraDto = new PuestoDeTrabajoDto
            {
                Id = 1,
                NombrePuesto = "Nombre modificado",
            };

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(camaraDto, "", "", "", "", new DatosUsuario(),"") as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificarModalidad()
        {
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(1))
                .Returns(puestos[0]);

            var result = target.ModificarModalidad(1) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarModalidadPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarModalidadPuestoDeTrabajo>()))
                .Returns(new Resultado());

            var datosUsuario = new DatosUsuario
                {
                    NombreUsuario = "diego"
                };

            var cambioModel = new CambioModalidadPuestoModel
            {
                Id = 1,
                Automatico = true,
                Motivo = "Por que si"
            };

            var result = target.ModificarModalidad(cambioModel, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.Is<ModificarModalidadPuestoDeTrabajo>(c => c.IdPuesto == 1 && c.Automatico == cambioModel.Automatico && c.Usuario == datosUsuario.NombreUsuario)), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarPuestoDeTrabajo>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

        private void ConfigurarMocksSetearVista()
        {
            orquestadorMock.Setup(s => s.ListarLectores()).Returns(new[] { new DispositivoDto() });
            orquestadorMock.Setup(s => s.ListarBarrerasSemaforos()).Returns(new[] { new DispositivoDto() });
            orquestadorMock.Setup(s => s.ListarCamaras()).Returns(new[] { new DispositivoDto() });
            orquestadorMock.Setup(s => s.ListarSensores()).Returns(new[] { new DispositivoDto() });
            orquestadorMock.Setup(s => s.ListarSensoresVehiculares()).Returns(new[] { new DispositivoDto() });
            orquestadorMock.Setup(s => s.ListarConfigIdentificacionVehicular()).Returns(new[] { new Molinos.Scato.Servicios.Orquestador.ConfigIdentificacionVehicularDto { Codigo = "TEST01", Nombre = "Test Vehicular" } });
            orquestadorMock.Setup(s => s.ListarLectoresQr()).Returns(new[] { new DispositivoDto() });
            orquestadorMock.Setup(s => s.ListarCartelesLed()).Returns(new[] { new DispositivoDto() });
            orquestadorMock.Setup(s => s.ListarIntercomunicadores()).Returns(new[] { new DispositivoDto() });
            orquestadorMock.Setup(s => s.ListarGruposBarrera()).Returns(new[] { new DispositivoDto() });
            servRepositorioMock.Setup(x => x.ListarTodasLasBalanzasActivas(It.IsAny<int>())).Returns(new List<BalanzaDto>());
            servRepositorioMock.Setup(x => x.ListarConfiguracionSensores(It.IsAny<int>())).Returns(new List<ConfigSensoresDto>());
            servRepositorioMock.Setup(x => x.ObtenerGruposBarrerasPorCentro(It.IsAny<int>())).Returns(new List<VisualizacionBarreraDto>());
        }

        // --- Ciclo 15: FirmwareGaritaIngreso sin CodigoConfigIdentificacionVehicular → error ---

        [Test]
        public void Crear_ConFirmwareGaritaIngreso_SinConfigIdentificacionVehicular_AgregaErrorModelState()
        {
            ConfigurarMocksSetearVista();
            var model = new PuestoDeTrabajoDto
            {
                Firmware = "Molinos.Scato.Web.Firmware.FirmwareGaritaIngreso, Molinos.Scato.Web",
                CodigoConfigIdentificacionVehicular = null
            };
            var datosUsuario = new DatosUsuario { CentroId = 1 };

            target.Crear(datosUsuario, "[{\"Codigo\":\"Bar01\",\"Descripcion\":\"Bar1\"}]", "[{\"Codigo\":\"Bar01\",\"Descripcion\":\"Bar1\"}]", "[]", "[]", model, "[]");

            Assert.That(target.ModelState.IsValid, Is.False);
            Assert.That(target.ModelState.ContainsKey("CodigoConfigIdentificacionVehicular"), Is.True);
        }

        // --- Ciclo 16: FirmwareGaritaIngreso con CodigoConfigIdentificacionVehicular → sin error de validación ---

        [Test]
        public void Crear_ConFirmwareGaritaIngreso_ConConfigIdentificacionVehicular_NoAgregaError()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearPuestoDeTrabajo>())).Returns(new Resultado());
            var model = new PuestoDeTrabajoDto
            {
                Firmware = "Molinos.Scato.Web.Firmware.FirmwareGaritaIngreso, Molinos.Scato.Web",
                CodigoConfigIdentificacionVehicular = "CFG-IDVEH-01"
            };
            var datosUsuario = new DatosUsuario { CentroId = 1 };

            target.Crear(datosUsuario, "[{\"Codigo\":\"Bar01\",\"Descripcion\":\"Bar1\"}]", "[{\"Codigo\":\"Bar01\",\"Descripcion\":\"Bar1\"}]", "[]", "[]", model, "[]");

            Assert.That(target.ModelState.ContainsKey("CodigoConfigIdentificacionVehicular"), Is.False);
        }

        // --- Ciclo 17: Otro firmware sin CodigoConfigIdentificacionVehicular → sin error de validación ---

        [Test]
        public void Crear_ConOtroFirmware_SinConfigIdentificacionVehicular_NoAgregaError()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearPuestoDeTrabajo>())).Returns(new Resultado());
            var model = new PuestoDeTrabajoDto
            {
                Firmware = "Molinos.Scato.Web.Firmware.FirmwarePuestoDesatendido, Molinos.Scato.Web",
                CodigoConfigIdentificacionVehicular = null
            };
            var datosUsuario = new DatosUsuario { CentroId = 1 };

            target.Crear(datosUsuario, "[{\"Codigo\":\"Bar01\",\"Descripcion\":\"Bar1\"}]", "[{\"Codigo\":\"Bar01\",\"Descripcion\":\"Bar1\"}]", "[]", "[]", model, "[]");

            Assert.That(target.ModelState.ContainsKey("CodigoConfigIdentificacionVehicular"), Is.False);
        }

        // --- Ciclo 18: Modificar con FirmwareGaritaIngreso sin config → error ---

        [Test]
        public void Modificar_ConFirmwareGaritaIngreso_SinConfigIdentificacionVehicular_AgregaErrorModelState()
        {
            ConfigurarMocksSetearVista();
            var model = new PuestoDeTrabajoDto
            {
                Id = 1,
                NombrePuesto = "Garita",
                Firmware = "Molinos.Scato.Web.Firmware.FirmwareGaritaIngreso, Molinos.Scato.Web",
                CodigoConfigIdentificacionVehicular = null
            };

            target.Modificar(model, "[{\"Codigo\":\"Bar01\",\"Descripcion\":\"Bar1\"}]", "[{\"Codigo\":\"Bar01\",\"Descripcion\":\"Bar1\"}]", "[]", "[]", new DatosUsuario(), "[]");

            Assert.That(target.ModelState.IsValid, Is.False);
            Assert.That(target.ModelState.ContainsKey("CodigoConfigIdentificacionVehicular"), Is.True);
        }

    }
}
