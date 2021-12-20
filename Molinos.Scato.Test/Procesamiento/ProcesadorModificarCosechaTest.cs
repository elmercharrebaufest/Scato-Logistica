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
    public class ProcesadorModificarCosechaTest
    {
        private ProcesadorModificarCosecha target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private CosechaDto tipoDto;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarCosecha(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new CosechaDto
                {
                    Id = 0,
                    Descripcion = "11-12",
                    EpaPesoDescontado = true
                };
        }

        [Test]
        public void TestModificarEntidad()
        {
            var tiposExistentes = new List<Cosecha>
                {
                    new Cosecha
                        {
                            Id = 1,
                            Descripcion = "11-12",
                            EpaPesoDescontado = true
                        }
                };
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Cosecha, bool>>>()))
                           .Returns<Expression<Func<Cosecha, bool>>>(q => tiposExistentes.Any((q.Compile())));

            var comando = new ModificarCosecha {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
