using System;
using System.Collections.Generic;
using System.IO;
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
    public class ProcesadorCrearRegistroStockEpaTest
    {
        private ProcesadorCrearRegistroStockEpa target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private EstablecimientoYCosechaDto establecimientoYCosecha;
        private RegistroStockEPADto dto;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearRegistroStockEpa(repositorioMock.Object, conversor, new NullLogger());
            establecimientoYCosecha = new EstablecimientoYCosechaDto(){CodigoEstablecimiento = "Establecimiento1", Cosecha = "11-12"};
            dto = new RegistroStockEPADto()
            {
                RecorridoId = 1,
                InstanceId = new Guid("00000000-0000-0000-0000-000000000000"),
                Cosecha = "11-12",
                CodigoEstablecimiento = "Establecimiento1",
                PesoNeto = 30000,
                EPApesoDescontadoTildado = true
            };
            repositorioMock.Setup(
                x =>
                    x.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, EstablecimientoYCosechaDto>>>())).Returns(establecimientoYCosecha);
        }

        [Test]
        public void TestCrear()
        {
            var comando = new CrearRegistroStockEpa() { Dto = dto, Usuario = "User1"};

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<RegistroStockEPA>()), Times.Once());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Once());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearFallido()
        {
            var comando = new CrearRegistroStockEpa() { Dto = dto, Usuario = "User1" };
            repositorioMock.Setup(
                x =>
                    x.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, EstablecimientoYCosechaDto>>>())).Throws(new Exception());
            
            var result = target.Ejecutar(comando);
            
            repositorioMock.Verify(s => s.Agregar(It.IsAny<RegistroStockEPA>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(true));
        }
    }
}
