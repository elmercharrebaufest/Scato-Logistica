using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Impl;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Servicios
{
    [TestFixture]
    public class CalculadoraDescuentoTest
    {
        private ICalculadoraDescuento calculadora;
        private CaracteristicaDeCalidad caracteristica;
        private Mock<IRepositorio> repositorioMock;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            calculadora = new CalculadoraDescuento(repositorioMock.Object);
        }

        [Test]
        public void MetodoTablaCaladoMinimoDistintoDeCero()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 5,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 5,ValorHasta = 10},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 12,ValorHasta = 21},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 20.5M, ValorHasta = 50}
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 5,new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnKg(caracteristica, 5, 30000, new Guid()), Is.EqualTo(0));
        }

        [Test]
        public void MetodoTablaDentroDeRangoConRangoCompleto()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 5,ValorHasta = 10},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 12,ValorHasta = 21},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 20.5M, ValorHasta = 50}
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 0,new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 1, new Guid()), Is.EqualTo(5));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 4, new Guid()), Is.EqualTo(5));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 10, new Guid()), Is.EqualTo(5));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 11, new Guid()), Is.EqualTo(12));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 15, new Guid()), Is.EqualTo(12));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 21, new Guid()), Is.EqualTo(12));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 22, new Guid()), Is.EqualTo(20.5M));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 25, new Guid()), Is.EqualTo(20.5M));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 50, new Guid()), Is.EqualTo(20.5M));
        }

        [Test]
        public void MetodoTablaDentroDeRangoConRangoIncompleto()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 100,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 5,ValorHasta = 10},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 12,ValorHasta = 21},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 20.5M, ValorHasta = 50}
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 0, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 1, new Guid()), Is.EqualTo(5));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 4, new Guid()), Is.EqualTo(5));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 10, new Guid()), Is.EqualTo(5));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 11, new Guid()), Is.EqualTo(12));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 15, new Guid()), Is.EqualTo(12));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 21, new Guid()), Is.EqualTo(12));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 22, new Guid()), Is.EqualTo(20.5M));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 25, new Guid()), Is.EqualTo(20.5M));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 50, new Guid()), Is.EqualTo(20.5M));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 51, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 80, new Guid()), Is.EqualTo(0));
        }

        [Test]
        public void MetodoTablaSinRangos()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>()
            };

            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 0, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 1, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 35, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 50, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnKg(caracteristica, 35, 30000, new Guid()), Is.EqualTo(0));
        }

        [Test]
        public void CaracteristicaSiempreEnvio()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.Siempre,
                DescuentoEnPorcentaje = FormulaDescuento.Acumulado,
                Descuentos = new List<Descuento>
                    {
                        new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 1},
                        new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 1,ValorHasta = 3},
                        new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 1.5m,ValorHasta = 50}
                    }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 0, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 1, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 2, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 35, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 50, new Guid()), Is.EqualTo(0));

            Assert.That(calculadora.CalcularDescuentoEnKg(caracteristica, 0, 30000, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnKg(caracteristica, 1, 30000, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnKg(caracteristica, 2, 30000, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnKg(caracteristica, 35, 30000, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnKg(caracteristica, 50, 30000, new Guid()), Is.EqualTo(0));
        }

        [Test]
        public void MetodoTablaFueraDeEscalaMax()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 1,ValorHasta = 3},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 1.5m,ValorHasta = 50}
                        }
            };
            Assert.That(() => calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 51, new Guid()), Is.EqualTo(1.5m));
        }

        [Test]
        public void MetodoTablaFueraDeEscalaMin()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 10,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 15},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 1,ValorHasta = 20},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 1.5m,ValorHasta = 50}
                        }
            };
            Assert.That(() => calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 5, new Guid()), Throws.Exception);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void MetodoAcumuladoCaladoMinimoDistintoDeCero()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 5,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.Acumulado,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 5,ValorHasta = 10},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 12,ValorHasta = 21},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 20.5M, ValorHasta = 50}
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 5, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnKg(caracteristica, 5, 30000, new Guid()), Is.EqualTo(0));
        }

        [Test]
        public void MetodoAcumulado()
        {
            caracteristica = new CaracteristicaDeCalidad
                {
                    CaladoMaximo = 50,
                    CaladoMinimo = 0,
                    SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                    DescuentoEnPorcentaje = FormulaDescuento.Acumulado,
                    Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 1,ValorHasta = 3},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 1.5m,ValorHasta = 50}
                        }
                };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 0, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 0.5M, new Guid()), Is.EqualTo((0.5M - 0) * 0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 1, new Guid()), Is.EqualTo((1 - 0) * 0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 2, new Guid()), Is.EqualTo((2 - 1) * 1 + (1 - 0) * 0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 3, new Guid()), Is.EqualTo((3 - 1) * 1 + (1 - 0) * 0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 4, new Guid()), Is.EqualTo((4 - 3) * 1.5M + (3 - 1) * 1 + (1 - 0) * 0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 10, new Guid()), Is.EqualTo((10 - 3) * 1.5M + (3 - 1) * 1 + (1 - 0) * 0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 50, new Guid()), Is.EqualTo((50 - 3) * 1.5M + (3 - 1) * 1 + (1 - 0) * 0));
        }

        [Test]
        public void MetodoAcumulado4Descuentos()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.Acumulado,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 2,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 4,ValorHasta = 3},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 6,ValorHasta = 20},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 8,ValorHasta = 50}
                        }
            };

            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 0, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 0.5M, new Guid()), Is.EqualTo((0.5M - 0) * 2));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 1, new Guid()), Is.EqualTo((1 - 0) * 2));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 2, new Guid()), Is.EqualTo((2 - 1) * 4 + (1 - 0) * 2));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 3, new Guid()), Is.EqualTo((3 - 1) * 4 + (1 - 0) * 2));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 4, new Guid()), Is.EqualTo((4 - 3) * 6 + (3 - 1) * 4 + (1 - 0) * 2));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 10, new Guid()), Is.EqualTo((10 - 3) * 6 + (3 - 1) * 4 + (1 - 0) * 2));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 20, new Guid()), Is.EqualTo((20 - 3) * 6 + (3 - 1) * 4 + (1 - 0) * 2));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 30, new Guid()), Is.EqualTo((30 - 20) * 8 + (20 - 3) * 6 + (3 - 1) * 4 + (1 - 0) * 2));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 50, new Guid()), Is.EqualTo((50 - 20) * 8 + (20 - 3) * 6 + (3 - 1) * 4 + (1 - 0) * 2));
        }

        [Test]
        public void MetodoAcumulado1DescuentoPorDebajoDelValorMedido()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.Acumulado,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 1.5m,ValorHasta = 20},
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 30, new Guid()), Is.EqualTo((20 - 0) * 1.5M));
        }

        [Test]
        public void MetodoAcumuladoSinDescuentos()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.Acumulado,
                Descuentos = new List<Descuento>()
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 30, new Guid()), Is.EqualTo(0));
        }

        [Test]
        public void MetodoAcumuladoNn()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 400,
                CaladoMinimo = 400,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.Acumulado,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 25,ValorHasta = 400},
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 400, new Guid()), Is.EqualTo(0));
        }

        [Test]
        public void MetodoAcumuladoPruebaSCAT4858()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 15,
                CaladoMinimo = 0,
                DescuentoEnPorcentaje = FormulaDescuento.Acumulado,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 5,ValorHasta = 3},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 10,ValorHasta = 6},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 15,ValorHasta = 9},
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 5, new Guid()), Is.EqualTo(30));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void MetodoPuntoYFraccionCaladoMinimoDistintoDeCero()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 5,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.PuntoYFraccion,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 5,ValorHasta = 10},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 12,ValorHasta = 21},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 20.5M, ValorHasta = 50}
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 5, new Guid()), Is.EqualTo(0));
        }

        [Test]
        public void MetodoPuntoYFraccion()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.PuntoYFraccion,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 1,ValorHasta = 3},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 1.5m,ValorHasta = 50}
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 0, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 0.5M, new Guid()), Is.EqualTo((0.5M - 0) * 0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 1, new Guid()), Is.EqualTo((1 - 0) * 0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 2, new Guid()), Is.EqualTo((2 - 1) * 1));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 3, new Guid()), Is.EqualTo((3 - 1) * 1));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 4, new Guid()), Is.EqualTo((4 - 3) * 1.5M));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 10, new Guid()), Is.EqualTo((10 - 3) * 1.5M));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 50, new Guid()), Is.EqualTo((50 - 3) * 1.5M));
        }

        [Test]
        public void MetodoPuntoYFraccion4Descuentos()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.PuntoYFraccion,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 2,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 4,ValorHasta = 3},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 6,ValorHasta = 20},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 8,ValorHasta = 50}
                        }
            };

            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 0, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 0.5M, new Guid()), Is.EqualTo((0.5M - 0) * 2));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 1, new Guid()), Is.EqualTo((1 - 0) * 2));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 2, new Guid()), Is.EqualTo((2 - 1) * 4));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 3, new Guid()), Is.EqualTo((3 - 1) * 4));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 4, new Guid()), Is.EqualTo((4 - 3) * 6));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 10, new Guid()), Is.EqualTo((10 - 3) * 6));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 20, new Guid()), Is.EqualTo((20 - 3) * 6));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 30, new Guid()), Is.EqualTo((30 - 20) * 8));
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 50, new Guid()), Is.EqualTo((50 - 20) * 8));
        }

        [Test]
        public void MetodoPuntoYFraccion1DescuentoPorDebajoDelValorMedido()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.PuntoYFraccion,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 1.5m,ValorHasta = 20},
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 30, new Guid()), Is.EqualTo(0));
        }

        [Test]
        public void MetodoPuntoYFraccionSinDescuentos()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.PuntoYFraccion,
                Descuentos = new List<Descuento>()
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 30, new Guid()), Is.EqualTo(0));
        }

        [Test]
        public void MetodoPuntoYFraccionNn()
        {
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 400,
                CaladoMinimo = 400,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.Acumulado,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 25,ValorHasta = 400},
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 400, new Guid()), Is.EqualTo(0));
        }

        [Test]
        public void TieneExcepcionDescuento()
        {
            repositorioMock.Setup(x => x.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(1);
            repositorioMock.Setup(x => x.Existe<ExcepcionAlDescuento>(It.IsAny<Expression<Func<ExcepcionAlDescuento, bool>>>())).Returns(true);
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 5,ValorHasta = 10},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 12,ValorHasta = 21},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 20.5M, ValorHasta = 50}
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 15, new Guid()), Is.EqualTo(0));
            Assert.That(calculadora.CalcularDescuentoEnKg(caracteristica, 15, 30000, new Guid()), Is.EqualTo(0));
        }

        [Test]
        public void NoTieneExcepcionDescuento()
        {
            repositorioMock.Setup(x => x.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(1);
            repositorioMock.Setup(x => x.Existe<ExcepcionAlDescuento>(It.IsAny<Expression<Func<ExcepcionAlDescuento, bool>>>())).Returns(false);
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 50,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 5,ValorHasta = 10},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 12,ValorHasta = 21},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 20.5M, ValorHasta = 50}
                        }
            };
            Assert.That(calculadora.CalcularDescuentoEnPorcentaje(caracteristica, 15, new Guid()), Is.EqualTo(12));
            Assert.That(calculadora.CalcularDescuentoEnKg(caracteristica, 15, 30000, new Guid()), Is.EqualTo(3600));
        }

        [Test]
        public void EnviaACamaraSiembre()
        {
            repositorioMock.Setup(x => x.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(1);
            repositorioMock.Setup(x => x.Existe<ExcepcionAlDescuento>(It.IsAny<Expression<Func<ExcepcionAlDescuento, bool>>>())).Returns(false);
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 3,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.Siempre,
                SiSuperaValorCamara = 3,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 2},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0, ValorHasta = 3}
                        }
            };

            Assert.That(calculadora.EnviaACamara(new CaladoPorCaracteristica { CaracteristicaDeCalidad = caracteristica, ValorCalado = 1 }, new Guid()), Is.EqualTo(true));
        }


        [Test]
        public void EnviaACamaraNo1()
        {
            repositorioMock.Setup(x => x.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(1);
            repositorioMock.Setup(x => x.Existe<ExcepcionAlDescuento>(It.IsAny<Expression<Func<ExcepcionAlDescuento, bool>>>())).Returns(false);
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 3,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.SiempreSiSuperaValorCamara,
                SiSuperaValorCamara = 3,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 2},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0, ValorHasta = 3}
                        }
            };

            Assert.That(calculadora.EnviaACamara(new CaladoPorCaracteristica { CaracteristicaDeCalidad = caracteristica , ValorCalado = 1 }, new Guid()), Is.EqualTo(false));
        }

        [Test]
        public void EnviaACamaraNo2()
        {
            repositorioMock.Setup(x => x.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(1);
            repositorioMock.Setup(x => x.Existe<ExcepcionAlDescuento>(It.IsAny<Expression<Func<ExcepcionAlDescuento, bool>>>())).Returns(false);
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 3,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.SiempreSiSuperaValorCamara,
                SiSuperaValorCamara = 3,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 2},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0, ValorHasta = 3}
                        }
            };

            Assert.That(calculadora.EnviaACamara(new CaladoPorCaracteristica { CaracteristicaDeCalidad = caracteristica, ValorCalado = 2 }, new Guid()), Is.EqualTo(false));
        }

        [Test]
        public void EnviaACamaraSi3()
        {
            repositorioMock.Setup(x => x.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(1);
            repositorioMock.Setup(x => x.Existe<ExcepcionAlDescuento>(It.IsAny<Expression<Func<ExcepcionAlDescuento, bool>>>())).Returns(false);
            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 3,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.SiempreSiSuperaValorCamara,
                SiSuperaValorCamara = 3,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 2},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0, ValorHasta = 3}
                        }
            };

            Assert.That(calculadora.EnviaACamara(new CaladoPorCaracteristica { CaracteristicaDeCalidad = caracteristica, ValorCalado = 3 }, new Guid()), Is.EqualTo(true));
        }

        [Test]
        public void EnviaACamaraConAgenteDecompraSi2()
        {
            repositorioMock.Setup(x => x.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(1);
            repositorioMock.Setup(x => x.Existe<ExcepcionAlDescuento>(It.IsAny<Expression<Func<ExcepcionAlDescuento, bool>>>())).Returns(false);
            repositorioMock.Setup(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 3,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.SiempreSiSuperaValorCamara,
                SiSuperaValorCamara = 3,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 2, MercadoATermino = true},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0, ValorHasta = 3}
                        }
            };

            Assert.That(calculadora.EnviaACamara(new CaladoPorCaracteristica { CaracteristicaDeCalidad = caracteristica, ValorCalado = 2 }, new Guid()), Is.EqualTo(true));
        }

        [Test]
        public void EnviaACamaraSinAgenteDecompraNo2()
        {
            repositorioMock.Setup(x => x.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(1);
            repositorioMock.Setup(x => x.Existe<ExcepcionAlDescuento>(It.IsAny<Expression<Func<ExcepcionAlDescuento, bool>>>())).Returns(false);
            repositorioMock.Setup(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(false);

            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 3,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.SiempreSiSuperaValorCamara,
                SiSuperaValorCamara = 3,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 2, MercadoATermino = true},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0, ValorHasta = 3}
                        }
            };

            Assert.That(calculadora.EnviaACamara(new CaladoPorCaracteristica { CaracteristicaDeCalidad = caracteristica, ValorCalado = 2 }, new Guid()), Is.EqualTo(false));
        }

        [Test]
        public void EnviaACamaraConAgenteDecompraNo1()
        {
            repositorioMock.Setup(x => x.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(1);
            repositorioMock.Setup(x => x.Existe<ExcepcionAlDescuento>(It.IsAny<Expression<Func<ExcepcionAlDescuento, bool>>>())).Returns(false);
            repositorioMock.Setup(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            caracteristica = new CaracteristicaDeCalidad
            {
                CaladoMaximo = 3,
                CaladoMinimo = 0,
                SituacionEnvioACamara = EnvioACamara.SiempreSiSuperaValorCamara,
                SiSuperaValorCamara = 3,
                DescuentoEnPorcentaje = FormulaDescuento.DeTabla,
                Descuentos = new List<Descuento>
                        {
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 1},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0,ValorHasta = 2, MercadoATermino = true},
                            new Descuento{ CaracteristicaDeCalidad = caracteristica,PorcentajeDescuento = 0, ValorHasta = 3}
                        }
            };

            Assert.That(calculadora.EnviaACamara(new CaladoPorCaracteristica { CaracteristicaDeCalidad = caracteristica, ValorCalado = 1 }, new Guid()), Is.EqualTo(false));
        }
    }
}
