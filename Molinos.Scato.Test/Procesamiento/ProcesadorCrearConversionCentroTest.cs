using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using NUnit.Framework;
using Moq;
namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearConversionCentroTest
    {
        private ProcesadorCrearConversionCentro target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ConversionCentroDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearConversionCentro(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ConversionCentroDto
            {
                Id = 1,
                CentroId = 1,
                CamaraId = 1,
                CodigoCamara = "1"
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var tiposExistentesT = new List<Camara>
                {
                    new Camara
                        {
                            Id = 1,
                            Descripcion = "Camara 1"
                        },
                };
            var tiposExistentesM = new List<Centro>
                {
                    new Centro()
                        {
                            Id = 1,
                            Descripcion = "Localidad 1"
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Camara, bool>>>()))
                           .Returns<Expression<Func<Camara, bool>>>(q => tiposExistentesT.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Centro, bool>>>()))
                           .Returns<Expression<Func<Centro, bool>>>(q => tiposExistentesM.Any((q.Compile())));

            var comando = new CrearConversionCentro() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
