using System;
using System.Collections.Generic;
using System.IO;
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
    public class ProcesadorCrearOrdenDeDescargaTest
    {
        private ProcesadorCrearOrdenDeDescarga target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private OrdenDeDescargaDto dto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearOrdenDeDescarga(repositorioMock.Object, conversor, new NullLogger());
            dto = new OrdenDeDescargaDto
            {
                Numero = "77777777",
                FechaMovimiento = DateTime.Now.AddDays(2),
                TipoComercial = "Tipo Comercial",
                Proveedor = "Test",
                Transportista = "Test",
                Chofer = new ChoferDto { Id = 1, Nombre = "Bruce", Apellido = "Wayne", NumeroDeDocumento = "123" },
            };
        }

        [Test]
        public void TestNoEncontroWf()
        {
            var workflows = new List<Workflow> { new Workflow { Id = 1, Codigo = "C1", Descripcion = "D1" } };
            var comando = new CrearOrdenDeDescarga { Orden = dto, NombreWorkflow = "W1", InstanciaWorkflowId = new Guid("00000000-0000-0000-0000-000000000000") };

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                    .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Recorrido>()), Times.Never());
            repositorioMock.Verify(s => s.Agregar(It.IsAny<OrdenDeDescarga>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrear()
        {
            var workflows = new List<Workflow> { new Workflow { Id = 1, Codigo = "W1", Descripcion = "D1" } };
            var comando = new CrearOrdenDeDescarga { Orden = dto, NombreWorkflow = "W1", InstanciaWorkflowId = new Guid("00000000-0000-0000-0000-000000000000"), CentroId = 3 };

            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>())).Returns(new Chofer { Id = dto.Chofer.Id });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.Is<int>(i => i == comando.CentroId))).Returns(new Centro { Id = comando.CentroId });
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial { Id = dto.TipoComercialId });
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(new Transportista { Id = dto.TransportistaId });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                    .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());
            comando.Orden.TipoDeWorkflow = TipoDeWorkflow.Ingreso;
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Recorrido>(r => r.InstanciaWorkflow == comando.InstanciaWorkflowId && r.Workflow.Codigo == "W1" && r.Centro.Id == comando.CentroId && r.Chofer.Id == dto.Chofer.Id && r.Transportista.Id == dto.TransportistaId && r.TipoComercial.Id == dto.TipoComercialId)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestExcepción()
        {
            var comando = new CrearOrdenDeDescarga { Orden = dto, NombreWorkflow = "W1", InstanciaWorkflowId = new Guid("00000000-0000-0000-0000-000000000000") };

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Throws(new IOException("Error"));

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Recorrido>()), Times.Never());
            repositorioMock.Verify(s => s.Agregar(It.IsAny<OrdenDeDescarga>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.First().Value.Contains("error"), Is.EqualTo(true));
        }
    }
}
