using System;
using System.Configuration;
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
    public class ImpresionCertificadoDeAnalisisTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IFirmaProvider> firmaProvider;
        private Mock<IServicioComandos> servcomando;
        private ImpresionCertificadoDeAnalisis target;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            firmaProvider = new Mock<IFirmaProvider>();
            servcomando = new Mock<IServicioComandos>();
            target = new ImpresionCertificadoDeAnalisis();
            host = WorkflowInvokerTest.Create(target);
            servRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new DocumentoDeImpresionPorCentroDto());
            servRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                           .Returns(new CentroDto());

            firmaProvider.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto());

            Guid instanceId = Guid.NewGuid();

            host.InArguments.CentroId = 1;
            host.InArguments.Calado = new[]{new CaladoPorCaracteristicaDto{EsHumedad = true} };
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirCertificadoDeAnalisis>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Returns(new Resultado());


            host.InArguments.CodigoDeImpresion = "CartaPorte";
            host.InArguments.NumeroIngreso = "123456789012";
            host.InArguments.WorkflowId = instanceId;
            host.InArguments.PuestoDeTrabajoId = 1;
            host.InArguments.CentroId = 1;

            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(firmaProvider.Object);
            host.Extensions.Add(servcomando.Object);
        }

        [Test]
        public void ExecuteTest()
        {
            //host.InArguments.DestinatarioCodigoSap = "9950085862";
            //var result = host.TestActivity();
            //var resultado = result.First(s => s.Key == "Result").Value;
            
            //Assert.NotNull(result);
            //Assert.False(((Resultado)resultado).HayErrores);
            //servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirCertificadoDeAnalisis>()), Times.Once());
            //servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            //servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        //[Test]
        //public void ExecuteExceptionsTest()
        //{
        //    servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception("Error"));
        //    servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirCertificadoDeAnalisis>())).Throws(new Exception("Error"));
        //    servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception("Error"));
        //    var result = host.TestActivity();
        //    var resultado = result.First(s => s.Key == "Result").Value;

        //    Assert.NotNull(result);
        //    Assert.True(((Resultado)resultado).HayErrores);
        //    Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "2").Value, "Ocurrió un error en la finalización de la actividad");
        //    Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "Error");
        //    Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
        //    servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirCertificadoDeAnalisis>()), Times.Once());
        //    servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
        //    servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        //}

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
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirCertificadoDeAnalisis>()), Times.Never());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }
    }
}
