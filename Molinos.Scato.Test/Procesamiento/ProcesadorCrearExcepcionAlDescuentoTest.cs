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
    public class ProcesadorCrearExcepcionAlDescuentoTest
    {
        private ProcesadorCrearExcepcionAlDescuento target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ExcepcionAlDescuentoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearExcepcionAlDescuento(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ExcepcionAlDescuentoDto
            {
                Id = 1,
                ProveedorId = 1,
                ProveedorDescripcion = "Proveedor 1",
                MaterialId = 2,
                MaterialDescripcion = "Semilla Soja",
                FechaDesde = DateTime.Now,
                FechaHasta = DateTime.Now,
                CaracteristicaDeCalidadId = 40,
                CaracteristicaDeCalidadDescripcion = "Humedad",
                CentroId = 1,
                CamaraId = 3,
                Motivo = "Motivo 1",
                Usuario = "User 1"
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var tiposExistentesP = new List<Proveedor>
                {
                    new Proveedor
                        {
                            Id = 1,
                            Descripcion = "Proveedor 1"
                        },
                };
            var tiposExistentesM = new List<Material>
                {
                    new Material
                        {
                            Id = 2,
                            Descripcion = "Semilla Soja"
                        },
                };
            var tiposExistentesC = new List<Centro>
                {
                    new Centro
                        {
                            Id = 1
                        },
                };
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                           .Returns<Expression<Func<Proveedor, bool>>>(q => tiposExistentesP.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                           .Returns<Expression<Func<Material, bool>>>(q => tiposExistentesM.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Centro, bool>>>()))
                           .Returns<Expression<Func<Centro, bool>>>(q => tiposExistentesC.Any((q.Compile())));

            repositorioMock.Setup(s => s.Obtener<Proveedor>(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { Id = 3, Descripcion = "Proveedor 1" });

            var comando = new CrearExcepcionAlDescuento { Dto = tipoDto ,Usuario = "usuario 1"};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<ExcepcionAlDescuento>(x => x.Id == 0)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}