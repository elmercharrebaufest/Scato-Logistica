using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
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
    public class AutorizarTransportistaInhabilitadoControllerTest
    {
        private AutorizarTransportistaInhabilitadoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IAutorizarTransportistaInhabilitadoService>> actFactoryMock;
        private List<InhabilitacionCamionDto> inthabilitacionesCamion;
        private List<InhabilitacionChoferDto> inthabilitacionesChofer;
        private Mock<IAutorizarTransportistaInhabilitadoService> contractMock;
        private Guid guid;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IAutorizarTransportistaInhabilitadoService>>();
            target = new AutorizarTransportistaInhabilitadoController(null, actFactoryMock.Object, servRepositorioMock.Object);
            contractMock = new Mock<IAutorizarTransportistaInhabilitadoService>();

            inthabilitacionesCamion = new List<InhabilitacionCamionDto>
                {
                    new InhabilitacionCamionDto
                        {
                            Id = 1,
                            Patente = "AAA123",
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            Motivo = "m"
                        },
                    new InhabilitacionCamionDto
                        {
                            Id = 2,
                            Patente = "AAA123",
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            Motivo = "m"
                        }
                };
            inthabilitacionesChofer = new List<InhabilitacionChoferDto>
                {
                    new InhabilitacionChoferDto
                        {
                            Id = 1,
                            TipoDocumentoIdentidadId = 1,
                            NumeroDeDocumento = "123",
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            Motivo = "m"
                        },
                    new InhabilitacionChoferDto
                        {
                            Id = 2,
                            TipoDocumentoIdentidadId = 1,
                            NumeroDeDocumento = "1234",
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            Motivo = "m"
                        },
                };

            guid = new Guid("11111111111111111111111111111111");
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarInhabilitacionCamionPaginada(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<InhabilitacionCamionDto>(inthabilitacionesCamion, 1, 2, 2));
            servRepositorioMock.Setup(s => s.ListarInhabilitacionChoferPaginada(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<InhabilitacionChoferDto>(inthabilitacionesChofer, 1, 2, 2));

            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(guid))
                .Returns(new RecorridoDto{ Centro = new CentroDto{Id = 1}, Patente = "TES123", Chofer = new ChoferDto{ Id = 1}});


            var result = target.Index(guid) as ViewResult;
            IEnumerable<InhabilitacionCamionDto> camion = target.ViewBag.InhabilitacionCamion;
            IEnumerable<InhabilitacionChoferDto> chofer = target.ViewBag.InhabilitacionChofer;

            Assert.IsTrue(camion.Count() == inthabilitacionesCamion.Count);
            Assert.IsTrue(chofer.Count() == inthabilitacionesChofer.Count);
        }

        [Test]
        public void TestTerminarInhabilitaciones()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.AutorizarTransportistaInhabilitado(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>())).Returns(true);

            var viewResult = target.TerminarInhabilitaciones("", 1, guid, new DatosUsuario(), "") as ViewResult;

            contractMock.Verify(p => p.AutorizarTransportistaInhabilitado(It.IsAny<Guid>(), true, true, It.IsAny<ControlRecorridoDto>()));
        }

        [Test]
        public void TestAutorizar()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.AutorizarTransportistaInhabilitado(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>())).Returns(true);
            
            var viewResult = target.Autorizar("", 1, guid, new DatosUsuario(), "") as ViewResult;

            contractMock.Verify(p => p.AutorizarTransportistaInhabilitado(It.IsAny<Guid>(), true, false, It.IsAny<ControlRecorridoDto>()));
        }

        [Test]
        public void TestRechazar()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.AutorizarTransportistaInhabilitado(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>())).Returns(true);

            var viewResult = target.Rechazar("", 1, guid, new DatosUsuario()) as ViewResult;

            contractMock.Verify(p => p.AutorizarTransportistaInhabilitado(It.IsAny<Guid>(), false, false, It.IsAny<ControlRecorridoDto>()));
        }
    }
}
