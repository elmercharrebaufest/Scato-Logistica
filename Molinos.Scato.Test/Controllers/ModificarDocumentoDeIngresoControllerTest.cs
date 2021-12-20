using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class ModificarDocumentoDeIngresoControllerTest
    {
        private ModificarDocumentoDeIngresoController target;
        private Mock<IListaDeWorkflows> listaWf;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomandos;
        private NullLogger log;
        private DatosUsuario user;
        private Guid instanceId;
        private RecorridoDto recorridoDto;
        private List<PesoMaximoPorTipoVehiculoDto> pesosMax;

        [SetUp]
        public void SetUp()
        {
            user = new DatosUsuario
            {
                PuestoDeTrabajoId = 1,
                BalanzaId = 1,
                CentroId = 1,
                NombreUsuario = "W",
                NombrePc = "Bf"
            };
            instanceId = Guid.NewGuid();
            recorridoDto = new RecorridoDto
            {
                Id = 1,
                WorkflowDefinicionId = 1,
                InstanciaWorkflow = instanceId,
                TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                Vehiculo = new VehiculoDto { Id = 1 },
                Workflow = new WorkflowDto { TipoDeWorkflow = TipoDeWorkflow.Ingreso, CentroId = 1, Codigo = "w", Descripcion = "Wf", Id = 1 },
                Centro = new CentroDto { Id = 1 },
                Patente = "AAA111"
            };
            pesosMax = new List<PesoMaximoPorTipoVehiculoDto>()
            {
                new PesoMaximoPorTipoVehiculoDto()
                {
                    Activo = true,
                    CentroId = 1,
                    Id = 1,
                    PesoMaxEgreso = 65000,
                    PesoMaxIngreso = 65000,
                    TipoVehiculo = TipoVehiculo.Bitren
                }
            };
            log = new NullLogger();
            listaWf = new Mock<IListaDeWorkflows>();
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomandos = new Mock<IServicioComandos>();
            target = new ModificarDocumentoDeIngresoController(log, servRepositorio.Object, servcomandos.Object, listaWf.Object);


            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>()))
                           .Returns(recorridoDto);
            servRepositorio.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>())).Returns(new WorkflowDto());
            servRepositorio.Setup(
                s =>
                s.ListarPaginadoRecorridosPorDocumentoPatenteYCentro(It.IsAny<TipoDocumentoIngreso>(), It.IsAny<string>(),
                                                                     It.IsAny<string>(), It.IsAny<string>(),
                                                                     It.IsAny<int>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<RecorridoDto>(new List<RecorridoDto> { recorridoDto }, 1, 1, 1));

            servRepositorio.Setup(s => s.ListarPaginadoRecorridosPorModificarDocumentoDeIngreso(
                                                    It.IsAny<int>(),
                                                    It.IsAny<Paginacion>(),
                                                    It.IsAny<ModificarDocumentoDeIngresoDto>()))
                    .Returns(new ListaPaginada<RecorridoDto>(new List<RecorridoDto> { recorridoDto }, 1, 1, 1));

            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns(recorridoDto);
            servRepositorio.Setup(s => s.BuscarChoferes(It.IsAny<ChoferFiltro>()))
                           .Returns(new List<ChoferDto> { new ChoferDto { Id = 1, Nombre = "C", Apellido = "a" } });
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto());
            servRepositorio.Setup(x => x.ListarPesoMaximoPorTipoVehiculoPorCentro(It.IsAny<int>())).Returns(pesosMax);
            servRepositorio.Setup(s => s.ListarAlmacenesPorCentro(It.IsAny<int>()))
                 .Returns(new List<AlmacenDto>());
        }


        [Test]
        public void DocumentoOrigenDocumentoNoEditableError()
        {
            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.RemitoBodegaUvaPropia, false) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["error"], "El Tipo de Documento seleccionado no puede ser modificado");
        }

        [Test]
        public void DocumentoOrigenCartaPorte()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad())
                .Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>()))
                .Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(materiales);
            servRepositorio.Setup(s => s.ListarTecnologias())
                           .Returns(new List<TecnologiaDto> { new TecnologiaDto { Codigo = "a", Id = 1, Nombre = "T" } });
            servRepositorio.Setup(s => s.ObtenerCartaPorte(It.IsAny<int>()))
                .Returns(new CartaPorteDto());
            servRepositorio.Setup(s => s.ListarCategorias())
              .Returns(new List<CategoriaDto> { new CategoriaDto { Id = 1, Clasificacion = "clasificacion1" }, new CategoriaDto { Id = 2, Clasificacion = "clasificacion2" } });
            servRepositorio.Setup(x => x.ObtenerFotoCPDeCartaDePortePorrecorrido(It.IsAny<Guid>())).Returns(new FotosDto { Fotos = new List<FotoDto>() });
            


           var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.CartaPorte, false) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "CartaPorte");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void DocumentoOrigenOrdenCargaInterna()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenCargaInternaPorInstanceId(It.IsAny<Guid>())).Returns(new OrdenCargaInternaDto());

            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.OrdenCargaInterna, false) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "OrdenCargaInterna");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void DocumentoOrigenOrdenDeDescarga()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenDeDescargaPorNumero(It.IsAny<string>())).Returns(new OrdenDeDescargaDto());

            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.OrdenDeDescarga, false) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "OrdenDeDescarga");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void DocumentoOrigenOrdenCargaFas()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenCargaFasPorInstanceId(It.IsAny<Guid>())).Returns(new OrdenCargaFasDto());
            servRepositorio.Setup(s => s.ListarKmPorProveedorYCentro(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<KmPorProveedorDto>());

            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.OrdenCargaFas, false) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "OrdenCargaFas");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void DocumentoOrigenOrdenCargaInternaFason()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenCargaInternaFasonPorInstanceId(It.IsAny<Guid>())).Returns(new OrdenCargaInternaFasonDto());
            servRepositorio.Setup(s => s.ListarKmPorProveedorYCentro(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<KmPorProveedorDto>());

            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.OrdenCargaInternaFason, false) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "OrdenCargaInternaFason");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void DocumentoOrigenOrdenDeDescargaFason()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenDeDescargaFasonPorNumero(It.IsAny<string>())).Returns(new OrdenDeDescargaFasonDto());

            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.OrdenDeDescargaFason, false) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "OrdenDeDescargaFason");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void DocumentoOrigenOrdenEntrePlantas()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenEntrePlantasPorInstanceId(It.IsAny<Guid>())).Returns(new OrdenEntrePlantasDto());

            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.OrdenEntrePlantas, false) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "OrdenEntrePlantas");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void DocumentoOrigenOrdenDeCargaContenedor()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenDeCargaContenedorPorInstanceId(It.IsAny<Guid>())).Returns(new OrdenDeCargaContenedorDto());
            servRepositorio.Setup(s => s.ListarTaraContenedores())
                           .Returns(new List<TaraContenedorDto> { new TaraContenedorDto { Descripcion = "TaraContenedor", Id = 1 } });

            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.OrdenDeCargaContenedor, false) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "OrdenDeCargaContenedor");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void DocumentoOrigenRemito()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerRemitoPorOrdenDeDescarga(It.IsAny<string>())).Returns(new RemitoDto());

            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.Remito, false) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "Remito");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void DocumentoOrigenRemitoBodegaUvaPropia()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerRemitoBodegaUvaPorInstanceId(It.IsAny<Guid>())).Returns(new RemitoBodegaUvaDto());
            servRepositorio.Setup(
                s => s.ListarMaterialesPorWorkflowYVinedo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new List<MaterialPorWorkflowDto>
                               {
                                   new MaterialPorWorkflowDto {MaterialDesc = "MatUva", MaterialId = 1}
                               });
            servRepositorio.Setup(s => s.ListarTiposVehiculoBodega())
                           .Returns(new List<TipoVehiculoBodegaDto>
                               {
                                   new TipoVehiculoBodegaDto {Descripcion = "A", Id = 1}
                               });
            servRepositorio.Setup(s => s.ListarVinedosPropios())
                           .Returns(new List<VinedoPropioDto>
                               {
                                   new VinedoPropioDto {NumeroINV = "123", Descripcion = "V", Id = 1}
                               });
            servRepositorio.Setup(s => s.ListarMaterialesBin(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ClaseBin?>()))
                           .Returns(new List<TipoBinDto> { new TipoBinDto { Descripcion = "Bin", Id = 1, Peso = 1 } });
            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.RemitoBodegaUvaPropia, true) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "RemitoBodegaUvaPropia");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void DocumentoOrigenRemitoBodegaUvaTerceros()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerRemitoBodegaUvaPorInstanceId(It.IsAny<Guid>())).Returns(new RemitoBodegaUvaDto());
            servRepositorio.Setup(
                s => s.ListarMaterialesPorWorkflowYVinedo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new List<MaterialPorWorkflowDto>
                               {
                                   new MaterialPorWorkflowDto {MaterialDesc = "MatUva", MaterialId = 1}
                               });
            servRepositorio.Setup(s => s.ListarTiposVehiculoBodega())
                           .Returns(new List<TipoVehiculoBodegaDto>
                               {
                                   new TipoVehiculoBodegaDto {Descripcion = "A", Id = 1}
                               });
            servRepositorio.Setup(s => s.ListarVinedosPropios())
                           .Returns(new List<VinedoPropioDto>
                               {
                                   new VinedoPropioDto {NumeroINV = "123", Descripcion = "V", Id = 1}
                               });
            servRepositorio.Setup(s => s.ListarMaterialesBin(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ClaseBin?>()))
                           .Returns(new List<TipoBinDto> { new TipoBinDto { Descripcion = "Bin", Id = 1, Peso = 1 } });
            servRepositorio.Setup(s => s.ListarVinedosTerceros(It.IsAny<int>()))
                           .Returns(new List<VinedoTercerosDto>
                               {
                                   new VinedoTercerosDto {NumeroINV = "1234", Descripcion = "UT"}
                               });
            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.RemitoBodegaUvaTerceros, true) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "RemitoBodegaUvaTerceros");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void DocumentoOrigenRemitoBodegaVino()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerRemitoBodegaVinoPorInstanceId(It.IsAny<Guid>())).Returns(new RemitoBodegaVinoDto());
            servRepositorio.Setup(
                s => s.ListarMaterialesPorWorkflowYVinedo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new List<MaterialPorWorkflowDto>
                               {
                                   new MaterialPorWorkflowDto {MaterialDesc = "MatUva", MaterialId = 1}
                               });
            servRepositorio.Setup(s => s.ListarTiposVehiculoBodega())
                           .Returns(new List<TipoVehiculoBodegaDto>
                               {
                                   new TipoVehiculoBodegaDto {Descripcion = "A", Id = 1}
                               });
            servRepositorio.Setup(s => s.ListarVinedosPropios())
                           .Returns(new List<VinedoPropioDto>
                               {
                                   new VinedoPropioDto {NumeroINV = "123", Descripcion = "V", Id = 1}
                               });
            servRepositorio.Setup(s => s.ListarMaterialesBin(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ClaseBin?>()))
                           .Returns(new List<TipoBinDto> { new TipoBinDto { Descripcion = "Bin", Id = 1, Peso = 1 } });
            servRepositorio.Setup(s => s.ListarVinedosTerceros(It.IsAny<int>()))
                           .Returns(new List<VinedoTercerosDto>
                               {
                                   new VinedoTercerosDto {NumeroINV = "1234", Descripcion = "UT"}
                               });
            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.RemitoBodegaVino, true) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "RemitoBodegaVino");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void DocumentoOrigenHojaDeRuta()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerHojaDeRutaPorInstanceId(It.IsAny<Guid>())).Returns(new HojaDeRutaDto());
            servRepositorio.Setup(
                s => s.ListarMaterialesPorWorkflowYVinedo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new List<MaterialPorWorkflowDto>
                               {
                                   new MaterialPorWorkflowDto {MaterialDesc = "MatUva", MaterialId = 1}
                               });
            servRepositorio.Setup(s => s.ListarTiposVehiculoBodega())
                           .Returns(new List<TipoVehiculoBodegaDto>
                               {
                                   new TipoVehiculoBodegaDto {Descripcion = "A", Id = 1}
                               });
            servRepositorio.Setup(s => s.ListarVinedosPropios())
                           .Returns(new List<VinedoPropioDto>
                               {
                                   new VinedoPropioDto {NumeroINV = "123", Descripcion = "V", Id = 1}
                               });
            servRepositorio.Setup(s => s.ListarMaterialesBin(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ClaseBin?>()))
                           .Returns(new List<TipoBinDto> { new TipoBinDto { Descripcion = "Bin", Id = 1, Peso = 1 } });
            servRepositorio.Setup(s => s.ListarVinedosTerceros(It.IsAny<int>()))
                           .Returns(new List<VinedoTercerosDto>
                               {
                                   new VinedoTercerosDto {NumeroINV = "1234", Descripcion = "UT"}
                               });
            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.HojaDeRuta, true) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "HojaDeRuta");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void DocumentoOrigenHojaDeRutaYerbatera()
        {
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerHojaDeRutaYerbateraPorInstanceId(It.IsAny<Guid>())).Returns(new HojaDeRutaYerbateraDto());
            servRepositorio.Setup(
                s => s.ListarMaterialesPorWorkflowYVinedo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new List<MaterialPorWorkflowDto>
                               {
                                   new MaterialPorWorkflowDto {MaterialDesc = "MatUva", MaterialId = 1}
                               });
            servRepositorio.Setup(s => s.ListarTiposVehiculoBodega())
                           .Returns(new List<TipoVehiculoBodegaDto>
                               {
                                   new TipoVehiculoBodegaDto {Descripcion = "A", Id = 1}
                               });
            servRepositorio.Setup(s => s.ListarVinedosPropios())
                           .Returns(new List<VinedoPropioDto>
                               {
                                   new VinedoPropioDto {NumeroINV = "123", Descripcion = "V", Id = 1}
                               });
            servRepositorio.Setup(s => s.ListarMaterialesBin(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ClaseBin?>()))
                           .Returns(new List<TipoBinDto> { new TipoBinDto { Descripcion = "Bin", Id = 1, Peso = 1 } });
            servRepositorio.Setup(s => s.ListarVinedosTerceros(It.IsAny<int>()))
                           .Returns(new List<VinedoTercerosDto>
                               {
                                   new VinedoTercerosDto {NumeroINV = "1234", Descripcion = "UT"}
                               });
            var result = target.DocumentoOrigen(1, TipoDocumentoIngreso.HojaDeRutaYerbatera, true) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.AreEqual(result.ViewName, "HojaDeRutaYerbatera");
            Assert.That(codigo, Is.EqualTo("w"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void OrdenDeCargaInternaResultadoSetearChoferFalse()
        {
            var res = new Resultado();
            res.Error("E", "SetearChofer false");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(res);

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenCargaInternaPorInstanceId(It.IsAny<Guid>())).Returns(new OrdenCargaInternaDto());

            var result = target.OrdenCargaInterna("w",
                                                  new OrdenCargaInternaDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      PatenteCamion = "AAA111",
                                                      PatenteAcoplado = "213123"
                                                  }, 1, user) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void OrdenDeCargaInternaResultadoSetearTransportistaFalse()
        {
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { TransportistaEsProveedor = true, Id = 0 });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenCargaInternaPorInstanceId(It.IsAny<Guid>())).Returns(new OrdenCargaInternaDto());

            var result = target.OrdenCargaInterna("w",
                                                  new OrdenCargaInternaDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      PatenteCamion = "AAA111",
                                                      PatenteAcoplado = "213123"
                                                  }, 1, user) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void OrdenDeCargaInternaModificar()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenCargaInterna>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenCargaInternaPorInstanceId(It.IsAny<Guid>())).Returns(new OrdenCargaInternaDto());

            var result = target.OrdenCargaInterna("w",
                                                  new OrdenCargaInternaDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      PatenteCamion = "AAA111",
                                                      PatenteAcoplado = "213123",
                                                      EsTransportista = true
                                                  }, 1, user) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void OrdenDeCargaInternaModificarError()
        {
            var res = new Resultado();
            res.Error("E", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenCargaInterna>())).Returns(res);
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenCargaInternaPorInstanceId(It.IsAny<Guid>())).Returns(new OrdenCargaInternaDto());

            var result = target.OrdenCargaInterna("w",
                                                  new OrdenCargaInternaDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      PatenteCamion = "AAA111",
                                                      PatenteAcoplado = "213123",
                                                      EsTransportista = true
                                                  }, 1, user) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(target.ModelState.FirstOrDefault().Value.Errors.FirstOrDefault().ErrorMessage, "error");
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void CartaPorteResultadoSetearChoferFalse()
        {
            var res = new Resultado();
            res.Error("E", "SetearChofer false");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(res);

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ListarTecnologias())
                           .Returns(new List<TecnologiaDto> { new TecnologiaDto { Codigo = "a", Id = 1, Nombre = "T" } });
            servRepositorio.Setup(s => s.ObtenerCartaPorte(It.IsAny<int>())).Returns(new CartaPorteDto());
            servRepositorio.Setup(s => s.ListarCategorias())
              .Returns(new List<CategoriaDto> { new CategoriaDto { Id = 1, Clasificacion = "clasificacion1" }, new CategoriaDto { Id = 2, Clasificacion = "clasificacion2" } });

            var result = target.CartaPorte("w", new CartaPorteDto { Chofer = new ChoferDto { Cuil = "30123456798" } }, 1, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void CartaPorteResultadoSetearTransportistaFalse()
        {
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { TransportistaEsProveedor = true, Id = 0 });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ListarTecnologias())
                           .Returns(new List<TecnologiaDto> { new TecnologiaDto { Codigo = "a", Id = 1, Nombre = "T" } });
            servRepositorio.Setup(s => s.ObtenerCartaPorte(It.IsAny<int>())).Returns(new CartaPorteDto());
            servRepositorio.Setup(s => s.ListarCategorias())
              .Returns(new List<CategoriaDto> { new CategoriaDto { Id = 1, Clasificacion = "clasificacion1" }, new CategoriaDto { Id = 2, Clasificacion = "clasificacion2" } });

            var result = target.CartaPorte("w", new CartaPorteDto { Chofer = new ChoferDto { Cuil = "30123456798" } }, 1, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void CartaPorteModificar()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarCartaPorte>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });

            var result = target.CartaPorte("w",
                                                  new CartaPorteDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      Vehiculos = new Collection<VehiculoDto> { new VehiculoDto { Patente = "AAA111", PatenteAcoplado = "AAA121" } },
                                                      TipoVehiculo = TipoVehiculo.Camión
                                                  }, 1, user) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void CartaPorteModificarError()
        {
            var res = new Resultado();
            res.Error("E", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarCartaPorte>())).Returns(res);
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });


            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ListarTecnologias())
                           .Returns(new List<TecnologiaDto> { new TecnologiaDto { Codigo = "a", Id = 1, Nombre = "T" } });
            servRepositorio.Setup(s => s.ObtenerCartaPorte(It.IsAny<int>())).Returns(new CartaPorteDto());
            servRepositorio.Setup(s => s.ListarCategorias())
              .Returns(new List<CategoriaDto> { new CategoriaDto { Id = 1, Clasificacion = "clasificacion1" }, new CategoriaDto { Id = 2, Clasificacion = "clasificacion2" } });
            var result = target.CartaPorte("w",
                                                  new CartaPorteDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      Vehiculos = new Collection<VehiculoDto> { new VehiculoDto { Id = 1, Patente = "Aaa111" } }
                                                  }, 1, user) as ViewResult;
            Assert.NotNull(result);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            ModelState errorModificarCP;
            target.ModelState.TryGetValue("E", out errorModificarCP);
            Assert.NotNull(errorModificarCP);
            Assert.AreEqual(errorModificarCP.Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void CartaPorteSinVehiculoError()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarCartaPorte>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });


            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ListarTecnologias())
                           .Returns(new List<TecnologiaDto> { new TecnologiaDto { Codigo = "a", Id = 1, Nombre = "T" } });
            servRepositorio.Setup(s => s.ObtenerCartaPorte(It.IsAny<int>())).Returns(new CartaPorteDto());
            servRepositorio.Setup(s => s.ListarCategorias())
              .Returns(new List<CategoriaDto> { new CategoriaDto { Id = 1, Clasificacion = "clasificacion1" }, new CategoriaDto { Id = 2, Clasificacion = "clasificacion2" } });
            var result = target.CartaPorte("w",
                                                  new CartaPorteDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true
                                                  }, 1, user) as ViewResult;
            Assert.NotNull(result);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            ModelState errorVehiculo;
            target.ModelState.TryGetValue("Vehiculo", out errorVehiculo);
            Assert.NotNull(errorVehiculo);
            Assert.AreEqual(errorVehiculo.Errors.FirstOrDefault().ErrorMessage, string.Format(Textos.Error_Requerido, "Vagones"));
        }

        [Test]
        public void OrdenDeDescargaFasonResultadoSetearChoferFalse()
        {
            var res = new Resultado();
            res.Error("E", "SetearChofer false");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(res);

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenDeDescargaFasonPorNumero(It.IsAny<string>())).Returns(new OrdenDeDescargaFasonDto());

            var result = target.OrdenDeDescargaFason("w", new OrdenDeDescargaFasonDto { Chofer = new ChoferDto { Cuil = "123456789" } }, 1, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void OrdenDeDescargaFasonResultadoSetearTransportistaFalse()
        {
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { TransportistaEsProveedor = true, Id = 0 });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerOrdenDeDescargaFasonPorNumero(It.IsAny<string>())).Returns(new OrdenDeDescargaFasonDto());

            var result = target.OrdenDeDescargaFason("w", new OrdenDeDescargaFasonDto { Chofer = new ChoferDto { Cuil = "12345678" } }, 1, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));

        }

        [Test]
        public void OrdenDeDescargaFasonModificar()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenDeDescargaFason>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });

            var result = target.OrdenDeDescargaFason("w",
                                                  new OrdenDeDescargaFasonDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true
                                                  }, 1, user) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void OrdenDeDescargaFasonModificarError()
        {
            var res = new Resultado();
            res.Error("E", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenDeDescargaFason>())).Returns(res);
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });


            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            var result = target.OrdenDeDescargaFason("w",
                                                  new OrdenDeDescargaFasonDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true
                                                  }, 1, user) as ViewResult;
            Assert.NotNull(result);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            ModelState errorModificar;
            target.ModelState.TryGetValue("E", out errorModificar);
            Assert.NotNull(errorModificar);
            Assert.AreEqual(errorModificar.Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void OrdenDeDescargaResultadoSetearChoferFalse()
        {
            var res = new Resultado();
            res.Error("E", "SetearChofer false");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(res);

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);

            var result = target.OrdenDeDescarga("w", new OrdenDeDescargaDto { Chofer = new ChoferDto { Cuil = "123456789" }, RecorridoId = recorridoDto.Id }, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void OrdenDeDescargaResultadoSetearTransportistaFalse()
        {
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { TransportistaEsProveedor = true, Id = 0 });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);

            var result = target.OrdenDeDescarga("w", new OrdenDeDescargaDto { Chofer = new ChoferDto { Cuil = "12345678" }, RecorridoId = recorridoDto.Id }, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void OrdenDeDescargaModificar()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenDeDescarga>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });

            var result = target.OrdenDeDescarga("w",
                                                  new OrdenDeDescargaDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      RecorridoId = recorridoDto.Id,
                                                      PatenteCamion = "aaa111",
                                                      PatenteAcoplado = "aaa222"
                                                  }, user) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void OrdenDeDescargaModificarError()
        {
            var res = new Resultado();
            res.Error("E", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenDeDescarga>())).Returns(res);
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });


            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            var result = target.OrdenDeDescarga("w",
                                                  new OrdenDeDescargaDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      RecorridoId = recorridoDto.Id,
                                                      PatenteCamion = "aaa111",
                                                      PatenteAcoplado = "aaa222"
                                                  }, user) as ViewResult;
            Assert.NotNull(result);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            ModelState errorModificar;
            target.ModelState.TryGetValue("E", out errorModificar);
            Assert.NotNull(errorModificar);
            Assert.AreEqual(errorModificar.Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void OrdenEntrePlantasResultadoSetearChoferFalse()
        {
            var res = new Resultado();
            res.Error("E", "SetearChofer false");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(res);

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);

            var result = target.OrdenEntrePlantas("w", new OrdenEntrePlantasDto { Chofer = new ChoferDto { Cuil = "123456789" } }, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void OrdenEntrePlantasResultadoSetearTransportistaFalse()
        {
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { TransportistaEsProveedor = true, Id = 0 });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);

            var result = target.OrdenEntrePlantas("w", new OrdenEntrePlantasDto { Chofer = new ChoferDto { Cuil = "12345678" } }, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));

        }

        [Test]
        public void OrdenEntrePlantasModificar()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenEntrePlantas>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });

            var result = target.OrdenEntrePlantas("w",
                                                  new OrdenEntrePlantasDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      PatenteCamion = "aaa111",
                                                      PatenteAcoplado = "aaa222"
                                                  }, user) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void OrdenEntrePlantasModificarError()
        {
            var res = new Resultado();
            res.Error("E", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenEntrePlantas>())).Returns(res);
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });


            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            var result = target.OrdenEntrePlantas("w",
                                                  new OrdenEntrePlantasDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      PatenteAcoplado = "aaa111",
                                                      PatenteCamion = "aaa112"
                                                  }, user) as ViewResult;
            Assert.NotNull(result);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            ModelState errorModificar;
            target.ModelState.TryGetValue("E", out errorModificar);
            Assert.NotNull(errorModificar);
            Assert.AreEqual(errorModificar.Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void OrdenEntrePlantasModeloInvalido()
        {
            target.ModelState.AddModelError("E", "error");

            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });


            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            var result = target.OrdenEntrePlantas("w",
                                                  new OrdenEntrePlantasDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      PatenteAcoplado = "aaa111",
                                                      PatenteCamion = "aaa112"
                                                  }, user) as ViewResult;
            Assert.NotNull(result);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.False(target.ModelState.IsValid);
            ModelState errorModificar;
            target.ModelState.TryGetValue("E", out errorModificar);
            Assert.NotNull(errorModificar);
            Assert.AreEqual(errorModificar.Errors.FirstOrDefault().ErrorMessage, "error");

        }

        [Test]
        public void OrdenCargaFasResultadoSetearChoferFalse()
        {
            var res = new Resultado();
            res.Error("E", "SetearChofer false");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(res);

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);

            var result = target.OrdenCargaFas("w", new OrdenCargaFasDto { Chofer = new ChoferDto { Cuil = "123456789" } }, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void OrdenCargaFasResultadoCrearTransportista()
        {
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { TransportistaEsProveedor = true, Id = 0 });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ObtenerProveedorPorCuit(It.IsAny<string>(), It.IsAny<TiposProveedor>()))
                           .Returns(new ProveedorDto { Cuil = "12345", Domicilio = "Dom 123" });
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenCargaFas>())).Returns(new Resultado());
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<CrearTransportista>())).Returns(new ResultadoCrear());
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });

            var result = target.OrdenCargaFas("w",
                                                  new OrdenCargaFasDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      PatenteCamion = "aaa111",
                                                      PatenteAcoplado = "aaa222",
                                                      TransportistaId = 1,
                                                      CuitTransporte = "1234"
                                                  }, user) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");

        }

        [Test]
        public void OrdenCargaFasResultadoCrearTransportistaError()
        {
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { TransportistaEsProveedor = true, Id = 0 });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ObtenerProveedorPorCuit(It.IsAny<string>(), It.IsAny<TiposProveedor>()))
                           .Returns(new ProveedorDto { Cuil = "12345", Domicilio = "Dom 123" });
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenCargaFas>())).Returns(new Resultado());
            var errorRCrear = new ResultadoCrear();
            errorRCrear.Error("E", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<CrearTransportista>())).Returns(errorRCrear);
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });

            var result = target.OrdenCargaFas("w",
                                              new OrdenCargaFasDto
                                              {
                                                  Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                  PatenteCamion = "aaa111",
                                                  PatenteAcoplado = "aaa222",
                                                  TransportistaId = 1,
                                                  CuitTransporte = "1234"
                                              }, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));

            ModelState errorT;
            target.ModelState.TryGetValue("E", out errorT);
            Assert.NotNull(errorT);
            Assert.AreEqual(errorT.Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void OrdenCargaFasModificar()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenCargaFas>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });
            servRepositorio.Setup(s => s.ObtenerTransportistaPorCuit(It.IsAny<string>()))
                           .Returns(new TransportistaDto());

            var result = target.OrdenCargaFas("w",
                                                  new OrdenCargaFasDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      PatenteCamion = "aaa111",
                                                      PatenteAcoplado = "aaa222",
                                                      TransportistaId = 1,

                                                  }, user) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void OrdenCargaFasModificarError()
        {
            var res = new Resultado();
            res.Error("E", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenCargaFas>())).Returns(res);
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });
            servRepositorio.Setup(s => s.ObtenerTransportistaPorCuit(It.IsAny<string>()))
                           .Returns(new TransportistaDto());

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            var result = target.OrdenCargaFas("w",
                                                  new OrdenCargaFasDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      PatenteAcoplado = "aaa111",
                                                      PatenteCamion = "aaa112",
                                                      TransportistaId = 1
                                                  }, user) as ViewResult;
            Assert.NotNull(result);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            ModelState errorModificar;
            target.ModelState.TryGetValue("E", out errorModificar);
            Assert.NotNull(errorModificar);
            Assert.AreEqual(errorModificar.Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void OrdenCargaFasModeloInvalido()
        {
            target.ModelState.AddModelError("E", "error");

            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });


            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);

            var result = target.OrdenCargaFas("w",
                                                  new OrdenCargaFasDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      PatenteAcoplado = "aaa111",
                                                      PatenteCamion = "aaa112"
                                                  }, user) as ViewResult;
            Assert.NotNull(result);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.False(target.ModelState.IsValid);
            ModelState errorModificar;
            target.ModelState.TryGetValue("E", out errorModificar);
            Assert.NotNull(errorModificar);
            Assert.AreEqual(errorModificar.Errors.FirstOrDefault().ErrorMessage, "error");

        }

        [Test]
        public void OrdenCargaInternaFasonResultadoSetearChoferFalse()
        {
            var res = new Resultado();
            res.Error("E", "SetearChofer false");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(res);

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);

            var result = target.OrdenCargaInternaFason("w", new OrdenCargaInternaFasonDto { Chofer = new ChoferDto { Cuil = "123456789" } }, 1, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void OrdenCargaInternaFasonResultadoSetearTransportistaFalse()
        {
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { TransportistaEsProveedor = true, Id = 0 });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);

            var result = target.OrdenCargaInternaFason("w", new OrdenCargaInternaFasonDto { Chofer = new ChoferDto { Cuil = "12345678" } }, 1, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));

        }

        [Test]
        public void OrdenCargaInternaFasonModificar()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenCargaInternaFason>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });

            var result = target.OrdenCargaInternaFason("w",
                                                  new OrdenCargaInternaFasonDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      PatenteCamion = "aaa111",
                                                      PatenteAcoplado = "aaa222"
                                                  }, 1, user) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void OrdenCargaInternaFasonModificarError()
        {
            var res = new Resultado();
            res.Error("E", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenCargaInternaFason>())).Returns(res);
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });


            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            var result = target.OrdenCargaInternaFason("w",
                                                  new OrdenCargaInternaFasonDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      PatenteAcoplado = "aaa111",
                                                      PatenteCamion = "aaa112"
                                                  }, 1, user) as ViewResult;
            Assert.NotNull(result);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            ModelState errorModificar;
            target.ModelState.TryGetValue("E", out errorModificar);
            Assert.NotNull(errorModificar);
            Assert.AreEqual(errorModificar.Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void OrdenDeCargaContenedorResultadoSetearChoferFalse()
        {
            var res = new Resultado();
            res.Error("E", "SetearChofer false");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(res);

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ListarTaraContenedores())
                           .Returns(new List<TaraContenedorDto>
                               {
                                   new TaraContenedorDto {CodigoContenedor = "A", Descripcion = "TC", Id = 1}
                               });
            servRepositorio.Setup(s => s.ObtenerTaraContenedor(It.IsAny<int>()))
                           .Returns(new TaraContenedorDto { Descripcion = "T", Id = 1, PesoTara = 45000 });

            var result = target.OrdenDeCargaContenedor("w", new OrdenDeCargaContenedorDto { Chofer = new ChoferDto { Cuil = "123456789" } }, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void OrdenDeCargaContenedorResultadoSetearTransportistaFalse()
        {
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { TransportistaEsProveedor = true, Id = 0 });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ListarTaraContenedores())
                           .Returns(new List<TaraContenedorDto>
                               {
                                   new TaraContenedorDto {CodigoContenedor = "A", Descripcion = "TC", Id = 1}
                               });
            servRepositorio.Setup(s => s.ObtenerTaraContenedor(It.IsAny<int>()))
                           .Returns(new TaraContenedorDto { Descripcion = "T", Id = 1, PesoTara = 45000 });

            var result = target.OrdenDeCargaContenedor("w", new OrdenDeCargaContenedorDto { Chofer = new ChoferDto { Cuil = "12345678" }, ContenedorEntradaId = 1, ContenedorSalidaId = 1 }, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));

        }

        [Test]
        public void OrdenDeCargaContenedorModificar()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenDeCargaContenedor>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });
            servRepositorio.Setup(s => s.ObtenerTaraContenedor(It.IsAny<int>()))
                           .Returns(new TaraContenedorDto { Descripcion = "T", Id = 1, PesoTara = 45000 });

            var result = target.OrdenDeCargaContenedor("w",
                                                  new OrdenDeCargaContenedorDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      PatenteCamion = "aaa111",
                                                      PatenteAcoplado = "aaa222",
                                                      ContenedorEntradaId = 1,
                                                      ContenedorSalidaId = 1
                                                  }, user) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void OrdenDeCargaContenedorModificarError()
        {
            var res = new Resultado();
            res.Error("E", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenDeCargaContenedor>())).Returns(res);
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });
            servRepositorio.Setup(s => s.ObtenerTaraContenedor(It.IsAny<int>()))
                           .Returns(new TaraContenedorDto { Descripcion = "T", Id = 1, PesoTara = 45000 });
            servRepositorio.Setup(s => s.ListarTaraContenedores())
                           .Returns(new List<TaraContenedorDto>
                               {
                                   new TaraContenedorDto {CodigoContenedor = "A", Descripcion = "TC", Id = 1}
                               });
            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            var result = target.OrdenDeCargaContenedor("w",
                                                  new OrdenDeCargaContenedorDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      PatenteAcoplado = "aaa111",
                                                      PatenteCamion = "aaa112",
                                                      ContenedorEntradaId = 1,
                                                      ContenedorSalidaId = 1
                                                  }, user) as ViewResult;
            Assert.NotNull(result);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            ModelState errorModificar;
            target.ModelState.TryGetValue("E", out errorModificar);
            Assert.NotNull(errorModificar);
            Assert.AreEqual(errorModificar.Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void RemitoResultadoSetearChoferFalse()
        {
            var res = new Resultado();
            res.Error("E", "SetearChofer false");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(res);

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);

            var result = target.Remito("w", new RemitoDto { Chofer = new ChoferDto { Cuil = "123456789" } }, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void RemitoResultadoSetearTransportistaFalse()
        {
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { TransportistaEsProveedor = true, Id = 0 });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);

            var result = target.Remito("w", new RemitoDto { Chofer = new ChoferDto { Cuil = "12345678" } }, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));

        }

        [Test]
        public void RemitoModificar()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarRemito>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });
            servRepositorio.Setup(s => s.ObtenerTaraContenedor(It.IsAny<int>()))
                           .Returns(new TaraContenedorDto { Descripcion = "T", Id = 1, PesoTara = 45000 });
            servRepositorio.Setup(s => s.ObtenerProveedor(It.IsAny<int>()))
                           .Returns(new ProveedorDto { CodigoSap = "1234" });
            var result = target.Remito("w",
                                                  new RemitoDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      PatenteCamion = "aaa111",
                                                      PatenteAcoplado = "aaa222",
                                                      OrigenId = 1,
                                                      EsRemitoProveedor = true,
                                                      Remito = "123-123"
                                                  }, user) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void RemitoModificarError()
        {
            var res = new Resultado();
            res.Error("E", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarRemito>())).Returns(res);
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                           .Returns(new CentroDto { CodigoSAP = "1234" });
            var result = target.Remito("w",
                                                  new RemitoDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      PatenteCamion = "aaa111",
                                                      PatenteAcoplado = "aaa222",
                                                      OrigenId = 1,
                                                      EsRemitoProveedor = false,
                                                      Remito = "123-123"
                                                  }, user) as ViewResult;
            Assert.NotNull(result);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            ModelState errorModificar;
            target.ModelState.TryGetValue("E", out errorModificar);
            Assert.NotNull(errorModificar);
            Assert.AreEqual(errorModificar.Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void RemitoBodegaVinoResultadoSetearChoferFalse()
        {
            var res = new Resultado();
            res.Error("E", "SetearChofer false");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(res);

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ListarTiposVehiculoBodega())
                           .Returns(new List<TipoVehiculoBodegaDto>
                               {
                                   new TipoVehiculoBodegaDto {Descripcion = "B", Id = 1}
                               });
            var result = target.RemitoBodegaVino("w", new RemitoBodegaVinoDto { Chofer = new ChoferDto { Cuil = "123456789" } }, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { }));
        }

        [Test]
        public void RemitoBodegaVinoResultadoSetearTransportistaFalse()
        {
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { TransportistaEsProveedor = true, Id = 0 });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ListarTiposVehiculoBodega())
                           .Returns(new List<TipoVehiculoBodegaDto>
                               {
                                   new TipoVehiculoBodegaDto {Descripcion = "B", Id = 1}
                               });

            var result = target.RemitoBodegaVino("w", new RemitoBodegaVinoDto { Chofer = new ChoferDto { Cuil = "12345678" } }, user) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { }));

        }

        [Test]
        public void RemitoBodegaVinoModificar()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarRemitoBodegaVino>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });
            servRepositorio.Setup(s => s.ObtenerTaraContenedor(It.IsAny<int>()))
                           .Returns(new TaraContenedorDto { Descripcion = "T", Id = 1, PesoTara = 45000 });
            servRepositorio.Setup(s => s.ObtenerProveedor(It.IsAny<int>()))
                           .Returns(new ProveedorDto { CodigoSap = "1234" });
            var result = target.RemitoBodegaVino("w",
                                                  new RemitoBodegaVinoDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      Patente = "aaa111"
                                                  }, user) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void RemitoBodegaVinoModificarError()
        {
            var res = new Resultado();
            res.Error("E", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarRemitoBodegaVino>())).Returns(res);
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto { Id = 1 });
            servRepositorio.Setup(s => s.ListarTiposVehiculoBodega())
                           .Returns(new List<TipoVehiculoBodegaDto>
                               {
                                   new TipoVehiculoBodegaDto {Descripcion = "B", Id = 1}
                               });

            var tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            var materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            var tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                           .Returns(new CentroDto { CodigoSAP = "1234" });
            var result = target.RemitoBodegaVino("w",
                                                  new RemitoBodegaVinoDto
                                                  {
                                                      Chofer = new ChoferDto { Cuil = "30-123456789-01" },
                                                      EsTransportista = true,
                                                      Patente = "aaa111"
                                                  }, user) as ViewResult;
            Assert.NotNull(result);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;

            Assert.AreEqual(result.ViewName, "");
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            ModelState errorModificar;
            target.ModelState.TryGetValue("E", out errorModificar);
            Assert.NotNull(errorModificar);
            Assert.AreEqual(errorModificar.Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void AsignacionDeRuta()
        {
            recorridoDto.BalanzaBrutoId = 1;
            recorridoDto.BalanzaTaraId = 1;
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns(recorridoDto);
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto());

            var result = target.AsignacionDeRuta(1, TipoDocumentoIngreso.HojaDeRuta, "1234", "aaa111", false) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(result.ViewBag.RecorridoId, 1);
            Assert.That(result.Model, Is.AssignableTo(typeof(RecorridoDto)));
        }

        [Test]
        public void AsignacionDeRutaObtenerRecorridoNull()
        {
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns((RecorridoDto)null);

            var result = target.AsignacionDeRuta(1, TipoDocumentoIngreso.HojaDeRuta, "1234", "aaa111", false) as ViewResult;
            Assert.NotNull(result);
            Assert.That(result.Model, Is.Not.AssignableTo(typeof(RecorridoDto)));

        }

        [Test]
        public void PesadaObtenerRecorridoNull()
        {
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns((RecorridoDto)null);
            var result = target.Pesada(1, TipoDocumentoIngreso.CartaPorte, "1234", "aaa111", false) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewBag.FiltrarPorTarjeta);
            Assert.AreEqual(result.ViewName, "");
        }

        [Test]
        public void PesadaObtenerRecorridoNotNullSinDocSap()
        {
            recorridoDto.BalanzaBrutoId = 1;
            recorridoDto.BalanzaTaraId = 1;
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns(recorridoDto);
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto());
            recorridoDto.Terminado = false;
            recorridoDto.Rechazado = false;
            recorridoDto.AnalisisDeCalidad = new AnalisisDeCalidadDto { FechaCreacion = new DateTime(2015, 6, 6) };
            listaWf.Setup(s => s.ObtenerWorkflowProximaAccion(It.IsAny<Guid>())).Returns(new ProximaAccionDto { ProximaAccion = "Proxima" });
            servRepositorio.Setup(s => s.ListarLotesPorWorkflow(It.IsAny<Guid>()))
                           .Returns(new List<LoteDto>
                               {
                                   new LoteDto {NumeroDeLote = "1", Fecha = new DateTime(2015, 6, 2)},
                                   new LoteDto {NumeroDeLote = "2", Fecha = new DateTime(2015, 6, 3)}
                               });
            servRepositorio.Setup(s => s.ListarControlRecorridoServiciosSap(It.IsAny<Guid>()))
                           .Returns(new List<ControlRecorridoDto>
                               {
                                   new ControlRecorridoDto {Actividad = "A", Comentario = "MnsjError"}
                               });
            recorridoDto.NumeroDeDocumentoSap = null;
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns(recorridoDto);
            var result = target.Pesada(1, TipoDocumentoIngreso.CartaPorte, "1234", "aaa111", false) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewBag.FiltrarPorTarjeta);
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(result.ViewBag.DocSap, "A / MnsjError");
        }

        [Test]
        public void PesadaObtenerRecorridoNotNullConDocSap()
        {
            recorridoDto.BalanzaBrutoId = 1;
            recorridoDto.BalanzaTaraId = 1;
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns(recorridoDto);
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto());
            recorridoDto.Terminado = false;
            recorridoDto.Rechazado = false;
            recorridoDto.AnalisisDeCalidad = new AnalisisDeCalidadDto { FechaCreacion = new DateTime(2015, 6, 6) };
            listaWf.Setup(s => s.ObtenerWorkflowProximaAccion(It.IsAny<Guid>())).Returns(new ProximaAccionDto { ProximaAccion = "Proxima" });
            servRepositorio.Setup(s => s.ListarLotesPorWorkflow(It.IsAny<Guid>()))
                           .Returns(new List<LoteDto>
                               {
                                   new LoteDto {NumeroDeLote = "1", Fecha = new DateTime(2015, 6, 2)},
                                   new LoteDto {NumeroDeLote = "2", Fecha = new DateTime(2015, 6, 3)}
                               });
            recorridoDto.NumeroDeDocumentoSap = "1223";
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns(recorridoDto);
            var result = target.Pesada(1, TipoDocumentoIngreso.CartaPorte, "1234", "aaa111", false) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewBag.FiltrarPorTarjeta);
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(result.ViewBag.DocSap, "1223");
        }

        [Test]
        public void AnalisisDeCalidadRecorridoNull()
        {
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns((RecorridoDto)null);
            var result = target.AnalisisDeCalidad(1, TipoDocumentoIngreso.CartaPorte, "1234", "aaa111", false) as ViewResult;
            ListaPaginada<CalidadCaracteristicaDto> calidades = result.ViewBag.calidades;

            Assert.NotNull(result);
            Assert.False(result.ViewBag.FiltrarPorTarjeta);
            Assert.AreEqual(result.ViewName, "");
            Assert.Null(calidades);
        }

        [Test]
        public void AnalisisDeCalidadRecorridoNotNull()
        {
            recorridoDto.BalanzaBrutoId = 1;
            recorridoDto.BalanzaTaraId = 1;
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns(recorridoDto);
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto());
            recorridoDto.Terminado = false;
            recorridoDto.Rechazado = false;
            recorridoDto.AnalisisDeCalidad = new AnalisisDeCalidadDto


            {
                Usuario = "Puli",
                FechaCreacion = new DateTime(2015, 6, 6),
                CaracteristicasAnalizadas =
                        new List<AnalisisPorCaracteristicaDto>
                            {
                                new AnalisisPorCaracteristicaDto
                                    {
                                        AnalisisDeCalidadId = 1,
                                        Caracteristica = "Caracteristica1",
                                        CaracteristicaId = 1,
                                        Rango = "12-1000"
                                    }
                            }
            };

            recorridoDto.Calado = new CaladoDto
            {
                CaladosPorCaracteristica =
                        new List<CaladoPorCaracteristicaDto>
                            {
                                new CaladoPorCaracteristicaDto {CaracteristicaId = 1, Rango = "12-1000"}
                            }
            };

            listaWf.Setup(s => s.ObtenerWorkflowProximaAccion(It.IsAny<Guid>())).Returns(new ProximaAccionDto { ProximaAccion = "Proxima" });
            servRepositorio.Setup(s => s.ListarLotesPorWorkflow(It.IsAny<Guid>()))
                           .Returns(new List<LoteDto>
                               {
                                   new LoteDto {NumeroDeLote = "1", Fecha = new DateTime(2015, 6, 2)},
                                   new LoteDto {NumeroDeLote = "2", Fecha = new DateTime(2015, 6, 3)}
                               });
            servRepositorio.Setup(s => s.ListarControlRecorridoServiciosSap(It.IsAny<Guid>()))
                           .Returns(new List<ControlRecorridoDto>
                               {
                                   new ControlRecorridoDto {Actividad = "A", Comentario = "MnsjError"}
                               });
            recorridoDto.NumeroDeDocumentoSap = null;
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns(recorridoDto);
            servRepositorio.Setup(s => s.ListarUsuarios()).Returns(new List<UsuarioDto> { new UsuarioDto { Nombre = "Puli", Apellido = "Palacios", NombreUsuario = "Puli" } });
            var result = target.AnalisisDeCalidad(1, TipoDocumentoIngreso.CartaPorte, "1234", "aaa111", false) as ViewResult;

            ListaPaginada<CalidadCaracteristicaDto> calidades = result.ViewBag.Items;

            Assert.NotNull(result);
            Assert.False(result.ViewBag.FiltrarPorTarjeta);
            Assert.AreEqual(result.ViewName, "");
            Assert.NotNull(calidades);
            Assert.AreEqual(calidades.FirstOrDefault().CaracteristicaId, "1");
        }

        [Test]
        public void Listar()
        {
            var result =
                target.Listar(
                    new ModificarDocumentoDeIngresoDto
                    {
                        TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                        NumeroDocumentoIngreso = "1234",
                        Patente = "aaa111"
                    }, user) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Listar");
            Assert.Null(result.ViewBag.Error);
        }

        [Test]
        public void ListarSinItemsFiltraPorTarjeta()
        {
            servRepositorio.Setup(s => s.ListarPaginadoRecorridosPorModificarDocumentoDeIngreso(
                                                 It.IsAny<int>(),
                                                 It.IsAny<Paginacion>(),
                                                 It.IsAny<ModificarDocumentoDeIngresoDto>()))
                 .Returns(new ListaPaginada<RecorridoDto>(new List<RecorridoDto> { }, 1, 1, 0));
            servRepositorio.Setup(
                s =>
                s.ListarPaginadoRecorridosPorDocumentoPatenteYCentro(It.IsAny<TipoDocumentoIngreso>(), It.IsAny<string>(),
                                                                     It.IsAny<string>(), It.IsAny<string>(),
                                                                     It.IsAny<int>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<RecorridoDto>(new List<RecorridoDto> { }, 1, 1, 0));
            var result =
                target.Listar(
                    new ModificarDocumentoDeIngresoDto
                    {
                        TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                        NumeroDocumentoIngreso = "1234",
                        Patente = "aaa111",
                        FiltrarPorTarjeta = true
                    }, user) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Listar");
            Assert.AreEqual(result.ViewBag.Error, "La tarjeta de acceso no se encuentra asignada a un vehículo");
        }

        [Test]
        public void ListarSinItemsNoFiltraPorTarjeta()
        {
            servRepositorio.Setup(s => s.ListarPaginadoRecorridosPorModificarDocumentoDeIngreso(
                                                 It.IsAny<int>(),
                                                 It.IsAny<Paginacion>(),
                                                 It.IsAny<ModificarDocumentoDeIngresoDto>()))
                 .Returns(new ListaPaginada<RecorridoDto>(new List<RecorridoDto> { }, 1, 1, 0));
            servRepositorio.Setup(
                s =>
                s.ListarPaginadoRecorridosPorDocumentoPatenteYCentro(It.IsAny<TipoDocumentoIngreso>(), It.IsAny<string>(),
                                                                     It.IsAny<string>(), It.IsAny<string>(),
                                                                     It.IsAny<int>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<RecorridoDto>(new List<RecorridoDto> { }, 1, 1, 0));
            var result =
                target.Listar(
                    new ModificarDocumentoDeIngresoDto
                    {
                        TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                        NumeroDocumentoIngreso = "1234",
                        Patente = "aaa111",
                        FiltrarPorTarjeta = false
                    }, user) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Listar");
            Assert.AreEqual(result.ViewBag.Error, "La Búsqueda no ha arrojado resultados con los filtros ingresados");
        }

        [Test]
        public void Seleccionar()
        {
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns(recorridoDto);

            var result = target.Seleccionar(1, false, false) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "DocumentoOrigen");
            Assert.AreEqual(result.RouteValues["recorridoid"], 1);
        }

        [Test]
        public void RecorridoObtenerRecorridoNull()
        {
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns((RecorridoDto)null);
            var result = target.Recorrido(1, TipoDocumentoIngreso.CartaPorte, "1234", "aaa111", false) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewBag.FiltrarPorTarjeta);
            Assert.AreEqual(result.ViewName, "");
        }

        [Test]
        public void RecorridoObtenerRecorridoNotNull()
        {
            recorridoDto.BalanzaBrutoId = 1;
            recorridoDto.BalanzaTaraId = 1;
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns(recorridoDto);
            recorridoDto.Terminado = false;
            servRepositorio.Setup(s => s.ConsultaControlRecorridoLogActividad(It.IsAny<Guid>()))
                           .Returns(new List<ControlRecorridoLogActividadConsultaDto>
                               {
                                   new ControlRecorridoLogActividadConsultaDto {
                                       Fecha = DateTime.Now,
                                     Actividad = "Actividad",
                                     Comentario = "",
                                     Tabla = "LogActividad",
                                     Usuario = ""},
                                   new ControlRecorridoLogActividadConsultaDto {
                                       Fecha = DateTime.Now,
                                        Actividad = "Actividad",
                                        Comentario = "Comentario",
                                        Tabla = "ControlRecorrido",
                                        Usuario = "NombreUsuario"}
                               });

            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns(recorridoDto);
            var result = target.Recorrido(1, TipoDocumentoIngreso.CartaPorte, "1234", "aaa111", false) as ViewResult;
            var lista = result.ViewBag.LogItems as List<ControlRecorridoLogActividadConsultaDto>;
            Assert.NotNull(result);
            Assert.NotNull(lista);
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(result.ViewBag.WorkflowDescripcion, "Wf");
            Assert.AreEqual(lista.Count(), 2);
        }
    }
}
