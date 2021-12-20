using System;
using System.Collections.Generic;
using System.Linq;
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
   public class ProcesadorModificarTaraRomaneoTest
    {
        private ProcesadorModificarTaraRomaneo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private TaraRomaneoDto tipoDto;
        private TaraRomaneo tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarTaraRomaneo(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new TaraRomaneoDto
            {
                Id = 1,
                Descripcion = "Tara 1",
                Peso = 500
            };
            tipo = new TaraRomaneo
            {
                Id = 1,
                Descripcion = "Tara Dto 1",
                 Peso= 501
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<TaraRomaneo>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarTaraRomaneo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<TaraRomaneoDto>(), It.IsAny<TaraRomaneo>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorCodigo()
        {
            var tiposExistentes = new List<TaraRomaneo>
                {
                    new TaraRomaneo {Id = 5, Codigo = 2002, },
                    new TaraRomaneo() {Id = 6, Codigo = 2003, }
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TaraRomaneo, bool>>>()))
                    .Returns<Expression<Func<TaraRomaneo, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Codigo = 2002;
            var comando = new ModificarTaraRomaneo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Codigo"));
        }


    }
}
