using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorFinDeActividadTest
    {
        private ProcesadorFinDeActividad target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private ConversorAutoMapper conversor;
        private Guid instanceId;
        private string actividad;
        private int puestoDeTrabajoId;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorFinDeActividad(repositorioMock.Object, conversor, orquestadorMock.Object, new NullLogger());
            instanceId = new Guid();
            actividad = "Actividad 1";
            puestoDeTrabajoId = 1;
        }

        [Test]
        public void TestEjecutar()
        {
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns(new Recorrido { Workflow = new Workflow { Id = 1 }, Centro = new Centro() });
            repositorioMock.Setup(s => s.Obtener<ActividadPorDispositivo>(It.IsAny<Expression<Func<ActividadPorDispositivo, bool>>>()))
                .Returns(new ActividadPorDispositivo { Salida = "Dispositivo Salida", VideoCamaras = new List<VideoCamara>() { new VideoCamara { Codigo = "CAM1" } } });
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarAperturaBarrera>())).Returns(new ResultadoEjecutar{Mensaje = new Mensaje{Codigo = 0}});
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarTomarFoto>())).Returns(new ResultadoEjecutar { Mensaje = new Mensaje { Codigo = 0 } });
            var comando = new FinDeActividad { InstanceId = instanceId, Actividad = actividad, PuestoDeTrabajoId = puestoDeTrabajoId};
            var resultado = target.Ejecutar(comando);
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestEjecutarConError()
        {
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns(new Recorrido { Workflow = new Workflow { Id = 1 } });
            repositorioMock.Setup(s => s.Obtener<ActividadPorDispositivo>(It.IsAny<Expression<Func<ActividadPorDispositivo, bool>>>()))
                .Returns(new ActividadPorDispositivo { Salida = "Dispositivo Salida" });
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarAperturaBarrera>())).Returns(new ResultadoEjecutar { Mensaje = new Mensaje { Codigo = 1, Descripcion = "Error" } });
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarTomarFoto>())).Returns(new ResultadoEjecutar { Mensaje = new Mensaje { Codigo = 1, Descripcion = "Error" } });
            var comando = new FinDeActividad { InstanceId = instanceId, Actividad = actividad, PuestoDeTrabajoId = puestoDeTrabajoId };
            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}