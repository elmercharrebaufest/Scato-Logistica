using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarReimpresionTest
    {
        private VerificarReimpresion target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> srvComando;

        [SetUp]
        public void SetUp()
        {
            target = new VerificarReimpresion();
            srvComando = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComando.Object);
        }

        [Test]
        public void ImpresionOK()
        {
            var controlRecorrido = new ControlRecorridoDto { Decision = false, Mensaje = "a", Comentario = "b"};
            var resultadoInOut = new Resultado();
            host.InArguments.ControlRecorrido = controlRecorrido;
            host.InArguments.Resultado = resultadoInOut;
            host.InArguments.ImpresionId = 1;

            var resultado = host.TestActivity();
          
            Assert.That(resultado,Is.Not.Null);
            Assert.That(host.OutArguments.ReimprimeDocumento,Is.False);
            Assert.That(resultadoInOut.HayErrores,Is.False);
        }

        [Test]
        public void Reimprimir()
        {
            var controlRecorrido = new ControlRecorridoDto { Decision = true, Mensaje = "a", Comentario = "b" };
            var resultadoInOut = new Resultado();
            host.InArguments.ControlRecorrido = controlRecorrido;
            host.InArguments.Resultado = resultadoInOut;
            host.InArguments.ImpresionId = 1;
            srvComando.Setup(x => x.Ejecutar(It.IsAny<EliminarDocumento>())).Returns(new Resultado());
            var resultado = host.TestActivity();

            Assert.That(resultado, Is.Not.Null);
            Assert.That(host.OutArguments.ReimprimeDocumento, Is.True);
            Assert.That(resultadoInOut.HayErrores, Is.False);
            srvComando.Verify(x => x.Ejecutar(It.Is<EliminarDocumento>(i => i.Id == 1)), Times.Exactly(1));
        }

        [Test]
        public void ReimprimirErrorEliminarImpresionAnterior()
        {
            var controlRecorrido = new ControlRecorridoDto { Decision = true, Mensaje = "a", Comentario = "b" };
            var resultadoInOut = new Resultado();
            host.InArguments.ControlRecorrido = controlRecorrido;
            host.InArguments.Resultado = resultadoInOut;
            host.InArguments.ImpresionId = 1;
            srvComando.Setup(x => x.Ejecutar(It.IsAny<EliminarDocumento>())).Throws(new Exception());
            var resultado = host.TestActivity();

            Assert.That(resultado, Is.Not.Null);
            Assert.That(host.OutArguments.ReimprimeDocumento, Is.True);
            Assert.That(resultadoInOut.HayErrores, Is.True);
            srvComando.Verify(x => x.Ejecutar(It.Is<EliminarDocumento>(i => i.Id == 1)), Times.Exactly(1));
        }
        
    }
}
