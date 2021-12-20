using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorReasignarCamionPostcaladoTest
    {
        private ProcesadorReasignarCamionPostcalado target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;
        private Recorrido recorrido;
        private Mock<IAdministradorDeCalles> administradorCallesMock;
        private Mock<IServicioComandos> serviciosMock;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            serviciosMock = new Mock<IServicioComandos>();
            log = new NullLogger();
            administradorCallesMock = new Mock<IAdministradorDeCalles>();
            target = new ProcesadorReasignarCamionPostcalado(repositorioMock.Object, conversorMock.Object, log, administradorCallesMock.Object, serviciosMock.Object);
            administradorCallesMock.Setup(x => x.ObtenerEspacioDisponibleEnCalle(6)).Returns(true);
            recorrido = new Recorrido
            {
                Id = 1,
                Almacen = new Almacen { Id = 1, Centro = new Centro { Id = 1 }, Descripcion = "Almacen 1" },
                Calle = new Calle { Id = 1, CentroId = 1 },
                PuestosDeCargaDescargas = new Collection<PuestosDeCargaDescarga> { new PuestosDeCargaDescarga { Id = 1, Centro = new Centro { Id = 1 } } },
                BalanzaBruto = new Balanza { Id = 1, Centro = new Centro { Id = 1 } },
                BalanzaTara = new Balanza { Id = 1, Centro = new Centro { Id = 1 } },
                Centro = new Centro { Id = 1, RequiereCupo = true },
                Chofer = new Chofer { Id = 1, Nombre = "Chofer" },
                DatosProximaActividad = "Tara",
                InstanciaWorkflow = new Guid("25892e17-80f6-415f-9c65-7395632f0223"),
                Material = new Material { Id = 1, Descripcion = "MaterialDesc 1" },
                NumeroDocumentoIngreso = "1111",
                Patente = "AAA111",
                TipoComercial = new TipoComercial { Id = 1, Descripcion = "Tipo 1" },
                TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                Workflow = new Workflow { Id = 1, Codigo = "a", Descripcion = "IngresoPorComprasDeGranos" },
                WorkflowDefinicion = new WorkflowDefinicion() { Id = 1 },
                Vehiculo = new Vehiculo { Id = 1, CartaPorte = new CartaPorte { Id = 1, Cupo = "MOV1" } },

            };

            repositorioMock.Setup(s => s.ObtenerProyeccion<Recorrido, CallePorRecorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CallePorRecorrido>>>())).Returns(
                                    new CallePorRecorrido { 
                                        Calle = new Calle { 
                                             Id = 1,
                                             Nombre = "Fila 1"
                                        }
                                    });

            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido
            {
                Id = 1,
                Centro = new Centro
                {
                    Id = 1,
                    RequiereCupo = false,
                },
                InstanciaWorkflow = new Guid("25892e17-80f6-415f-9c65-7395632f0223"),
                Vehiculo = new Vehiculo()
            });
        }

        [Test]
        public void TestReasignarCamionPostcalado()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Calle, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);
            var resultado = target.Ejecutar(new ReasignarCamionPostcalado { InstanciaWorkflow = new Guid("25892e17-80f6-415f-9c65-7395632f0223"), CalleId = 6 });

            Assert.That(resultado.HayErrores, Is.False);
            repositorioMock.Verify(v => v.GuardarCambios());

        }
    }
}
