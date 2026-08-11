using System;
using System.Collections.Generic;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;
using ActividadCrearCartaPorteByPass = Molinos.Scato.Actividades.CrearCartaPorteByPass;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class CrearCartaPorteByPassTest
    {
        private ActividadCrearCartaPorteByPass target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> servicioComandosMock;
        private Mock<IServicioRepositorio> servicioRepositorioMock;
        private Guid instanciaWorkflowId;

        [SetUp]
        public void SetUp()
        {
            instanciaWorkflowId = Guid.NewGuid();
            target = new ActividadCrearCartaPorteByPass();
            servicioComandosMock = new Mock<IServicioComandos>();
            servicioRepositorioMock = new Mock<IServicioRepositorio>();

            servicioRepositorioMock
                .Setup(s => s.ObtenerTipoVariedadRecorridoAnterior(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(7);
            servicioRepositorioMock
                .Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                .Returns(new CentroDto { LocalidadId = 12, CodigoEstablecimiento = "21145" });
            servicioRepositorioMock
                .Setup(s => s.ObtenerConfiguracionGeneral(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()))
                .Returns((string pantalla, string nombre, int? centroId) =>
                {
                    switch (nombre)
                    {
                        case Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Planta:
                            return new ConfiguracionGeneralDto { Valor = "10" };
                        case Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Localidad:
                            return new ConfiguracionGeneralDto { Valor = "20" };
                        case Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Provincia:
                            return new ConfiguracionGeneralDto { Valor = "30" };
                        case Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Centro:
                            return new ConfiguracionGeneralDto { Valor = "40" };
                        case Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.TipoComercialEgreso:
                            return new ConfiguracionGeneralDto { Valor = "TCB" };
                        default:
                            return null;
                    }
                });
            servicioRepositorioMock
                .Setup(s => s.ObtenerProveedorPorCuit(Constantes.Proveedores.CuitMolinos, It.IsAny<TiposProveedor>()))
                .Returns(new ProveedorDto { Id = 77, CodigoSap = "50000001" });
            servicioRepositorioMock
                .Setup(s => s.ObtenerCategoriaPorClasificacion(Constantes.ClasificacionCategorias.OPERADOR))
                .Returns(new CategoriaDto { Id = 5, Clasificacion = Constantes.ClasificacionCategorias.OPERADOR });
            servicioRepositorioMock
                .Setup(s => s.BuscarEntregador("Sin Entrega"))
                .Returns(new EntregadorDto { Id = 88, Cuil = "20-11111111-1" });
            servicioRepositorioMock
                .Setup(s => s.ObtenerTipoComercialPorCodigoSap("TCB"))
                .Returns(new TipoComercialDto { Id = 99, CodigoSap = "TCB" });
            servicioRepositorioMock
                .Setup(s => s.ObtenerCartaPorte(123))
                .Returns(new CartaPorteDto { Id = 123, NroCartaPorte = "000000000001" });
            servicioRepositorioMock
                .Setup(s => s.ObtenerRecorridoPorNumeroDocumento("000000000001"))
                .Returns(new List<RecorridoDto>
                {
                    new RecorridoDto { Id = 456, TipoComercial = new TipoComercialDto { Id = 99 } }
                });

            servicioComandosMock
                .Setup(s => s.Ejecutar(It.IsAny<Molinos.Scato.Dominio.Comandos.CrearCartaPorteByPass>()))
                .Returns(new ResultadoCrear { Id = 123 });
            servicioComandosMock
                .Setup(s => s.Ejecutar(It.IsAny<CrearCaracteristicasAnalizadas>()))
                .Returns(new ResultadoCrear());

            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(new NullLogger());
            host.Extensions.Add(servicioComandosMock.Object);
            host.Extensions.Add(servicioRepositorioMock.Object);

            host.InArguments.Orden = CrearOrdenBase();
            host.InArguments.Vehiculo = new VehiculoDto { Patente = "AAA111", PesoTaraOrigen = 12000, PesoBrutoOrigen = 30000 };
            host.InArguments.NombreWorkflow = "wfByPass";
            host.InArguments.InstanciaWorkflowId = instanciaWorkflowId;
            host.InArguments.CentroId = 4;
            host.InArguments.Usuario = "testUser";
            host.InArguments.WorkflowDefinicionId = 3;
        }

        [Test]
        public void CuandoSeEjecuta_BlancaIntervinientesYFuerzaDestinatarioYEntregadorYObservacion()
        {
            var result = host.TestActivity();

            Assert.That(result, Is.Not.Null);
            Assert.That(host.OutArguments.NumeroCartaPorte, Is.EqualTo("000000000001"));
            Assert.That(host.OutArguments.PesoTara, Is.EqualTo(12000));
            Assert.That(host.OutArguments.PesoBruto, Is.EqualTo(30000));

            servicioComandosMock.Verify(
                s => s.Ejecutar(It.Is<Molinos.Scato.Dominio.Comandos.CrearCartaPorteByPass>(c =>
                    c.Orden.RtteComercial == string.Empty &&
                    c.Orden.RtteComercialId == 0 &&
                    c.Orden.RtteComercialProductor == string.Empty &&
                    c.Orden.RtteComercialProductorId == 0 &&
                    c.Orden.CorredorVendedor == string.Empty &&
                    c.Orden.CorredorVendedorId == 0 &&
                    c.Orden.CorredorVendedorSecundario == string.Empty &&
                    c.Orden.CorredorVendedorSecundarioId == 0 &&
                    c.Orden.AgenteCompras == string.Empty &&
                    c.Orden.AgenteComprasId == 0 &&
                    c.Orden.RepresentanteRecibidor == string.Empty &&
                    c.Orden.RepresentanteRecibidorId == null &&
                    c.Orden.Entregador == "Sin Entrega" &&
                    c.Orden.EntregadorId == 88 &&
                    c.Orden.Destinatario == "MOLINOS AGRO S.A." &&
                    c.Orden.DestinatarioId == 77 &&
                    c.Orden.DestinatarioCuil == Constantes.ValoresPorDefecto.CuitMOA.ToString() &&
                    c.Orden.Observacion == string.Empty &&
                    c.Orden.Intermediario == string.Empty &&
                    c.Orden.IntermediarioId == null &&
                    c.Orden.IntermediarioCodigoSap == string.Empty &&
                    c.Orden.CodEstab == "21145")),
                Times.Exactly(1));

            servicioComandosMock.Verify(
                s => s.Ejecutar(It.Is<CrearCaracteristicasAnalizadas>(x => x.IdRecorridoIngreso == 321 && x.IdRecorridoEgreso == 456)),
                Times.Exactly(1));
        }

        private static CartaPorteDto CrearOrdenBase()
        {
            return new CartaPorteDto
            {
                NroCartaPorte = "000000000001",
                DestinoId = 1,
                DestinoProvincia = "BA",
                DestinoLocalidadCodigoSap = "LOC01",
                KmARecorrer = "250",
                Observacion = "Observacion previa",
                VehiculoDemorado = false,
                Intermediario = "INTERMEDIARIO PREVIO",
                IntermediarioId = 999,
                IntermediarioCodigoSap = "50000999",
                Vehiculos = new List<VehiculoDto>
                {
                    new VehiculoDto { Patente = "AAA111", PesoTaraOrigen = 12000, PesoBrutoOrigen = 30000 }
                },
                CartaPorteByPass = new AdicionalesCartaPorteByPassDto
                {
                    RecorridoIdIngreso = 321
                }
            };
        }
    }
}
