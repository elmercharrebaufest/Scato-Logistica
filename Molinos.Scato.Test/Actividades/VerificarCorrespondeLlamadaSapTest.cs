using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarCorrespondeLlamadaSapTest
    {
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> servRepositorio;
        private VerificarCorrespondeLlamadaSap target;
        

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            target = new VerificarCorrespondeLlamadaSap();
            host = WorkflowInvokerTest.Create(target);
            servRepositorio.Setup(
                s => s.ExisteTransaccionSAP(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<FuncionSAP>(), It.IsAny<Guid>()))
                           .Returns(true);

            servRepositorio.Setup(s => s.ObtenerTipoDeWorkflowPorGuid(It.IsAny<Guid>())).Returns(TipoDeWorkflow.Ingreso);

            host.InArguments.InstanceId = Guid.NewGuid();
            host.InArguments.FuncionSap = FuncionSAP.AjusteDeDiferencias;

            host.Extensions.Add(servRepositorio.Object);
            
        }

        [Test]
        public void FuncionSapExisteExecute()
        {
            var result = host.TestActivity();
            object value;
            Assert.NotNull(result);
            Assert.True(result.TryGetValue("CorrespondeLlamadaSap", out value));
            Assert.True((bool)value);
            Assert.True(result.TryGetValue("EsIngreso", out value));
            Assert.True((bool)value);
        }

        [Test]
        public void FuncionSapInexistenteExecute()
        {
            servRepositorio.Setup(
                s => s.ExisteTransaccionSAP(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<FuncionSAP>(), It.IsAny<Guid>()))
                           .Returns(false);
            servRepositorio.Setup(s => s.ObtenerTipoDeWorkflowPorGuid(It.IsAny<Guid>())).Returns(TipoDeWorkflow.Egreso);

            var result = host.TestActivity();
            object value;
            Assert.NotNull(result);
            Assert.True(result.TryGetValue("CorrespondeLlamadaSap", out value));
            Assert.False((bool)value);
            Assert.True(result.TryGetValue("EsIngreso", out value));
            Assert.False((bool)value);
        }

    }
}
