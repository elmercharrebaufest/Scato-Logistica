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
    public class ProcesadorModificarOrdenDeCargaContenedorTest
    {
        private ProcesadorModificarOrdenDeCargaContenedor target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private OrdenDeCargaContenedorDto dto;
        private OrdenDeCargaContenedor OrdenDeCargaContenedor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarOrdenDeCargaContenedor(repositorioMock.Object, conversor, new NullLogger());

            OrdenDeCargaContenedor = new OrdenDeCargaContenedor
            {
                Chofer = new Chofer { Id = 1 },
                Destino = new Cliente(),
                Material = new Material(),
                NroOrdenDeCargaContenedor = "11",
                PatenteCamion = "AAA111",
                TipoComercial = new TipoComercial(),
                Transportista = new Transportista(),
                ContenedorEntrada = new TaraContenedor(),
                ContenedorSalida = new TaraContenedor()
            };
            dto = new OrdenDeCargaContenedorDto
            {
                Chofer = new ChoferDto { Id = 1 },
                DestinoId = 1,
                MaterialId = 1,
                OrdenDeCargaContenedor = "11",
                PatenteCamion = "AAABBB",
                TipoComercialId = 1,
                TransportistaId = 1,
                ContenedorEntradaId = 1,
                ContenedorSalidaId = 2,
                RecorridoId = 1
            };

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<LogModificacionDocumentoIngreso, bool>>>()))
                .Returns(new LogModificacionDocumentoIngreso());
        }


        [Test]
        public void TestCrear()
        {
            var workflows = new List<Workflow> { new Workflow { Id = 1, Codigo = "W1", Descripcion = "D1" } };
            var comando = new ModificarOrdenDeCargaContenedor { Orden = dto, NombreWorkflow = "W1" };

            repositorioMock.Setup(s => s.Obtener<OrdenDeCargaContenedor>(It.IsAny<int>())).Returns(OrdenDeCargaContenedor);
            repositorioMock.Setup(s => s.Obtener<Recorrido>(1)).Returns(new Recorrido { Id = 1 });
            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>())).Returns(new Chofer { Id = dto.Chofer.Id });
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(new Material { Id = dto.MaterialId });
            repositorioMock.Setup(s => s.Obtener<Cliente>(It.Is<int>(i => i == dto.DestinoId))).Returns(new Cliente { Id = dto.DestinoId });
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial { Id = dto.TipoComercialId });
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(new Transportista { Id = dto.TransportistaId });
            repositorioMock.Setup(s => s.Obtener<TaraContenedor>(It.IsAny<int>())).Returns(new TaraContenedor { Id = dto.ContenedorEntradaId });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                    .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Chofer, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<OrdenDeCargaContenedor, bool>>>())).Returns(false);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoComercial, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Transportista, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Cliente, bool>>>())).Returns(true);
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
