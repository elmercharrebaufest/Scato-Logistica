using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ProcesadorActualizarLecturaDeTarjetaTest
    {
        private ProcesadorActualizarLecturaDeTarjeta target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;
        private Recorrido recorrido;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            log = new NullLogger();
            target = new ProcesadorActualizarLecturaDeTarjeta(repositorioMock.Object, conversorMock.Object, log);

            recorrido = new Recorrido
            {
                Id = 1,
                Almacen = new Almacen { Id = 1, Centro = new Centro { Id = 1 }, Descripcion = "Almacen 1" },
                Calle = new Calle { Id = 1, CentroId = 1},
                PuestosDeCargaDescargas = new Collection<PuestosDeCargaDescarga> { new PuestosDeCargaDescarga { Id = 1, Centro = new Centro { Id = 1 } }},
                BalanzaBruto = new Balanza { Id = 1, Centro = new Centro { Id = 1 } },
                BalanzaTara = new Balanza { Id = 1, Centro = new Centro { Id = 1 }},
                Centro = new Centro{Id = 1},
                Chofer = new Chofer { Id = 1, Nombre = "Chofer" },
                DatosProximaActividad = "Tara",
                InstanciaWorkflow = new Guid("25892e17-80f6-415f-9c65-7395632f0223"),
                Material = new Material { Id = 1, Descripcion = "MaterialDesc 1" },
                NumeroDocumentoIngreso = "1111",
                Patente = "AAA111",
                TipoComercial = new TipoComercial { Id = 1, Descripcion = "Tipo 1" },
                TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                Workflow = new Workflow { Id = 1, Codigo = "a",Descripcion = "EgresoMaterialNoProductivo" },
                WorkflowDefinicion = new WorkflowDefinicion(){ Id = 1}
            };


        }

        [Test]
        public void TestCodigoDeDespositivoDesconocido()
        {
            repositorioMock.Setup(s => s.Listar<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new List<PuestoDeTrabajo>());

            var resultado = target.Ejecutar(new ActualizarLecturaDeTarjeta { Dto = new LecturaDeTarjetaDto() }) as ResultadoActualizarLecturaDeTarjeta;

            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.LecturaPuestosDeTrabajo.Any(), Is.EqualTo(false));
        }

        [Test]
        public void TestSinLectura()
        {
            Assert.Inconclusive("TODO: revisar test");
            //repositorioMock.Setup(s => s.Obtener<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new PuestoDeTrabajo { Centro = new Centro { Id = 1 }, Lecturas = new Collection<LecturaDeTarjeta> (), PidePatente = true});
            //repositorioMock.Setup(s => s.Obtener<LecturaDeTarjeta>(It.IsAny<Expression<Func<LecturaDeTarjeta, bool>>>())).Returns((LecturaDeTarjeta)null);
            //repositorioMock.Setup(s => s.Existe<TarjetaRango>(It.IsAny<Expression<Func<TarjetaRango, bool>>>())).Returns(false);

            //var resultado = target.Ejecutar(new ActualizarLecturaDeTarjeta { Dto = new LecturaDeTarjetaDto(){Lectura = "1111111111"} }) as ResultadoActualizarLecturaDeTarjeta;

            //repositorioMock.Verify(v => v.Agregar(It.IsAny<LecturaDeTarjeta>()), Times.Once());
            //repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            //Assert.That(resultado.LecturaPuestoDeTrabajo.PuestoDeTrabajoEncontrado, Is.EqualTo(true));
        }

        [Test]
        public void TestConLectura()
        {
            repositorioMock.Setup(s => s.Listar<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new List<PuestoDeTrabajo>() { new PuestoDeTrabajo { Centro = new Centro { Id = 1 }, VideoCamaras = new Collection<VideoCamara>(), Lecturas = new Collection<LecturaDeTarjeta> { new LecturaDeTarjeta() } } });
            repositorioMock.Setup(s => s.Obtener<LecturaDeTarjeta>(It.IsAny<Expression<Func<LecturaDeTarjeta, bool>>>())).Returns(new LecturaDeTarjeta());

            var resultado = target.Ejecutar(new ActualizarLecturaDeTarjeta { Dto = new LecturaDeTarjetaDto() { Lectura = "1111111111" } }) as ResultadoActualizarLecturaDeTarjeta;

            repositorioMock.Verify(v => v.Agregar(It.IsAny<LecturaDeTarjeta>()), Times.Never());
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.LecturaPuestosDeTrabajo.Any(), Is.EqualTo(true));
        }  
    }
}