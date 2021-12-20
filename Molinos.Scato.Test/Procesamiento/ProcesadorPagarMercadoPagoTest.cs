using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]

    public class ProcesadorPagarMercadoPagoTest
    {
        private ProcesadorPagarMercadoPago target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private Mock<IServicioMercadoPago> servicioMercadoPago;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            servicioMercadoPago = new Mock<IServicioMercadoPago>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorPagarMercadoPago(repositorioMock.Object, conversor, new NullLogger(), servicioMercadoPago.Object);
        }


        [Test]
        public void TestEjecutar()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<PagoConMercadoPago, bool>>>())).Returns((PagoConMercadoPago)null);
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { Id = 5});
            repositorioMock.Setup(s => s.Agregar(It.IsAny<PagoConMercadoPago>())).Returns(new PagoConMercadoPago { Id = 1, Recorrido = new Recorrido { Id = 5 }, Estado = "Pendiente" });
            repositorioMock.Setup(s => s.GuardarCambios()).Returns(1);
            servicioMercadoPago.Setup(x =>
                x.Pagar(It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(new EstadoPagoDto { DetalleDeEstado = "TEST-1" });
            
            var comando = new PagarMercadoPago { dto = new ValoresPagarConMercadoPagoDto { Token = "123", Monto = 100.5M, RecorridoId = 1 } };

            var resultado = target.Ejecutar(comando);


            repositorioMock.Verify(s => s.Agregar(It.IsAny<PagoConMercadoPago>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            servicioMercadoPago.Verify(s => s.Pagar(It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(1));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
