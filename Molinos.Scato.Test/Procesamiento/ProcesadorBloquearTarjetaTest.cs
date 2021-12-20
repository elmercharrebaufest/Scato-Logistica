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
    public class ProcesadorBloquearTarjetaTest
    {
        private ProcesadorBloquearTarjeta target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TarjetaBloqueadaDto dto;
        private List<TarjetaBloqueada> tarjetasBloqueadas;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorBloquearTarjeta(repositorioMock.Object, conversor, new NullLogger());
            dto = new TarjetaBloqueadaDto
                {
                    Id = 1,
                    CentroId = 1,
                    Motivo = "Motivo1",
                    Numero = "33333"
                };

            tarjetasBloqueadas = new List<TarjetaBloqueada>
                {
                    new TarjetaBloqueada
                        {
                            Id = 1,
                            Numero = "11111",
                            Motivo = "Motivo1",
                            Centro = new Centro()
                        },
                    new TarjetaBloqueada
                        {
                            Id = 2,
                            Numero = "22222",
                            Motivo = "Motivo2",
                            Centro = new Centro()
                        },
                };
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(new Centro());
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TarjetaBloqueada, bool>>>()))
                           .Returns<Expression<Func<TarjetaBloqueada, bool>>>(q => tarjetasBloqueadas.Any((q.Compile())));

            var comando = new BloquearTarjeta {Dto = dto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TarjetaBloqueada>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

        [Test]
        public void TestNumeroExistente()
        {
            dto.Numero = "22222";
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TarjetaBloqueada, bool>>>()))
                           .Returns<Expression<Func<TarjetaBloqueada, bool>>>(q => tarjetasBloqueadas.Any((q.Compile())));
            var comando = new BloquearTarjeta {Dto = dto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TarjetaBloqueada>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

        [Test]
        public void TestCrearEntidadTarjetaNoExistenteONoVigente()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TarjetaBloqueada, bool>>>()))
                           .Returns<Expression<Func<TarjetaBloqueada, bool>>>(q => tarjetasBloqueadas.Any((q.Compile())));
            var resultadoFijo = new ResultadoCrear();
            resultadoFijo.Error("Numero", "Tarjeta inexistente o no vigente");
            var comando = new BloquearTarjeta {Dto = dto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TarjetaBloqueada>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado.Errores, (Is.EqualTo(resultadoFijo.Errores)));
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

    }
}

