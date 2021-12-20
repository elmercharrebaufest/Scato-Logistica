using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ImpresionControlDeCargaTest
    {
        private ImpresionControlDeCarga target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;
        private ScatoPersistenceParticipant participant;

        [SetUp]
        public void SetUp()
        {
            target = new ImpresionControlDeCarga();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
            participant = new ScatoPersistenceParticipant();
            host.Extensions.Add(participant);

            host.InArguments.CentroId = 5;
            host.InArguments.CodigoDeImpresion = "ControlDeCarga";
            host.InArguments.Patente = "AAABBB";
            host.InArguments.WorkflowId = new Guid();
            host.InArguments.PuestoDeTrabajoId = 5;
            host.InArguments.FechaDocumentoDeIngreso = new DateTime(2010, 1, 1);
            host.InArguments.PesoBruto = Convert.ToDecimal(30000);
            host.InArguments.PesoTara = Convert.ToDecimal(1000);
            host.InArguments.PesoNeto = Convert.ToDecimal(29000);
            host.InArguments.TotalDescargado = Convert.ToDecimal(30000);
            host.InArguments.Diferencia = Convert.ToDecimal(0);
        }

        [Test]
        public void TestImprimeInforme()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto {Descripcion = "Centro1", CodigoSAP = "1234"});
            srvRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new DocumentoDeImpresionPorCentroDto());
            srvRepositorio.Setup(s => s.ObtenerImpresora(It.IsAny<int>())).Returns(new ImpresoraDto {Direccion = "Direccion"});
            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            srvComandos.Verify(p => p.Ejecutar(It.Is<CrearLogActividad>(i => i.Dto.Actividad == "Impresion Control de Carga")), Times.Exactly(1));
            srvComandos.Verify(p => p.Ejecutar(It.Is<ImprimirControlDeCarga>(i => i.Dto.Impresora == "Direccion" && i.Dto.Centro == "Centro1")), Times.Exactly(1));
            srvComandos.Verify(p => p.Ejecutar(It.Is<FinDeActividad>(i => i.Actividad == "ImpresionControlDeCarga")), Times.Exactly(1));
        }

        [Test]
        public void TestImpresionCentroError()
        {

            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto { Descripcion = "Centro1", CodigoSAP = "1234" });
            srvRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns((DocumentoDeImpresionPorCentroDto)null);
            srvRepositorio.Setup(s => s.ObtenerImpresora(It.IsAny<int>())).Returns(new ImpresoraDto { Direccion = "Direccion" });
            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            Assert.That(((Resultado)resultado.Values.First()).Errores.First().Value.Contains(Textos.Impresion_DocumentoDeImpresionPorCentroCodigoError), Is.EqualTo(true));
            srvComandos.Verify(p => p.Ejecutar(It.Is<CrearLogActividad>(i => i.Dto.Actividad == "Impresion Control de Carga")), Times.Exactly(1));
            srvComandos.Verify(p => p.Ejecutar(It.Is<ImprimirControlDeCarga>(i => i.Dto.Impresora == "Direccion" && i.Dto.Centro == "Centro1")), Times.Exactly(0));
            srvComandos.Verify(p => p.Ejecutar(It.Is<FinDeActividad>(i => i.Actividad == "ImpresionControlDeCarga")), Times.Exactly(1));
        }
    }
}
