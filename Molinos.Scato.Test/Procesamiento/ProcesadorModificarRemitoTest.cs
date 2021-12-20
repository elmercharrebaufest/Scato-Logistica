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
    public class ProcesadorModificarRemitoTest
    {
        private ProcesadorModificarRemito target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private RemitoDto dto;
        private Remito Remito;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarRemito(repositorioMock.Object, conversor, new NullLogger());

            Remito = new Remito
                {
                    Chofer = new Chofer {Id = 1},
                    Material = new Material(),
                    OrdenDeDescarga = "11",
                    PatenteCamion = "AAABBB",
                    TipoComercial = new TipoComercial(),
                    Transportista = new Transportista(),
                    CentroOrigen = new Centro{ Id = 1},
                    ProveedorOrigen = new Proveedor() { Id = 1 },
                };
            dto = new RemitoDto
                {
                    Chofer = new ChoferDto{Id = 1},
                    MaterialId = 1,
                    OrdenDeDescarga = "11",
                    PatenteCamion = "AAABBB",
                    TipoComercialId = 1,
                    TransportistaId = 1,
                    OrigenId = 2,
                    RecorridoId = 1
                };

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<LogModificacionDocumentoIngreso, bool>>>()))
                .Returns(new LogModificacionDocumentoIngreso());
        }


        [Test]
        public void TestModificarRemito()
        {
            var workflows = new List<Workflow> { new Workflow { Id = 1, Codigo = "W1", Descripcion = "D1" } };
            var comando = new ModificarRemito { Orden = dto, NombreWorkflow = "W1"};

            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(new Centro(){ Id = 2});
            repositorioMock.Setup(s => s.Obtener<Remito>(It.IsAny<int>())).Returns(Remito);
            repositorioMock.Setup(s => s.Obtener<Recorrido>(1)).Returns(new Recorrido { Id = 1 });
            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>())).Returns(new Chofer {Id = dto.Chofer.Id});
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(new Material {Id = dto.MaterialId});
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial {Id = dto.TipoComercialId});
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(new Transportista {Id = dto.TransportistaId});
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                    .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Chofer, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Remito, bool>>>())).Returns(false);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoComercial, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Transportista, bool>>>())).Returns(true);
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(Remito.CentroOrigen.Id, Is.EqualTo(2));
        }

        [Test]
        public void TestModificarRemitoTerceros()
        {
            var workflows = new List<Workflow> { new Workflow { Id = 1, Codigo = "W1", Descripcion = "D1" } };
            var comando = new ModificarRemito { Orden = dto, NombreWorkflow = "W1" };
            dto.EsRemitoProveedor = true;
            repositorioMock.Setup(s => s.Obtener<Proveedor>(It.IsAny<int>())).Returns(new Proveedor() { Id = 2 });
            repositorioMock.Setup(s => s.Obtener<Remito>(It.IsAny<int>())).Returns(Remito);
            repositorioMock.Setup(s => s.Obtener<Recorrido>(1)).Returns(new Recorrido { Id = 1 });
            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>())).Returns(new Chofer { Id = dto.Chofer.Id });
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(new Material { Id = dto.MaterialId });
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial { Id = dto.TipoComercialId });
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(new Transportista { Id = dto.TransportistaId });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                    .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Chofer, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Remito, bool>>>())).Returns(false);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoComercial, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Transportista, bool>>>())).Returns(true);
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(Remito.ProveedorOrigen.Id, Is.EqualTo(2));
        }

    }
}
