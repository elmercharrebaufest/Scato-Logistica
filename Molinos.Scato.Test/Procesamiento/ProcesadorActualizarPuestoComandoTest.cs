using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ProcesadorActualizarPuestoComandoTest
    {
        private ProcesadorActualizarPuestoComando target;
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
            target = new ProcesadorActualizarPuestoComando(repositorioMock.Object, conversorMock.Object, log);

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
        public void TestModificar()
        {
            repositorioMock.Setup(s => s.Obtener<Almacen>(It.IsAny<int>())).Returns(new Almacen());
            repositorioMock.Setup(s => s.Obtener<Balanza>(It.IsAny<int>())).Returns(new Balanza());
            repositorioMock.Setup(s => s.Obtener<Calle>(It.IsAny<int>())).Returns(new Calle());
            repositorioMock.Setup(s => s.Obtener<PuestosDeCargaDescarga>(It.IsAny<int>())).Returns(new PuestosDeCargaDescarga());
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new List<Recorrido> { recorrido});
            repositorioMock.Setup(
                s => s.Listar(It.IsAny<Expression<Func<PuestosDeCargaDescarga, bool>>>())).Returns(new List<PuestosDeCargaDescarga> {new PuestosDeCargaDescarga{Id = 1}});
            var resultado = target.Ejecutar(new ActualizarPuestocomando { BalanzasObligatorias = false, Dto = new AsignacionDto { HidraulicasId = new int[] { 1 }, InstanceIds = "25892e17-80f6-415f-9c65-7395632f0223" } });
            
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.False);
        }    
    }
}