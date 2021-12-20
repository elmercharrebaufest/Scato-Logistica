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
    public class ProcesadorModificarOrdenDeDescargaFasonTest
    {
        private ProcesadorModificarOrdenDeDescargaFason target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private OrdenDeDescargaFasonDto dto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarOrdenDeDescargaFason(repositorioMock.Object, conversor, new NullLogger());
            dto = new OrdenDeDescargaFasonDto
            {
                Chofer = new ChoferDto { Id = 1 },
                PatenteCamion = "AAABBB",
                TipoComercialId = 1,
                TransportistaId = 1,
                ClienteId = 1,
                MaterialId = 1,
                RecorridoId = 1
            };
        }


        [Test]
        public void TestCrear()
        {
            var workflows = new List<Workflow> { new Workflow { Id = 1, Codigo = "W1", Descripcion = "D1" } };
            var comando = new ModificarOrdenDeDescargaFason { Orden = dto };

            repositorioMock.Setup(s => s.Obtener<OrdenDeDescargaFason>(It.IsAny<int>())).Returns(new OrdenDeDescargaFason { Id = 1 });
            repositorioMock.Setup(s => s.Obtener<Recorrido>(1)).Returns(new Recorrido { Id = 1 });
            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>())).Returns(new Chofer { Id = dto.Chofer.Id });
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial { Id = dto.TipoComercialId });
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(new Transportista { Id = dto.TransportistaId });
            repositorioMock.Setup(s => s.Obtener<Cliente>(It.IsAny<int>())).Returns(new Cliente { Id = dto.ClienteId });
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(new Material { Id = dto.MaterialId });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                    .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
