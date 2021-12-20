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
    public class ProcesadorModificarFormatoDeImpresionTest
    {
        private ProcesadorModificarFormatoDeImpresion target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private FormatoDeImpresionDto tipoDto;
        private FormatoDeImpresion tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarFormatoDeImpresion(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new FormatoDeImpresionDto()
            {
                Id = 1,
                Descripcion = "Descripcion",
                FormatosDeCampo = new List<FormatoDeCampoDto>()

            };
            tipo = new FormatoDeImpresion
            {
                Id = 1,
                Descripcion = "Descripcion",
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<FormatoDeImpresion, bool>>>()))
                    .Returns(false);
            repositorioMock.Setup(s => s.Obtener<FormatoDeImpresion>(It.IsAny<int>()))
                    .Returns(new FormatoDeImpresion(){FormatoDePapel = new FormatoDePapel()});

            var comando = new ModificarFormatoDeImpresion() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
