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
    public class ProcesadorModificarExcepcionEnvioCamaraTest
    {
        private ProcesadorModificarMaterialPorWorkflow target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private MaterialPorWorkflowDto tipoDto;
        private MaterialPorWorkflow tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarMaterialPorWorkflow(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new MaterialPorWorkflowDto
            {
                Id = 1,
                CentroId = 1,
                MaterialId = 1,
                WorkflowId = 1
            };
            tipo = new MaterialPorWorkflow
            {
                Id = 1,
                Centro = new Centro(){ Id=1, Descripcion = "Centro 1" },
                Material = new Material(){ Id = 1, Descripcion = "Material 1"},
                Workflow = new Workflow(){ Id = 1, Descripcion = "Workflow 1"}
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            var workflows = new List<Workflow>
                {
                    new Workflow() {Id = 1, Descripcion = "Workflow 1"},
                };
            var materiales = new List<Material>
                {
                    new Material() {Id = 1, Descripcion = "Material 1"},
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Workflow, bool>>>()))
                    .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                    .Returns<Expression<Func<Material, bool>>>(q => materiales.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<MaterialPorWorkflow>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarMaterialPorWorkflow { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
