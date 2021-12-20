using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearBalanzaModificarModalidadTest
    {
        private ProcesadorCrearBalanzaModificarModalidad target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private BalanzaModificacionModalidadDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearBalanzaModificarModalidad(repositorioMock.Object, conversorMock, new NullLogger());
            tipoDto = new BalanzaModificacionModalidadDto
                {
                    Id = 1,
                    Modalidad = Modalidad.Automática,
                    BalanzaId = 1,
                    Fecha = DateTime.Now
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Balanza>(It.IsAny<int>())).Returns(new Balanza
            {
                    Id = 1,
                    CentroEmisor = 2,
                    CodigoCabezal = "2",
                    Color = "#FFFFFF",
                    EstaEnCero = false,
                    MaximoValorCereo = 2,
                    Modalidad = Modalidad.Manual,
                    Nombre = "PruebaMod",
                    TipoBalanza = TipoBalanza.Piso,
                    TipoAcceso = TipoAcceso.EntradaSalida,
                    ToleranciaIndianapolis = 2,
                    ToleranciaOrigen = 2,
                    ToleranciaRechazo = 2,
                    ToleranciaxMil = 2
            });
            var comando = new CrearBalanzaModificarModalidad { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<BalanzaModificacionModalidad>(o => o.Modalidad == tipoDto.Modalidad)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            
                
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
