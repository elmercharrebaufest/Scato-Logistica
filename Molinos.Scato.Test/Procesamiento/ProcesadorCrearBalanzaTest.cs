using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearBalanzaTest
    {
        private ProcesadorCrearBalanza target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private BalanzaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearBalanza(repositorioMock.Object, conversorMock, new NullLogger());
            tipoDto = new BalanzaDto
                {
                    Id = 1,
                    CentroEmisor = 1,
                    CodigoCabezal = "1",
                    Color = "#FFFFFF",
                    EstaEnCero = false,
                    MaximoValorCereo = 1,
                    Modalidad = Modalidad.Automática,
                    Nombre = "Prueba",
                    TipoBalanza = TipoBalanza.Aérea,
                    TipoAcceso = TipoAcceso.Entrada,
                    ToleranciaIndianapolis = 1,
                    ToleranciaOrigen = 1,
                    ToleranciaRechazo = 1,
                    ToleranciaxMil = 1,
                    PuestoDeTrabajo = "PC3"
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearBalanza {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Balanza>(o => o.Nombre == tipoDto.Nombre)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<Balanza>
                {
                    new Balanza {Id = 5, Nombre = "B1", Centro = new Centro {Id = 1}, PuestoDeTrabajo = "PC1"},
                    new Balanza {Id = 6, Nombre = "B2", Centro = new Centro {Id = 1}, PuestoDeTrabajo = "PC2"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Balanza, bool>>>()))
                    .Returns<Expression<Func<Balanza, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Nombre = "B1";
            tipoDto.CentroId = 1;
            var comando = new CrearBalanza { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Balanza>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Nombre"));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorPuestoDeTrabajo()
        {
            var tiposExistentes = new List<Balanza>
                {
                    new Balanza {Id = 5, Nombre = "B1", Centro = new Centro {Id = 1}, PuestoDeTrabajo = "PC1"},
                    new Balanza {Id = 6, Nombre = "B2", Centro = new Centro {Id = 1}, PuestoDeTrabajo = "PC2"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Balanza, bool>>>()))
                    .Returns<Expression<Func<Balanza, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.PuestoDeTrabajo = "PC1";
            tipoDto.CentroId = 1;
            var comando = new CrearBalanza { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Balanza>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("PuestoDeTrabajo"));
        }
    }
}
