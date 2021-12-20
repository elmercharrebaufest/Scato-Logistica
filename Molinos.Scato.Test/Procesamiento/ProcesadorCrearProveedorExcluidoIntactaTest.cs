using System;
using System.Collections.Generic;
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
    public class ProcesadorCrearProveedorExcluidoIntactaTest
    {
        private ProcesadorCrearProveedorExcluidoIntacta target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private ProveedorExcluidoIntactaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearProveedorExcluidoIntacta(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new ProveedorExcluidoIntactaDto
            {
                Id = 1,
                ProveedorId = 6,
                RazonSocial = "Molinos Rio de la Plata"
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { Id = 6 });
            var comando = new CrearProveedorExcluidoIntacta { ProveedorId = tipoDto.ProveedorId };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorId()
        {
            var tiposExistentes = new List<ProveedorExcluidoIntacta>
                {
                    new ProveedorExcluidoIntacta() {Id = 5, Proveedor = new Proveedor() {Id = 6}},
                    new ProveedorExcluidoIntacta() {Id = 6, Proveedor = new Proveedor() {Id = 7}}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<ProveedorExcluidoIntacta, bool>>>()))
                    .Returns<Expression<Func<ProveedorExcluidoIntacta, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.ProveedorId = 6;
            var comando = new CrearProveedorExcluidoIntacta() { ProveedorId = tipoDto.ProveedorId };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<ProveedorExcluidoIntacta>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Proveedor"));
        }
    }
}
