using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarVinedoPropioTest
    {
        private ProcesadorModificarVinedoPropio target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private VinedoPropioDto tipoDto;
        private List<CuartelDto> cuarteles;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarVinedoPropio(repositorioMock.Object, conversorMock, new NullLogger());
            cuarteles = new List<CuartelDto>
                {
                    new CuartelDto
                        {
                           Id = 1,
                           Codigo = "5000",
                           Activo = true
                        },
                    new CuartelDto
                        {
                           Id = 2,
                           Codigo = "5001",
                           Activo = false
                        }
                };
            tipoDto = new VinedoPropioDto
            {
                Id = 1,
                NumeroINV = "1000",
                Cuarteles = cuarteles,
                Descripcion = "Viñedo1",
                CuartelesJson = "[{\"Id\":1,\"Codigo\":\"5000\",\"Activo\":true},{\"Id\":2,\"Codigo\":\"5001\",\"Activo\":false}]",
                IngresosBrutos = "7000"
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(
                x => x.Existe<VinedoPropio>(It.IsAny<Expression<Func<VinedoPropio, bool>>>())).Returns(false);
            repositorioMock.Setup(
                x => x.Existe<CuartelDto>(It.IsAny<Expression<Func<CuartelDto, bool>>>())).Returns(true);

            repositorioMock.Setup(s => s.Obtener<VinedoPropio>(It.IsAny<int>())).Returns(conversorMock.Convertir<VinedoPropioDto, VinedoPropio>(tipoDto));
            repositorioMock.Setup(s => s.Obtener<Cuartel>(It.IsAny<int>())).Returns(new Cuartel());
            var comando = new ModificarVinedoPropio { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadValidoPorId()
        {
            var tiposExistentes = new List<VinedoPropio>
                {
                    new VinedoPropio
                        {
                            Id = 1,
                            Descripcion = "Descripcion1",
                            IngresosBrutos = "7000",
                            NumeroINV = "4500"
                        },
                };
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<VinedoPropio, bool>>>()))
                           .Returns<Expression<Func<VinedoPropio, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<VinedoPropio, bool>>>()))
                           .Returns<Expression<Func<VinedoPropio, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<VinedoPropio>(It.IsAny<int>())).Returns(conversorMock.Convertir<VinedoPropioDto, VinedoPropio>(tipoDto));
            repositorioMock.Setup(s => s.Obtener<Cuartel>(It.IsAny<int>())).Returns(new Cuartel());

            var comando = new ModificarVinedoPropio { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}