using System;
using System.Collections.Generic;
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
    public class ProcesadorCargaDeCupoTest
    {
        private ProcesadorCrearCargaDeCupo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IServicioComandos> srepositorioMock;
        private Mock<IServicioOrquestador> sorquestadorMock;

        private IConversor conversor;
        private CargaDeCupoDto dto;       

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            srepositorioMock = new Mock<IServicioComandos>();
            sorquestadorMock = new Mock<IServicioOrquestador>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearCargaDeCupo(repositorioMock.Object, conversor, new NullLogger(), srepositorioMock.Object, sorquestadorMock.Object);
            dto = new CargaDeCupoDto
                {
                    Numero="100000",
                    SinCupo = true
                };
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(new Centro());
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearCargaDeCupo {Dto = dto};
            srepositorioMock.Setup(x => x.Ejecutar(It.IsAny<GuardarfotoMesaDigitalizacion>())).Returns(new ResultadoGuardarFoto() { Path = "path"});
            repositorioMock.Setup(x => x.Obtener<PuestoDeTrabajo>(It.IsAny<int>())).Returns(new PuestoDeTrabajo {
                VideoCamaras = new List<VideoCamara> { new VideoCamara { } }
                });
            sorquestadorMock.Setup(x => x.Ejecutar(It.IsAny<EjecutarTomarFoto>())).Returns(new ResultadoEjecutar() { Mensaje = new Mensaje()});
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<CargaDeCupo>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        public void TestCrearEntidadError()
        {
            var comando = new CrearCargaDeCupo { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<CargaDeCupo>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
