using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorFinalizarRomaneoTest
    {
        private ProcesadorFinalizarRomaneo target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private Romaneo romaneo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorFinalizarRomaneo(repositorioMock.Object, conversor, new NullLogger());
            romaneo = new Romaneo() {Estado = EstadoRomaneo.EnProceso};
            repositorioMock.Setup(x => x.Listar<Romaneo>(It.IsAny<Expression<Func<Romaneo, bool>>>())).Returns(new List<Romaneo>() { romaneo });
        }
        [Test]
        public void EjecutarTest()
        {

            target.Ejecutar(new FinalizarRomaneo {WorkflowId = new Guid()});

            Assert.That(romaneo.Estado, Is.EqualTo(EstadoRomaneo.Finalizado));
        }
    }
}