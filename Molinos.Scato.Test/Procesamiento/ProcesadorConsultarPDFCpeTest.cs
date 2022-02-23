using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorConsultarPDFCpeTest
    {
        private ProcesadorConsultarPDFCpe target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            log = new NullLogger();
            target = new ProcesadorConsultarPDFCpe(repositorioMock.Object, conversorMock.Object, log);
        }

        [Test]
        public void ObtenerPDF()
        {
            var cartaPorte = new CartaPorteElectronica
            {
                Id = 1,
                NroCTG = 20200008069,
                Pdf = new byte[1] 
            };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>())).Returns(new List<CartaPorteElectronica> { cartaPorte });
            var resultado = target.Ejecutar(new ConsultarPDFCpe { NroCtg = 20200008069 });
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void ObtenerPDFError()
        {
            var cartaPorte = new CartaPorteElectronica
            {
                Id = 1,
                NroCTG = 20200008069
            };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>())).Returns(new List<CartaPorteElectronica> { cartaPorte });
            var resultado = target.Ejecutar(new ConsultarPDFCpe { NroCtg = 20200008069 });
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}
