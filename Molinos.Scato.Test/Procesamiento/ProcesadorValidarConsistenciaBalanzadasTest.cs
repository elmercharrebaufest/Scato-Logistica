using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
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
    public class ProcesadorValidarConsistenciaBalanzadasTest
    {
        private ProcesadorValidarConsistenciaBalanzadas target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;
        private Mock<IServicioComandos> servicioComandosMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private RegistroBalanzaPuerto registro;
        private BalanzaPuerto validarBalanzada;
        private List<int> balanzadasFaltantes;
        private Dictionary<string, string> lecturaBalanzada;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            servicioComandosMock = new Mock<IServicioComandos>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            log = new NullLogger();
            target = new ProcesadorValidarConsistenciaBalanzadas(repositorioMock.Object, conversorMock.Object, log, servicioComandosMock.Object, orquestadorMock.Object);

            registro = new RegistroBalanzaPuerto {
                Fecha = DateTime.UtcNow,
                Id = 4,
                NumeroBalanza = "7",
                Tipo = "balanzada"
            };
            validarBalanzada = new BalanzaPuerto() { Id = 123, CodigoBalanza = "7", UltimaValidacion = 1 };
            balanzadasFaltantes = new List<int>() { 7, 10, 12 };
            lecturaBalanzada = new Dictionary<string, string>() {
                { "numeroBalanza", "8" },
                { "id", "0000000002" },
                { "fecha", "13-11-2012:28" },
                { "tipoBalanzada", "error" }
            };
        }

        [Test]
        public void TestProcesarOK()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<BalanzaPuerto, bool>>>())).Returns(validarBalanzada);           
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<RegistroBalanzaPuerto, int>>>(), It.IsAny<Expression<Func<RegistroBalanzaPuerto, bool>>>())).Returns(balanzadasFaltantes);
            var orqResultado = new ResultadoConsultaBalanzada { Mensaje = new Mensaje { Codigo = 0 }, ValoresBalanzada = lecturaBalanzada };
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarConsultaBalanzada>())).Returns(orqResultado);
            servicioComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearLecturaBalanzada>())).Returns(new Resultado());
            var resultado = target.Ejecutar(new ValidarConsistenciaBalanzadas { Balanza = registro.NumeroBalanza, CodigoDispositivo="321" });
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestProcesar()
        {
            var resultado = target.Ejecutar(new ValidarConsistenciaBalanzadas { Balanza = registro.NumeroBalanza });
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Never());
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}