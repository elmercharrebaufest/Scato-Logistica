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
    public class ProcesadorCrearHumedimetroModificarModalidadTest
    {
        private ProcesadorCrearHumedimetroModificarModalidad target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private HumedimetroModificacionModalidadDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearHumedimetroModificarModalidad(repositorioMock.Object, conversorMock, new NullLogger());
            tipoDto = new HumedimetroModificacionModalidadDto
            {
                Id = 1,
                Modalidad = Modalidad.Automática,
                HumedimetroId = 1,
                Fecha = DateTime.Now,
                HumedimetroDescripcion = "Humedimetro 1",
                Motivo = "Motivo 1",
                NombreUsuarioResponsable = "Usuario"
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Humedimetro>(It.IsAny<int>())).Returns(new Humedimetro
            {
                Id = 1,
                Modalidad = Modalidad.Manual,
                Descripcion = "Humedimetro 1",
                Centro = new Centro{Id = 1},
                Codigo = "1"
            });
            var comando = new CrearHumedimetroModificarModalidad { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<HumedimetroModificacionModalidad>(o => o.Modalidad == tipoDto.Modalidad)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));


            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
