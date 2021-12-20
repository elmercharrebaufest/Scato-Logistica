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
    public class ProcesadorCrearLecturaBalanzadaTest
    {
        private ProcesadorCrearLecturaBalanzada target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<IServicioComandos> comandosMock;
        private Mock<IServicioOrquestador> orqMock;
        private NullLogger log;
        private Dictionary<string,string> lecturaBalanzada;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            comandosMock = new Mock<IServicioComandos>();
            orqMock = new Mock<IServicioOrquestador>();
            log = new NullLogger();
            target = new ProcesadorCrearLecturaBalanzada(repositorioMock.Object, conversorMock.Object, log, comandosMock.Object, orqMock.Object);

            lecturaBalanzada = new Dictionary<string,string>() {                
                { "numeroBalanza", "8" },
                { "id", "0000000002" },
                { "fecha", "13-11-201200:28" }
            };

        }

        [Test]
        public void TestExisteBalanzada()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<RegistroBalanzaPuerto, bool>>>())).Returns(true);
            lecturaBalanzada.Add("tipoBalanzada", "balanzada");
            var resultado = target.Ejecutar(new CrearLecturaBalanzada { CodigoDispositivo = "321", Informacion = lecturaBalanzada });
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Never());
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

        [Test]
        public void TestCrearBalanzadaTipoIncio()
        {
            repositorioMock.Setup(s => s.Obtener<BalanzaPuerto>(It.IsAny<Expression<Func<BalanzaPuerto, bool>>>())).Returns(new BalanzaPuerto());

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<RegistroBalanzaPuerto, bool>>>())).Returns(false);
            lecturaBalanzada.Add("tipoBalanzada", "inicio");
            lecturaBalanzada.Add("vapor","BUQUE");
            lecturaBalanzada.Add("bodega", "BOD 7");
            lecturaBalanzada.Add("destino", "ESPANA");
            lecturaBalanzada.Add("exportador", "MOLINOS");
            lecturaBalanzada.Add("commodity", "HARINA DE SOJA");
            lecturaBalanzada.Add("pesoProgramado", "01175000");
            lecturaBalanzada.Add("toneladasaw", "00000000");
            var resultado = target.Ejecutar(new CrearLecturaBalanzada { CodigoDispositivo = "321", Informacion = lecturaBalanzada });
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Exactly(7));
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.Errores.Values.Contains("error"), Is.EqualTo(false));
        }

        [Test]
        public void TestCrearBalanzadaTipoFin()
        {
            repositorioMock.Setup(s => s.Obtener<BalanzaPuerto>(It.IsAny<Expression<Func<BalanzaPuerto, bool>>>())).Returns(new BalanzaPuerto());

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<RegistroBalanzaPuerto, bool>>>())).Returns(false);
            lecturaBalanzada.Add("tipoBalanzada", "fin");
            lecturaBalanzada.Add("vapor", "BUQUE");
            lecturaBalanzada.Add("bodega", "BOD 7");
            lecturaBalanzada.Add("destino", "ESPANA");
            lecturaBalanzada.Add("exportador", "MOLINOS");
            lecturaBalanzada.Add("commodity", "HARINA DE SOJA");
            lecturaBalanzada.Add("toneladasnw", "01175000");
            lecturaBalanzada.Add("toneladasaw", "00000000");
            lecturaBalanzada.Add("pesoProgramado", "00000000");
            
            lecturaBalanzada.Add("fechaInicio", "13-11-201223:28");
            var resultado = target.Ejecutar(new CrearLecturaBalanzada { CodigoDispositivo = "321", Informacion = lecturaBalanzada });
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Exactly(6));
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.Errores.Values.Contains("error"), Is.EqualTo(false));
        }

        [Test]
        public void TestCrearBalanzadaTipoBalanzada()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<RegistroBalanzaPuerto, bool>>>())).Returns(false);
            repositorioMock.Setup(s => s.Obtener<BalanzaPuerto>(It.IsAny<Expression<Func<BalanzaPuerto, bool>>>())).Returns(new BalanzaPuerto());
            lecturaBalanzada.Add("tipoBalanzada", "balanzada");
            lecturaBalanzada.Add("gramos", "BUQUE");
            lecturaBalanzada.Add("toneladas", "BOD 7");
            lecturaBalanzada.Add("capacidad", "ESPANA");
            lecturaBalanzada.Add("toneladasaw", "00000000");
            
            lecturaBalanzada.Add("pesoBruto", "00000000");
            lecturaBalanzada.Add("pesoTara", "00000000");
            lecturaBalanzada.Add("pesoNeto", "00000000");

            var resultado = target.Ejecutar(new CrearLecturaBalanzada { CodigoDispositivo = "321", Informacion = lecturaBalanzada });
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.Errores.Values.Contains("error"), Is.EqualTo(false));
        }

        [Test]
        public void TestCrearBalanzadaTipoError()
        {
            repositorioMock.Setup(s => s.Obtener<BalanzaPuerto>(It.IsAny<Expression<Func<BalanzaPuerto, bool>>>())).Returns(new BalanzaPuerto());

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<RegistroBalanzaPuerto, bool>>>())).Returns(false);
            lecturaBalanzada.Add("tipoBalanzada", "error");
            var resultado = target.Ejecutar(new CrearLecturaBalanzada { CodigoDispositivo = "321", Informacion = lecturaBalanzada });
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.Errores.Values.Contains("error"), Is.EqualTo(false));
        }

    }
}