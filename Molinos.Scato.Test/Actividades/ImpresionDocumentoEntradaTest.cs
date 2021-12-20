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
    public class ImpresionDocumentoEntradaTest
    {
        private ImpresionDocumentoEntrada target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;
        private ScatoPersistenceParticipant participant;

        [SetUp]
        public void SetUp()
        {
            target = new ImpresionDocumentoEntrada();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
            participant = new ScatoPersistenceParticipant();
            host.Extensions.Add(participant);
        }

        [Test]
        public void TestImprimeDocumentoEntrada()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto());
            srvRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new DocumentoDeImpresionPorCentroDto());
            srvRepositorio.Setup(s => s.ObtenerImpresora(It.IsAny<int>())).Returns(new ImpresoraDto());
            host.InArguments.CentroId = 2;
            host.InArguments.CodigoDeImpresion = "DocumentoEntrada";
            host.InArguments.NumeroDeIngreso = "1";
            host.InArguments.Patente = "AAA000";
            host.InArguments.WorkflowId = new Guid();
            host.InArguments.FechaEmision = DateTime.Now;
            host.InArguments.PuestoDeTrabajoId = 1;

            var result = host.TestActivity();
            Assert.That(result,Is.Not.Null);

            srvRepositorio.Verify(s => s.ObtenerCentro(It.IsAny<int>()),Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerImpresora(It.IsAny<int>()), Times.Exactly(1));
            srvComandos.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Exactly(1));
            srvComandos.Verify(s => s.Ejecutar(It.IsAny<ImprimirDocumentoDeEntrada>()), Times.Exactly(1));
            srvComandos.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Exactly(1));
        }

        [Test]
        public void TestImprimeDocumentoEntradaExcepciones()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Throws(new Exception());
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto());
            srvRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new DocumentoDeImpresionPorCentroDto());
            srvRepositorio.Setup(s => s.ObtenerImpresora(It.IsAny<int>())).Returns(new ImpresoraDto());
            host.InArguments.CentroId = 2;
            host.InArguments.CodigoDeImpresion = "DocumentoEntrada";
            host.InArguments.NumeroDeIngreso = "1";
            host.InArguments.Patente = "AAA000";
            host.InArguments.WorkflowId = new Guid();
            host.InArguments.FechaEmision = DateTime.Now;
            host.InArguments.PuestoDeTrabajoId = 1;

            var result = host.TestActivity();
            Assert.That(result, Is.Not.Null);

            srvRepositorio.Verify(s => s.ObtenerCentro(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerImpresora(It.IsAny<int>()), Times.Exactly(1));
            srvComandos.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Exactly(1));
            srvComandos.Verify(s => s.Ejecutar(It.IsAny<ImprimirDocumentoDeEntrada>()), Times.Exactly(1));
            srvComandos.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Exactly(1));
        }
    }
}
