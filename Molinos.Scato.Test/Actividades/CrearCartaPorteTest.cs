using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;
using CrearCartaPorte = Molinos.Scato.Actividades.Internas.CrearCartaPorte;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class CrearCartaPorteTest
    {
        private CrearCartaPorte target;
        private CartaPorteDto orden = new CartaPorteDto();
        private Mock<IServicioComandos> srvComandosMock;
        private Mock<IServicioRepositorio> srvRepopsitorio;

        [SetUp]
        public void SetUp()
        {
            target = new CrearCartaPorte();
            srvComandosMock = new Mock<IServicioComandos>();
            srvRepopsitorio = new Mock<IServicioRepositorio>();

            var cartaPorte = new CartaPorte()
                {
                    TipoVehiculo = TipoVehiculo.Camión,
                    NroCartaPorte = "77777777",
                    CTG = "77777777",
                    FechaCP = DateTime.Now.AddDays(2),
                    TipoComercial = new TipoComercial() { Id = 1, Descripcion = "Tipo Comercial", CodigoSap = "TC", Sentido = "Derecha", ToleranciaDifPesoE = 4 },
                    CEE = "77777777",
                    FechaEmision = DateTime.Now,
                    FechaVto = DateTime.Now.AddDays(10),
                    TitularCartaPorte = new Proveedor() { Id = 1, Descripcion = "Proveedor", Cuil = "20312345057" },
                    Destinatario = new Proveedor() { Id = 1, Descripcion = "Proveedor", Cuil = "20312345058" },
                    Transportista = new Transportista() { Id = 1, Cuit = "20312345057", RazonSocial = "Pedro SRL" },
                    Chofer = new Chofer { Id = 1, Nombre = "Lautaro", NumeroDeDocumento = "31234505" },
                    Cosecha = "12-13",
                    Procedencia = new Localidad() { Id = 1, Descripcion = "Lautaro", Provincia = new Provincia() { Id = 1, Descripcion = "Malos Aires" } },
                    OrigenVehiculo = OrigenVehiculo.Argentina,
                    KmRecorrer = 798,
                    TarifaTonelada = 12,
                    FleteAPagar = true,
                    CentroDestino = new Centro() { Id = 1, Descripcion = "Centro 1" }
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
                                            { "WorkflowDefinicionId", 4 }
                                        };

            srvComandosMock.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.CrearCartaPorte>())).Returns(new ResultadoCrear());

            var cartaPorte = new CartaPorteDto()
            {
                TipoVehiculo = TipoVehiculo.Camión,
                NroCartaPorte = "77777777",
                CTG = "77777777",
                FechaCP = DateTime.Now.AddDays(2),
                TipoComercial = "tipoComercial",
                CEE = "77777777",
                FechaEmision = DateTime.Now,
                FechaVto = DateTime.Now.AddDays(10),
                TitularCartaPorte = "Test",
                Destinatario = "Test",
                Transportista = "Test",
                //Chofer.Id = 1,
                Cosecha = "12-13",
                Procedencia = "Laurencena",
                OrigenVehiculo = OrigenVehiculo.Argentina,
                KmRecorrer = 798,
                TarifaTonelada = 12,
                FleteAPagar = true,
                Destino = "4",
                VehiculoDemorado = false
            };

            srvRepopsitorio.Setup(s => s.ObtenerCartaPorte(It.IsAny<int>())).Returns(cartaPorte);
            srvRepopsitorio.Setup(s => s.ObtenerUltimaWorkflowDefinicionPorCordigo(It.IsAny<string>())).Returns(1);

            target = new CrearCartaPorte();
            var invoker = new WorkflowInvoker(target);
            invoker.Extensions.Add(() => srvComandosMock.Object);
            invoker.Extensions.Add(() => srvRepopsitorio.Object);

            var resultado = invoker.Invoke(entrada);

            var result = resultado.First(f => f.Key == "Result").Value as ResultadoCrear;
            var nroCartaPorte = resultado.First(f => f.Key == "NumeroCartaPorte").Value;
            var ordenResultado = resultado.First(f => f.Key == "CartaPorte").Value as CartaPorteDto; 

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
            Assert.That(ordenResultado.Chofer, Is.EqualTo(cartaPorte.Chofer));
            Assert.That(ordenResultado.Transportista, Is.EqualTo(cartaPorte.Transportista));
            Assert.That(ordenResultado.Material, Is.EqualTo(cartaPorte.Material));
        }

      
    }
}
