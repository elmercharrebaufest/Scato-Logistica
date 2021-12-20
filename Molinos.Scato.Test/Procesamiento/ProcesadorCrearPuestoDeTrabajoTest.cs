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
    public class ProcesadorCrearPuestoDeTrabajoTest
    {
        private ProcesadorCrearPuestoDeTrabajo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IServicioOrquestador> servicioOrquestador;
        private Mock<IConfiguracionProvider> config;
        private IConversor conversor;
        private PuestoDeTrabajoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            servicioOrquestador = new Mock<IServicioOrquestador>();
            config = new Mock<IConfiguracionProvider>();
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearPuestoDeTrabajo(repositorioMock.Object, conversor, new NullLogger(), servicioOrquestador.Object, config.Object);
            tipoDto = new PuestoDeTrabajoDto
            {
                Id = 1,
                CentroId = 1,
                NombrePuesto = "Puesto 1",
                NombrePc = "PC 1",
                Entrada = "Barrera 1",
                Lector = "Lector 1",
                Automatico = true,
                PidePatente = true
            };
        }

        [Test]
        public void TestCrearEntidad()
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

            var comando = new CrearPuestoDeTrabajo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<PuestoDeTrabajo>(o => o.NombrePuesto == tipoDto.NombrePuesto)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
