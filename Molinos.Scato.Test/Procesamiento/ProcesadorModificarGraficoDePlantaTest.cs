using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarGraficoDePlantaTest
    {
        private ProcesadorModificarGraficoDePlanta target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private GraficoDePlantaDto tipoDto;
        private GraficoDePlanta tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarGraficoDePlanta(repositorioMock.Object, conversorMock.Object, new NullLogger());
            
            tipoDto = new GraficoDePlantaDto()
            {
                Id = 1,
                NombreActividad = "Actividad 1",
                CantidadCamionesNoDemorados = 18,
                CantidadCamionesDemorados = 14,
                Color = "Rojo",
                Rango = 10
            };
            tipo = new GraficoDePlanta()
            {
                Id = 1,
                NombreActividad = "Actividad 1",
                CantidadCamionesNoDemorados = 18,
                CantidadCamionesDemorados = 14,
                Color = "Rojo",
                Rango = 10
            };
        }

        [Test]
        public void TestModificarEntidadNoExistente()
        {
            repositorioMock.Setup(s => s.Obtener<GraficoDePlanta>(It.IsAny<string>())).Returns(tipo);
            conversorMock.Setup(s => s.Convertir<GraficoDePlantaDto, GraficoDePlanta>(tipoDto)).Returns(tipo);
            repositorioMock.Setup(s => s.Agregar(tipo));

            var comando = new ModificarGraficoDePlanta() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);

            conversorMock.Verify(s => s.Convertir<GraficoDePlantaDto, GraficoDePlanta>(tipoDto), Times.Exactly(1));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<GraficoDePlanta>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<GraficoDePlanta, bool>>>())).Returns(tipo);
            conversorMock.Setup(s => s.Convertir<GraficoDePlantaDto, GraficoDePlanta>(tipoDto)).Returns(tipo);
            repositorioMock.Setup(s => s.Agregar(tipo));

            var comando = new ModificarGraficoDePlanta() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);

            conversorMock.Verify(s => s.Convertir<GraficoDePlantaDto, GraficoDePlanta>(tipoDto), Times.Exactly(0));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<GraficoDePlanta>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
