using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarCorrespondeLlamadaComplianceTest
    {
        private VerificarCorrespondeLlamadaCompliance target;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            
            target = new VerificarCorrespondeLlamadaCompliance();
            host = WorkflowInvokerTest.Create(target);
            host.InArguments.ValidaCompliance = true;
        }

        [Test]
        public void ValidaComplianceTrue()
        {
            var result = host.TestActivity();

            Assert.NotNull(result);
            Assert.True((bool)result.Values.FirstOrDefault());
            
        }
        [Test]
        public void ValidaComplianceFalse()
        {
            host.InArguments.ValidaCompliance = false;
            var result = host.TestActivity();

            Assert.NotNull(result);
            Assert.False((bool)result.Values.FirstOrDefault());

        }
    }
}
