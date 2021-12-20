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
    public class ProcesadorModificarControlDeTiempoTest
    {
        private ProcesadorModificarControlDeTiempo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ControlDeTiempoDto dto;
        private List<ControlDeTiempo> controlesDeTiempos;


        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarControlDeTiempo(repositorioMock.Object, conversorMock.Object, new NullLogger());
            dto = new ControlDeTiempoDto
                {
                    Id = 0,
                    ActividadDesde = "A11",
                    ActividadHasta = "A22",
                    WorkflowCodigo = "wc",
                    TiempoMaximo = 4,
                    CodigoControl = "111",
                    WorkflowId = 5
                };

            controlesDeTiempos = new List<ControlDeTiempo>
                {
                    new ControlDeTiempo
                        {
                            Id = 1,
                            ActividadDesde = "A1",
                            ActividadHasta = "A2",
                            Workflow = new Workflow {Id =5},
                            TiempoMaximo = 4,
                            CodigoControl = "222",
                        },
                    new ControlDeTiempo
                        {
                            Id = 2,
                            ActividadDesde = "A3",
                            ActividadHasta = "A6",
                            Workflow = new Workflow {Id = 5},
                            TiempoMaximo = 4,
                            CodigoControl = "333",
                        },
                };

            repositorioMock.Setup(s => s.Obtener<Workflow>(It.Is<int>(i => i == 5))).Returns(new Workflow());
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<ControlDeTiempo, bool>>>()))
                           .Returns<Expression<Func<ControlDeTiempo, bool>>>(q => controlesDeTiempos.Any((q.Compile())));


            repositorioMock.Setup(s => s.Obtener<ControlDeTiempo>(It.IsAny<int>())).Returns(controlesDeTiempos[0]);

            var comando = new ModificarControlDeTiempo() {Dto = dto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestExistenteError()
        {
            dto.ActividadDesde = "A1";
            dto.ActividadHasta = "A2";
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<ControlDeTiempo, bool>>>()))
                           .Returns<Expression<Func<ControlDeTiempo, bool>>>(q => controlesDeTiempos.Any((q.Compile())));

            var comando = new ModificarControlDeTiempo() { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

        [Test]
        public void TestMismasActividadesError()
        {
            dto.ActividadDesde = "A7";
            dto.ActividadHasta = "A7";
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<ControlDeTiempo, bool>>>()))
                           .Returns<Expression<Func<ControlDeTiempo, bool>>>(q => controlesDeTiempos.Any((q.Compile())));

            var comando = new ModificarControlDeTiempo() { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<ControlDeTiempo>(x => x.Id == 0)), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}
