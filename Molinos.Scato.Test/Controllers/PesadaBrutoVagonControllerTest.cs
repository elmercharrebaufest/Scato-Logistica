using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
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
    public class PesadaBrutoVagonControllerTest
    {
        private PesadaBrutoVagonController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioActividadFactory<IPesadaService>> actFactoryMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private Mock<IListaDeWorkflows> workflowMock;
        private NullLogger logger;
        private RecorridoDto recorrido;
        private CentroDto centro;
        private Guid instancia;
        private List<BalanzaDto> balanzas;
        private List<AlmacenDto> almacenes;
        private DatosUsuario datosUsuario;
        private List<MotivoDto> motivos;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<IPesadaService>>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            workflowMock = new Mock<IListaDeWorkflows>();
            logger = new NullLogger();
            target = new PesadaBrutoVagonController(logger, servRepositorioMock.Object, actFactoryMock.Object, servComandosMock.Object, orquestadorMock.Object, workflowMock.Object);
            instancia = new Guid("25892e17-80f6-415f-9c65-7395632f0223");

            centro = new CentroDto { Id = 1, Descripcion = "Centro 1" };

            recorrido = new RecorridoDto
            {
                Id = 1,
                Almacen = new AlmacenDto { Id = 1, CentroId = 1, Descripcion = "Almacen 1" },
                Centro = centro,
                Chofer = new ChoferDto { Id = 1, Nombre = "Chofer" },
                DatosProximaActividad = "Tara",
                InstanciaWorkflow = instancia,
                Material = new MaterialDto { Id = 1, Descripcion = "MaterialDesc 1" },
                NumeroDocumentoIngreso = "1111",
                Patente = "AAA111",
                TipoComercial = new TipoComercialDto { Id = 1, Descripcion = "Tipo 1" },
                TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                Workflow = new WorkflowDto { Id = 1, Descripcion = "EgresoMaterialNoProductivo" }
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
            motivos = new List<MotivoDto>
                {
                    new MotivoDto
                        {
                            Id = 1,
                            Descripcion = "motivo1",
                            DescripcionCorta = "U1"
                        },
                    new MotivoDto
                        {
                            Id = 2,
                             Descripcion = "motivo2",
                            DescripcionCorta = "U2"
                        },
                };
            almacenes = new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "Almacen 1", CentroId = 1 } };

            datosUsuario = new DatosUsuario { CentroId = 1 };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ListarBalanzas(It.IsAny<int>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarBalanzasPorNombrePc(It.IsAny<int>(), It.IsAny<string>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarCalles(1)).Returns(new List<CalleDto>());
            servRepositorioMock.Setup(s => s.ListarHidraulicas(It.IsAny<int>(), It.IsAny<bool>())).Returns(new List<PuestosDeCargaDescargaDto>());
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorMaterialYCentro(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            servRepositorioMock.Setup(s => s.ListarMotivos()).Returns(motivos);

            var result = target.Index(It.IsAny<Guid>(), datosUsuario) as ViewResult;
            var model = (Pesada)result.Model;
            Assert.That(result.ViewName, Is.EqualTo("~/Views/Pesada/Index.cshtml"));
            Assert.That(((List<SelectListItem>)target.ViewBag.Balanzas).Select(s => s.Text), Is.EquivalentTo(balanzas.Select(s => s.Nombre)));
            Assert.That(((List<SelectListItem>)target.ViewBag.Almacenes).Select(s => s.Text), Is.EquivalentTo(almacenes.Select(s => s.DescripcionCorta)));
            Assert.That(((List<SelectListItem>)target.ViewBag.Motivos).Select(s => s.Text), Is.EquivalentTo(motivos.Select(s => s.Descripcion)));
            Assert.That((string)target.ViewBag.NumeroDocumentoIngreso, Is.EqualTo(recorrido.NumeroDocumentoIngreso));
            Assert.That((TipoDocumentoIngreso)target.ViewBag.DocumentoIngreso, Is.EqualTo(recorrido.TipoDocumentoIngreso));
            Assert.That((string)target.ViewBag.Material, Is.EqualTo(recorrido.Material.Descripcion));
            Assert.That((int?)target.ViewBag.PesoBrutoOrigen, Is.EqualTo(recorrido.PesoBrutoOrigen));
            Assert.That((int?)target.ViewBag.PesoTaraOrigen, Is.EqualTo(recorrido.PesoTaraOrigen));
            Assert.That((int?)target.ViewBag.PesoBruto, Is.EqualTo(recorrido.PesoBruto));
            Assert.That((int?)target.ViewBag.PesoTara, Is.EqualTo(recorrido.PesoTara));
            Assert.That(model.WorkflowInstanceId, Is.EqualTo(instancia));
            Assert.That(model.TipoPesada, Is.EqualTo(TipoPesada.Bruto));
            Assert.That(model.AlmacenId, Is.EqualTo(recorrido.Almacen.Id));
            Assert.That(model.BalanzaId, Is.EqualTo(datosUsuario.BalanzaId));
            Assert.That(model.Patente, Is.EqualTo(recorrido.Patente));
        }
    }
}
