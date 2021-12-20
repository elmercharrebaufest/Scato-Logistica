using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarExcepcionAlDescuentoTest
    {
        private ProcesadorModificarExcepcionAlDescuento target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ExcepcionAlDescuentoDto tipoDto;
        private ExcepcionAlDescuento tipo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarExcepcionAlDescuento(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ExcepcionAlDescuentoDto
                {
                    Id = 0,
                    ProveedorId = 0,
                    ProveedorDescripcion = "Proveedor 1",
                    MaterialId = 0,
                    MaterialDescripcion = "Semilla Soja",
                    FechaDesde = new DateTime(),
                    FechaHasta = new DateTime(),
                    CentroId = 0,
                    CaracteristicaDeCalidadId = 1,
                    CaracteristicaDeCalidadDescripcion = "Humedad",
                    CamaraId = 1,
                    Motivo = "Motivo 1",
                    Usuario = "User 1"
                };
            tipo = new ExcepcionAlDescuento
                {
                    Id = 1,
                    Proveedor = new Proveedor(),
                    FechaDesde = new DateTime(),
                    FechaHasta = new DateTime(),
                    CaracteristicaDeCalidad = new CaracteristicaDeCalidad() {Id = 1, DescripcionCorta = "HUMEDAD"},
                    Camara = new Camara(),
                    Motivo = "Motivo 1",
                    Usuario = "User 1"
                };
        }

        [Test]
        public void TestModificarEntidad()
        {

            var tiposExistentes = new List<ExcepcionAlDescuento>
                {
                    new ExcepcionAlDescuento
                        {
                            Id = 1,
                            Proveedor = new Proveedor(),
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            CaracteristicaDeCalidad = new CaracteristicaDeCalidad(),
                            Camara = new Camara(),
                            Motivo = "Motivo 1",
                            Usuario = "User 1"
                        },
                };
            var tiposExistentesP = new List<Proveedor>
                {
                    new Proveedor
                        {
                            Id = 0
                        },
                };
            var tiposExistentesM = new List<MaterialPorCentro>
                {
                    new MaterialPorCentro
                        {
                            Id = 0
                        },
                };
            var tiposExistentesC = new List<Centro>
                {
                    new Centro
                        {
                            Id = 0
                        },
                };
            var tiposExistentesCa = new List<Camara>
                {
                    new Camara
                        {
                            Id = 0
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                           .Returns<Expression<Func<Proveedor, bool>>>(q => tiposExistentesP.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>()))
                           .Returns<Expression<Func<MaterialPorCentro, bool>>>(q => tiposExistentesM.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Centro, bool>>>()))
                           .Returns<Expression<Func<Centro, bool>>>(q => tiposExistentesC.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<ExcepcionAlDescuento, bool>>>()))
                           .Returns<Expression<Func<ExcepcionAlDescuento, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Camara, bool>>>()))
                           .Returns<Expression<Func<Camara, bool>>>(q => tiposExistentesCa.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<ExcepcionAlDescuento>(It.IsAny<int>())).Returns(tipo);


            var comando = new ModificarExcepcionAlDescuento { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
