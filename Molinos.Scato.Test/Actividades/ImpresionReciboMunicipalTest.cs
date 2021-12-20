using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ImpresionReciboMunicipalTest
    {
        private ImpresionReciboMunicipal target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;
        private ScatoPersistenceParticipant participant;

        [SetUp]
        public void SetUp()
        {
            target = new ImpresionReciboMunicipal();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
            participant = new ScatoPersistenceParticipant();
            host.Extensions.Add(participant);
        }

        [Test]
        public void TestImprimeInforme()
        {
            srvRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new DocumentoDeImpresionPorCentroDto());
            srvRepositorio.Setup(s => s.ObtenerRecorridoImpresionReciboMunicipal(It.IsAny<Guid>())).Returns(new ImpresionReciboMunicipalRecorridoDto { Ordenanza = "4" });
            srvRepositorio.Setup(s => s.ObtenerNumGaritaEntrada(It.IsAny<int>())).Returns("5");
            srvRepositorio.Setup(s => s.ObtenerNumeroDeTicketGenerado(It.IsAny<int>(), It.IsAny<bool>())).Returns(5);
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            host.InArguments.CodigoDeImpresion = "ReciboMunicipal";
            host.InArguments.WorkflowId = new Guid();
            host.InArguments.CentroId = 5;
            host.InArguments.PuestoDeTrabajoId = 5;
            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            srvComandos.Verify(p => p.Ejecutar(It.IsAny<ImprimirReciboMunicipal>()), Times.Exactly(1));
            srvComandos.Verify(p => p.Ejecutar(It.IsAny<FinDeActividad>()), Times.Exactly(1));
            srvComandos.Verify(p => p.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerRecorridoImpresionReciboMunicipal(It.IsAny<Guid>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerNumeroDeTicketGenerado(It.IsAny<int>(), It.IsAny<bool>()), Times.Exactly(1));
        }

        [Test]
        public void ImprimeInformeExcepcionCrearLogActividad()
        {
            srvRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new DocumentoDeImpresionPorCentroDto());
            srvRepositorio.Setup(s => s.ObtenerRecorridoImpresionReciboMunicipal(It.IsAny<Guid>())).Returns(new ImpresionReciboMunicipalRecorridoDto { Ordenanza = "4" });
            srvRepositorio.Setup(s => s.ObtenerNumGaritaEntrada(It.IsAny<int>())).Returns("5");
            srvRepositorio.Setup(s => s.ObtenerNumeroDeTicketGenerado(It.IsAny<int>(), It.IsAny<bool>())).Returns(5);
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Throws(new Exception());
            host.InArguments.CodigoDeImpresion = "ReciboMunicipal";
            host.InArguments.WorkflowId = new Guid();
            host.InArguments.CentroId = 5;
            host.InArguments.PuestoDeTrabajoId = 5;
            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            srvComandos.Verify(p => p.Ejecutar(It.IsAny<ImprimirReciboMunicipal>()), Times.Exactly(1));
            srvComandos.Verify(p => p.Ejecutar(It.IsAny<FinDeActividad>()), Times.Exactly(1));
            srvComandos.Verify(p => p.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerRecorridoImpresionReciboMunicipal(It.IsAny<Guid>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerNumeroDeTicketGenerado(It.IsAny<int>(), It.IsAny<bool>()), Times.Exactly(1));
        }
    }
}
