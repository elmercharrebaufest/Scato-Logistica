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
    public class ProcesadorModificarMaterialPorWorkflowTest
    {
        private ProcesadorModificarExcepcionEnvioCamara target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private ExcepcionEnvioCamaraDto tipoDto;
        private ExcepcionEnvioCamara tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarExcepcionEnvioCamara(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new ExcepcionEnvioCamaraDto
            {
                Id = 1,
                CaracteristicaId = 1,
                ProveedorId = 1,
                EntregadorId = 1,
                MaterialId = 1,
                TipoComercialId = 1
            };
            tipo = new ExcepcionEnvioCamara
            {
                Id = 1,
                Proveedor = new Proveedor { Id = 1, Descripcion = "Proveedor1" },
                Entregador = new Entregador { Id = 1, RazonSocial = "Proveedor1" },
                Material = new Material { Id = 1, Descripcion = "Material1" },
                CaracteristicaMaterial =
                    new CaracteristicaDeCalidad() { Id = 1, CaracteristicaDeCalidadMaestro = new CaracteristicaDeCalidadMaestro { Descripcion = "Carac" }, MaterialPorCentro = new MaterialPorCentro { Id = 1, Material = new Material { Id = 1, Descripcion = "Material1"}, Centro = new Centro { Id = 1 } } },
                TipoComercial = new TipoComercial { Id = 1, Descripcion = "T Comercial 1" },
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            var tiposExistentes = new List<Proveedor>
                {
                    new Proveedor() {Id = 1, Descripcion = "Proveedor 1"},
                };
            var entregadores = new List<Entregador>
                {
                    new Entregador() {Id = 1, RazonSocial = "Razon Social 1"},
                };
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                    .Returns<Expression<Func<Proveedor, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Entregador, bool>>>()))
                    .Returns<Expression<Func<Entregador, bool>>>(q => entregadores.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<ExcepcionEnvioCamara>(It.IsAny<int>())).Returns(tipo);

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CaracteristicaDeCalidad, bool>>>()))
               .Returns<Expression<Func<CaracteristicaDeCalidad, bool>>>(q => true);

            var comando = new ModificarExcepcionEnvioCamara { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
