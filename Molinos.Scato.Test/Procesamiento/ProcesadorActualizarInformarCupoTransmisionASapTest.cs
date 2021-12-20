using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorActualizarInformarCupoTransmisionASapTest
    {
        private ProcesadorActualizarInformarCupoTransmisionASap target;
        private Mock<IRepositorio> servRepositorio;
        private ActualizarInformarCupoTransmisionASap comando;
        private IConversor conversor;
        private Guid instanceId;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IRepositorio>();
            instanceId = Guid.NewGuid();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorActualizarInformarCupoTransmisionASap(servRepositorio.Object, conversor, new NullLogger());
            comando = new ActualizarInformarCupoTransmisionASap { Dto = new InformarCupoTransmisionASap { Id = 1, InstanciaWorkflow = instanceId, MensajeError = "Error",Estado = EstadoTransmisionASap.Pendiente} };
        }

        [Test]
        public void ExecuteTransmisionNoNula()
        {
            servRepositorio.Setup(s => s.Obtener(It.IsAny<Expression<Func<InformarCupoTransmisionASap, bool>>>()))
                           .Returns(new InformarCupoTransmisionASap());

            var result = target.Ejecutar(comando);
            Assert.NotNull(result);
            Assert.False(result.HayErrores);
            servRepositorio.Verify(s => s.Agregar(It.IsAny<InformarCupoTransmisionASap>()), Times.Never());
            servRepositorio.Verify(s => s.GuardarCambios(), Times.Once());
        }

        [Test]
        public void ExecuteTransmisionNula()
        {
            servRepositorio.Setup(s => s.Obtener(It.IsAny<Expression<Func<InformarCupoTransmisionASap, bool>>>()))
                           .Returns((InformarCupoTransmisionASap)null);

            var result = target.Ejecutar(comando);
            Assert.NotNull(result);
            Assert.False(result.HayErrores);
            servRepositorio.Verify(s => s.Agregar(It.IsAny<InformarCupoTransmisionASap>()), Times.Once());
            servRepositorio.Verify(s => s.GuardarCambios(), Times.Once());
        }
    }
}
