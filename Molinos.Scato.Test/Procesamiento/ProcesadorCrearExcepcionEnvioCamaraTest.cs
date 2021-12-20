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
    public class ProcesadorCrearExcepcionEnvioCamaraTest
    {
        private ProcesadorCrearExcepcionEnvioCamara target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ExcepcionEnvioCamaraDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearExcepcionEnvioCamara(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ExcepcionEnvioCamaraDto
            {
                Id = 1,
                CaracteristicaId = 1,
                EntregadorId = 1,
                MaterialId = 1,
                ProveedorId = 1,
                TipoComercialId = 1
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var tiposExistentesT = new List<Proveedor>
                {
                    new Proveedor
                        {
                            Id = 1,
                            Descripcion = "Proveedor 1"
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                           .Returns<Expression<Func<Proveedor, bool>>>(q => tiposExistentesT.Any((q.Compile())));

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Entregador, bool>>>()))
               .Returns<Expression<Func<Entregador, bool>>>(q => true);

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CaracteristicaDeCalidad, bool>>>()))
               .Returns<Expression<Func<CaracteristicaDeCalidad, bool>>>(q => true);

            var comando = new CrearExcepcionEnvioCamara {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<ExcepcionAlControl>(x => x.Id == 0)), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
