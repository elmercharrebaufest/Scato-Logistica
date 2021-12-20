using System;
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
    public class ProcesadorModificarCaladoTest
    {
        private ProcesadorModificarCalado target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private CaladoPorCaracteristicaDto[] tiposDto;
        private CaladoDto dto;
        private Mock<ICalculadoraDescuento> calculadora;
        private CaracteristicaDeCalidad tipo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            calculadora =  new Mock<ICalculadoraDescuento>();
            target = new ProcesadorModificarCalado(repositorioMock.Object, conversor, new NullLogger(), calculadora.Object);
            tiposDto = new[]
            {
                new CaladoPorCaracteristicaDto
                    {
                        Id = 1, Caracteristica = "caracteristica 1",  ValorCalado = 20
                    }, 
                new CaladoPorCaracteristicaDto
                    {
                        Caracteristica = "caracteristica 2",  ValorCalado = 20
                    }

            };
            dto = new CaladoDto { Id = 1, CicloDeCalado = 1, MuestraConjunto = 10, WorkflowInstanceId = new Guid() };

                tipo = new CaracteristicaDeCalidad
                {
                    Id = 1,
                    Analisis = TipoAnalisis.Calado,
                    MaterialPorCentro = new MaterialPorCentro { Id = 1, Material = new Material { Id = 1 }, Centro = new Centro { Id = 1 } }
                };
        }


    }
}