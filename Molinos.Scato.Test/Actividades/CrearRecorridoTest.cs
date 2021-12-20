using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class CrearRecorridoTest
    {
        private Scato.Actividades.Internas.CrearRecorrido target;
        private Mock<IServicioComandos> srvComandosMock;
        private Mock<IServicioRepositorio> srvRepopsitorio;

        [SetUp]
        public void SetUp()
        {
            target = new Scato.Actividades.Internas.CrearRecorrido();
            srvComandosMock = new Mock<IServicioComandos>();
            srvRepopsitorio = new Mock<IServicioRepositorio>();
        }
        [Test]
        public void TestCrearRecorrido()
        {
            var entrada = new Dictionary<string, object> 
                                        {
                                            { "InstanceIdViejo", new Guid() },
                                            { "InstanceIdNuevo", new Guid() },
                                            { "NombreWorkflow", "W1" },
                                            { "WorkflowDefinicionId", 1 },
                                            { "NombreUsuario", "" }
                                        };

            srvComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearRecorrido>())).Returns(new ResultadoCrear());
            srvRepopsitorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto());
            target = new Scato.Actividades.Internas.CrearRecorrido();
            var invoker = new WorkflowInvoker(target);
            invoker.Extensions.Add(() => srvComandosMock.Object);
            invoker.Extensions.Add(() => srvRepopsitorio.Object);
            var resultado = invoker.Invoke(entrada);
            var result = resultado.First(f => f.Key == "Result").Value as ResultadoCrear;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
            srvComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearRecorrido>()), Times.Once());

        }


    }
}
