using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearDocumentoDeImpresionTest
    {
        private ProcesadorCrearDocumentoDeImpresion target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private DocumentoDeImpresionDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearDocumentoDeImpresion(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new DocumentoDeImpresionDto()
            {
                Id = 1,
                Codigo = "Codigo",
                Descripcion = "Descripcion",
                DescripcionCorta = "DescripcionCorta"

            };
        }


        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<DocumentoDeImpresion, bool>>>()))
                    .Returns(false);

            var comando = new CrearDocumentoDeImpresion { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<DocumentoDeImpresion>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
