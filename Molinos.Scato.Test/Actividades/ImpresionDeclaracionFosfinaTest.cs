using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ImpresionDeclaracionFosfinaTest
    {
        private ImpresionDeclaracionFosfina target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomando;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            target = new ImpresionDeclaracionFosfina();

            servRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                           .Returns(new CentroDto { Descripcion = "Centro1", Id = 1 });
            servRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>()))
                           .Returns(new TransportistaDto {Id = 1, Cuit = "30-12345678-1"});
            servRepositorio.Setup(s => s.ObtenerProveedor(It.IsAny<int>()))
                           .Returns(new ProveedorDto {Id = 1, Activo = true, Descripcion = "P1"});
            servRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new DocumentoDeImpresionPorCentroDto());

            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirDeclaracionFosfina>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Returns(new Resultado());

            host = WorkflowInvokerTest.Create(target);

            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servcomando.Object);

            host.InArguments.CentroId = 1;
            host.InArguments.CantCopias = 1;
            host.InArguments.Orden = new CartaPorteDto
                {
                    TransportistaId = 1,
                    DestinatarioId = 1,
                    CTG = "A",
                    NroCartaPorte = "000545124578",
                    Material = "M",
                    Chofer =
                        new ChoferDto
                            {
                                Id = 1,
                                NumeroDeDocumento = "1141",
                                Nombre = "Chofer",
                                Apellido = "Chofer",
                                Cuil = "12-12345678-1"
                            },
                    DestinoDireccion = "DestDir",
                    DestinoLocalidad = "DestLoc",
                    DestinoProvincia = "DestProv"
                };
            host.InArguments.FechaCarga = new DateTime(2015, 6, 6);
            host.InArguments.PesoNeto = 15000;
            host.InArguments.WorkflowId = Guid.NewGuid();
            host.InArguments.Patente = "AAA001";
            host.InArguments.PuestoDeTrabajoId = 1;

        }

        [Test]
        public void ExecuteTest()
        {
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.False(((Resultado)resultado).HayErrores);
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirDeclaracionFosfina>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        [Test]
        public void ExecuteExceptionsTest()
        {
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirDeclaracionFosfina>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception("Error"));
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "2").Value, "Ocurrió un error en la finalización de la actividad");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "Error");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirDeclaracionFosfina>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        [Test]
        public void ExecuteExceptionsDocumentoNullTest()
        {
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception());
            servRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns((DocumentoDeImpresionPorCentroDto)null);

            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "2").Value, "Ocurrió un error en la finalización de la actividad");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "No Existe una Impresión para el Código ");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirDeclaracionFosfina>()), Times.Never());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }
    }
}
