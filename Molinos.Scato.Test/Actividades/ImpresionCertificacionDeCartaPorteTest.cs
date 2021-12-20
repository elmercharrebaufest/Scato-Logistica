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
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{

    [TestFixture]
    public class ImpresionCertificadoDeCartaPorteTest
    {
        private ImpresionCertificadoDeCartaPorte target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomando;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            target = new ImpresionCertificadoDeCartaPorte();

            servRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                           .Returns(new CentroDto {Descripcion = "Centro1", Id = 1});
            servRepositorio.Setup(s => s.ObtenerRecorridoIdPorGuid(It.IsAny<Guid>())).Returns(1);
            servRepositorio.Setup(s => s.ObtenerAsignacionDePuestoComando(It.IsAny<string>()))
                           .Returns(new AsignacionDto{BalanzaBrutoId = 1});
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto{Nombre = "Balanza1"});
            servRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new DocumentoDeImpresionPorCentroDto());

            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirCertificadoDeCartaPorte>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Returns(new Resultado());

            host = WorkflowInvokerTest.Create(target);

            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servcomando.Object);

            host.InArguments.CentroId = 1;
            host.InArguments.NumeroIngreso = "123456789012";
            host.InArguments.NumeroDocumento = "123456789012";
            host.InArguments.TipoDocumento = "CartaPorte";
            host.InArguments.PesoBruto = 45000;
            host.InArguments.PesoTara = 30000;
            host.InArguments.PesoNeto = 15000;
            host.InArguments.FechaEntrada = new DateTime(2015, 6, 6);
            host.InArguments.WorkflowId = Guid.NewGuid();
            host.InArguments.Material = "Material";
            host.InArguments.Patente = "AAA001";
            host.InArguments.PesoNetoOrigen = 45000;
            host.InArguments.PuestoDeTrabajoId = 1;

        }

        [Test]
        public void ExecuteTest()
        {
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.False(((Resultado)resultado).HayErrores);
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirCertificadoDeCartaPorte>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        [Test]
        public void ExecuteExceptionsTest()
        {
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirCertificadoDeCartaPorte>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception("Error"));
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "2").Value, "Ocurrió un error en la finalización de la actividad");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "Error");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirCertificadoDeCartaPorte>()), Times.Once());
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
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirCertificadoDeCartaPorte>()), Times.Never());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }
    }
}
