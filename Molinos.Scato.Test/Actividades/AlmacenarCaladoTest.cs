using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
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
    public class AlmacenarCaladoTest
    {
        private AlmacenarCalado target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> srvComando;
        [SetUp]
        public void SetUp()
        {
            target = new AlmacenarCalado();
            srvRepositorio = new Mock<IServicioRepositorio>();
            srvComando = new Mock<IServicioComandos>();
            var scatoPErsistance = new Mock<ScatoPersistenceParticipant>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(srvComando.Object);
            host.Extensions.Add(scatoPErsistance.Object);
            srvRepositorio.Setup(x => x.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto{ Calado = new CaladoDto { Id = 2 }});

        }

        [Test]
        public void TestAlmacenarCalado()
        {
            var caracteristicas = new[] { new CaladoPorCaracteristicaDto { EsHumedad = true, ValorCalado = 5, CaracteristicaSituacionEnvioACamara = EnvioACamara.Nunca} };
            srvRepositorio.Setup(x => x.ObtenerCalidadMaterialPorHumedadEInstanceId(It.IsAny<Decimal>(),It.IsAny<bool>(),new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D"))).Returns(new CalidadMaterialDto{ Id = 1, Descripcion = "calidad"});
            srvRepositorio.Setup(x => x.ObtenerCaladoPorGuid(It.IsAny<Guid>())).Returns(new CaladoDto { Id = 2, CaladosPorCaracteristica = caracteristicas });

            srvComando.Setup(s => s.Ejecutar(It.IsAny<ModificarCalado>())).Returns(new Resultado());


            host.InArguments.InstanceId = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");
            host.InArguments.CaladosPorCaracteristica = caracteristicas;
            host.InArguments.CicloDeCalado = 1;
            host.InArguments.Rechazar = false;
            host.InArguments.ControlRecorrido = new ControlRecorridoDto();

            var resultado = host.TestActivity();

            var calado = (CaladoDto)resultado.First(f => f.Key == "Calado").Value;
            var enviaACamara = (bool)resultado.First(f => f.Key == "EnviaACamara").Value;


            Assert.That(resultado, Is.Not.Null);
            Assert.That(calado.Id, Is.EqualTo(2));
            Assert.That(enviaACamara, Is.EqualTo(false));

        }

        [Test]
        public void TestAlmacenarCaladoEnviaACamara()
        {
            var caracteristicas = new[] { new CaladoPorCaracteristicaDto { EsHumedad = true, ValorCalado = 5, CaracteristicaSituacionEnvioACamara = EnvioACamara.Siempre, EnviaACamara = true } };

            srvRepositorio.Setup(x => x.ObtenerCalidadMaterialPorHumedadEInstanceId(It.IsAny<Decimal>(), It.IsAny<bool>(), new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D"))).Returns(new CalidadMaterialDto { Id = 1, Descripcion = "calidad" });
            srvRepositorio.Setup(x => x.ObtenerCaladoPorGuid(It.IsAny<Guid>())).Returns(new CaladoDto { Id = 2, CaladosPorCaracteristica = caracteristicas });

            srvComando.Setup(s => s.Ejecutar(It.IsAny<ModificarCalado>())).Returns(new Resultado());


            host.InArguments.InstanceId = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");
            host.InArguments.CaladosPorCaracteristica = caracteristicas;
            host.InArguments.CicloDeCalado = 1;
            host.InArguments.Rechazar = false;
            host.InArguments.ControlRecorrido = new ControlRecorridoDto();

            var resultado = host.TestActivity();

            var calado = (CaladoDto)resultado.First(f => f.Key == "Calado").Value;
            var enviaACamara = (bool)resultado.First(f => f.Key == "EnviaACamara").Value;


            Assert.That(resultado, Is.Not.Null);
            Assert.That(calado.Id, Is.EqualTo(2));
            Assert.That(enviaACamara, Is.EqualTo(true));

        }

        [Test]
        public void TestAlmacenarCaladoRechazado()
        {
            srvRepositorio.Setup(x => x.ObtenerCalidadMaterialPorHumedadEInstanceId(It.IsAny<Decimal>(), It.IsAny<bool>(), new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D"))).Returns(new CalidadMaterialDto { Id = 1, Descripcion = "calidad" });
            srvRepositorio.Setup(x => x.ObtenerCaladoPorGuid(It.IsAny<Guid>())).Returns(new CaladoDto { Id = 2 });

            srvComando.Setup(s => s.Ejecutar(It.IsAny<ModificarCalado>())).Returns(new Resultado());


            host.InArguments.InstanceId = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");
            host.InArguments.CaladosPorCaracteristica = new[] { new CaladoPorCaracteristicaDto { EsHumedad = true, ValorCalado = 5 } };
            host.InArguments.CicloDeCalado = 1;
            host.InArguments.Rechazar = true;
            host.InArguments.ControlRecorrido = new ControlRecorridoDto{Mensaje = "a", Comentario = "b"};

            var resultado = host.TestActivity();

            var observacion = (string)resultado.First(f => f.Key == "Observacion").Value;


            Assert.That(resultado, Is.Not.Null);
            Assert.That(observacion, Is.EqualTo("a\nb"));

        }

    }
}
