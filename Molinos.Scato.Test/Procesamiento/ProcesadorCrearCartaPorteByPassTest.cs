using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearCartaPorteByPassTest
    {
        private ProcesadorCrearCartaPorteByPass target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private CartaPorteDto dto;
        private List<Workflow> workflows;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearCartaPorteByPass(repositorioMock.Object, conversor, new NullLogger());

            workflows = new List<Workflow> { new Workflow { Id = 1, Codigo = "W1", Descripcion = "D1" } };

            dto = new CartaPorteDto
            {
                NroCartaPorte = "000000000001",
                TipoVehiculo = TipoVehiculo.Camión,
                Chofer = new ChoferDto { Id = 1, Nombre = "Bruce", Apellido = "Wayne", NumeroDeDocumento = "123" },
                MaterialId = 2,
                TipoComercialId = 3,
                TransportistaId = 4,
                DestinoId = 6,
                ProcedenciaId = 8,
                TitularCartaPorteId = 7,
                DestinatarioId = 7,
                AgenteComprasId = 0,
                PrestadorId = 0,
                BocaDestinoId = 9,
                IntermediarioId = null,
                TipoCategoriaId = 10,
                EntregadorId = 11,
                EsClienteDestinatario = false,
                Cpe = false,
                FotoRutaDestino = "foto.jpg",
                Vehiculos = new List<VehiculoDto>
                {
                    new VehiculoDto { Patente = "AAA111", TipoVehiculo = TipoVehiculo.Camión, PesoTaraOrigen = 1000, PesoBrutoOrigen = 2000 }
                },
                CartaPorteByPass = new AdicionalesCartaPorteByPassDto
                {
                    Calado = new CaladoDto { Id = 13 },
                    EstablecimientoId = null,
                    AlmacenId = null,
                    NombreUsuario = "tester",
                    RecorridoIdIngreso = 1
                }
            };

            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>()))
                .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where(q.Compile()).SingleOrDefault());
            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<object>())).Returns(new Chofer { Id = 1 });
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<object>())).Returns(new Material { Id = 2 });
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<object>())).Returns(new TipoComercial { Id = 3 });
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<object>())).Returns(new Transportista { Id = 4 });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.Is<object>(id => (int)id == 5))).Returns(new Centro { Id = 5 });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.Is<object>(id => (int)id == 6))).Returns(new Centro { Id = 6 });
            repositorioMock.Setup(s => s.Obtener<Proveedor>(It.IsAny<object>())).Returns(new Proveedor { Id = 7, CodigoSap = "50000007" });
            repositorioMock.Setup(s => s.Obtener<Localidad>(It.IsAny<object>())).Returns(new Localidad { Id = 8 });
            repositorioMock.Setup(s => s.Obtener<BocaDestino>(It.IsAny<object>())).Returns(new BocaDestino { Id = 9 });
            repositorioMock.Setup(s => s.Obtener<Categoria>(It.IsAny<object>())).Returns(new Categoria { Id = 10 });
            repositorioMock.Setup(s => s.Obtener<Entregador>(It.IsAny<object>())).Returns(new Entregador { Id = 11 });
            repositorioMock.Setup(s => s.Obtener<WorkflowDefinicion>(It.IsAny<object>())).Returns(new WorkflowDefinicion { Id = 12 });
            repositorioMock.Setup(s => s.Obtener<Calado>(It.IsAny<object>())).Returns(new Calado());
        }

        [Test]
        public void CuandoEsPrimerVehiculo_InsertaCartaPorteYRecorridoEnBaseDeDatos()
        {
            var comando = new CrearCartaPorteByPass
            {
                Orden = dto,
                NombreWorkflow = "W1",
                InstanciaWorkflowId = Guid.NewGuid(),
                CentroId = 5,
                WorkflowDefinicionId = 12,
                Vehiculo = new VehiculoDto { Patente = "AAA111", Primero = true, TipoVehiculo = TipoVehiculo.Camión, PesoTaraOrigen = 1000, PesoBrutoOrigen = 2000 }
            };

            var resultado = target.Ejecutar(comando);

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));

            repositorioMock.Verify(
                s => s.Agregar(It.Is<CartaPorte>(c =>
                    c.NroCartaPorte == "000000000001" &&
                    c.TitularCartaPorte.Id == 7 &&
                    c.Destinatario.Id == 7 &&
                    c.CentroDestino.Id == 6 &&
                    c.Chofer.Id == 1 &&
                    c.TipoComercial.Id == 3)),
                Times.Exactly(1));

            repositorioMock.Verify(
                s => s.Agregar(It.Is<Recorrido>(r =>
                    r.InstanciaWorkflow == comando.InstanciaWorkflowId &&
                    r.Workflow.Codigo == "W1" &&
                    r.Centro.Id == 5 &&
                    r.Chofer.Id == 1 &&
                    r.Transportista.Id == 4 &&
                    r.PesoBruto == 2000 &&
                    r.PesoTara == 1000)),
                Times.Exactly(1));

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void CuandoNoEsPrimerVehiculoYNoEsTrenCpe_NoInsertaCartaPorteEnBaseDeDatos()
        {
            var comando = new CrearCartaPorteByPass
            {
                Orden = dto,
                NombreWorkflow = "W1",
                InstanciaWorkflowId = Guid.NewGuid(),
                CentroId = 5,
                WorkflowDefinicionId = 12,
                Vehiculo = new VehiculoDto { Patente = "AAA111", Primero = false, TipoVehiculo = TipoVehiculo.Camión, PesoTaraOrigen = 1000, PesoBrutoOrigen = 2000 }
            };

            var resultado = target.Ejecutar(comando);

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));

            repositorioMock.Verify(s => s.Agregar(It.IsAny<CartaPorte>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void CuandoFksOpcionalesSonNulas_InsertaCartaPorteSinLanzarExcepcion()
        {
            // Todos estos son int? en el DTO: si vienen null, el procesador usa "?? 0" o Obtener(null)
            // antes de llegar al repositorio. Este test asegura que ninguno de esos casos genere NRE.
            dto.TransportistaId = null;
            dto.IntermediarioId = null;
            dto.TecnologiaId = null;
            dto.CodigoRamalId = null;
            dto.PagadorFleteId = null;
            dto.RepresentanteRecibidorId = null;
            dto.CartaPorteByPass.AlmacenId = null;
            dto.CartaPorteByPass.EstablecimientoId = null;

            var comando = new CrearCartaPorteByPass
            {
                Orden = dto,
                NombreWorkflow = "W1",
                InstanciaWorkflowId = Guid.NewGuid(),
                CentroId = 5,
                WorkflowDefinicionId = 12,
                TipoVariedadId = null,
                Vehiculo = new VehiculoDto { Patente = "AAA111", Primero = true, TipoVehiculo = TipoVehiculo.Camión, PesoTaraOrigen = 1000, PesoBrutoOrigen = 2000 }
            };

            var resultado = target.Ejecutar(comando);

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<CartaPorte>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void CuandoChoferEsNulo_NoPropagaExcepcionYRetornaResultadoConError()
        {
            // comando.Orden.Chofer.Id se accede sin chequeo de nulidad: si Chofer es null, se produce un
            // NullReferenceException. Este test verifica que quede contenido por el catch general del
            // procesador y no se propague como excepción no controlada fuera de Ejecutar.
            dto.Chofer = null;

            var comando = new CrearCartaPorteByPass
            {
                Orden = dto,
                NombreWorkflow = "W1",
                InstanciaWorkflowId = Guid.NewGuid(),
                CentroId = 5,
                WorkflowDefinicionId = 12,
                Vehiculo = new VehiculoDto { Patente = "AAA111", Primero = true, TipoVehiculo = TipoVehiculo.Camión, PesoTaraOrigen = 1000, PesoBrutoOrigen = 2000 }
            };

            Resultado resultado = null;
            Assert.DoesNotThrow(() => resultado = target.Ejecutar(comando));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.First().Value, Is.EqualTo(Textos.CartaDePorte_Error));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<CartaPorte>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
        }

        [Test]
        public void CuandoCartaPorteByPassEsNulo_NoPropagaExcepcionYRetornaResultadoConError()
        {
            // comando.Orden.CartaPorteByPass.Calado.Id y AlmacenId se acceden sin chequeo de nulidad.
            // Si CartaPorteByPass viene null (por ejemplo, un dato no cargado en el flujo), debe quedar
            // contenido por el catch general en vez de tirar abajo el workflow.
            dto.CartaPorteByPass = null;

            var comando = new CrearCartaPorteByPass
            {
                Orden = dto,
                NombreWorkflow = "W1",
                InstanciaWorkflowId = Guid.NewGuid(),
                CentroId = 5,
                WorkflowDefinicionId = 12,
                Vehiculo = new VehiculoDto { Patente = "AAA111", Primero = true, TipoVehiculo = TipoVehiculo.Camión, PesoTaraOrigen = 1000, PesoBrutoOrigen = 2000 }
            };

            Resultado resultado = null;
            Assert.DoesNotThrow(() => resultado = target.Ejecutar(comando));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.First().Value, Is.EqualTo(Textos.CartaDePorte_Error));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<CartaPorte>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
        }

        [Test]
        public void CuandoVehiculoEsNulo_NoPropagaExcepcionYRetornaResultadoConError()
        {
            // comando.Vehiculo.TipoVehiculo/Patente se acceden sin chequeo de nulidad.
            var comando = new CrearCartaPorteByPass
            {
                Orden = dto,
                NombreWorkflow = "W1",
                InstanciaWorkflowId = Guid.NewGuid(),
                CentroId = 5,
                WorkflowDefinicionId = 12,
                Vehiculo = null
            };

            Resultado resultado = null;
            Assert.DoesNotThrow(() => resultado = target.Ejecutar(comando));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.First().Value, Is.EqualTo(Textos.CartaDePorte_Error));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<CartaPorte>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
        }

        [Test]
        public void CuandoNoEncuentraElWorkflow_NoInsertaNiGuardaCambios()
        {
            var comando = new CrearCartaPorteByPass
            {
                Orden = dto,
                NombreWorkflow = "WorkflowInexistente",
                InstanciaWorkflowId = Guid.NewGuid(),
                CentroId = 5,
                WorkflowDefinicionId = 12,
                Vehiculo = new VehiculoDto { Patente = "AAA111", Primero = true, TipoVehiculo = TipoVehiculo.Camión, PesoTaraOrigen = 1000, PesoBrutoOrigen = 2000 }
            };

            var resultado = target.Ejecutar(comando);

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<CartaPorte>()), Times.Never());
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Recorrido>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
        }

        [Test]
        public void CuandoOcurreUnaExcepcionGenericaDeBaseDeDatos_RetornaResultadoConError()
        {
            var comando = new CrearCartaPorteByPass
            {
                Orden = dto,
                NombreWorkflow = "W1",
                InstanciaWorkflowId = Guid.NewGuid(),
                CentroId = 5,
                WorkflowDefinicionId = 12,
                Vehiculo = new VehiculoDto { Patente = "AAA111", Primero = true, TipoVehiculo = TipoVehiculo.Camión, PesoTaraOrigen = 1000, PesoBrutoOrigen = 2000 }
            };

            repositorioMock.Setup(s => s.GuardarCambios()).Throws(new IOException("Error de conexión a la base de datos"));

            var resultado = target.Ejecutar(comando);

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.First().Value, Is.EqualTo(Textos.CartaDePorte_Error));
        }

        [Test]
        public void CuandoGuardarCambiosLanzaDbEntityValidationException_RetornaResultadoConErrorDeValidacion()
        {
            var comando = new CrearCartaPorteByPass
            {
                Orden = dto,
                NombreWorkflow = "W1",
                InstanciaWorkflowId = Guid.NewGuid(),
                CentroId = 5,
                WorkflowDefinicionId = 12,
                Vehiculo = new VehiculoDto { Patente = "AAA111", Primero = true, TipoVehiculo = TipoVehiculo.Camión, PesoTaraOrigen = 1000, PesoBrutoOrigen = 2000 }
            };

            // El catch del procesador detecta el tipo por nombre ("DbEntityValidationException") y accede
            // dinámicamente a EntityValidationErrors/ValidationErrors, por lo que un tipo local con la misma
            // forma alcanza para simular la excepción real de EF sin depender de EntityFramework en el test.
            var excepcionValidacion = new DbEntityValidationException
            {
                EntityValidationErrors = new[]
                {
                    new FakeEntityValidationResult
                    {
                        ValidationErrors = new[]
                        {
                            new FakeValidationError { PropertyName = "NroCartaPorte", ErrorMessage = "El campo es requerido" }
                        }
                    }
                }
            };
            repositorioMock.Setup(s => s.GuardarCambios()).Throws(excepcionValidacion);

            var resultado = target.Ejecutar(comando);

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.First().Value, Is.EqualTo(Textos.CartaDePorte_Error));
        }

        // Tipos "duck-typed" que imitan la forma de las excepciones reales de Entity Framework
        // (DbEntityValidationException / DbEntityValidationResult / DbValidationError), evitando
        // agregar una referencia a EntityFramework solo para este test.
        // Deben ser públicas: el DLR (dynamic) resuelve miembros según la visibilidad del tipo en tiempo
        // de ejecución. Como ProcesadorCrearCartaPorteByPass vive en otro assembly, una clase/miembro
        // privado no sería accesible para el binder dinámico y el catch fallaría en encontrar el miembro.
        public class DbEntityValidationException : Exception
        {
            public IEnumerable EntityValidationErrors { get; set; }
        }

        public class FakeEntityValidationResult
        {
            public IEnumerable ValidationErrors { get; set; }
        }

        public class FakeValidationError
        {
            public string PropertyName { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
