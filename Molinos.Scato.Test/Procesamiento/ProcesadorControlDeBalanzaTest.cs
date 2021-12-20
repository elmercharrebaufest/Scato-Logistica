using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProcesadorControlDeBalanzaTest
    {
        private ProcesadorControlDeBalanza target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private ControlDeBalanza controlDeBalanza;
        private ControlDeBalanzaDto controlDeBalanzaDto;
        private Recorrido recorrido;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorControlDeBalanza(repositorioMock.Object, conversor, new NullLogger());
            var centro = new Centro
                {
                    Id = 1,
                    Descripcion = "Centro1"
                };
            recorrido = new Recorrido
            {
                Id = 1,
                Almacen = new Almacen { Id = 1, Centro = centro, Descripcion = "Almacen 1" },
                Centro = centro,
                Chofer = new Chofer { Id = 1, Nombre = "Chofer" },
                DatosProximaActividad = "Tara",
                InstanciaWorkflow = new Guid(),
                Material = new Material { Id = 1, Descripcion = "MaterialDesc 1" },
                NumeroDocumentoIngreso = "1111",
                Patente = "AAA111",
                TipoComercial = new TipoComercial { Id = 1, Descripcion = "Tipo 1" },
                TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                Workflow = new Workflow { Id = 1, Descripcion = "EgresoMaterialNoProductivo" },
                ControlBalanza = true,
            };
            controlDeBalanza = new ControlDeBalanza
            {
                Recorrido = recorrido,
                TipoPesada = TipoPesada.Bruto,
                Observaciones = "Observaciones",
                ControlesDeBalanzasPesadas = new List<ControlDeBalanzaPesada>{
                    new ControlDeBalanzaPesada
                        {
                            Balanza = new Balanza{Id = 2, Nombre = "Balanza2",},
                            Fecha = new DateTime(2014,1,1),
                            Peso = 200
                        }}
            };

            controlDeBalanzaDto = new ControlDeBalanzaDto
            {
                Patente = "AAA111",
                TipoPesada = TipoPesada.Bruto,
                Material = "Mat1",
                NumeroDocumentoIngreso = "1111",
                RecorridoId = 1,
                TipoComercial = "T1",
                ControlesDeBalanzasPesadas = new List<ControlDeBalanzaPesadaDto>{new ControlDeBalanzaPesadaDto
                        {
                            BalanzaId = 1, BalanzaNombre = "B1", Peso = 100
                        }}
            };

        }
        [Test]
        public void TestAceptarConNuevaPesada()
        {
            repositorioMock.Setup(s => s.Obtener<ControlDeBalanza>(It.IsAny<int>())).Returns(controlDeBalanza);
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<int>())).Returns(recorrido);
            var resultado = target.Ejecutar(new ControlDeBalanzaComando{Dto = controlDeBalanzaDto});
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Recorrido>(It.IsAny<int>()), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
        [Test]
        public void TestAceptarConPesadaExistente()
        {
            repositorioMock.Setup(s => s.Obtener<ControlDeBalanza>(It.IsAny<int>())).Returns(controlDeBalanza);
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<int>())).Returns(recorrido);
            repositorioMock.Setup(s => s.Obtener<ControlDeBalanzaPesada>(It.IsAny<int>())).Returns(controlDeBalanza.ControlesDeBalanzasPesadas.First());
            var resultado = target.Ejecutar(new ControlDeBalanzaComando { Dto = controlDeBalanzaDto });
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Recorrido>(It.IsAny<int>()), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
        [Test]
        public void TestAceptarAgregarControl()
        {
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<int>())).Returns(recorrido);
            var resultado = target.Ejecutar(new ControlDeBalanzaComando { Dto = controlDeBalanzaDto });
            repositorioMock.Verify(s => s.Agregar(It.IsAny<ControlDeBalanza>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Recorrido>(It.IsAny<int>()), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
