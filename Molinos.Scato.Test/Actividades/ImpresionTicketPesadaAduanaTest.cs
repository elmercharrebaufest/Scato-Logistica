using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class ImpresionTicketPesadaAduanaTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomando;
        private ImpresionTicketPesadaAduana target;
        private WorkflowInvokerTest host;
        private Mock<IFirmaProvider> firmaProvider;


        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            target = new ImpresionTicketPesadaAduana();
            firmaProvider = new Mock<IFirmaProvider>();

            firmaProvider.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto());

            host = WorkflowInvokerTest.Create(target);
            servRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new DocumentoDeImpresionPorCentroDto());
            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>()))
                           .Returns(new RecorridoDto
                               {
                                   Material = new MaterialDto(),
                                   Chofer = new ChoferDto(),
                                   Transportista = new TransportistaDto(),
                                   Centro = new CentroDto(),
                                   BalanzaBrutoId = 1
                               });
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>()))
                           .Returns(new BalanzaDto());
            servRepositorio.Setup(s => s.ObtenerIngresoDeDatosDeExportacionPorRecorrido(It.IsAny<int>()))
                           .Returns(new IngresoDeDatosDeExportacionDto());

            Guid instanceId = Guid.NewGuid();

            host.InArguments.CentroId = 1;
            host.InArguments.CantCopias = 1;
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirTicketPesadaAduana>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Returns(new Resultado());


            host.InArguments.CodigoDeImpresion = "CartaPorte";
            host.InArguments.NumeroDocumento = "123456789012";
            host.InArguments.NumeroIngreso = "123456789012";
            host.InArguments.TipoDocumento = "1";
            host.InArguments.Patente = "AAA001";
            host.InArguments.Observaciones = "AAA001";
            host.InArguments.PatenteAcoplado = "ACC001";
            host.InArguments.PesoBruto = 45000;
            host.InArguments.PesoNeto = 0;
            host.InArguments.WorkflowId = instanceId;
            host.InArguments.PesoTara = 45000;
            host.InArguments.PuestoDeTrabajoId = 1;

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
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirTicketPesadaAduana>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        [Test]
        public void ExecuteExceptionsTest()
        {
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirTicketPesadaAduana>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception("Error"));
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "2").Value, "Ocurrió un error en la finalización de la actividad");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "Error");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirTicketPesadaAduana>()), Times.Once());
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
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirTicketPesadaAduana>()), Times.Never());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }
    }
}
