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
    public class ProcesadorModificarExcepcionAlControlTest
    {
        private ProcesadorModificarExcepcionAlControl target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ExcepcionAlControlDto tipoDto;
        private ExcepcionAlControl tipo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarExcepcionAlControl(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ExcepcionAlControlDto
                {
                    Id = 0,
                    TransportistaId = 0,
                    MaterialId = 0,
                    FechaDesde = new DateTime(),
                    FechaHasta = new DateTime(),
                    CentroId = 0
                };
            tipo = new ExcepcionAlControl
                {
                    Id = 1,
                    Transportista = new Transportista(),
                    Material = new Material(),
                    FechaDesde = new DateTime(),
                    FechaHasta = new DateTime(),
                    Centro = new Centro()
                };
        }

        [Test]
        public void TestModificarEntidad()
        {

            var tiposExistentes = new List<ExcepcionAlControl>
                {
                    new ExcepcionAlControl
                        {
                            Id = 1,
                            Transportista = new Transportista(),
                            Material = new Material(),
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            Centro = new Centro(),
                            TipoDestino = TipoDestino.Centro,
                            CentroDestino = new Centro()
                        },
                };
            var tiposExistentesT = new List<Transportista>
                {
                    new Transportista
                        {
                            Id = 0
                        },
                };
            var tiposExistentesM = new List<Material>
                {
                    new Material
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
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Transportista, bool>>>()))
                           .Returns<Expression<Func<Transportista, bool>>>(q => tiposExistentesT.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                           .Returns<Expression<Func<Material, bool>>>(q => tiposExistentesM.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Centro, bool>>>()))
                           .Returns<Expression<Func<Centro, bool>>>(q => tiposExistentesC.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<ExcepcionAlControl, bool>>>()))
                    .Returns<Expression<Func<ExcepcionAlControl, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<ExcepcionAlControl>(It.IsAny<int>())).Returns(tipo);
            

            var comando = new ModificarExcepcionAlControl {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
