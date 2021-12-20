using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class VerificacionCotControllerTest
    {
        private VerificacionCotController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandoMock;
        private Mock<IServicioActividadFactory<IVerificacionCotService>> actFactory;
        private Mock<IVerificacionCotService> contractMock;
        private NullLogger logger;
        private RecorridoDto recorridoDto;
        private ControlRecorridoDto controlRecorridoDto;
        private DatosUsuario datosUsuario;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandoMock = new Mock<IServicioComandos>();
            actFactory = new Mock<IServicioActividadFactory<IVerificacionCotService>>();
            contractMock = new Mock<IVerificacionCotService>();
            logger = new NullLogger();
            target = new VerificacionCotController(logger, actFactory.Object, servRepositorioMock.Object);

            recorridoDto = new RecorridoDto
                {
                    Centro = new CentroDto {Id = 1, Descripcion = "W"},
                    Patente = "AAA111",
                    Chofer = new ChoferDto {Nombre = "Walter", Id = 1, NumeroDeDocumento = "111111111"},
                    Id = 1,
                    InstanciaWorkflow = new Guid("E969E570-E929-4E29-9ACC-B3A8F96ACACC"),
                    FechaInicio = new DateTime(2015,6,4),
                    Workflow = new WorkflowDto
                        {
                            Activo = true,
                            CentroId = 1,
                            Id = 1, 
                            Codigo = "W1"
                        },
                    NumeroDocumentoIngreso = "111",
                    WorkflowDefinicionId = 1
                };

            controlRecorridoDto = new ControlRecorridoDto
                {
                    ActividadXaml = "VerificacionCotController",
                    Fecha = new DateTime(2015, 6, 4),
                    Id = 1,
                    WorkflowInstanceId = recorridoDto.InstanciaWorkflow,
                    NombreUsuario = "wandino"
                };

            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorridoDto);
            datosUsuario = new DatosUsuario
            {
                CentroId = 1,
                NombreUsuario = "wandino"
            };
        }

        [Test]
        public void IndexTest()
        {
            
            servRepositorioMock.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                               .Returns(controlRecorridoDto);

            var result = target.Index(recorridoDto.InstanciaWorkflow, datosUsuario) as ViewResult;
            Assert.NotNull(result);
        }

        [Test]
        public void IndexSinControlRecorridoTest()
        {
            var result = target.Index(recorridoDto.InstanciaWorkflow, datosUsuario) as ViewResult;
            Assert.NotNull(result);
        }
        
        [Test]
        public void IndexPostTest()
        {
            actFactory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.VerificacionCot(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<bool>(),
                                  It.IsAny<string>())).Returns(new Resultado());
                          
            var result = target.Index(recorridoDto.Workflow.Codigo, recorridoDto.WorkflowDefinicionId,
                                      recorridoDto.InstanciaWorkflow, datosUsuario, true, "123") as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
        }

    }
}
