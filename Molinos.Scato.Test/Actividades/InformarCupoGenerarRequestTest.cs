using System;
using System.Configuration;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class InformarCupoGenerarRequestTest
    {
        private InformarCupoGenerarRequest target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomandos;
        private Mock<IServicioSapAsincronico> servSap;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomandos = new Mock<IServicioComandos>();
            servSap = new Mock<IServicioSapAsincronico>();
            target = new InformarCupoGenerarRequest();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servcomandos.Object);
            host.Extensions.Add(servSap.Object);

            servRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto());
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<CrearControlRecorrido>())).Returns(new Resultado());
            host.InArguments.CartaPorte = new CartaPorteDto();
            host.InArguments.CodigoCupo = "C";
            host.InArguments.FechaIngreso = new DateTime(2015,6,6);
            host.InArguments.CentroId = 1;
            host.InArguments.FechaPesadaTara = new DateTime(2015, 6, 6);
            host.InArguments.FechaEgreso = new DateTime(2015, 6, 6);
            host.InArguments.CamionRechazado = false;
            host.InArguments.InstanceId = Guid.NewGuid();

            ConfigurationManager.AppSettings["LoguearRequestsSap"] = "1";
        }

        [Test]
        public void Execute()
        {
            var result = host.TestActivity();
            Assert.NotNull(result);
            object resultado;
            result.TryGetValue("Resultado", out resultado);
            Assert.NotNull(resultado);
            Assert.False(((Resultado)resultado).HayErrores);
            object request;
            result.TryGetValue("Request", out request);
            Assert.NotNull(request);
            Assert.AreEqual(((Z_SDMF_Z2200NRequest)request).Z_SDMF_Z2200N.IM_Z2200[0].CODIGO, "C");
            Assert.AreEqual(((Z_SDMF_Z2200NRequest)request).Z_SDMF_Z2200N.IM_Z2200[0].RECHAZADO, null);
            Assert.AreEqual(((Z_SDMF_Z2200NRequest)request).Z_SDMF_Z2200N.IM_Z2200[0].FECHA_EGRESO, "20150606");
            Assert.AreEqual(((Z_SDMF_Z2200NRequest)request).Z_SDMF_Z2200N.IM_Z2200[0].FECHA_INGRESO, "20150606");
        }

        [Test]
        public void ExecuteException()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<CrearControlRecorrido>())).Throws(new Exception("Error"));

            var result = host.TestActivity();
            Assert.NotNull(result);
            object resultado;
            result.TryGetValue("Resultado", out resultado);
            Assert.NotNull(resultado);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.FirstOrDefault().Value, "Error");
            
        }
    }
}
