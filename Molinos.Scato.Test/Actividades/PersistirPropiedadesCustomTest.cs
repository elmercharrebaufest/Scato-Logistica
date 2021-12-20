using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class PersistirPropiedadesCustomTest
    {
        private PersistirPropiedadesCustom target;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;
        private ScatoPersistenceParticipant participant;

        [SetUp]
        public void SetUp()
        {
            target = new PersistirPropiedadesCustom();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            participant = new ScatoPersistenceParticipant();
            host.Extensions.Add(participant);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestPersistirTodosLosValores()
        {
            host.InArguments.Patente = "ABC123";
            host.InArguments.MaterialId = 1;
            host.InArguments.TransportistaId = 1;
            host.InArguments.Cuit = "20-22222222-0";
            host.InArguments.Calidad = "Calidad1";
            host.InArguments.TipoDocumentoDeIngreso = 1;
            host.InArguments.NumeroDocumentoDeIngreso = "12345";
            host.InArguments.CentroId = 1;
            host.InArguments.Workflow = "TestWorkflow";

            host.TestActivity();

            Assert.That(participant.Patente, Is.EqualTo("ABC123"));
            Assert.That(participant.MaterialId, Is.EqualTo(1));
            Assert.That(participant.TransportistaId, Is.EqualTo(1));
            Assert.That(participant.Cuit, Is.EqualTo("20-22222222-0"));
            Assert.That(participant.Calidad, Is.EqualTo("Calidad1"));
            Assert.That(participant.TipoDocumentoDeIngreso, Is.EqualTo(1));
            Assert.That(participant.NumeroDocumentoDeIngreso, Is.EqualTo("12345"));
            Assert.That(participant.CentroId, Is.EqualTo(1));
            Assert.That(participant.Workflow, Is.EqualTo("TestWorkflow"));
        }

        [Test]
        public void TestPersistirSoloIds()
        {

            host.InArguments.Patente = "ABC123";
            host.InArguments.MaterialId = 1;
            host.InArguments.TransportistaId = 1;
            host.InArguments.CentroId = 1;
            host.InArguments.Workflow = "TestWorkflow";

            srvRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto {Id = 1, Descripcion = "Material1"});
            srvRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>())).Returns(new TransportistaDto { Id = 1, RazonSocial = "Transportista1" });

            host.TestActivity();

            Assert.That(participant.Patente, Is.EqualTo("ABC123"));
            Assert.That(participant.Material, Is.EqualTo("Material1"));
            Assert.That(participant.MaterialId, Is.EqualTo(1));
            Assert.That(participant.Transportista, Is.EqualTo("Transportista1"));
            Assert.That(participant.TransportistaId, Is.EqualTo(1));
            Assert.That(participant.CentroId, Is.EqualTo(1));
            Assert.That(participant.Workflow, Is.EqualTo("TestWorkflow"));
        }

      

    }
}
