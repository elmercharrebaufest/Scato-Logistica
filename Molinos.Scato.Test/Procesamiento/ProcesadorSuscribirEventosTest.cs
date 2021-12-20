using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorSuscribirEventosTest
    {
        private ProcesadorSuscribirEventos target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private Mock<IConfiguracionProvider> configMock;
        private Mock<IServicioOrquestador> orquestadorMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            configMock = new Mock<IConfiguracionProvider>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            target = new ProcesadorSuscribirEventos(repositorioMock.Object, conversor, new NullLogger(), configMock.Object, orquestadorMock.Object);
        }

        [Test]
        public void TestSuscribirSensores()
        {
            var puestos = new List<PuestoDeTrabajo>
                {
                    new PuestoDeTrabajo {SensorQuiebre = "SENSOR1", Id = 5},
                    new PuestoDeTrabajo {SensorQuiebre = "SENSOR2", Id = 6}
                };

            configMock.Setup(cfg => cfg.AppSettings)
                .Returns(new NameValueCollection { { "UrlNotificaciones", "http://localhost/Scato.serviciosWeb" } });
            configMock.Setup(cfg => cfg.AppSettings)
                .Returns(new NameValueCollection { { "UrlNotificacionesWeb", "http://localhost/Scato.serviciosWeb" } });

            repositorioMock.Setup(r => r.Listar<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo,bool>>>())).Returns(puestos);

            orquestadorMock.Setup(orq => orq.Suscribir(It.IsAny<ComandoSuscribir>())).Returns(new ResultadoSuscribir
                {
                    IdSuscripcion = 1,
                    Mensaje = new Mensaje {Codigo = 0, Descripcion = "Todo OK"}
                });
            var resultado = target.Ejecutar(new SuscribirEventos());

            orquestadorMock.Verify(orq => orq.Suscribir(It.IsAny<ComandoSuscribir>()), Times.Exactly(10));
            orquestadorMock.Verify(orq => 
                    orq.Suscribir(It.Is<ComandoSuscribir>(cmd => cmd.CodigoDispositivo == "SENSOR1" && cmd.CodigoEvento == "EntradaActivada")), 
                    Times.Once());
            orquestadorMock.Verify(orq =>
                    orq.Suscribir(It.Is<ComandoSuscribir>(cmd => cmd.CodigoDispositivo == "SENSOR2" && cmd.CodigoEvento == "EntradaActivada")),
                    Times.Once());

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
