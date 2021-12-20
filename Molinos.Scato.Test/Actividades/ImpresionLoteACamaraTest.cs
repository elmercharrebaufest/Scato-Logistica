using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ImpresionLoteACamaraTest
    {
        private ImpresionLoteACamara target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomando;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            target = new ImpresionLoteACamara();
            host = WorkflowInvokerTest.Create(target);
            var instanceId = Guid.NewGuid();

            servRepositorio.Setup(s => s.ListarPrecintos(It.IsAny<Guid>()))
                           .Returns(new List<PrecintoDto>
                               {
                                   new PrecintoDto
                                       {
                                           Id = 1,
                                           NumeroPrecinto = "1",
                                           WorkflowInstanceId = instanceId,
                                           Detalle = "A",
                                           Eliminar = false
                                       }
                               });

            servRepositorio.Setup(s => s.ObtenerNumeroMuestraEnvioACamara(It.IsAny<int>()))
                           .Returns("");
            servRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new DocumentoDeImpresionPorCentroDto());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirEnvioLoteACamara>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Returns(new Resultado());

            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servcomando.Object);

            host.InArguments.MuestraId = 1;
            host.InArguments.CentroId = 1;
            host.InArguments.Calado = new CaladoDto
                {
                    WorkflowInstanceId = instanceId,
                    FechaCreacion = new DateTime(2011, 6, 6)
                };
            host.InArguments.CodigoDeImpresion = "CartaPorte";
            host.InArguments.NumeroDeOrden = "123456789012";
            host.InArguments.Patente = "AAA001";
            host.InArguments.WorkflowId = instanceId;
            host.InArguments.PuestoDeTrabajoId = 1;
        }

        [Test]
        public void ExecuteTest()
        {
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.False(((Resultado)resultado).HayErrores);
            //servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirEnvioLoteACamara>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        [Test]
        public void ExecuteExceptionsTest()
        {
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirEnvioLoteACamara>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception("Error"));
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "2").Value, "Ocurrió un error en la finalización de la actividad");
            //Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "Error");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
            //servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirEnvioLoteACamara>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        [Test]
        public void ExecuteExceptionsDocumentoNullTest()
        {
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception());
            servRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns((DocumentoDeImpresionPorCentroDto)null);

            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "2").Value, "Ocurrió un error en la finalización de la actividad");
            //Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "No Existe una Impresión para el Código CartaPorte");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
            //servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirEnvioLoteACamara>()), Times.Never());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }
    }
}
