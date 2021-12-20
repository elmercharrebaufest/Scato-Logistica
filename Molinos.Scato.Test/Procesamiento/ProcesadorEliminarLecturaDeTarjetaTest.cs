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
    public class ProcesadorEliminarLecturaDeTarjetaTest
    {
        private ProcesadorEliminarLecturaDeTarjeta target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private LecturaDeTarjetaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarLecturaDeTarjeta(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new LecturaDeTarjetaDto
            {
                Id = 1,
                CodigoDispositivo = "a",
                Lectura = "1111111111"
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            repositorioMock.Setup(x => x.Existe<LecturaDeTarjeta>(It.IsAny<Expression<Func<LecturaDeTarjeta, bool>>>()))
                           .Returns(true);
            repositorioMock.Setup(x => x.Listar<LecturaDeTarjeta>(It.IsAny<Expression<Func<LecturaDeTarjeta, bool>>>()))
                           .Returns(new List<LecturaDeTarjeta> { new LecturaDeTarjeta { Id = 1 } });
            var comando = new EliminarLecturaDeTarjeta() { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
