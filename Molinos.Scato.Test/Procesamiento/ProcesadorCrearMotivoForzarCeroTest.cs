using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
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
    public class ProcesadorCrearMotivoForzarCeroTest
    {
        private ProcesadorCrearMotivoForzarCero target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearMotivoForzarCero(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestCrearMotivo()
        {
            var comando = new CrearMotivoForzarCero { Motivo = "Motivo", BalanzaId = 3, Fecha = new DateTime(2015, 10, 07, 10, 11, 22), InstanceId = new Guid(), Usuario = "manuel" };
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<Balanza, bool>>>())).Returns(new Balanza {Id = 3});
                           
            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(r => r.Agregar(It.IsAny<MotivoForzarCero>()), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
            repositorioMock.Verify(r => r.Obtener(It.IsAny<Expression<Func<Balanza, bool>>>()),Times.Exactly(1));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));

        }
    }
}
