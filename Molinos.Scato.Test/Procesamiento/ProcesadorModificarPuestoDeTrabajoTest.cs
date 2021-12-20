using System.Collections.Specialized;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
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
    public class ProcesadorModificarPuestoDeTrabajoTest
    {
        private ProcesadorModificarPuestoDeTrabajo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private PuestoDeTrabajoDto tipoDto;
        private PuestoDeTrabajo tipo;
        private Mock<IServicioOrquestador> servicioOrquestador;
        private Mock<IConfiguracionProvider> config;

        [SetUp]
        public void SetUp()
        {
            servicioOrquestador = new Mock<IServicioOrquestador>();
            config = new Mock<IConfiguracionProvider>();
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarPuestoDeTrabajo(repositorioMock.Object, conversorMock.Object, new NullLogger(), servicioOrquestador.Object, config.Object);
            tipoDto = new PuestoDeTrabajoDto
            {
                Id = 1,
                CentroId = 1,
                NombrePuesto = "Puesto 1"
            };
            tipo = new PuestoDeTrabajo
            {
                Id = 1,
                Centro = new Centro{Id = 1},
                NombrePuesto = "Puesto 1",
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            config.Setup(cfg => cfg.AppSettings)
                .Returns(new NameValueCollection { { "UrlNotificaciones", "http://localhost/Scato.serviciosWeb" } });
            config.Setup(cfg => cfg.AppSettings)
                .Returns(new NameValueCollection { { "UrlNotificacionesWeb", "http://localhost/Scato.serviciosWeb" } });

            servicioOrquestador.Setup(orq => orq.Suscribir(It.IsAny<ComandoSuscribir>())).Returns(new ResultadoSuscribir
            {
                IdSuscripcion = 1,
                Mensaje = new Mensaje { Codigo = 0, Descripcion = "Todo OK" }
            });

            servicioOrquestador.Setup(orq => orq.CancelarSuscripcion(It.IsAny<ComandoCancelarSuscripcion>())).Returns(new ResultadoCancelarSuscripcion
            {
                Mensaje = new Mensaje { Codigo = 0, Descripcion = "Todo OK" }
            });

            repositorioMock.Setup(s => s.Obtener<PuestoDeTrabajo>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarPuestoDeTrabajo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<PuestoDeTrabajoDto>(), It.IsAny<PuestoDeTrabajo>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
