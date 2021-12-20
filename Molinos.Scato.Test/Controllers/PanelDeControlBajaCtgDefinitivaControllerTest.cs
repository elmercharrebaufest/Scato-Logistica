using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
    [TestFixture]
    public class PanelDeControlBajaCtgDefinitivaControllerTest
    {
        private PanelDeControlBajaCtgDefinitivaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private FiltroPanelDeBajaCtgDefinitivaDto filtro;
        private DatosUsuario datosUsuario;
        private NullLogger logger;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            logger = new NullLogger();
            target = new PanelDeControlBajaCtgDefinitivaController(logger, servRepositorioMock.Object, servComandosMock.Object);
            
            filtro = new FiltroPanelDeBajaCtgDefinitivaDto() {CentroId = 1, EstadoTransmisionASap = EstadoTransmisionASap.Correcto, FechaDesde = DateTime.Now,
                                                                FechaHasta = DateTime.Now, NumeroDocumentoIngreso = "1", Patente = "abc123"};
            DirOrden dirOrden = DirOrden.Asc;
            datosUsuario = new DatosUsuario(){CentroId = 1, NombreUsuario = "User 1"};
        }

        [Test]
        public void TestIndex()
        {
            var result = target.Index(datosUsuario) as ViewResult;
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(result.Model, Is.InstanceOf<FiltroPanelDeBajaCtgDefinitivaDto>());
        }

        [Test]
        public void TestListar()
        {
            var result = target.Listar(datosUsuario, filtro) as ViewResult;
            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(result.Model, Is.InstanceOf<FiltroPanelDeBajaCtgDefinitivaDto>());
        }

        [Test]
        public void TestTransmitir()
        {
            var transmisiones = "1002|2004|1112|1234";
            servRepositorioMock.Setup(x => x.ObtenerBajasCtgDefinitivas(It.IsAny<int[]>())).Returns(new List<BajaCTGRetransmisionDto>()
            {
                new BajaCTGRetransmisionDto()
                {
                    CentroId = 1,
                    Dto = new CartaPorteDto()
                    {
                        Id = 1,
                        Material = "Soja"
                    },
                    WorkflowId = new Guid()
                }
            });
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<DarDeBajaCTGDefinitivo>())).Returns(new Resultado());
            servComandosMock.Setup(x => x.Ejecutar(new CrearControlRecorrido()
            {
                Dto = new ControlRecorridoDto
                {
                    Actividad = "Baja CTG Definitivo",
                    WorkflowInstanceId = new Guid(),
                    NombreUsuario = datosUsuario.NombreUsuario,
                    Fecha = DateTime.Now
                }
            }));

            var result = target.Transmitir(transmisiones, new DatosUsuario()) as ContentResult;
            Assert.That(result.Content, Is.EqualTo("OK"));
        }

        [Test]
        public void TestTransmitirTodasErroneas()
        {
            var transmisiones = "1002|2004|1112|1234";
            var resultado = new Resultado();
            servRepositorioMock.Setup(x => x.ObtenerBajasCtgDefinitivas(It.IsAny<int[]>())).Throws(new Exception()); ;
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<DarDeBajaCTGDefinitivo>())).Returns(resultado);
            servComandosMock.Setup(x => x.Ejecutar(new CrearControlRecorrido()
            {
                Dto = new ControlRecorridoDto
                {
                    Actividad = "Baja CTG Definitivo",
                    WorkflowInstanceId = new Guid(),
                    NombreUsuario = datosUsuario.NombreUsuario,
                    Fecha = DateTime.Now
                }
            }));

            resultado.Errores["Key"] = "Value";
            var result = target.Transmitir(transmisiones, new DatosUsuario()) as ContentResult;
            Assert.That(result.Content, Is.EqualTo("E-Las Transacciones Seleccionadas no se Reprocesaron Correctamente"));
        }
    }
}