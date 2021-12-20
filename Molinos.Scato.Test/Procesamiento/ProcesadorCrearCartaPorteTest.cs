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
    public class ProcesadorCrearCartaPorteTest
    {
        private ProcesadorCrearCartaPorte target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private CartaPorteDto dto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearCartaPorte(repositorioMock.Object, conversor, new NullLogger());
            dto = new CartaPorteDto
            {
                TipoVehiculo = TipoVehiculo.Camión,
                NroCartaPorte = "77777777",
                CTG = "77777777",
                FechaCP = DateTime.Now.AddDays(2),
                TipoComercial = "Tipo Comercial",
                CEE = "77777777",
                FechaEmision = DateTime.Now,
                FechaVto = DateTime.Now.AddDays(10),
                TitularCartaPorte = "Test",
                Destinatario = "Test",
                Transportista = "Test",
                Chofer = new ChoferDto { Id = 1, Nombre = "Bruce", Apellido = "Wayne", NumeroDeDocumento = "123"},
                Cosecha = "12-13",
                Procedencia = "Laurencena",
                OrigenVehiculo = OrigenVehiculo.Argentina,
                KmRecorrer = 798,
                TarifaTonelada = 12,
                FleteAPagar = true,
                Destino = "4",
                AgenteComprasId = 1,
                BocaDestinoId = 1,
                Vehiculos = new List<VehiculoDto>{ new VehiculoDto()}
            };
        }

        [Test]
        public void TestNoEncontroWf()
        {
            var workflows = new List<Workflow> {new Workflow {Id = 1, Codigo = "C1", Descripcion = "D1"}};
            var comando = new CrearCartaPorte { Orden = dto, NombreWorkflow = "W1", InstanciaWorkflowId = new Guid("00000000-0000-0000-0000-000000000000") };

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                    .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Recorrido>()), Times.Never());
            repositorioMock.Verify(s => s.Agregar(It.IsAny<CartaPorte>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrear()
        {
            var workflows = new List<Workflow> { new Workflow { Id = 1, Codigo = "W1", Descripcion = "D1" } };
            var comando = new CrearCartaPorte { Orden = dto, NombreWorkflow = "W1", InstanciaWorkflowId = new Guid("00000000-0000-0000-0000-000000000000"), CentroId = 3,Vehiculo = new VehiculoDto{Primero = true}};

            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>())).Returns(new Chofer {Id = dto.Chofer.Id});
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(new Material {Id = dto.MaterialId});
            repositorioMock.Setup(s => s.Obtener<Centro>(It.Is<int>(i => i == dto.DestinoId))).Returns(new Centro { Id = dto.DestinoId });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.Is<int>(i => i == comando.CentroId))).Returns(new Centro { Id = comando.CentroId });
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial {Id = dto.TipoComercialId});
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(new Transportista {Id = dto.TransportistaId ?? 0});
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                    .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());
            comando.Orden.TipoDeWorkflow = TipoDeWorkflow.Egreso;
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Recorrido>(r => r.InstanciaWorkflow == comando.InstanciaWorkflowId && r.Workflow.Codigo == "W1" && r.Centro.Id == comando.CentroId && r.Chofer.Id == dto.Chofer.Id && r.Transportista.Id == (dto.TransportistaId ?? 0) && r.TipoComercial.Id == dto.TipoComercialId)), Times.Exactly(1));
            repositorioMock.Verify(s => s.Agregar(It.Is<CartaPorte>(o => o.Chofer.Id == dto.Chofer.Id && o.CentroDestino.Id == dto.DestinoId)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearConRecorridoExistente()
        {
            var workflows = new List<Workflow> { new Workflow { Id = 1, Codigo = "W1", Descripcion = "D1" } };
            var comando = new CrearCartaPorte { Orden = dto, NombreWorkflow = "W1", InstanciaWorkflowId = new Guid("00000000-0000-0000-0000-000000000000"), CentroId = 3, Vehiculo = new VehiculoDto { Primero = true } };

            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>())).Returns(new Chofer { Id = dto.Chofer.Id });
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(new Material { Id = dto.MaterialId });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.Is<int>(i => i == dto.DestinoId))).Returns(new Centro { Id = dto.DestinoId });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.Is<int>(i => i == comando.CentroId))).Returns(new Centro { Id = comando.CentroId });
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial { Id = dto.TipoComercialId });
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(new Transportista { Id = dto.TransportistaId ?? 0 });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido());
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                    .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());
            comando.Orden.TipoDeWorkflow = TipoDeWorkflow.Egreso;
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Recorrido>()), Times.Never());
            repositorioMock.Verify(s => s.Agregar(It.Is<CartaPorte>(o => o.Chofer.Id == dto.Chofer.Id && o.CentroDestino.Id == dto.DestinoId)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestExcepción()
        {
            var comando = new CrearCartaPorte { Orden = dto, NombreWorkflow = "W1", InstanciaWorkflowId = new Guid("00000000-0000-0000-0000-000000000000") };

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Throws(new IOException("Error"));

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Recorrido>()), Times.Never());
            repositorioMock.Verify(s => s.Agregar(It.IsAny<CartaPorte>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.First().Value.Contains("error"), Is.EqualTo(true));
        }
    }
}
