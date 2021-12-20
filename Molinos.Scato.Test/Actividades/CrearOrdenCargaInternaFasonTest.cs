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

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class CrearOrdenCargaInternaFasonTest
    {
        private Molinos.Scato.Actividades.Internas.CrearOrdenCargaInternaFason target;
        private OrdenCargaInternaFasonDto orden;
        private Mock<IServicioComandos> srvComandosMock;
        private Mock<IServicioRepositorio> srvRepopsitorio;

        [SetUp]
        public void SetUp()
        {
            target = new Molinos.Scato.Actividades.Internas.CrearOrdenCargaInternaFason();
            srvComandosMock = new Mock<IServicioComandos>();
            srvRepopsitorio = new Mock<IServicioRepositorio>();

            orden = new OrdenCargaInternaFasonDto
            {
                Chofer = new ChoferDto { Id = 1, Nombre = "Emilio", NumeroDeDocumento = "123" },
                FechaEmision = new DateTime(2010, 1, 1),
                ClienteId = 1,
                MaterialId = 1,
                NumeroOrden = "11",
                PatenteCamion = "AAABBB",
                TipoComercialId = 1,
                TransportistaId = 1,
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
                                            { "NombreUsuario", "usuario" }
                                        };

            srvComandosMock.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.CrearOrdenCargaInternaFason>())).Returns(new ResultadoCrear());

            var newOrden = new OrdenCargaInternaFasonDto
            {
                Chofer = orden.Chofer,
                Id = 1,
                MaterialId = 1,
                MaterialDesc = "M",
                NumeroOrden = orden.NumeroOrden,
                TipoComercialId = orden.TipoComercialId,
                TransportistaId = orden.TransportistaId,
                Transportista = "T",
                PatenteCamion = orden.PatenteCamion
            };

            srvRepopsitorio.Setup(s => s.ObtenerOrdenCargaInternaFason(It.IsAny<int>())).Returns(newOrden);

            target = new Molinos.Scato.Actividades.Internas.CrearOrdenCargaInternaFason();
            var invoker = new WorkflowInvoker(target);
            invoker.Extensions.Add(() => srvComandosMock.Object);
            invoker.Extensions.Add(() => srvRepopsitorio.Object);

            var resultado = invoker.Invoke(entrada);

            var result = resultado.First(f => f.Key == "Result").Value as ResultadoCrear;
            var tipoDocumento = resultado.First(f => f.Key == "TipoDocumentoIngreso").Value as TipoDocumentoIngreso?;
            var ordenResultado = resultado.First(f => f.Key == "OrdenCargaInternaFason").Value as OrdenCargaInternaFasonDto;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
            Assert.That(tipoDocumento, Is.EqualTo(TipoDocumentoIngreso.OrdenCargaInterna));
            Assert.That(ordenResultado.Chofer, Is.EqualTo(newOrden.Chofer));
            Assert.That(ordenResultado.Transportista, Is.EqualTo(newOrden.Transportista));
            Assert.That(ordenResultado.MaterialDesc, Is.EqualTo(newOrden.MaterialDesc));
        }


    }
}
