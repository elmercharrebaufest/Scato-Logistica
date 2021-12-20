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
    public class ProcesadorCrearTransportistaOrdenCargaInternaFasonTest
    {
        private ProcesadorCrearTransportistaOrdenCargaInternaFason target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private TransportistaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearTransportistaOrdenCargaInternaFason(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new TransportistaDto
            {
                Id = 1,
                Cuit = "20-3485016-8",
                RazonSocial = "a"
            };

        }

        [Test]
        public void TestCrearEntidad()
        {
            var tiposExistentes2 = new List<OrdenCargaInternaFason>
                {
                    new OrdenCargaInternaFason
                        {
                            Id = 1,
                        },
                };
            var carta = new OrdenCargaInternaFason() { Id = 1 };
            repositorioMock.Setup(s => s.Obtener<OrdenCargaInternaFason>(It.IsAny<int>()))
                           .Returns(carta);
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
               .Returns(new Recorrido());
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<OrdenCargaInternaFason, bool>>>()))
                           .Returns<Expression<Func<OrdenCargaInternaFason, bool>>>(q => tiposExistentes2.Any((q.Compile())));

            var comando = new CrearTransportistaOrdenCargaInternaFason { Dto = tipoDto, OrdenCargaInternaFasonId = 1 };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}