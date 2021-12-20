using System;
using System.Collections.Generic;
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
    public class ImpresionTicketPesadaBodegaTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomando;
        private ImpresionTicketPesadaBodega target;
        private WorkflowInvokerTest host;
        private Mock<IFirmaProvider> firmaProvider;


        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            target = new ImpresionTicketPesadaBodega();
            firmaProvider = new Mock<IFirmaProvider>();

            firmaProvider.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto());

            host = WorkflowInvokerTest.Create(target);
            servRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new DocumentoDeImpresionPorCentroDto());
            servRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto());
            servRepositorio.Setup(s => s.ObtenerRecorrido(It.IsAny<int>())).Returns(new RecorridoDto{Centro = new CentroDto{Id = 1},PesoBruto = 40000,PesoTara = 10000,FechaInicio = new DateTime(),FechaEgreso = new DateTime()});
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto{Nombre = "bal1"});
            servRepositorio.Setup(s => s.ObtenerObservacion(It.IsAny<Guid>())).Returns(new ObservacionDto{Observaciones = "obs1"});
            servRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>())).Returns(new TransportistaDto());
            servRepositorio.Setup(s => s.ObtenerChofer(It.IsAny<int>())).Returns(new ChoferDto{Nombre = "jose", Apellido = "marmol"});
            servRepositorio.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto{RazonSocial = "PROVERSAL"});
            servRepositorio.Setup(s => s.ObtenerCaladoPorGuid(It.IsAny<Guid>())).Returns(new CaladoDto { CaladosPorCaracteristica = new List<CaladoPorCaracteristicaDto>{new CaladoPorCaracteristicaDto{Caracteristica = "car1",ValorCalado = 12.0m}}});
            Guid instanceId = Guid.NewGuid();
           
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirTicketPesadaBodega>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Returns(new Resultado());

            host.InArguments.CantCopias = 1;
            host.InArguments.CodigoDeImpresion = "CartaPorte";
            host.InArguments.NroDocumento = "123456789012";
            host.InArguments.PatenteCamion = "AAA001";
            host.InArguments.PatenteAcoplado = "ACC001";
            host.InArguments.ChoferId = 1;
            host.InArguments.CIU = "";
            host.InArguments.WorkflowId = instanceId;
            host.InArguments.Pedido = "";
            host.InArguments.PuestoDeTrabajoId = 1;
            host.InArguments.MaterialId = 1;
            host.InArguments.TransportistaId = 1;
            host.InArguments.ProveedorId = 1;
            host.InArguments.TransportistaId = 1;
            host.InArguments.Comprobante = "";
            host.InArguments.RecorridoId = 1;


            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servcomando.Object);
            host.Extensions.Add(firmaProvider.Object);

        }

        [Test]
        public void ExecuteTest()
        {
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.False(((Resultado)resultado).HayErrores);
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirTicketPesadaBodega>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        [Test]
        public void ExecuteExceptionsTest()
        {
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirTicketPesadaBodega>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception("Error"));
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "2").Value, "Ocurrió un error en la finalización de la actividad");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "Error");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirTicketPesadaBodega>()), Times.Once());
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
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "No Existe una Impresión para el Código CartaPorte");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirTicketPesadaBodega>()), Times.Never());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }
    }
}
