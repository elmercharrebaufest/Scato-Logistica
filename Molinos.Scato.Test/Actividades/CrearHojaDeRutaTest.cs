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
using CrearHojaDeRuta = Molinos.Scato.Actividades.Internas.CrearHojaDeRuta;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class CrearHojaDeRutaTest
    {
        private CrearHojaDeRuta target;
        private HojaDeRutaDto orden;
        private Mock<IServicioComandos> srvComandosMock;
        private Mock<IServicioRepositorio> srvRepopsitorio;

        [SetUp]
        public void SetUp()
        {
            target = new CrearHojaDeRuta();
            srvComandosMock = new Mock<IServicioComandos>();
            srvRepopsitorio = new Mock<IServicioRepositorio>();

            orden = new HojaDeRutaDto
            {
                Chofer = new ChoferDto { Id = 1, Nombre = "Emilio", NumeroDeDocumento = "123" },
                PatenteCamion = "AAA111",
                TipoComercialId = 1,
                TransportistaId = 1,
                MaterialId = 1,
                Numero = "1234-12345678"
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
                                            { "Usuario", "usuario" }
                                        };

            srvComandosMock.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.CrearHojaDeRuta>())).Returns(new ResultadoCrear());

            var newOrden = new HojaDeRutaDto
            {
                Chofer = orden.Chofer,
                Id = 1,
                TipoComercialId = orden.TipoComercialId,
                TransportistaId = orden.TransportistaId,
                Transportista = "T",
                PatenteCamion = orden.PatenteCamion,
                MaterialId = orden.MaterialId
            };

            srvRepopsitorio.Setup(s => s.ObtenerHojaDeRuta(It.IsAny<int>())).Returns(newOrden);

            target = new CrearHojaDeRuta();
            var invoker = new WorkflowInvoker(target);
            invoker.Extensions.Add(() => srvComandosMock.Object);
            invoker.Extensions.Add(() => srvRepopsitorio.Object);

            var resultado = invoker.Invoke(entrada);

            var result = resultado.First(f => f.Key == "Result").Value as ResultadoCrear;
            var tipoDocumento = resultado.First(f => f.Key == "TipoDocumentoIngreso").Value as TipoDocumentoIngreso?;
            var ordenResultado = resultado.First(f => f.Key == "HojaDeRuta").Value as HojaDeRutaDto;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
            Assert.That(tipoDocumento, Is.EqualTo(TipoDocumentoIngreso.HojaDeRuta));
            Assert.That(ordenResultado.Chofer, Is.EqualTo(newOrden.Chofer));
            Assert.That(ordenResultado.Transportista, Is.EqualTo(newOrden.Transportista));
        }


    }
}
