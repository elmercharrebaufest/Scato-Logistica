using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class IngresarBinSalidaTest
    {
        private IngresarBinSalida target;
        private Mock<IServicioComandos> srvComandos;
        private WorkflowInvokerTest host;
        private ScatoPersistenceParticipant participant;

        [SetUp]
        public void SetUp()
        {
            target = new IngresarBinSalida();
            srvComandos = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            participant = new ScatoPersistenceParticipant();
            host.Extensions.Add(participant);
        }

        [Test]
        public void TestControlRecorrido()
        {
            var guid = new Guid();
            var bines = new []
                {
                    new CargaDeBinesDto {CantidadBines = 10, RemitoBodegaUvaId = 1, Tipo = "Bin30", TipoId = 100},
                    new CargaDeBinesDto {CantidadBines = 20, RemitoBodegaUvaId = 1, Tipo = "Bin40", TipoId = 200},
                };
            CargarBinesSalida comandoEnviado = null;
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<CargarBinesSalida>()))
                .Returns<CargarBinesSalida>(cmd =>
                    {
                        comandoEnviado = cmd;
                        return new ResultadoCargarBinesSalida {PesoTaraBodega = 10500};
                    });

            host.InArguments.InstanciaWorkflowId = guid;
            host.InArguments.CargaDeBines = bines;
            host.InArguments.Observacion = "una observacion";

            var resultado = host.TestActivity();
            var result = resultado.First(f => f.Key == "Result").Value as Resultado;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));

            srvComandos.Verify(srv => srv.Ejecutar(It.IsAny<Comando>()), Times.Once());
            Assert.That(comandoEnviado, Is.Not.Null);
            Assert.That(comandoEnviado.Bines, Is.EqualTo(bines));
            Assert.That(comandoEnviado.InstanciaWorkflow, Is.EqualTo(guid));
            Assert.That(comandoEnviado.Observacion, Is.EqualTo("una observacion"));
            Assert.That(host.OutArguments.PesoTaraBodega, Is.EqualTo(10500));
        }
    }
}
