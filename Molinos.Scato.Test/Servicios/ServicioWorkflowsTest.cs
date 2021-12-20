using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Servicios
{
    [TestFixture]
    public class ServicioWorkflowsTest
    {
        private ServicioWorkflows target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ServicioWorkflows(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestListarWorkflows()
        {
            var workflows = new List<Workflow>
                {
                    new Workflow {Codigo = "WF1", Descripcion = "Workflow 1", Activo = true},
                    new Workflow {Codigo = "WF2", Descripcion = "Workflow 2", Activo = true},
                    new Workflow {Codigo = "WF3", Descripcion = "Workflow 3", Activo = true}
                };
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Workflow, bool>>>())).Returns(workflows);

            var resultado = target.ListarWorkflows();
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado, Has.Count.EqualTo(3));

            for (int i = 0; i < resultado.Count; i++)
            {
                Assert.That(resultado[i].Codigo, Is.EqualTo(workflows[i].Codigo));
                Assert.That(resultado[i].Descripcion, Is.EqualTo(workflows[i].Descripcion));
                Assert.That(resultado[i].Activo, Is.EqualTo(workflows[i].Activo));
            }
        }

        [Test]
        public void TestListarDefinicionesWorkflow()
        {
            var wf1 = new Workflow {Id = 1, Codigo = "WF1"};
            var wf2 = new Workflow {Id = 2, Codigo = "WF2"};
            var definiciones = new List<WorkflowDefinicion>
                {
                    new WorkflowDefinicion {Id = 1, Workflow = wf1},
                    new WorkflowDefinicion {Id = 2, Workflow = wf2},
                    new WorkflowDefinicion {Id = 3, Workflow = wf1},
                    new WorkflowDefinicion {Id = 4, Workflow = wf2},
                };

            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<WorkflowDefinicion, bool>>>()))
                           .Returns<Expression<Func<WorkflowDefinicion, bool>>>(
                               filtro => definiciones.Where(filtro.Compile()).ToList());

            var resultado = target.ListarDefinicionesWorkflow(2);
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado, Has.Count.EqualTo(2));
            Assert.That(resultado[0].Id, Is.EqualTo(2));
            Assert.That(resultado[1].Id, Is.EqualTo(4));
        }

        [Test]
        public void TestObtenerUltimadefinicion()
        {
            var wf1 = new Workflow {Id = 1, Codigo = "WF1"};
            var wf2 = new Workflow {Id = 2, Codigo = "WF2"};
            var definiciones = new List<WorkflowDefinicion>
                {
                    new WorkflowDefinicion {Id = 1, Workflow = wf1},
                    new WorkflowDefinicion {Id = 2, Workflow = wf2},
                    new WorkflowDefinicion {Id = 3, Workflow = wf1},
                    new WorkflowDefinicion {Id = 4, Workflow = wf2},
                };

            repositorioMock.Setup(r => r.ObtenerMayor(
                It.IsAny<Expression<Func<WorkflowDefinicion, bool>>>(),
                It.IsAny<Expression<Func<WorkflowDefinicion, int>>>()))
                           .Returns
                <Expression<Func<WorkflowDefinicion, bool>>, Expression<Func<WorkflowDefinicion, int>>>(
                    (filtro, orden) =>
                    definiciones.Where(filtro.Compile()).OrderByDescending(orden.Compile()).FirstOrDefault()
                );

            var resultado = target.ObtenerUltimaDefinicionWorkflow(1);
            Assert.That(resultado.Id, Is.EqualTo(3));
        }

        [Test]
        public void TestObtenerArchivoDefinicionWorkflow()
        {
            var definicion = new WorkflowDefinicion
                {
                    Id = 2,
                    Definicion = new byte[] {1, 2, 3}
                };
            repositorioMock.Setup(r => r.Obtener<WorkflowDefinicion>(2)).Returns(definicion);

            var resultado = target.ObtenerArchivoDefinicionWorkflow(2);
            Assert.That(resultado, Is.EqualTo(definicion.Definicion));
        }

        [Test]
        public void TestActualizarDefinicionInactiva()
        {
            var definicion = new WorkflowDefinicion
            {
                Id = 2,
                Definicion = new byte[] { 1, 2, 3 },
                Activa = false
            };
            repositorioMock.Setup(r => r.Obtener<WorkflowDefinicion>(2)).Returns(definicion);
            var nuevaDefinicion = new byte[] {4, 5, 6};
            target.ActualizarDefinicion(2, nuevaDefinicion, "MiActividad", false);
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
            Assert.That(definicion.Definicion, Is.EqualTo(nuevaDefinicion));
        }

        [Test]
        public void TestActualizarDefinicionActiva()
        {
            var definicion = new WorkflowDefinicion
            {
                Id = 2,
                Definicion = new byte[] { 1, 2, 3 },
                Activa = true
            };
            repositorioMock.Setup(r => r.Obtener<WorkflowDefinicion>(2)).Returns(definicion);
            var nuevaDefinicion = new byte[] { 4, 5, 6 };

            Assert.That(() => target.ActualizarDefinicion(2, nuevaDefinicion, "MiActividad",false), Throws.InvalidOperationException);
            
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Never());
            Assert.That(definicion.Definicion, Is.Not.EqualTo(nuevaDefinicion));
        }

        [Test]
        public void TestCrearWorkflow()
        {
            var definicionDto = new WorkflowDefinicionDto
                {
                    FechaCreacion = DateTime.MinValue,
                    Comentario = "Comentario versión",
                    NombreUsuario = "diego",
                    Activa = true,
                    ActividadInicial = "Act1",
                    Workflow = new WorkflowDto
                        {
                            Codigo = "WF1",
                            Descripcion = "Descripcion",
                            Activo = false,
                            TipoDeWorkflow = TipoDeWorkflow.Egreso,
                        }
                };

            var xaml = new byte[] {1, 2, 3};

            WorkflowDefinicion definicionGuardada = null;

            repositorioMock.Setup(r => r.Agregar(It.IsAny<WorkflowDefinicion>()))
                           .Callback<WorkflowDefinicion>(def =>
                               {
                                   definicionGuardada = def;
                                   def.Id = 1;
                                   def.Workflow.Id = 2;
                               });
            
            var definicionDevuelta = target.CrearWorkflow(definicionDto, xaml);
            Assert.That(definicionDevuelta, Is.Not.Null);
            Assert.That(definicionDevuelta.Id, Is.EqualTo(1));
            Assert.That(definicionDevuelta.Workflow, Is.Not.Null);
            Assert.That(definicionDevuelta.Workflow.Id, Is.EqualTo(2));

            Assert.That(definicionGuardada, Is.Not.Null);
            Assert.That(definicionGuardada.Definicion, Is.EqualTo(xaml));
            Assert.That(definicionGuardada.FechaCreacion, Is.GreaterThanOrEqualTo(DateTime.Today));
            Assert.That(definicionGuardada.Workflow, Is.Not.Null);
            
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }

        [Test]
        public void TestCrearWorkflowCodigoExistente()
        {
            var definicionDto = new WorkflowDefinicionDto
            {
                FechaCreacion = DateTime.MinValue,
                Comentario = "Comentario versión",
                NombreUsuario = "diego",
                Activa = true,
                ActividadInicial = "Act1",
                Workflow = new WorkflowDto
                {
                    Codigo = "WF1",
                    Descripcion = "Descripcion",
                    Activo = false,
                    TipoDeWorkflow = TipoDeWorkflow.Egreso,
                }
            };

            var xaml = new byte[] { 1, 2, 3 };

            var workflow = new Workflow { Codigo = "WF1" };

            repositorioMock.Setup(r => r.Existe(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Returns<Expression<Func<Workflow, bool>>>(
                               filtro => filtro.Compile().Invoke(workflow));

            Assert.That(() => target.CrearWorkflow(definicionDto, xaml), Throws.InvalidOperationException);

            repositorioMock.Verify(r => r.Agregar(It.IsAny<WorkflowDefinicion>()), Times.Never());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Never());
        }

        [Test]
        public void TestCrearDefinicionWorkflow()
        {
            var workflow = new Workflow
                {
                    Id = 2,
                    Codigo = "WF1"
                };
            repositorioMock.Setup(r => r.Obtener<Workflow>(2)).Returns(workflow);

            WorkflowDefinicion definicionGuardada = null;
            repositorioMock.Setup(r => r.Agregar(It.IsAny<WorkflowDefinicion>()))
               .Callback<WorkflowDefinicion>(def =>
               {
                   definicionGuardada = def;
                   def.Id = 1;
               });

            var definicionDto = new WorkflowDefinicionDto
            {
                FechaCreacion = DateTime.MinValue,
                Comentario = "Comentario versión",
                NombreUsuario = "diego",
                Activa = true,
                Workflow = null
            };
            var xaml = new byte[] { 1, 2, 3 };
            var definicionDevuelta = target.CrearDefinicionWorkflow(2, definicionDto, xaml);

            Assert.That(definicionDevuelta, Is.Not.Null);
            Assert.That(definicionDevuelta.Id, Is.EqualTo(1));
            Assert.That(definicionDevuelta.Workflow.Codigo, Is.EqualTo("WF1"));

            Assert.That(definicionGuardada, Is.Not.Null);
            Assert.That(definicionGuardada.Workflow, Is.EqualTo(workflow));
            Assert.That(definicionGuardada.FechaCreacion, Is.GreaterThanOrEqualTo(DateTime.Today));
            Assert.That(definicionGuardada.Definicion, Is.EqualTo(xaml));

            repositorioMock.Verify(r => r.Agregar(It.IsAny<WorkflowDefinicion>()), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }



    }
}
