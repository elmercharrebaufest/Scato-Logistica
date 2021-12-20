using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Servicios.Orquestador;
using Moq;
using NUnit.Framework;
using Comando = Molinos.Scato.Dominio.Comandos.Comando;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CaladoAntiguoControllerTest
    {
        private CaladoAntiguoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<ICaladoService>> actFactoryMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<ICaladoService> contractMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private CaladoDto dto;
        private List<HumedimetroDto> humedimetros;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<ICaladoService>>();
            servComandosMock = new Mock<IServicioComandos>();
            contractMock = new Mock<ICaladoService>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            target = new CaladoAntiguoController(new NullLogger(), actFactoryMock.Object, servRepositorioMock.Object, servComandosMock.Object, orquestadorMock.Object);

            dto = new CaladoDto
            {
                WorkflowInstanceId = new Guid(),
                CicloDeCalado = 1,
                MuestraConjunto = 10,
            };

            humedimetros = new List<HumedimetroDto>
                {
                    new HumedimetroDto
                        {
                            Id = 1,
                            Descripcion = "Humedimetro 1",
                            Codigo = "1",
                            Modalidad = Modalidad.Manual,
                            CentroId = 1
                        },
                    new HumedimetroDto
                        {
                            Id = 2,
                            Descripcion = "Humedimetro 2",
                            Codigo = "2",
                            Modalidad = Modalidad.Automática,
                            CentroId = 1
                        },
                };
        }

        [Test]
        public void TestIndex()
        {
            var recorridoDto = new RecorridoDto
            {
                Centro = new CentroDto{ReingresaPatenteAlPesar = false, Descripcion = "Centro 1"},
                Patente = "AAA111",
                TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                NumeroDocumentoIngreso = "123456789",
                Material = new MaterialDto{Descripcion = "MaterialDesc 1"},
                TipoComercial = new TipoComercialDto{Descripcion = "Tipo Comercial 1"},
                Workflow = new WorkflowDto { Codigo = "Workflow 1" },
                Calado = new CaladoDto{Id = 1}
            };

            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorridoDto);
            servRepositorioMock.Setup(s => s.ListarHumedimetrosPorCentro(It.IsAny<int>()))
                .Returns(new List<HumedimetroDto> { new HumedimetroDto() { Id = 1, Descripcion = "Humedimetro 1"}, new HumedimetroDto() { Id = 2, Descripcion = "Humedimetro 2"} });
            servRepositorioMock.Setup(s => s.ListarCaracteristicasDeCalidadPorMaterialSinHumedad(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CaracteristicaDeCalidadDto> { new CaracteristicaDeCalidadDto { Id = 1, Descripcion = "caracteristica 1" }, new CaracteristicaDeCalidadDto() { Id = 2, Descripcion = "caracteristica 2" } });
            servRepositorioMock.Setup(s => s.ListarCaracteristicasDeCalidadObligatorias(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CaracteristicaDeCalidadDto> { new CaracteristicaDeCalidadDto { Id = 2, Descripcion = "AnalisisHumedad", TipoCaracteristica = CaracteristicasCalidad.EsHumedad, UnidadDeMedida = "unidad", CaladoMinimo = 1, CaladoMaximo = 10, Obligatorio = true }, new CaracteristicaDeCalidadDto() { Id = 2, Descripcion = "caracteristica 2", Obligatorio = true } });
            servRepositorioMock.Setup(s => s.ListarCaracteristicasDeCalidadObligatorias(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CaracteristicaDeCalidadDto> { new CaracteristicaDeCalidadDto { Id = 2, Descripcion = "AnalisisHumedad", TipoCaracteristica = CaracteristicasCalidad.EsHumedad, UnidadDeMedida = "unidad", CaladoMinimo = 1, CaladoMaximo = 10 } });
            servRepositorioMock.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>()))
                .Returns(new WorkflowDto{TipoDeWorkflow = TipoDeWorkflow.Ingreso});

            var result = target.Index(new Guid(), new DatosUsuario{CentroId = 1}) as ViewResult;
            var material = (string)target.ViewBag.Material;
            var patente = (string)target.ViewBag.Patente;
            var tipoDocumento = (TipoDocumentoIngreso)target.ViewBag.TipoDocumentoIngreso;
            var numeroDocumento = (string)target.ViewBag.NumeroDocumentoIngreso;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(material, Is.EqualTo("MaterialDesc 1"));
            Assert.That(patente, Is.EqualTo("AAA111"));
            Assert.That(tipoDocumento, Is.EqualTo(TipoDocumentoIngreso.CartaPorte));
            Assert.That(numeroDocumento, Is.EqualTo("123456789"));
        }

        [Test]
        public void TestCalado()
        {
            const string json = "[]";
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.Calado(It.IsAny<CaladoPorCaracteristicaDto[]>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>())).Returns(new Resultado());

            var result = target.Calado(json, "Workflow",1, 1, "10", It.IsAny<string>(),  "[]", 1, new Guid(), new DatosUsuario()) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void TestCaladoInvalido()
        {
            const string json = "[]";
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("Key", "Error"));
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.Calado(It.IsAny<CaladoPorCaracteristicaDto[]>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>())).Returns(resultado);

            var result = target.Calado(json, "Workflow",1, 1, "10", It.IsAny<string>(),  "[]", 1, new Guid(),new DatosUsuario()) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void TestObtenerHumedimetro()
        {
            servRepositorioMock.Setup(s => s.ObtenerHumedimetro(It.IsAny<int>())).Returns(humedimetros[0]);
            var resultado = target.ObtenerHumedimetro(It.IsAny<int>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"Descripcion\":\"Humedimetro 1\",\"Modalidad\":0}"));
        }

        [Test]
        public void TomarHumedad()
        {
            const int humedad = 3;
            var resultado = new ResultadoEjecutar { Mensaje = new Mensaje { Codigo = 0 }, Valores = new Dictionary<string, decimal> { { "AnalisisHumedad", humedad } } };
            servRepositorioMock.Setup(s => s.ObtenerHumedimetro(It.IsAny<int>())).Returns(humedimetros[0]);
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarAnalisisHumedad>())).Returns(resultado);

            var json = target.TomarHumedad(It.IsAny<int>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(json.Data);
            Assert.That(output, Is.EqualTo(humedad.ToString(CultureInfo.InvariantCulture)));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(null));
        }

        [Test]
        public void TomarHumedadErrorOrquestadorDesconectado()
        {
            servRepositorioMock.Setup(s => s.ObtenerHumedimetro(It.IsAny<int>())).Returns(humedimetros[0]);
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Throws(new Exception());
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarAnalisisHumedad>())).Throws(new Exception());

            var json = target.TomarHumedad(It.IsAny<int>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(json.Data);
            Assert.That(output.Contains(Textos.Humedad_AutomaticaError), Is.EqualTo(true));
        }


        [Test]
        public void TomarHumedadOrquestadorCodigoError()
        {
            var resultado = new ResultadoEjecutar { Mensaje = new Mensaje { Codigo = 555, Descripcion = "Error" } };
            servRepositorioMock.Setup(s => s.ObtenerHumedimetro(It.IsAny<int>())).Returns(humedimetros[0]);
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Returns(resultado);
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarAnalisisHumedad>())).Returns(resultado);

            var json = target.TomarHumedad(It.IsAny<int>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(json.Data);
            Assert.That(output.Contains(Textos.Error), Is.EqualTo(true));
            Assert.That(output.Contains(resultado.Mensaje.Codigo.ToString(CultureInfo.InvariantCulture)), Is.EqualTo(true));
            Assert.That(output.Contains(resultado.Mensaje.Descripcion), Is.EqualTo(true));
        }
    
        [Test]
        public void ConfirmarMuestraInvalida()
        {
            const string json = "[{\"Id\":0,\"NumeroMuestra\":1,\"Valor\":\"5.00\",\"Fecha\":\"/Date(1379429932148)/\",\"FechaFormateada\":\"17/09/13 11:58\",\"NombreHumedimetro\":\"Humedimetro 1\"}]";
            var guid = new Guid();
            var result = target.ConfirmarMuestra(json, 2, guid) as RedirectToRouteResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Never());
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual(guid, result.RouteValues["id"]);
        }
    }
}
