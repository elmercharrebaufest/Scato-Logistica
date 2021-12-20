using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServicioImpresion;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorImprimirResumenDeRecepcionTest
    {
        private ProcesadorImprimirResumenDeRecepcion target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ImpResumenDeRecepcionDto dto;
        private Mock<IServicioImpresion> servicioImpresion;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            servicioImpresion = new Mock<IServicioImpresion>();

            target = new ProcesadorImprimirResumenDeRecepcion(repositorioMock.Object, conversorMock.Object, new NullLogger(), servicioImpresion.Object);

            dto = new ImpResumenDeRecepcionDto
                {
                    Codigo = "RES"
                };

        }
        [Test]
        public void TestImpresoraNoEncontrada()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<DocumentoDeImpresionPorCentro, bool>>>())).Returns((DocumentoDeImpresionPorCentro) null);
            var resultado = target.Ejecutar(new ImprimirResumenDeRecepcion{Dto = dto});
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}
