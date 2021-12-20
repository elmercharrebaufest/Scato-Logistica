using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarTiempoMaximoActividadesTest
    {
        private VerificarTiempoMaximoActividades target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new VerificarTiempoMaximoActividades();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestVerificarTiempoMaximoMenor()
        {
            var guid = new Guid();
            srvRepositorio.Setup(s => s.ObtenerControlDeTiempoPorCodigoControlPorGuid("Control", guid))
                    .Returns(new ControlDeTiempoDto
                        {
                            Id = 1,
                            ActividadDesde = "Act1",
                            ActividadHasta = "Act2",
                            TiempoMaximo = 10,
                            CodigoControl = "Control"
                        });
            srvRepositorio.Setup(s => s.ObtenerTiempoEntreActividades(guid, "Act1", "Act2"))
                          .Returns(TimeSpan.FromMinutes(5));

            host.InArguments.CodigoControl = "Control";
            host.InArguments.InstanceId = guid;

            host.TestActivity();

            Assert.That(host.OutArguments.Autorizado, Is.True);
        }

        [Test]
        public void TestVerificarTiempoMaximoExcedido()
        {
            var guid = new Guid();
            srvRepositorio.Setup(s => s.ObtenerControlDeTiempoPorCodigoControlPorGuid("Control", guid))
                    .Returns(new ControlDeTiempoDto
                    {
                        Id = 1,
                        ActividadDesde = "Act1",
                        ActividadHasta = "Act2",
                        TiempoMaximo = 10,
                        CodigoControl = "Control"
                    });
            srvRepositorio.Setup(s => s.ObtenerTiempoEntreActividades(guid, "Act1", "Act2"))
                          .Returns(TimeSpan.FromMinutes(11));

            host.InArguments.CodigoControl = "Control";
            host.InArguments.InstanceId = guid;

            host.TestActivity();

            Assert.That(host.OutArguments.Autorizado, Is.False);
        }

        [Test]
        public void TestVerificarTiempoMaximoIgual()
        {
            var guid = new Guid();
            srvRepositorio.Setup(s => s.ObtenerControlDeTiempoPorCodigoControlPorGuid("Control", guid))
                    .Returns(new ControlDeTiempoDto
                    {
                        Id = 1,
                        ActividadDesde = "Act1",
                        ActividadHasta = "Act2",
                        TiempoMaximo = 10,
                        CodigoControl = "Control"
                    });
            srvRepositorio.Setup(s => s.ObtenerTiempoEntreActividades(guid, "Act1", "Act2"))
                          .Returns(TimeSpan.FromMinutes(10));

            host.InArguments.CodigoControl = "Control";
            host.InArguments.InstanceId = guid;

            host.TestActivity();

            Assert.That(host.OutArguments.Autorizado, Is.True);
        }

        [Test]
        public void TestVerificarTiempoControlNoDefinido()
        {
            var guid = new Guid();
            srvRepositorio.Setup(s => s.ObtenerControlDeTiempoPorCodigoControlPorGuid("Control", guid))
                    .Returns((ControlDeTiempoDto) null);

            host.InArguments.CodigoControl = "Control";
            host.InArguments.InstanceId = guid;

            host.TestActivity();

            Assert.That(host.OutArguments.Autorizado, Is.False);
        }

        [Test]
        public void TestVerificarTiempoMaximoNoPasaPorActividad()
        {
            var guid = new Guid();
            srvRepositorio.Setup(s => s.ObtenerControlDeTiempoPorCodigoControlPorGuid("Control", guid))
                    .Returns(new ControlDeTiempoDto
                    {
                        Id = 1,
                        ActividadDesde = "Act1",
                        ActividadHasta = "Act2",
                        TiempoMaximo = 10,
                        CodigoControl = "Control"
                    });
            srvRepositorio.Setup(s => s.ObtenerTiempoEntreActividades(guid, "Act1", "Act2"))
                          .Returns((TimeSpan?)null);

            host.InArguments.CodigoControl = "Control";
            host.InArguments.InstanceId = guid;

            host.TestActivity();

            Assert.That(host.OutArguments.Autorizado, Is.False);
        }

        [Test]
        public void TestVerificarTiempoMaximoNegativo()
        {
            var guid = new Guid();
            srvRepositorio.Setup(s => s.ObtenerControlDeTiempoPorCodigoControlPorGuid("Control", guid))
                    .Returns(new ControlDeTiempoDto
                    {
                        Id = 1,
                        ActividadDesde = "Act1",
                        ActividadHasta = "Act2",
                        TiempoMaximo = 10,
                        CodigoControl = "Control"
                    });
            srvRepositorio.Setup(s => s.ObtenerTiempoEntreActividades(guid, "Act1", "Act2"))
                          .Returns(TimeSpan.FromMinutes(-2));

            host.InArguments.CodigoControl = "Control";
            host.InArguments.InstanceId = guid;

            host.TestActivity();

            Assert.That(host.OutArguments.Autorizado, Is.False);
        }
    }
}
