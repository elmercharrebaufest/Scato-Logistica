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
    public class ProcesadorCrearExcepcionAlControlTest
    {
        private ProcesadorCrearExcepcionAlControl target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ExcepcionAlControlDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearExcepcionAlControl(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ExcepcionAlControlDto
            {
                Id = 1,
                TransportistaId = 1,
                MaterialId = 2,
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(),
                CentroId = 1
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var tiposExistentesT = new List<Transportista>
                {
                    new Transportista
                        {
                            Id = 1
                        },
                };
            var tiposExistentesM = new List<Material>
                {
                    new Material
                        {
                            Id = 2
                        },
                };
            var tiposExistentesC = new List<Centro>
                {
                    new Centro
                        {
                            Id = 1
                        },
                };
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Transportista, bool>>>()))
                           .Returns<Expression<Func<Transportista, bool>>>(q => tiposExistentesT.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                           .Returns<Expression<Func<Material, bool>>>(q => tiposExistentesM.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Centro, bool>>>()))
                           .Returns<Expression<Func<Centro, bool>>>(q => tiposExistentesC.Any((q.Compile())));

            var comando = new CrearExcepcionAlControl {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<ExcepcionAlControl>(x => x.Id == 0)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}