using System.Collections.Generic;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ListaDeTareasAutomatizadaControllerTest
    {
        private ListaDeTareasAutomatizadaController target;
        private Mock<IServicioOrquestador> orquestadorMock;
        private Mock<IServicioComandos> comandosMock;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IListaDeWorkflows> listaDeWorkflowsMock;

        [SetUp]
        public void SetUp()
        {
            orquestadorMock = new Mock<IServicioOrquestador>();
            comandosMock = new Mock<IServicioComandos>();
            servRepositorioMock = new Mock<IServicioRepositorio>();
            listaDeWorkflowsMock = new Mock<IListaDeWorkflows>();

            target = new ListaDeTareasAutomatizadaController(new NullLogger(), servRepositorioMock.Object, comandosMock.Object, orquestadorMock.Object, listaDeWorkflowsMock.Object);


            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorNombrePc(It.IsAny<string>(), It.IsAny<int>())).Returns(
                new List<PuestoDeTrabajoDto>
                    {
                        new PuestoDeTrabajoDto
                            {
                                Automatico = true,
                                Lector = "lector"
                            },
                        new PuestoDeTrabajoDto
                            {
                                Automatico = true,
                                Lector = "lector"
                            },
                        new PuestoDeTrabajoDto
                            {
                                Automatico = false,
                            }
                    }
                );

            comandosMock.Setup(s => s.Ejecutar(It.IsAny<AlertarAnalisisObligatorio>())).Returns(new ResultadoAlertarAnalisisObligatorio {AlertarAnalisisObligatorio = false, Material = new List<string>() });

            orquestadorMock.Setup(s => s.Suscribir(It.IsAny<ComandoSuscribir>()))
                .Returns(new ResultadoSuscribir());
        }

        [Test]
        public void TestIndex()
        {
            var result = target.Index(new DatosUsuario{NombrePc = "Pc", CentroId = 1}) as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);

            //orquestadorMock.Verify(p => p.Suscribir(It.IsAny<ComandoSuscribir>()), Times.Exactly(2));
            servRepositorioMock.Verify(p => p.ListarPuestosDeTrabajoPorNombrePc("Pc", It.IsAny<int>()), Times.Exactly(1));
        }

    }
}
