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
    public class ProcesadorCrearRemitoBodegaVinoTest
    {

        private ProcesadorCrearRemitoBodegaVino target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private RemitoBodegaVinoDto dto;

        [SetUp]
        public void SetUp()
        {
            
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearRemitoBodegaVino(repositorioMock.Object,conversor,new NullLogger());
            dto = new RemitoBodegaVinoDto
                {
                    Chofer = new ChoferDto
                        {
                            Id = 2,
                            Nombre = "Walter",
                            NumeroDeDocumento = "39987987"
                        },
                    NroRemito = "1234-11111111",
                    Patente = "WMD001",
                    TipoComercialId = 2,
                    TransportistaId = 2,
                    ProveedorId = 2
                };

        }

        [Test]
        public void NoEncontroWf()
        {
            var workflows = new List<Workflow> {new Workflow {Id = 1, Codigo = "A1", Descripcion = "B1"}};
            var comando = new CrearRemitoBodegaVino {Orden = dto, NombreWorkflow = "C1", InstanciaWorkflowId = new Guid("00000000-0000-0000-0000-000000000000")};

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Returns<Expression<Func<Workflow, bool>>>(
                               q => workflows.Where((q.Compile())).SingleOrDefault());

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.Agregar(It.IsAny<Recorrido>()), Times.Never());
            repositorioMock.Verify(s => s.Agregar(It.IsAny<RemitoBodegaVinoDto>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearRemitoBodegaVino()
        {
            var workflows = new List<Workflow> {new Workflow {Id = 2, Codigo = "P1", Descripcion = "D1"}};
            var comando = new CrearRemitoBodegaVino
                {
                    Orden = dto,
                    NombreWorkflow = "P1",
                    InstanciaWorkflowId = new Guid("00000000-0000-0000-0000-000000000000"), 
                    CentroId = 3
                };
            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>())).Returns(new Chofer {Id = dto.Chofer.Id});
            repositorioMock.Setup(s => s.Obtener<Centro>(It.Is<int>(i => i == comando.CentroId))).Returns(new Centro {Id = comando.CentroId});
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>()))
                           .Returns(new Transportista {Id = dto.TransportistaId});
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>()))
                           .Returns(new TipoComercial {Id = dto.TipoComercialId});

            repositorioMock.Setup(s=> s.Obtener(It.IsAny<Expression<Func<Workflow,bool>>>()))
                           .Returns<Expression<Func<Workflow,bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());

            comando.Orden.TipoDeWorkflow = TipoDeWorkflow.Ingreso;
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Recorrido>(r => r.InstanciaWorkflow == comando.InstanciaWorkflowId &&
                                                                   r.Centro.Id == comando.CentroId && 
                                                                   r.Transportista.Id == comando.Orden.TransportistaId && 
                                                                   r.Workflow.Codigo == "P1" && 
                                                                   r.Chofer.Id == comando.Orden.Chofer.Id && 
                                                                   r.TipoComercial.Id == comando.Orden.TipoComercialId)), Times.Exactly(1));

            repositorioMock.Verify(s => s.Agregar(It.Is<RemitoBodegaVino>(r => r.NroRemito == comando.Orden.NroRemito)));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));


        }

        [Test]
        public void TestExcepcion()
        {

            var comando = new CrearRemitoBodegaVino
                {
                    Orden = dto,
                    NombreWorkflow = "P1",
                    InstanciaWorkflowId = new Guid("00000000-0000-0000-0000-000000000000")
                };
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Throws(new IOException("Error"));

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Recorrido>()), Times.Never());
            repositorioMock.Verify(s => s.Agregar(It.IsAny<RemitoBodegaVino>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.First().Value.Contains("error"), Is.EqualTo(true));
        }



    }
}
