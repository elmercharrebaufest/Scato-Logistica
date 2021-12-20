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
    public class ProcesadorModificarInhabilitacionCamionTest
    {
        private ProcesadorModificarInhabilitacionCamion target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private InhabilitacionCamionDto dto;
        private ModificarInhabilitacionCamion comando;
        private InhabilitacionCamion inhabilitacion;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarInhabilitacionCamion(repositorioMock.Object, conversorMock.Object, new NullLogger());
            inhabilitacion = new InhabilitacionCamion() { Centro = new Centro() { Id = 5 }, Id = 1, Motivo = "Motivo", NombreUsuarioResponsable = "User1", Adjuntos = new List<Adjunto>() };

            dto = new InhabilitacionCamionDto()
            {
                CentroId = 5,
                Id = 1,
                Motivo = "Motivo1",
                NombreUsuarioResponsable = "User1",
                Patente = "POE148",
                Adjuntos = new List<AdjuntoDto>()
            };

            comando = new ModificarInhabilitacionCamion() { Dto = dto, Usuario = "User1" };
        }
        
        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(x => x.Obtener<InhabilitacionCamion>(It.IsAny<int>())).Returns(inhabilitacion);
            repositorioMock.Setup(x => x.Existe(It.IsAny<Expression<Func<Vehiculo, bool>>>())).Returns(true);
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Adjunto, bool>>>())).Returns(new List<Adjunto>());

            var result = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.HayErrores, Is.False);
        }
    }
}
