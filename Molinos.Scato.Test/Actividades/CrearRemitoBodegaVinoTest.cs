using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;
using CrearRemitoBodegaVino = Molinos.Scato.Actividades.Internas.CrearRemitoBodegaVino;

namespace Molinos.Scato.Test.Actividades
{  
    [TestFixture]
    public class CrearRemitoBodegaVinoTest
    {

        private CrearRemitoBodegaVino target;
        private RemitoBodegaVinoDto orden;
        private Mock<IServicioComandos> srvComandosMock;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new CrearRemitoBodegaVino();
            srvComandosMock = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();

            orden = new RemitoBodegaVinoDto
                {
                    Chofer = new ChoferDto
                        {
                            Id = 2,
                            Nombre = "Walter",
                            NumeroDeDocumento = "39987987"
                        },
                    Patente = "WMD001",
                    TipoComercialId = 2,
                    TransportistaId = 2,
                    ProveedorId = 2,
                    NroRemito = "1234-11111111"
                   
                };

        }

        [Test]
        public void TestIngresar()
        {

            var guid = new Guid();
            const string codigo = "W2";

            var entrada = new Dictionary<string, Object>
                {
                    {"Orden", orden},
                    {"NombreWorkflow", codigo},
                    {"InstanciaWorkflowId", guid},
                    {"CentroId", 3},
                    {"WorkflowDefinicionId", 4},
                   
                };

            srvComandosMock.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.CrearRemitoBodegaVino>()))
                           .Returns(new ResultadoCrear());
            var nuevaOrden = new RemitoBodegaVinoDto
                {
                    Chofer = orden.Chofer,
                    Id = 2,
                    TipoComercialId = orden.TipoComercialId,
                    TransportistaId = orden.TransportistaId,
                    Transportista = "T",
                    Patente = orden.Patente
                };

            srvRepositorio.Setup(s => s.ObtenerRemitoBodegaVino(It.IsAny<int>())).Returns(nuevaOrden);

            target = new CrearRemitoBodegaVino();
            var invoker = new WorkflowInvoker(target);
            invoker.Extensions.Add(() => srvComandosMock.Object);
            invoker.Extensions.Add(() => srvRepositorio.Object);

            var resultado = invoker.Invoke(entrada);

            var result = resultado.First(f => f.Key == "Result").Value as ResultadoCrear;
            var ordenResultado = resultado.First(o => o.Key == "RemitoBodegaVino").Value as RemitoBodegaVinoDto;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
            Assert.That(ordenResultado.Chofer, Is.EqualTo(nuevaOrden.Chofer));
            Assert.That(ordenResultado.Transportista, Is.EqualTo(nuevaOrden.Transportista));
            
        }

    }
}
