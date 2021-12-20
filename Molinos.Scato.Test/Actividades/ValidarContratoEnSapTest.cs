using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ValidarContratoEnSapTest
    {
        private ValidarContratoEnSap target;
        private WorkflowInvokerTest host;
        private Mock<ZSDWS_SCATO> servSap;

        [SetUp]
        public void SetUp()
        {
            target = new ValidarContratoEnSap();
            servSap = new Mock<ZSDWS_SCATO>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servSap.Object);
            host.InArguments.Contrato = "C";
            host.InArguments.Material = "M";

            servSap.Setup(s => s.ValidaContratos(It.IsAny<ValidaContratosRequest>()))
                   .Returns(new ValidaContratosResponse1
                       {
                           ValidaContratosResponse =
                               new ValidaContratosResponse
                                   {
                                       Resultado = new ZMPES4760 {MENSAJE = "mensaje", CODIGO = 0}
                                   }
                       });
        }

        [Test]
        public void ExecuteCodigoCero()
        {
            var result = host.TestActivity();
            var resultado = result.First(x => x.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.False(((Resultado)resultado).HayErrores);
        }

        [Test]
        public void ExecuteCodigoDistintoACero()
        {
            servSap.Setup(s => s.ValidaContratos(It.IsAny<ValidaContratosRequest>()))
                   .Returns(new ValidaContratosResponse1
                   {
                       ValidaContratosResponse =
                           new ValidaContratosResponse
                           {
                               Resultado = new ZMPES4760 { MENSAJE = "mensaje", CODIGO = 1 }
                           }
                   });

            var result = host.TestActivity();
            var resultado = result.First(k => k.Key == "Result").Value;
            
            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.Values.FirstOrDefault(), "Contrato inexistente");

        }

        [Test]
        public void ExecuteException()
        {
            servSap.Setup(s => s.ValidaContratos(It.IsAny<ValidaContratosRequest>())).Throws(new Exception("Error"));
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.Values.FirstOrDefault(), "Problema al conectarse a SAP");

        }
    }
}
