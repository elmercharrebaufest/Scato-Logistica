using System;
using System.IO;
using System.Linq.Expressions;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
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
    public class ProcesadorAgregarMarcaSustentableTest
    {
        private ProcesadorAgregarMarcaSustentable target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();

            ConfigurarMockRepositorio();

            target = new ProcesadorAgregarMarcaSustentable(repositorioMock.Object, conversorMock.Object, new NullLogger());
        }

        private void ConfigurarMockRepositorio()
        {
            repositorioMock
                .Setup(r => r.Obtener<ConfiguracionGeneral>(It.IsAny<Expression<Func<ConfiguracionGeneral, bool>>>()))
                .Returns<Expression<Func<ConfiguracionGeneral, bool>>>(expresion =>
                {
                    var lambda = expresion.Compile();

                    var configPosX = new ConfiguracionGeneral
                    {
                        Pantalla = Constantes.ConfiguracionGeneral.Pantalla.MarcaSustentable,
                        Nombre = Constantes.ConfiguracionGeneral.MarcaSustentable.PosicionImagenSustentableX,
                        Valor = "0"
                    };

                    var configPosY = new ConfiguracionGeneral
                    {
                        Pantalla = Constantes.ConfiguracionGeneral.Pantalla.MarcaSustentable,
                        Nombre = Constantes.ConfiguracionGeneral.MarcaSustentable.PosicionImagenSustentableY,
                        Valor = "0"
                    };

                    if (lambda(configPosX)) return configPosX;
                    if (lambda(configPosY)) return configPosY;

                    return null;
                });
        }

        [Test]
        public void TestAgregarSello()
        {
            byte[] pdfImageBytes = File.ReadAllBytes(@"Images\cpe.png");

            var comando = new AgregarMarcaSustentable
            {
                RutaFotoCP = null,
                CodigoCentroSap = null,
                NroDocumento = null,
                SoloDibujar = true,
                PdfImage = pdfImageBytes
            };
            var resultado = target.Ejecutar(comando) as ResultadoCartaPorteElectronica;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.AreNotEqual(Convert.ToBase64String(resultado.PdfImageSustentable), Convert.ToBase64String(pdfImageBytes));
        }
    }
}
