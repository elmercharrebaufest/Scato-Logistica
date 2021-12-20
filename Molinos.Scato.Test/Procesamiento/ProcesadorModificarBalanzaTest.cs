using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarBalanzaTest
    {
        private ProcesadorModificarBalanza target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private BalanzaDto tipoDto;
        private Balanza tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarBalanza(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new BalanzaDto
                {
                    Id = 1,
                    CentroId = 1,
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
                    ToleranciaxMil = 1
                };
            tipo = new Balanza
            {
                    Id = 1,
                    CentroEmisor = 2,
                    Centro = new Centro{ Id = 1},
                    CodigoCabezal = "2",
                    Color = "#FFFFFF",
                    EstaEnCero = false,
                    MaximoValorCereo = 2,
                    Modalidad = Modalidad.Manual,
                    Nombre = "PruebaMod",
                    TipoBalanza = TipoBalanza.Piso,
                    TipoAcceso = TipoAcceso.EntradaSalida,
                    ToleranciaIndianapolis = 2,
                    ToleranciaOrigen = 2,
                    ToleranciaRechazo = 2,
                    ToleranciaxMil = 2,
                    PuestoDeTrabajo = "PC1"
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Balanza>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarBalanza {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<BalanzaDto>(), It.IsAny<Balanza>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<Balanza>
                {
                    new Balanza {Id = 5, Nombre = "B1", Centro = new Centro {Id = 1} , PuestoDeTrabajo = "PC2"},
                    new Balanza {Id = 6, Nombre = "B2", Centro = new Centro {Id = 1} , PuestoDeTrabajo = "PC3"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Balanza, bool>>>()))
                    .Returns<Expression<Func<Balanza, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Nombre = "B1";
            tipoDto.CentroId = 1;
            var comando = new ModificarBalanza { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Nombre"));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorPuestoDeTrabajo()
        {
            var tiposExistentes = new List<Balanza>
                {
                    new Balanza {Id = 5, Nombre = "B1", Centro = new Centro {Id = 1} , PuestoDeTrabajo = "PC2"},
                    new Balanza {Id = 6, Nombre = "B2", Centro = new Centro {Id = 1} , PuestoDeTrabajo = "PC3"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Balanza, bool>>>()))
                    .Returns<Expression<Func<Balanza, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.PuestoDeTrabajo = "PC2";
            tipoDto.CentroId = 1;
            var comando = new ModificarBalanza { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("PuestoDeTrabajo"));
        }
    }
}
