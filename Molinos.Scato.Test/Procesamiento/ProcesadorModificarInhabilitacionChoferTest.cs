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
    public class ProcesadorModificarInhabilitacionChoferTest
    {
        private ProcesadorModificarInhabilitacionChofer target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private InhabilitacionChoferDto dto;
        private Chofer chofer;
        private ModificarInhabilitacionChofer comando;
        private InhabilitacionChofer inhabilitacion;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarInhabilitacionChofer(repositorioMock.Object, conversorMock.Object, new NullLogger());
            inhabilitacion = new InhabilitacionChofer() { Centro = new Centro() { Id = 5 }, Chofer = chofer, Id = 1, Motivo = "Motivo", NombreUsuarioResponsable = "User1", Adjuntos = new List<Adjunto>() };

            dto = new InhabilitacionChoferDto()
            {
                Apellido = "Apellido1",
                Nombre = "Nombre1",
                CentroId = 5,
                DescripcionCorta = "San Lorenzo1",
                Id = 1,
                Motivo = "Motivo1",
                NumeroDeDocumento = "2040120",
                NombreUsuarioResponsable = "User1",
                TipoDocumentoIdentidadId = 1,
                Adjuntos = new List<AdjuntoDto>()
            };

            comando = new ModificarInhabilitacionChofer() { Dto = dto, Usuario = "User1" };
            chofer = new Chofer()
            {
                Apellido = "Apellido",
                Nombre = "Nombre",
                Cuil = "Cuil",
                Id = 1,
                NumeroDeDocumento = "2040120",
                TipoDocumentoIdentidad = new TipoDocumentoIdentidad()
                {
                    Descripcion = "Dni",
                    DescripcionCorta = "Dni",
                    Id = 1
                }
            };
        }
        
        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Chofer, bool>>>())).Returns(chofer);
            repositorioMock.Setup(x => x.Obtener<InhabilitacionChofer>(It.IsAny<int>())).Returns(inhabilitacion);
            repositorioMock.Setup(x => x.Existe(It.IsAny<Expression<Func<Chofer, bool>>>())).Returns(true);
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Adjunto, bool>>>())).Returns(new List<Adjunto>());

            var result = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.HayErrores, Is.False);
        }

        [Test]
        public void TestModificarEntidadInvalido()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Chofer, bool>>>())).Returns(chofer);
            repositorioMock.Setup(x => x.Obtener<InhabilitacionChofer>(It.IsAny<int>())).Returns(inhabilitacion);
            repositorioMock.Setup(x => x.Existe(It.IsAny<Expression<Func<Chofer, bool>>>())).Returns(false);
            var result = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.HayErrores, Is.True);
            Assert.That(result.Errores.Keys.Contains("NumeroDeDocumento"), Is.True);
        }

        [Test]
        public void TestModificarEntidadCambiandoTipoDocumento()
        {
            var dto2 = new InhabilitacionChoferDto()
            {
                Apellido = "Apellido1",
                Nombre = "Nombre1",
                CentroId = 5,
                DescripcionCorta = "San Lorenzo1",
                Id = 1,
                Motivo = "Motivo1",
                NumeroDeDocumento = "2040120",
                NombreUsuarioResponsable = "User1",
                TipoDocumentoIdentidadId = 2,
        };
            dto2.Adjuntos = new List<AdjuntoDto>();


            var tipoDocumento = new TipoDocumentoIdentidad()
            {
                Descripcion = "Dni",
                DescripcionCorta = "Dni",
                Id = 2
            };
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Adjunto, bool>>>())).Returns(new List<Adjunto>());

            repositorioMock.Setup(x => x.Obtener<TipoDocumentoIdentidad>(It.IsAny<int>())).Returns(tipoDocumento);
            comando = new ModificarInhabilitacionChofer() { Dto = dto2, Usuario = "User1" };
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Chofer, bool>>>())).Returns(chofer);
            repositorioMock.Setup(x => x.Obtener<InhabilitacionChofer>(It.IsAny<int>())).Returns(inhabilitacion);
            repositorioMock.Setup(x => x.Existe(It.IsAny<Expression<Func<Chofer, bool>>>())).Returns(true);
            var result = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.HayErrores, Is.False);
        }
    }
}
