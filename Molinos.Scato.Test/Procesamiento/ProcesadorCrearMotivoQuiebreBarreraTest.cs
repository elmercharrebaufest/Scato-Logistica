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
    public class ProcesadorCrearMotivoQuiebreBarreraTest
    {
        private ProcesadorCrearMotivoQuiebreBarrera target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearMotivoQuiebreBarrera(repositorioMock.Object, conversor, new NullLogger(), null);
        }

        [Test]
        public void TestCrearMotivo()
        {
            var puestos = new List<PuestoDeTrabajo>()
                {
                    new PuestoDeTrabajo {SensorQuiebre = "SENSOR1", Id = 5, Centro = new Centro{ CodigoSAP = "11" }},
                    new PuestoDeTrabajo {SensorQuiebre = "SENSOR2", Id = 6, Centro = new Centro{ CodigoSAP = "11" }}
                };
            MotivoQuiebreBarrera motivo = null;
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                           .Returns<Expression<Func<PuestoDeTrabajo, bool>>>(
                               expr => puestos.Where(expr.Compile()).ToList());
            repositorioMock.Setup(r => r.Agregar(It.IsAny<MotivoQuiebreBarrera>()))
                    .Returns<MotivoQuiebreBarrera>(x => { 
                        motivo = x;
                        return x;
                    });
            var resultado = target.Ejecutar(new CrearMotivoQuiebreBarrera {CodigoDispositivo = "SENSOR1"});
            repositorioMock.Verify(r => r.Agregar(It.IsAny<MotivoQuiebreBarrera>()), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
            Assert.That(motivo.Fecha.Date, Is.EqualTo(DateTime.Today));
            Assert.That(motivo.PuestoTrabajo.Id, Is.EqualTo(5));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
