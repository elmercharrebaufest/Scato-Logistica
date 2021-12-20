using System.Collections.Generic;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class TerminarInhabilitacionTest
    {
        private TerminarInhabilitacion target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> servComandosMock;

        [SetUp]
        public void SetUp()
        {
            target = new TerminarInhabilitacion();
            srvRepositorio = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(servComandosMock.Object);
        }

        [Test]
        public void TerminarInhabilitacion()
        {
            srvRepositorio.Setup(x => x.ListarInhabilitacionCamion(It.IsAny<string>(), It.IsAny<int>())).Returns(new List<InhabilitacionCamionDto>{new InhabilitacionCamionDto(),new InhabilitacionCamionDto()});
            srvRepositorio.Setup(x => x.ListarInhabilitacionChofer(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<InhabilitacionChoferDto>{new InhabilitacionChoferDto(),new InhabilitacionChoferDto()});
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.ModificarInhabilitacionCamion>())).Returns(new Resultado());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.ModificarInhabilitacionChofer>())).Returns(new Resultado());

            host.InArguments.Patente = "aaabbb";
            host.InArguments.ChoferId = 1;
            host.InArguments.CentroId = 2;
            host.InArguments.NombreUsuario = "user1";
            host.InArguments.ControlRecorrido = new ControlRecorridoDto {Comentario = "" };
            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Empty);
            servComandosMock.Verify(p => p.Ejecutar(It.Is<Dominio.Comandos.ModificarInhabilitacionCamion>(x => true)), Times.Exactly(2));
            servComandosMock.Verify(p => p.Ejecutar(It.Is<Dominio.Comandos.ModificarInhabilitacionChofer>(x => true)), Times.Exactly(2));

        }
    }
}
