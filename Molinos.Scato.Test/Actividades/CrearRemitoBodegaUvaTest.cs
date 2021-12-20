using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;
using CrearRemitoBodegaUva = Molinos.Scato.Actividades.Internas.CrearRemitoBodegaUva;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class CrearRemitoBodegaUvaTest
    {
        private CrearRemitoBodegaUva target;
        private RemitoBodegaUvaDto orden;
        private Mock<IServicioComandos> srvComandosMock;
        private Mock<IServicioRepositorio> srvRepopsitorio;

        [SetUp]
        public void SetUp()
        {
            target = new CrearRemitoBodegaUva();
            srvComandosMock = new Mock<IServicioComandos>();
            srvRepopsitorio = new Mock<IServicioRepositorio>();

            orden = new RemitoBodegaUvaDto
            {
                Chofer = new ChoferDto { Id = 1, Nombre = "Emilio", NumeroDeDocumento = "123" },
                Patente = "AAA111",
                TipoComercialId = 1,
                TransportistaId = 1,
                ProveedorId = 1,
                NroRemito = "1234-12345678"
            };

        }

        [Test]
        public void TestIngresar()
        {
            var guid = new Guid();
            const string codigo = "W1";
            var entrada = new Dictionary<string, object> 
                                        {
                                            { "Orden", orden },
                                            { "NombreWorkflow", codigo },
                                            { "InstanciaWorkflowId", guid },
                                            { "CentroId", 4 },
                                            { "WorkflowDefinicionId", 4 },
                                            { "EsPropia", false }
                                        };

            srvComandosMock.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.CrearRemitoBodegaUva>())).Returns(new ResultadoCrear());

            var newOrden = new RemitoBodegaUvaDto
            {
                Chofer = orden.Chofer,
                Id = 1,
                TipoComercialId = orden.TipoComercialId,
                TransportistaId = orden.TransportistaId,
                Transportista = "T",
                Patente = orden.Patente
            };

            srvRepopsitorio.Setup(s => s.ObtenerRemitoBodegaUva(It.IsAny<int>())).Returns(newOrden);

            target = new CrearRemitoBodegaUva();
            var invoker = new WorkflowInvoker(target);
            invoker.Extensions.Add(() => srvComandosMock.Object);
            invoker.Extensions.Add(() => srvRepopsitorio.Object);

            var resultado = invoker.Invoke(entrada);

            var result = resultado.First(f => f.Key == "Result").Value as ResultadoCrear;
            var tipoDocumento = resultado.First(f => f.Key == "TipoDocumentoIngreso").Value as TipoDocumentoIngreso?;
            var ordenResultado = resultado.First(f => f.Key == "RemitoBodegaUva").Value as RemitoBodegaUvaDto;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
            Assert.That(tipoDocumento, Is.EqualTo(TipoDocumentoIngreso.RemitoBodegaUvaTerceros));
            Assert.That(ordenResultado.Chofer, Is.EqualTo(newOrden.Chofer));
            Assert.That(ordenResultado.Transportista, Is.EqualTo(newOrden.Transportista));
        }


    }
}
