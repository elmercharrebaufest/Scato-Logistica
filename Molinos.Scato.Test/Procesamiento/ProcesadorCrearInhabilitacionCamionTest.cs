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
    public class ProcesadorCrearInhabilitacionCamionTest
    {
        private ProcesadorCrearInhabilitacionCamion target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private InhabilitacionCamionDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearInhabilitacionCamion(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new InhabilitacionCamionDto
                {
                    Id = 1,
                    Patente = "AAA123",
                    FechaDesde = new DateTime(),
                    FechaHasta = new DateTime(),
                    Motivo = "??",
                    CentroId = 1,
                    Adjuntos = new List<AdjuntoDto>()
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var tiposExistentesCentro = new List<Centro>
                {
                    new Centro
                        {
                            Id = 1,
                        }
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Centro, bool>>>()))
                    .Returns<Expression<Func<Centro, bool>>>(q => tiposExistentesCentro.Any((q.Compile())));

            var comando = new CrearInhabilitacionCamion {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<InhabilitacionCamion>(o => o.Patente == tipoDto.Patente)),
                                   Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
