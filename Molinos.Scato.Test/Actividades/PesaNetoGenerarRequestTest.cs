using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    class PesaNetoGenerarRequestTest
    {
        private Mock<IServicioComandos> srvComandosMock;
        private Mock<IServicioRepositorio> srvRepopsitorio;

        [SetUp]
        public void SetUp()
        {
            srvComandosMock = new Mock<IServicioComandos>();
            srvRepopsitorio = new Mock<IServicioRepositorio>();
            ConfigurationManager.AppSettings["LoguearRequestsSap"] = "1";
        }

        [Test]
        public void Test()
        {
            var target = new PesaNetoGenerarRequest();
            var invoker = new WorkflowInvoker(target);
            var entrada = new Dictionary<string, object> 
                                        {
                                            { "NumeroDocumento", "12345678912" },
                                            { "PesoBruto", 40000 },
                                            { "PesoNeto", 30000 },
                                        };


            invoker.Extensions.Add(() => srvComandosMock.Object);
            invoker.Extensions.Add(() => srvRepopsitorio.Object);

            var resultado = invoker.Invoke(entrada);

            var request = resultado.First(f => f.Key == "Request").Value as PesaNetoRequest;


            Assert.That(request.PesaNeto.Entrega, Is.EqualTo("12345678912"));
            Assert.That(request.PesaNeto.PesoNeto, Is.EqualTo(30000));
            Assert.That(request.PesaNeto.PesoBruto, Is.EqualTo(40000));
        }
    }
}
