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
    public class ProcesadorModificarVinedoTercerosTest
    {
        private ProcesadorModificarVinedoTerceros target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private VinedoTercerosDto tipoDto;
        private Proveedor proveedor;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarVinedoTerceros(repositorioMock.Object, conversorMock, new NullLogger());
            proveedor = new Proveedor
            {
                Id = 1,
                PR = true
            };

            tipoDto = new VinedoTercerosDto
            {
                Id = 10,
                NumeroINV = "1000",
                Descripcion = "Viñedo1",
                IngresosBrutos = "7000",
                ProveedorId = 1,
                esProveedorPR = proveedor.PR
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(
                x => x.Existe<VinedoTerceros>(It.IsAny<Expression<Func<VinedoTerceros, bool>>>())).Returns(false);

            repositorioMock.Setup(s => s.Obtener<VinedoTerceros>(It.IsAny<int>())).Returns(conversorMock.Convertir<VinedoTercerosDto, VinedoTerceros>(tipoDto));
            var comando = new ModificarVinedoTerceros { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadValidoPorId()
        {
            var tiposExistentes = new List<VinedoTerceros>
                {
                    new VinedoTerceros
                        {
                            Id = 10,
                            NumeroINV = "1000",
                            Descripcion = "Viñedo1",
                            Proveedor = proveedor,
                            IngresosBrutos = "7000",
                            
                            
                            
                        },
                };
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<VinedoTerceros, bool>>>()))
                           .Returns<Expression<Func<VinedoTerceros, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<VinedoTerceros, bool>>>()))
                           .Returns<Expression<Func<VinedoTerceros, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<VinedoTerceros>(It.IsAny<int>())).Returns(conversorMock.Convertir<VinedoTercerosDto, VinedoTerceros>(tipoDto));
            //repositorioMock.Setup(x => x.Existe<Variedad>(It.IsAny<Expression<Func<Variedad, bool>>>())).Returns(true);

            var comando = new ModificarVinedoTerceros { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}