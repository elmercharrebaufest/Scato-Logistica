using FluentValidation;
using FluentValidation.Results;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Validations.Interfaces;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Moq;
using Ninject.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorValidarOrdenCargaInternaFasonTests
    {
        private Mock<IServicioComandos> _mockServicioComandos;
        private Mock<IValidatorEntity<OrdenCargaInternaFasonDto>> _mockValidacion;
        private Mock<IValidator<OrdenCargaInternaFasonDto>> _mockValidador;
        private Mock<IServicioRepositorio> _mockServicioRepositorio;
        private Mock<IRepositorio> _mockRepositorio;
        private Mock<IConversor> _mockConversor;
        private Mock<ILogger> _mockLogger;
        private Mock<IServicioOperaciones> _mockServicioOperaciones;
        private ProcesadorValidarOrdenCargaInternaFason _procesador;
        private ChoferDto _Chofer;
        private OrdenCargaInternaFasonDto _ordenCarga;
        private OrdenDeCargaDto _ordenDeCarga;
        private WorkflowDto _workflow;
        private List<TipoComercialDto> _tiposComerciales;
        private List<MaterialPorWorkflowDto> _materiales;
        private ProveedorDto _proveedor;
        private ClienteDto _cliente;
        private ResultadoEscalables _resultadoEscalables;
        private TransportistaDto _transportista;
        private DomicilioDto _domicilio;
        private ClienteDto _pagadorFlete;

        [SetUp]
        public void SetUp()
        {
            _mockServicioComandos = new Mock<IServicioComandos>();
            _mockValidacion = new Mock<IValidatorEntity<OrdenCargaInternaFasonDto>>();
            _mockServicioRepositorio = new Mock<IServicioRepositorio>();
            _mockRepositorio = new Mock<IRepositorio>();
            _mockConversor = new Mock<IConversor>();
            _mockServicioOperaciones = new Mock<IServicioOperaciones>();
            _mockLogger = new Mock<ILogger>();
            _mockValidador = new Mock<IValidator<OrdenCargaInternaFasonDto>>();

            _procesador = new ProcesadorValidarOrdenCargaInternaFason(
                _mockServicioComandos.Object,
                _mockValidacion.Object,
                _mockServicioRepositorio.Object,
                _mockRepositorio.Object,
                _mockConversor.Object,
                _mockLogger.Object,
                _mockServicioOperaciones.Object);


            _tiposComerciales = new List<TipoComercialDto>
            {
                new TipoComercialDto { Descripcion = "SALIDA" },
                new TipoComercialDto { Descripcion = "EGRESO" }
            };

            // Initialize other properties as needed
            _Chofer = new ChoferDto
            {
                Cuil = "20-32063968-4",
                Nombre = "MARCOS MAXIMILIANO",
                Apellido = "GONZALES"
            };

            _materiales = new List<MaterialPorWorkflowDto>
            {
                new MaterialPorWorkflowDto { MaterialId = 1, MaterialDesc = "Material 1", EsDerivadoGranario = false, MaterialCodigoSap = "2333"},
                new MaterialPorWorkflowDto { MaterialId = 63734, MaterialDesc = "Material 2", EsDerivadoGranario = false, MaterialCodigoSap = "6897"}
            };

            _ordenCarga = new OrdenCargaInternaFasonDto
            {
                Id = 0,
                NumeroOrden = "75207",
                FechaEmision = new DateTime(2024, 11, 26, 0, 0, 0),
                PatenteCamion = "ira000",
                PatenteAcoplado = "ira001",
                Transportista = "20-20686662-5 - CANILONGO RUBEN",
                TransportistaId = 15845,
                EsTransportista = true,
                EsExtranjero = false,
                TipoComercialId = 1,
                TipoComercialDesc = null,
                MaterialId = 63734,
                MaterialDesc = null,
                Cliente = "MOLINOS RIO DE LA PLATA S A",
                ClienteId = 56508,
                ClienteDireccion = null,
                ClienteProvincia = null,
                ClienteLocalidad = null,
                ClienteCodigoSap = null,
                ClienteCuit = null,
                Chofer = _Chofer,
                RecorridoId = 0,
                InstanciaWorkflow = Guid.NewGuid(),
                LocalidadDestinoDescripcion = null,
                KmARecorrer = null,
                LocalidadDestinoId = null,
                TipoVehiculo = TipoVehiculo.Camión,
                DerivadoGranarioHabilitado = true,
                PlantaDGDestino = null,
                OrdenDomicilioDestino = 1,
                PagadorFlete = null,
                PagadorFleteId = null,
                NumeroCTG = null,
                NumeroCPE = null,
                Demorado = false,
                MotivoDemora = null,
                Rechazado = false,
                MotivoRechazo = null,
                Corredor = null,
                CorredorId = null,
                Comisionista = null,
                ComisionistaId = null,
                ComisionistaCodigoSap = null,
                Remitente = null,
                RemitenteId = null,
                RemitenteCodigoSap = null,
                IntermediarioFleteId = null,
                IntermediarioFlete = null,
                TipoDomicilioDestino = null,
                TipoYOrdenDestino = "1-1",
                Destinatario = "MOLINOS RIO DE LA PLATA S A",
                DestinatarioId = 56508,
                NumeroOrdenExterno = "75207",
                DestinoMercaderia = null,
                Observaciones = null
            };

            _ordenDeCarga = new OrdenDeCargaDto
            {
                FechaRetiro = null,
                KmARecorrer = "39",
                Cantidad = 30000,
                CUITCorredor = null,
                Reventa = false,
                FleteMOA = true,
                CUITDestinatario = "30500858628",
                CUITDestino = "30500858628",
                CUITIntermediarioFlete = null,
                DestinoMercaderia = null,
                Escalable = false,
                RazonSocialDestinatario = "MOLINOS RIO DE LA PLATA S A",
                RazonSocialDestino = "MOLINOS RIO DE LA PLATA S A",
                RazonSocialIntermediarioFlete = null,
                RemitenteComercial = null,
                PagadorFlete = "30715118773",
                Id = 75207,
                Cliente = "MOLINOS RIO DE LA PLATA",
                CodigoProducto = "94705",
                CUILChofer = "20326039684",
                CUITCliente = "30500858628",
                CUITTransporte = "20206866625",
                DescripcionProducto = "ACEITE DE SOJA CRUDO A GRANEL",
                DomicilioDescr = "(FISCAL) SOLIS 882, VILLA GOBERNADOR GALVEZ, SANTA FE",
                DomicilioOrden = 1,
                DomicilioTipo = "1",
                FechaCreacion = "1/11/2024 11:25:10",
                LocalidadDescripcion = null,
                LocalidadId = 0,
                ChoferNombre = "DIEGO",
                ChoferApellido = "LA ROSA",
                Observacion = null,
                PatenteAcoplado = "FAS341",
                PatenteChasis = "IRA000",
                PlantaCodigo = "22397",
                RazonSocialTransporte = "TRANSPORTE",
                TipoOrden = "FASON",
                MaterialId = "63734",
            };

            _workflow = new WorkflowDto { Codigo = "workflowCode", Descripcion = "Sin Flete" };
            _mockServicioRepositorio.Setup(x => x.ObtenerWorkflowPorCodigo(It.IsAny<string>())).Returns(_workflow);

            _mockServicioRepositorio.Setup(x => x.ListarMaterialesPorWorkflow(_workflow.Id,0)).Returns(_materiales);

            _domicilio = new DomicilioDto { Id = 1, Descripcion = "Domicilio 1", Orden = 1, Tipo = 1 };

            _pagadorFlete = new ClienteDto();
            _mockServicioRepositorio.Setup(x => x.ObtenerClientePorCuit(It.IsAny<string>())).Returns(_pagadorFlete);

            _cliente = new ClienteDto { Id = 0258, Cuit = "30500858628" };
            _mockServicioRepositorio.Setup(x => x.ObtenerClientePorCuit(It.IsAny<string>())).Returns(_cliente);

            _mockServicioRepositorio.Setup(x => x.ObtenerCliente(_cliente.Id)).Returns(_cliente);

            _mockServicioRepositorio.Setup(x => x.ObtenerChoferPorCuit(It.IsAny<string>())).Returns(_Chofer);

            _proveedor = new ProveedorDto { Cuil = "20-12345678-9" };
            _mockServicioRepositorio.Setup(x => x.ObtenerProveedorPorCuit(It.IsAny<string>(), It.IsAny<TiposProveedor>())).Returns(_proveedor);

            _mockServicioRepositorio.Setup(x => x.ObtenerProveedor(It.IsAny<int>())).Returns(_proveedor);

            _transportista = new TransportistaDto { Id = 123 };
            _mockServicioRepositorio.Setup(x => x.ObtenerTransportistaPorCuit(_proveedor.Cuil)).Returns(_transportista);

            _resultadoEscalables = new ResultadoEscalables { Categoria = TipoVehiculo.Camión };
            _mockServicioComandos.Setup(x => x.Ejecutar(It.IsAny<Dominio.Comandos.Comando>())).Returns(_resultadoEscalables);

            _mockServicioRepositorio.Setup(x => x.ObtenerNuevoNumeroDeOrdenFason()).Returns("01458");

            _mockServicioRepositorio.Setup(x => x.ObtenerTransportistaPorCuit(It.IsAny<string>())).Returns(_transportista);

            _mockServicioRepositorio.Setup(x => x.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto() { EsDerivadoGranario = true });

            _mockServicioComandos.Setup(x => x.Ejecutar(It.IsAny<ConsultarPlantasDG>()))
                .Returns(new ResultadoConsultaPlantasDG { Plantas = new List<int> { 22397, 1258, 4589 } });

            _mockServicioComandos.Setup(x => x.Ejecutar(It.IsAny<ConsultarDomiciliosDG>()))
                .Returns(new ResultadoConsultaDomiciliosDG { Domicilios = new List<DomicilioDto> { _domicilio } });

            _mockServicioComandos.Setup(x => x.Ejecutar(It.IsAny<AutorizarCpeDGDummy>())).Returns(new ResultadoCartaPorteElectronicaDummy());
            _mockServicioComandos.Setup(x => x.Ejecutar(It.IsAny<AnularCPEDGDummy>())).Returns(new ResultadoOrdenFason());


            _mockServicioRepositorio.Setup(x => x.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(_tiposComerciales);

            _mockValidacion.Setup(x => x.Validate(It.IsAny<OrdenCargaInternaFasonDto>())).Returns(new ValidationResult());
            _mockServicioRepositorio.Setup(x => x.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto());
            _mockServicioRepositorio.Setup(x => x.BuscarChoferes(It.IsAny<ChoferFiltro>())).Returns(new List<ChoferDto>() { _Chofer });
            _mockServicioComandos.Setup(x => x.Ejecutar(It.IsAny<ModificarChofer>())).Returns(new Resultado());
            _mockServicioComandos.Setup(x => x.Ejecutar(It.IsAny<ConfirmarCTGVencidas>())).Returns(new Resultado());
            _mockServicioComandos.Setup(x => x.Ejecutar(It.IsAny<AutorizarCpeDGDummy>())).Returns(new ResultadoCartaPorteElectronicaDummy());
            _mockServicioComandos.Setup(x => x.Ejecutar(It.IsAny<AnularCPEDGDummy>())).Returns(new ResultadoOrdenFason());
            _mockServicioOperaciones.Setup(s => s.ObtenerOrdenesDeCarga(_ordenDeCarga.PatenteChasis)).Returns(new List<OrdenDeCargaDto> { _ordenDeCarga });
        }

        private static IEnumerable<TestCaseData> WorkflowTestCases
        {
            get
            {
                yield return new TestCaseData(new WorkflowDto { Codigo = "workflowCode", Descripcion = "Con Flete" });
            }
        }



        [Test]
        [TestCaseSource("WorkflowTestCases")]
        public void Ejecutar_OrdenValida_ReturnsResultadoSinErrores(WorkflowDto workflow)
        {
            var materiales = new List<MaterialPorWorkflowDto>
                                {
                                    new MaterialPorWorkflowDto { 
                                        MaterialId = 63734, 
                                        MaterialDesc = "Material 2", 
                                        EsDerivadoGranario = true, 
                                        MaterialCodigoSap = "6897"}
                                };
            _mockServicioRepositorio.Setup(x => x.ObtenerWorkflowPorCodigo(It.IsAny<string>())).Returns(workflow);

            _ordenCarga.KmARecorrer = "0";
           
            var comando = new ValidarOrdenCargaInternaFason
            {
                Orden = _ordenDeCarga,
                CentroId = 5,
                Usuario = "UsuarioTest"
            };

            _mockServicioRepositorio.Setup(x => x.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto() { EsDerivadoGranario = true });
            _mockServicioRepositorio.Setup(x => x.ListarMaterialesPorWorkflow(_workflow.Id, comando.CentroId)).Returns(materiales);
           
            _mockRepositorio.Setup(r => r.ObtenerProyeccion<Material, string>(
                It.IsAny<Expression<Func<Material, bool>>>(),
                It.IsAny<Expression<Func<Material, string>>>()
            )).Returns("94705");


            var resultado = _procesador.Ejecutar(comando);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }

        [Test]
        public void Ejecutar_OrdenValidaNoDerivadoGranario_ReturnsResultadoSinErrores()
        {

            var comando = new ValidarOrdenCargaInternaFason
            {
                Orden = _ordenDeCarga,
                CentroId = 5,
                Usuario = "UsuarioTest"
            };

            _mockServicioRepositorio.Setup(x => x.ListarMaterialesPorWorkflow(_workflow.Id, comando.CentroId)).Returns(_materiales);
            _mockServicioRepositorio.Setup(x => x.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto() { EsDerivadoGranario = false });

            var resultado = _procesador.Ejecutar(comando);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }

        [Test]
        [TestCase(0)]
        public void Ejecutar_OrdenNoValidaFaltaDMaterialIdEsDerivadoGranario_ReturnsResultadoConErrores(int materialId)
        {
            _ordenCarga.MaterialId = materialId;
            var comando = new ValidarOrdenCargaInternaFason
            {
                Orden = _ordenDeCarga,
                CentroId = 5,
                Usuario = "UsuarioTest"
            };

            var materiales = new List<MaterialPorWorkflowDto>
                            {
                                new MaterialPorWorkflowDto { 
                                    MaterialId = 15789, 
                                    MaterialDesc = "Material 2", 
                                    EsDerivadoGranario = false, 
                                    MaterialCodigoSap = "6897"}
                            };

            var validationResult = new ValidationResult();
            validationResult.Errors.Add(new ValidationFailure("1", "El campo MaterialId es requerido."));

            _mockServicioRepositorio.Setup(x => x.ListarMaterialesPorWorkflow(_workflow.Id, comando.CentroId)).Returns(_materiales);
            _mockServicioRepositorio.Setup(x => x.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto() { EsDerivadoGranario = false });
            _mockValidacion.Setup(x => x.Validate(It.IsAny<OrdenCargaInternaFasonDto>())).Returns(validationResult);


            var resultado = _procesador.Ejecutar(comando);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
        }

        [Test]
        public void Ejecutar_OrdenValidaDerivadoGranario_CambiaClienteADestino()
        {
            // Arrange
            var comando = new ValidarOrdenCargaInternaFason
            {
                Orden = _ordenDeCarga,
                CentroId = 5,
                Usuario = "UsuarioTest"
            };

            var materiales = new List<MaterialPorWorkflowDto>
            {
                new MaterialPorWorkflowDto {
                    MaterialId = 63734,
                    MaterialDesc = "Material 2",
                    EsDerivadoGranario = true,
                    MaterialCodigoSap = "6897"
                }
            };

            _mockServicioRepositorio.Setup(x => x.ObtenerWorkflowPorCodigo(It.IsAny<string>())).Returns(_workflow);
            _mockServicioRepositorio.Setup(x => x.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto() { EsDerivadoGranario = true });
            _mockServicioRepositorio.Setup(x => x.ListarMaterialesPorWorkflow(_workflow.Id, comando.CentroId)).Returns(materiales);

            // Act
            var resultado = _procesador.Ejecutar(comando);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
            Assert.AreEqual(_ordenDeCarga.CUITDestinatario, _ordenDeCarga.CUITCliente);
        }

        [Test]
        public void Ejecutar_OrdenValidaNoDerivadoGranario_DevuelveCliente() 
        {
            // Arrange
            var comando = new ValidarOrdenCargaInternaFason
            {
                Orden = _ordenDeCarga,
                CentroId = 5,
                Usuario = "UsuarioTest"
            };

            var materiales = new List<MaterialPorWorkflowDto>
            {
                new MaterialPorWorkflowDto {
                    MaterialId = 63734,
                    MaterialDesc = "Material 2",
                    EsDerivadoGranario = true,
                    MaterialCodigoSap = "6897"
                }
            };

            _mockServicioRepositorio.Setup(x => x.ObtenerWorkflowPorCodigo(It.IsAny<string>())).Returns(_workflow);
            _mockServicioRepositorio.Setup(x => x.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto() { EsDerivadoGranario = false });
            _mockServicioRepositorio.Setup(x => x.ListarMaterialesPorWorkflow(_workflow.Id, comando.CentroId)).Returns(materiales);

            // Act
            var resultado = _procesador.Ejecutar(comando);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
            Assert.AreEqual(_ordenDeCarga.CUITCliente, _ordenDeCarga.CUITCliente);
        }
    }
}
