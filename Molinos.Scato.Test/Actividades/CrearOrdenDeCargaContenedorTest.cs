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
using CrearOrdenDeCargaContenedor = Molinos.Scato.Actividades.Internas.CrearOrdenDeCargaContenedor;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class CrearOrdenDeCargaContenedorTest
    {
        private CrearOrdenDeCargaContenedor target;
        private OrdenDeCargaContenedorDto orden;
        private Mock<IServicioComandos> srvComandosMock;
        private Mock<IServicioRepositorio> srvRepopsitorio;

        [SetUp]
        public void SetUp()
        {
            target = new CrearOrdenDeCargaContenedor();
            srvComandosMock = new Mock<IServicioComandos>();
            srvRepopsitorio = new Mock<IServicioRepositorio>();

            orden = new OrdenDeCargaContenedorDto()
            {
                Chofer = new ChoferDto { Id = 1, Nombre = "Bruno", NumeroDeDocumento = "123" },
                PatenteCamion = "AAABBB",
                TipoComercialId = 1,
                TransportistaId = 1,
                DestinoId = 1,
                OrdenDeCargaContenedor = "111111111111"
            };

        }

        [Test]
        public void TestIngresar()
        {
            var guid = new Guid();
            const string codigo = "W1";
            const string usuario = "Usuario 1";
            var entrada = new Dictionary<string, object> 
                                        {
                                            { "Orden", orden },
                                            { "NombreWorkflow", codigo },
                                            { "InstanciaWorkflowId", guid },
                                            { "CentroId", 4 },
                                            { "Usuario", usuario},
                                            { "WorkflowDefinicionId", 4 }
                                        };

            srvComandosMock.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.CrearOrdenDeCargaContenedor>())).Returns(new ResultadoCrear());

            var newOrden = new OrdenDeCargaContenedorDto()
            {
                Chofer = orden.Chofer,
                Id = 1,
                TipoComercialId = orden.TipoComercialId,
                TransportistaId = orden.TransportistaId,
                Transportista = "T",
                PatenteCamion = orden.PatenteCamion
            };

            srvRepopsitorio.Setup(s => s.ObtenerOrdenDeCargaContenedor(It.IsAny<int>())).Returns(newOrden);

            target = new CrearOrdenDeCargaContenedor();
            var invoker = new WorkflowInvoker(target);
            invoker.Extensions.Add(() => srvComandosMock.Object);
            invoker.Extensions.Add(() => srvRepopsitorio.Object);

            var resultado = invoker.Invoke(entrada);

            var result = resultado.First(f => f.Key == "Result").Value as ResultadoCrear;
            var tipoDocumento = resultado.First(f => f.Key == "TipoDocumentoIngreso").Value as TipoDocumentoIngreso?;
            var ordenResultado = resultado.First(f => f.Key == "OrdenDeCargaContenedor").Value as OrdenDeCargaContenedorDto;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
            Assert.That(tipoDocumento, Is.EqualTo(TipoDocumentoIngreso.OrdenDeCargaContenedor));
            Assert.That(ordenResultado.Chofer, Is.EqualTo(newOrden.Chofer));
            Assert.That(ordenResultado.Transportista, Is.EqualTo(newOrden.Transportista));
        }


    }
}
