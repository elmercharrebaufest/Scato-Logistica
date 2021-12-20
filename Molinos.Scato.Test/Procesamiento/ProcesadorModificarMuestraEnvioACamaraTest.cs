using System;
using System.Collections.Generic;
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
    public class ProcesadorModificarMuestraEnvioACamaraTest
    {
        private ProcesadorModificarMuestraEnvioACamara target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private MuestraEnvioACamaraDto tipoDto;
        private MuestraEnvioACamara tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarMuestraEnvioACamara(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new MuestraEnvioACamaraDto
            {
                Id = 1,
                Actividad = "Actividad 1",
                CaladoId = 1,
                CamaraId = 1,
                CaracteristicasDeCalidad = new List<CaracteristicaDeCalidadDto> { new CaracteristicaDeCalidadDto() { Descripcion = "Característica 1", Id = 1, SeEnviaACamara = true } },
                CorredorId = 1,
                Corredor = "Corredor 1",
                FechaDescarga = DateTime.Now,
                NombreUsuario = "utest",
                NroCartaPorte = "1",
                NroMuestra = "1234",
                NroMuestraTerceros = "1234",
                PesoNeto = 5,
                Vendedor = "Vendedor 1"
            };
            tipo = new MuestraEnvioACamara
            {
                Id = 1,
                CaracteristicasDeCalidad = new List<CaracteristicaDeCalidad> {new CaracteristicaDeCalidad(){ Id = 1, CaracteristicaDeCalidadMaestro = new CaracteristicaDeCalidadMaestro{Descripcion = "Carac"} }},
                Calado = null,
                Camara = new Camara { Id = 1, Descripcion = "Camrara1" },
                CartaPorte = new CartaPorte() { Id = 1 },
                FechaDescarga = DateTime.Now,
                NombreUsuario = "utest",
                NroMuestra = "1234",
                NroMuestraTerceros = "1234",
                PesoNeto = 50
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<MuestraEnvioACamara>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarEnvioACamara { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
