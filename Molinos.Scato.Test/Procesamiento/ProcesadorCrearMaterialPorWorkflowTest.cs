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
    public class ProcesadorCrearMaterialPorWorkflowTest
    {
        private ProcesadorCrearMaterialPorWorkflow target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private MaterialPorWorkflowDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearMaterialPorWorkflow(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new MaterialPorWorkflowDto
            {
                Id = 1,
                CentroId = 1,
                MaterialId = 1,
                WorkflowId = 1
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var materialesExistentesT = new List<Material>
                {
                    new Material
                        {
                            Id = 1,
                            Descripcion = "Material 1"
                        },
                };
            var workflowsExistentesT = new List<Workflow>
                {
                    new Workflow
                        {
                            Id = 1,
                            Descripcion = "Workflow 1"
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                           .Returns<Expression<Func<Material, bool>>>(q => materialesExistentesT.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Workflow, bool>>>()))
               .Returns<Expression<Func<Workflow, bool>>>(q => workflowsExistentesT.Any((q.Compile())));

            var comando = new CrearMaterialPorWorkflow() {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<MaterialPorWorkflow>(x => x.Id == 0)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
