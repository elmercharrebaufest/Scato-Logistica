using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class GenerarMuestraEnvioACamaraTest
    {
        private GenerarMuestraEnvioACamara target;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new GenerarMuestraEnvioACamara();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestGenerarMuestra()
        {
            srvRepositorio.Setup(s => s.ObtenerMaterialPorCentroPorInstanceId(It.IsAny<Guid>()))
                .Returns(new MaterialPorCentroDto() {Id = 1, MaterialId = 1});
            srvRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto { TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte, NumeroDocumentoIngreso = "111111111111", Centro = new CentroDto{Id = 1}});
            srvRepositorio.Setup(s => s.ListarAnalisisYCaladoPorCaracteristica(It.IsAny<Guid>())).Returns(new List<AnalisisPorCaracteristicaDto>());

            
            host.InArguments.InstanceId = new Guid();
            host.InArguments.CaladosPorCaracteristica = new List<CaladoPorCaracteristicaDto>().ToArray();
            host.InArguments.MaterialId = 1;
            host.InArguments.NombreUsuario = "Usuario";
            host.InArguments.CaladoId = 1;

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Result").Value as Resultado;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
        }
    }
}
