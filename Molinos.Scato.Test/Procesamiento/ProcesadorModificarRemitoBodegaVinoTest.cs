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
    public class ProcesadorModificarRemitoBodegaVinoTest
    {            
        private ProcesadorModificarRemitoBodegaVino target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private RemitoBodegaVinoDto remitoDto;
        private RemitoBodegaVino remito;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarRemitoBodegaVino(repositorioMock.Object,conversor,new NullLogger());

            remito = new RemitoBodegaVino
                {
                    Chofer = new Chofer{Id = 1, Nombre = "W"},
                    Material = new Material(),
                    NroRemito = "22",
                    Patente = "AAA111",
                    TipoComercial = new TipoComercial(),
                    Transportista = new Transportista(),
                    OrdenDeCompra = "888"
                };

            remitoDto = new RemitoBodegaVinoDto
                {
                    Chofer = new ChoferDto {Id = 1},
                    MaterialId = 1,
                    NroRemito = "22",
                    Patente = "AAA111",
                    TipoComercialId = 1,
                    TransportistaId = 1,
                    RecorridoId = 1
                };

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<LogModificacionDocumentoIngreso, bool>>>())).Returns(new LogModificacionDocumentoIngreso());

        }

        [Test]
        public void TestModificarRemitoBodegaVino()
        {
            var workflows = new List<Workflow> {new Workflow {Id = 1, Codigo = "w1", Descripcion = "d1"}};
            var comando = new ModificarRemitoBodegaVino {Orden = remitoDto, NombreWorkflow = "w1"};

            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(new Centro{Id = 2});
            repositorioMock.Setup(s => s.Obtener<RemitoBodegaVino>(It.IsAny<int>())).Returns(remito);
            repositorioMock.Setup(s => s.Obtener<Recorrido>(1)).Returns(new Recorrido());
            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>())).Returns(new Chofer{Id = remitoDto.Chofer.Id});
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial{Id = remitoDto.TipoComercialId});
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(new Material{Id = remitoDto.MaterialId});
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(new Transportista{Id = remitoDto.TransportistaId});
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Returns<Expression<Func<Workflow, bool>>>(
                               q => workflows.Where((q.Compile())).SingleOrDefault());

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Chofer, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<RemitoBodegaVino, bool>>>())).Returns(false);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Transportista, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoComercial, bool>>>())).Returns(true);

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
