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
using CrearOrdenDeDescarga = Molinos.Scato.Actividades.Internas.CrearOrdenDeDescarga;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class CrearOrdenDeDescargaTest
    {
        private CrearOrdenDeDescarga target;
        private OrdenDeDescargaDto orden;
        private Mock<IServicioComandos> srvComandosMock;
        private Mock<IServicioRepositorio> srvRepopsitorio;

        [SetUp]
        public void SetUp()
        {
            target = new CrearOrdenDeDescarga();
            srvComandosMock = new Mock<IServicioComandos>();
            srvRepopsitorio = new Mock<IServicioRepositorio>();

            orden = new OrdenDeDescargaDto
            {
                Chofer = new ChoferDto { Id = 1, Nombre = "Emilio", NumeroDeDocumento = "123" },
                PatenteCamion = "AAABBB",
                TipoComercialId = 1,
                TransportistaId = 1,
                ProveedorId = 1,
                FechaMovimiento = DateTime.Today,
                Numero = "111111111111"
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

            srvComandosMock.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.CrearOrdenDeDescarga>())).Returns(new ResultadoCrear());

            var newOrden = new OrdenDeDescargaDto
            {
                Chofer = orden.Chofer,
                Id = 1,
                TipoComercialId = orden.TipoComercialId,
                TransportistaId = orden.TransportistaId,
                Transportista = "T",
                PatenteCamion = orden.PatenteCamion
            };

            srvRepopsitorio.Setup(s => s.ObtenerOrdenDeDescarga(It.IsAny<int>())).Returns(newOrden);

            target = new CrearOrdenDeDescarga();
            var invoker = new WorkflowInvoker(target);
            invoker.Extensions.Add(() => srvComandosMock.Object);
            invoker.Extensions.Add(() => srvRepopsitorio.Object);

            var resultado = invoker.Invoke(entrada);

            var result = resultado.First(f => f.Key == "Result").Value as ResultadoCrear;
            var tipoDocumento = resultado.First(f => f.Key == "TipoDocumentoIngreso").Value as TipoDocumentoIngreso?;
            var ordenResultado = resultado.First(f => f.Key == "OrdenDeDescarga").Value as OrdenDeDescargaDto;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
            Assert.That(tipoDocumento, Is.EqualTo(TipoDocumentoIngreso.OrdenDeDescarga));
            Assert.That(ordenResultado.Chofer, Is.EqualTo(newOrden.Chofer));
            Assert.That(ordenResultado.Transportista, Is.EqualTo(newOrden.Transportista));
        }


    }
}
