using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ControlDeBalanzaControllerTest
    {
        private ControlDeBalanzaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IListaDeWorkflows> servListaWorkflowsMock;
        private Mock<IServicioActividadFactory<IControlDeBalanza2Service>> actFactoryMock;
        private Mock<IControlDeBalanza2Service> contractMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private NullLogger logger;
        private RecorridoDto recorrido;
        private CentroDto centro;
        private Guid instancia;
        private List<BalanzaDto> balanzas;
        private DatosUsuario datosUsuario;
        private ControlDeBalanzaDto controlDeBalanzaDto;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<IControlDeBalanza2Service>>();
            contractMock = new Mock<IControlDeBalanza2Service>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            servListaWorkflowsMock = new Mock<IListaDeWorkflows>();
            logger = new NullLogger();
            target = new ControlDeBalanzaController(logger, actFactoryMock.Object, servRepositorioMock.Object, servComandosMock.Object, servListaWorkflowsMock.Object, orquestadorMock.Object);
            instancia = new Guid("25892e17-80f6-415f-9c65-7395632f0223");

            centro = new CentroDto {Id = 1, Descripcion = "Centro 1"};

            recorrido = new RecorridoDto
                {
                    Id = 1,
                    Almacen = new AlmacenDto {Id = 1, CentroId = 1, Descripcion = "Almacen 1"},
                    Centro = centro,
                    Chofer = new ChoferDto {Id = 1, Nombre = "Chofer"},
                    DatosProximaActividad = "Tara",
                    InstanciaWorkflow = instancia,
                    Material = new MaterialDto {Id = 1, Descripcion = "MaterialDesc 1"},
                    NumeroDocumentoIngreso = "1111",
                    Patente = "AAA111",
                    TipoComercial = new TipoComercialDto {Id = 1, Descripcion = "Tipo 1"},
                    TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                    Workflow = new WorkflowDto {Id = 1, Descripcion = "EgresoMaterialNoProductivo"},
                    ControlBalanza = true,
                };

            balanzas = new List<BalanzaDto>
                {
                    new BalanzaDto
                        {
                            Id = 1,
                            Nombre = "Balanza1",
                            EstaEnCero = false,
                            Modalidad = Modalidad.Manual,
                            Color = "#FFFFFF",
                            PuestoDeTrabajo = "U1"
                        },
                    new BalanzaDto
                        {
                            Id = 2,
                            Nombre = "Balanza2",
                            EstaEnCero = false,
                            Modalidad = Modalidad.Automática,
                            Color = "#FFFFFF",
                            PuestoDeTrabajo = "U2"
                        },
                };

            controlDeBalanzaDto = new ControlDeBalanzaDto
                {
                    Patente = "AAA111",
                    TipoPesada = TipoPesada.Bruto,
                    Material = "Mat1",
                    NumeroDocumentoIngreso = "1111",
                    RecorridoId = 1,
                    TipoComercial = "T1",
                    ControlesDeBalanzasPesadas = new List<ControlDeBalanzaPesadaDto>{new ControlDeBalanzaPesadaDto
                        {
                            BalanzaId = 1, BalanzaNombre = "B1", Peso = 100
                        }}
                };
            datosUsuario = new DatosUsuario{CentroId = 1};
        }
        [Test]
        public void TestPesar()
        {
            servRepositorioMock.Setup(x => x.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>())).Returns(new DatosDeInstanciaDto { });
            servRepositorioMock.Setup(s => s.ListarBalanzasPorNombrePc(It.IsAny<int>(), It.IsAny<string>(), TipoVehiculo.Camión)).Returns(balanzas);
            var result = target.Index(new Guid(), datosUsuario) as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(((List<SelectListItem>)target.ViewBag.Balanzas).Select(s => s.Text), Is.EquivalentTo(balanzas.Select(s => s.Nombre)));
        }

        [Test]
        public void TestPesarSinBalnazas()
        {
            servRepositorioMock.Setup(x => x.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>())).Returns(new DatosDeInstanciaDto { });

            servRepositorioMock.Setup(s => s.ListarBalanzasPorNombrePc(It.IsAny<int>(), It.IsAny<string>(), TipoVehiculo.Camión)).Returns(new List<BalanzaDto>());
            var result = target.Index(new Guid(), datosUsuario) as ViewResult;
            var model = (ControlDeBalanzaDto) result.Model;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(model.Error, Is.EqualTo(Textos.Pesada_ErrorNoHayBalanzas));
        }

        [Test]
        public void TestPesarSinPuesto()
        {
            servRepositorioMock.Setup(x => x.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>())).Returns(new DatosDeInstanciaDto { });

            servRepositorioMock.Setup(s => s.ListarBalanzasPorNombrePc(It.IsAny<int>(), It.IsAny<string>(), TipoVehiculo.Camión)).Returns(new List<BalanzaDto>());
            datosUsuario.NombrePc = "NoTienePuesto";
            var result = target.Index(new Guid(), datosUsuario) as ViewResult;
            var model = (ControlDeBalanzaDto) result.Model;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(model.Error, Is.EqualTo(Textos.Pesada_ErrorPuestoDeTrabajo));
        }
       
        [Test]
        public void PesarDetallePatenteNoEnCircuito()
        {
            servListaWorkflowsMock.Setup(s => s.ObtenerWorkflowPorPatente(It.IsAny<string>())).Returns((InstanciaWorkflowDto) null);
            var result = target.PesarDetalle(controlDeBalanzaDto) as ViewResult;
            var model = (ControlDeBalanzaDto)result.Model;
            Assert.That(result.ViewName, Is.EqualTo("PesarDetalle"));
            Assert.That(model.Error, Is.EqualTo(Textos.ControlDeBalanza_PatenteInexistente));
        }
        [Test]
        public void PesarDetallePatenteNoEnControlBalanza()
        {
            servListaWorkflowsMock.Setup(s => s.ObtenerWorkflowPorPatente(It.IsAny<string>())).Returns(new InstanciaWorkflowDto());
            var result = target.PesarDetalle(controlDeBalanzaDto) as ViewResult;
            var model = (ControlDeBalanzaDto)result.Model;
            Assert.That(result.ViewName, Is.EqualTo("PesarDetalle"));
            Assert.That(model.Error, Is.EqualTo(Textos.ControlDeBalanza_Error));
        }
        [Test]
        public void PesarDetalleControlFinalizado()
        {
            servListaWorkflowsMock.Setup(s => s.ObtenerWorkflowPorPatente(It.IsAny<string>())).Returns(new InstanciaWorkflowDto{ProximaAccion = "ControlDeBalanza"});
            recorrido.ControlBalanza = false;
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            var result = target.PesarDetalle(controlDeBalanzaDto) as ViewResult;
            var model = (ControlDeBalanzaDto)result.Model;
            Assert.That(result.ViewName, Is.EqualTo("PesarDetalle"));
            Assert.That(model.Error, Is.EqualTo(Textos.ControlDeBalanza_ControlFinalizado));
        }
        [Test]
        public void PesarDetalleControlExitosoRegistroExistente()
        {
            servListaWorkflowsMock.Setup(s => s.ObtenerWorkflowPorPatente(It.IsAny<string>())).Returns(new InstanciaWorkflowDto { ProximaAccion = "ControlDeBalanza" });
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ObtenerControlDeBalanzaPorRecorrido(It.IsAny<int>())).Returns(controlDeBalanzaDto);
            var result = target.PesarDetalle(controlDeBalanzaDto) as ViewResult;
            var model = (ControlDeBalanzaDto)result.Model;
            Assert.That(result.ViewName, Is.EqualTo("PesarDetalle"));
            Assert.That(model.Error, Is.EqualTo(null));
            Assert.That(model.InstanciaWorkflow, Is.EqualTo(controlDeBalanzaDto.InstanciaWorkflow));
            Assert.That(model.Material, Is.EqualTo(controlDeBalanzaDto.Material));
            Assert.That(model.NumeroDocumentoIngreso, Is.EqualTo(controlDeBalanzaDto.NumeroDocumentoIngreso));
            Assert.That(model.Patente, Is.EqualTo(controlDeBalanzaDto.Patente));
            Assert.That(model.RecorridoId, Is.EqualTo(controlDeBalanzaDto.RecorridoId));
            Assert.That(model.TipoComercial, Is.EqualTo(controlDeBalanzaDto.TipoComercial));
            Assert.That(model.TipoPesada, Is.EqualTo(controlDeBalanzaDto.TipoPesada));
        }
        [Test]
        public void PesarDetalleControlErrorTipoPesada()
        {
            servListaWorkflowsMock.Setup(s => s.ObtenerWorkflowPorPatente(It.IsAny<string>())).Returns(new InstanciaWorkflowDto { ProximaAccion = "ControlDeBalanza" });
            recorrido.DatosProximaActividad = null;
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ObtenerControlDeBalanza(It.IsAny<int>())).Returns((ControlDeBalanzaDto)null);
            var result = target.PesarDetalle(controlDeBalanzaDto) as ViewResult;
            var model = (ControlDeBalanzaDto)result.Model;
            Assert.That(result.ViewName, Is.EqualTo("PesarDetalle"));
            Assert.That(model.Error, Is.EqualTo(Textos.ControlDeBalanza_ControlInicioError));
        }
        [Test]
        public void ObtenerBalanzaTest()
        {
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            var resultado = target.ObtenerBalanza(It.IsAny<int>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"Descripcion\":\"Balanza1\",\"Modalidad\":0,\"PuestoDeTrabajo\":\"U1\"}"));
        }
        [Test]
        public void TomarPesoTest()
        {
            const int peso = 1000;
            const string usuario = "U1";
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            var resultado = new ResultadoEjecutar { Mensaje = new Mensaje { Codigo = 0 }, Valores = new Dictionary<string, decimal> { { "Pesaje", peso } } };
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Returns(resultado);
            var json = target.TomarPeso(It.IsAny<int>(), new DatosUsuario{NombrePc = usuario}) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(json.Data);
            Assert.That(output, Is.EqualTo(peso.ToString(CultureInfo.InvariantCulture)));
            orquestadorMock.Verify(v => v.Ejecutar(It.IsAny<EjecutarPesaje>()), Times.Once());
        }
        [Test]
        public void TomarPesoErrorEnPuestoDeTrabajo()
        {
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Throws(new Exception());
            var json = target.TomarPeso(It.IsAny<int>(), new DatosUsuario{NombrePc = "NoTienePuesto"}) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(json.Data);
            Assert.That(output.Contains(Textos.Pesada_ErrorPuestoDeTrabajo), Is.EqualTo(true));
        }
        [Test]
        public void TomarPesoErrorEnBalanza()
        {
            const string usuario = "U2";
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Throws(new Exception());
            var json = target.TomarPeso(It.IsAny<int>(), new DatosUsuario{NombrePc = usuario}) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(json.Data);
            Assert.That(output.Contains(Textos.Pesada_ErrorBalanzaPuestoDeTrabajo), Is.EqualTo(true));
        }
        [Test]
        public void TomarPesoErrorOrquestadorDesconectadoTest()
        {
            const string usuario = "U1";
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Throws(new Exception());
            var json = target.TomarPeso(It.IsAny<int>(), new DatosUsuario{NombrePc = usuario}) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(json.Data);
            Assert.That(output.Contains(Textos.Pesada_AutomaticaError), Is.EqualTo(true));
        }
        [Test]
        public void TomarPesoOrquestadorCodigoError()
        {
            const string usuario = "U1";
            var resultado = new ResultadoEjecutar { Mensaje = new Mensaje { Codigo = 555, Descripcion = "Error" } };
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Returns(resultado);

            var json = target.TomarPeso(It.IsAny<int>(), new DatosUsuario { NombrePc = usuario }) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(json.Data);
            Assert.That(output.Contains(Textos.Error), Is.EqualTo(true));
            Assert.That(output.Contains(resultado.Mensaje.Codigo.ToString(CultureInfo.InvariantCulture)), Is.EqualTo(true));
            Assert.That(output.Contains(resultado.Mensaje.Descripcion), Is.EqualTo(true));
        }
        [Test]
        public void ObtenerPesadasTest()
        {
            servListaWorkflowsMock.Setup(s => s.ObtenerWorkflowPorPatente(It.IsAny<string>())).Returns(new InstanciaWorkflowDto {ProximaAccion = "ControlDeBalanza"});
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ObtenerControlDeBalanzaPorRecorrido(It.IsAny<int>())).Returns(controlDeBalanzaDto);
            var resultado = target.ObtenerPesadas(It.IsAny<string>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("[{\"Id\":0,\"ControlDeBalanzaId\":0,\"BalanzaId\":1,\"BalanzaNombre\":\"B1\",\"Peso\":100,\"Fecha\":\"\\/Date(-62135586000000)\\/\",\"Patente\":null,\"TipoDocumentoIngreso\":0,\"NumeroDocumentoIngreso\":null,\"TipoPesada\":0,\"Observacion\":null}]"));
        }
        [Test]
        public void ConfirmarSinMuestrasTest()
        {
            var result = target.Confirmar(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TipoPesada>(), datosUsuario, It.IsAny<int>()) as ContentResult;
            Assert.That(result.Content, Is.EqualTo(Textos.ControlDeBalanza_PesadasError));
        }
        [Test]
        public void ConfirmarTest()
        {
            const string muestras = "[{\"Id\":18,\"Peso\":555,\"Fecha\":\"/Date(1409774584093)/\",\"FechaFormateada\":\"03/09/2014\",\"HoraFormateada\":\"05:03 p.m.\",\"BalanzaNombre\":\"Manual\",\"BalanzaId\":1}]";
            servRepositorioMock.Setup(s => s.ObtenerControlDeBalanza(It.IsAny<int>())).Returns(controlDeBalanzaDto);
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ControlDeBalanzaComando>())).Returns(new Resultado());
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            servRepositorioMock.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>())).Returns(new DatosDeInstanciaDto());
            contractMock.Setup(s => s.ControlDeBalanza2(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<int>())).Returns(new Resultado());
            var result = target.Confirmar(muestras, It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TipoPesada>(), datosUsuario, It.IsAny<int>()) as ContentResult;
            Assert.That(result.Content, Is.EqualTo("OK"));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ControlDeBalanzaComando>()), Times.Once());
        }
        [Test]
        public void FinalizarTest()
        {
            const string muestras = "[{\"Id\":18,\"Peso\":555,\"Fecha\":\"/Date(1409774584093)/\",\"FechaFormateada\":\"03/09/2014\",\"HoraFormateada\":\"05:03 p.m.\",\"BalanzaNombre\":\"Manual\",\"BalanzaId\":1}]";
            servRepositorioMock.Setup(s => s.ObtenerControlDeBalanza(It.IsAny<int>())).Returns(controlDeBalanzaDto);
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ControlDeBalanzaComando>())).Returns(new Resultado());
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            servRepositorioMock.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>())).Returns(new DatosDeInstanciaDto());
            contractMock.Setup(s => s.ControlDeBalanza2(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<int>())).Returns(new Resultado());
            var result = target.Confirmar(muestras, It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TipoPesada>(), datosUsuario, It.IsAny<int>(), true) as ContentResult;
            Assert.That(result.Content, Is.EqualTo("OK"));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Exito));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.ControlDeBalanaza_FinalizadoExitosamente));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ControlDeBalanzaComando>()), Times.Once());
        }
        [Test]
        public void ConfirmarResultadoErrorTest()
        {
            const string muestras = "[{\"Id\":18,\"Peso\":555,\"Fecha\":\"/Date(1409774584093)/\",\"FechaFormateada\":\"03/09/2014\",\"HoraFormateada\":\"05:03 p.m.\",\"BalanzaNombre\":\"Manual\",\"BalanzaId\":1}]";
            servRepositorioMock.Setup(s => s.ObtenerControlDeBalanza(It.IsAny<int>())).Returns(controlDeBalanzaDto);
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("error", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ControlDeBalanzaComando>())).Returns(resultado);
            var result = target.Confirmar(muestras, It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TipoPesada>(), datosUsuario, It.IsAny<int>()) as ContentResult;
            Assert.That(result.Content, Is.EqualTo(Textos.ControlDeBalanza_CrearError));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ControlDeBalanzaComando>()), Times.Once());
        }
        


    }
}
