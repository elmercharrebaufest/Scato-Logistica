using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearAnalisisDeCalidadTest
    {
        private ProcesadorCrearAnalisisDeCalidad target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private AnalisisPorCaracteristicaDto[] caracteristicasDto;
        private Mock<ICalculadoraDescuento> calculadora;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            calculadora = new Mock<ICalculadoraDescuento>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearAnalisisDeCalidad(repositorioMock.Object, conversor, new NullLogger(), calculadora.Object);
            caracteristicasDto = new[]
            {
                new AnalisisPorCaracteristicaDto { CaracteristicaId = 1, Caracteristica = "C1", Rango = "R1-R2", Unidad = "KG", ValorAnalisis = 22, ValorCalado = 33}, 
                new AnalisisPorCaracteristicaDto { CaracteristicaId = 2, Caracteristica = "C2", Rango = "R1-R2", Unidad = "KG", ValorAnalisis = 33}
            };

            repositorioMock.Setup(
                s => s.Obtener<CaracteristicaDeCalidad>(It.Is<int>(i => caracteristicasDto[0].CaracteristicaId == i)))
                           .Returns(new CaracteristicaDeCalidad
                               {
                                   SituacionEnvioACamara = EnvioACamara.EnCoordinacion
                               });

            repositorioMock.Setup(
                s => s.Obtener<CaracteristicaDeCalidad>(It.Is<int>(i => caracteristicasDto[1].CaracteristicaId == i)))
                           .Returns(new CaracteristicaDeCalidad
                           {
                               SituacionEnvioACamara = EnvioACamara.Siempre
                           });
        }

        [Test]
        public void TestEjecutar()
        {
            var comando = new CrearAnalisisDeCalidad { WorkflowInstanceId = new Guid(), NumeroDeOrden = "1234", Caracteristicas = caracteristicasDto, PesoNetoOrigen = 1000};
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { Id = 1 });
            calculadora.Setup(x =>
                x.ActualizarMermaVolatil(null, It.IsAny<Dominio.Entidades.AnalisisDeCalidad>(),
                    1000)).Returns(200);
            Dominio.Entidades.AnalisisDeCalidad analisis = null;
            repositorioMock.Setup(s => s.Agregar(It.IsAny<Dominio.Entidades.AnalisisDeCalidad>())).Callback<Dominio.Entidades.AnalisisDeCalidad>(a =>
            {
                analisis = a;
            });

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Dominio.Entidades.AnalisisDeCalidad>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            calculadora.Verify(s => s.CalcularDescuentoEnPorcentaje(It.IsAny<CaracteristicaDeCalidad>(), It.IsAny<decimal>(), It.IsAny<Guid>()), Times.Exactly(2));
            calculadora.Verify(s => s.CalcularDescuentoEnKg(It.IsAny<CaracteristicaDeCalidad>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<Guid>()), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(analisis.CaracteristicasAnalizadas.Select(s => s.ValorAnalisis), Is.EquivalentTo(new List<decimal?> { 22, 33 }));
            Assert.That(analisis.CaracteristicasAnalizadas[1].DescuentoEnKg, Is.EqualTo(0));
        }

        [Test]
        public void TestEjecutarSinValor()
        {
            caracteristicasDto[1].ValorAnalisis = null;
            var comando = new CrearAnalisisDeCalidad { WorkflowInstanceId = new Guid(), NumeroDeOrden = "1234", Caracteristicas = new[]{caracteristicasDto[1]} };
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { Id = 1 });
            calculadora.Setup(x =>
                x.ActualizarMermaVolatil(null, It.IsAny<Dominio.Entidades.AnalisisDeCalidad>(),
                    1000)).Returns(200);
            Dominio.Entidades.AnalisisDeCalidad analisis = null;
            repositorioMock.Setup(s => s.Agregar(It.IsAny<Dominio.Entidades.AnalisisDeCalidad>())).Callback<Dominio.Entidades.AnalisisDeCalidad>(a =>
            {
                analisis = a;
            });

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Dominio.Entidades.AnalisisDeCalidad>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            calculadora.Verify(s => s.CalcularDescuentoEnPorcentaje(It.IsAny<CaracteristicaDeCalidad>(), It.IsAny<decimal>(), It.IsAny<Guid>()), Times.Never());
            calculadora.Verify(s => s.CalcularDescuentoEnKg(It.IsAny<CaracteristicaDeCalidad>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<Guid>()), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(analisis.CaracteristicasAnalizadas.Select(s => s.ValorAnalisis), Is.EquivalentTo(new List<decimal?> {null }));
        }
    }
}