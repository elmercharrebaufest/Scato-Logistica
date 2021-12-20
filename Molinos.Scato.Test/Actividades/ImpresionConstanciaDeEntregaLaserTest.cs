using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Dominio.Dto;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ImpresionConstanciaDeEntregaLaserTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servComando;
        private ImpresionConstanciaDeEntregaLaser target;
        private WorkflowInvokerTest host;
        private Mock<IFirmaProvider> firmaProvider;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servComando = new Mock<IServicioComandos>();
            target = new ImpresionConstanciaDeEntregaLaser();
            host = WorkflowInvokerTest.Create(target);
            firmaProvider = new Mock<IFirmaProvider>();

            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servComando.Object);
            host.Extensions.Add(firmaProvider.Object);

            var cartaDePorte = new CartaPorteDto
                {
                    Id = 1,
                    TipoDeWorkflow = TipoDeWorkflow.Ingreso,
                    DestinatarioCodigoSap = "DestCodSap",
                    Destinatario = "Destinatario",
                    RtteComercial = "RtteComercial",
                    TitularCartaPorte = "TitularCartaPorte",
                    FechaEmision = new DateTime(2015, 6, 6),
                    Corredor = "C",
                    Material = "Material",
                    NroCartaPorte = "123456789012",
                    Procedencia = "Procedencia"
                };
            host.InArguments.Orden = cartaDePorte;
            host.InArguments.CentroId = 1;
            host.InArguments.NumeroIngreso = "123456789012";
            host.InArguments.WorkflowId = Guid.NewGuid();
            host.InArguments.Vehiculo = new VehiculoDto
                {
                    Patente = "AAA001"
                };
            host.InArguments.PuestoDeTrabajoId = 1;

            firmaProvider.Setup(s => s.ObtenerFirmaSinLogo())
                           .Returns(new FirmaDto());

            servRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                           .Returns(new CentroDto
                               {
                                   Descripcion = "Centro1",
                                   LocalidadDesc = "San Lorenzo",
                                   ProvinciaDesc = "Santa Fe",
                                   Direccion = "Direccion1"
                               });
            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>()))
                           .Returns(new RecorridoDto
                               {
                                   Calado = new CaladoDto() {Id = 1},
                                   PesoBrutoFecha = new DateTime(2015, 6, 6),
                                   PesoTaraFecha = new DateTime(2015, 5, 5)
                               });
            servRepositorio.Setup(s => s.ListarAnalisisYCaladoPorCaracteristica(It.IsAny<Guid>()))
                           .Returns(new List<AnalisisPorCaracteristicaDto>
                               {
                                   new AnalisisPorCaracteristicaDto
                                       {
                                           Caracteristica = "Humedad",
                                           ValorAnalisis = 1,
                                           ValorCalado = 1
                                       }
                               });
            servRepositorio.Setup(s => s.ObtenerBalanzasPorGuid(It.IsAny<Guid>()))
                           .Returns(new BalanzasDto
                               {
                                   BalanzaBruto = new BalanzaDto {CentroId = 1, Nombre = "Bruto", Id = 1, Modelo = "M1"},
                                   BalanzaTara = new BalanzaDto {Nombre = "Tara", Modelo = "M2", Id = 2}
                               });

            servRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new DocumentoDeImpresionPorCentroDto {ImpresoraDireccion = "INT1"});

            servRepositorio.Setup(s => s.ObtenerMuestraEnvioACamaraPorCalado(It.IsAny<int>()))
                .Returns(It.IsAny<MuestraEnvioACamaraDto>());

            servComando.Setup(s => s.Ejecutar(It.IsAny<ImprimirConstanciaDeEntregaLaser>())).Returns(new Resultado());
            servComando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Returns(new Resultado());
            servComando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Returns(new Resultado());


        }

        [Test]
        public void ExecuteTest()
        {
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.False(((Resultado)resultado).HayErrores);
            servComando.Verify(s => s.Ejecutar(It.IsAny<ImprimirConstanciaDeEntregaLaser>()), Times.Once());
            servComando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servComando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        [Test]
        public void ExecuteExceptionsTest()
        {
            servComando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception("Error"));
            servComando.Setup(s => s.Ejecutar(It.IsAny<ImprimirConstanciaDeEntregaLaser>())).Throws(new Exception("Error"));
            servComando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception("Error"));
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "2").Value, "Ocurrió un error en la finalización de la actividad");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "Error");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
            servComando.Verify(s => s.Ejecutar(It.IsAny<ImprimirConstanciaDeEntregaLaser>()), Times.Once());
            servComando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servComando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        [Test]
        public void ExecuteExceptionsDocumentoNullTest()
        {
            servComando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception());
            servComando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception());
            servRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns((DocumentoDeImpresionPorCentroDto)null);

            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "2").Value, "Ocurrió un error en la finalización de la actividad");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "No Existe una Impresión para el Código ");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
            servComando.Verify(s => s.Ejecutar(It.IsAny<ImprimirConstanciaDeEntregaLaser>()), Times.Never());
            servComando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servComando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }
    }
}
