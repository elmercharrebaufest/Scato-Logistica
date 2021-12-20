using System.Collections.Generic;
using System.Linq;
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
    public class ProcesadorCrearTipoComercialPorWfTest
    {
        private ProcesadorCrearTipoComercialPorWf target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TipoComercialPorWfDto dto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearTipoComercialPorWf(repositorioMock.Object, conversor, new NullLogger());
            dto = new TipoComercialPorWfDto
                {
                    WorkflowId = 1,
                    TipoComercialId = 1,
                    WorkflowDescripcion = "W1",
                    TipoComercialDescripcion = "T2"
                };
        }

        [Test]
        public void TestCrear()
        {
            var comando = new CrearTipoComercialPorWf { Dto = dto };

            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<int>())).Returns(new Workflow{TiposComercialesAsociados = new List<TipoComercial>()});
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial());

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorExistencia()
        {
            var tipos = new List<TipoComercial>
                {
                    new TipoComercial {Id = 1, Descripcion = "T1"},
                    new TipoComercial {Id = 2, Descripcion = "T2"}
                };

            var workflows = new List<Workflow>
                {
                    new Workflow {Id = 1, Descripcion = "T1", TiposComercialesAsociados = new List<TipoComercial>{tipos[0]}},
                    new Workflow {Id = 2, Descripcion = "T2"}
                };

            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<int>())).Returns(workflows[0]);
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(tipos[0]);

            var comando = new CrearTipoComercialPorWf { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TipoDocumentoIdentidad>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo(""));
        }
    }
}
