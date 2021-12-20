using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    public class ProcesadorCrearEnvioACamaraTest
    {
        private ProcesadorCrearEnvioACamara target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private MuestraEnvioACamaraDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearEnvioACamara(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new MuestraEnvioACamaraDto
                {
                    Id = 1,
                    NombreUsuario = "a",
                    CaladoId = 1
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var caracteristicas = new List<CaracteristicaDeCalidad>()
                {
                    new CaracteristicaDeCalidad
                        {
                            Id = 1
                        },
                    new CaracteristicaDeCalidad
                        {
                            Id = 2
                        }
                };
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<CaracteristicaDeCalidad, bool>>>())).Returns(caracteristicas);
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<CaladoPorCaracteristica, bool>>>()))
                           .Returns(new CaladoPorCaracteristica());
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new Recorrido{ NumeroDocumentoIngreso = "000000000000", Centro = new Centro()});
            var comando = new CrearEnvioACamara {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Obtener(It.IsAny<Expression<Func<CaladoPorCaracteristica, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(s => s.Agregar(It.Is<MuestraEnvioACamara>(o => o.NombreUsuario == tipoDto.NombreUsuario)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}