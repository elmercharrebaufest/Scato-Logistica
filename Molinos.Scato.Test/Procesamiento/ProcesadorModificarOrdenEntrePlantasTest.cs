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
    public class ProcesadorModificarOrdenEntrePlantasTest
    {
        private ProcesadorModificarOrdenEntrePlantas target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private OrdenEntrePlantasDto dto;
        private OrdenEntrePlantas ordenEntrePlantas;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarOrdenEntrePlantas(repositorioMock.Object, conversor, new NullLogger());
            dto = new OrdenEntrePlantasDto
                {
                    Chofer = new ChoferDto{Id = 1},
                    PatenteCamion = "AAABBB",
                    TipoComercialId = 1,
                    TransportistaId = 1,
                    RecorridoId = 1
                };
            ordenEntrePlantas = new OrdenEntrePlantas
                {
                    Chofer = new Chofer { Id = 1, Nombre = "N", Apellido = "A"},
                    PatenteCamion = "AAABBB",
                    TipoComercial = new TipoComercial(),
                    Transportista = new Transportista(),
                    Material = new Material(),
                    CentroDestino = new Centro(),
                };
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<LogModificacionDocumentoIngreso, bool>>>()))
                .Returns(new LogModificacionDocumentoIngreso());
        }


        [Test]
        public void TestCrear()
        {
            var workflows = new List<Workflow> { new Workflow { Id = 1, Codigo = "W1", Descripcion = "D1" } };
            var comando = new ModificarOrdenEntrePlantas { Orden = dto};

            repositorioMock.Setup(s => s.Obtener<OrdenEntrePlantas>(It.IsAny<int>())).Returns(ordenEntrePlantas);
            repositorioMock.Setup(s => s.Obtener<Recorrido>(1)).Returns(new Recorrido { Id = 1 });
            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>())).Returns(new Chofer {Id = dto.Chofer.Id});
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial {Id = dto.TipoComercialId});
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(new Transportista {Id = dto.TransportistaId});
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(new Material { Id = dto.MaterialId });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(new Centro() { Id = dto.CentroDestinoId });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                    .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Chofer, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<OrdenEntrePlantas, bool>>>())).Returns(false);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoComercial, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Transportista, bool>>>())).Returns(true);
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
