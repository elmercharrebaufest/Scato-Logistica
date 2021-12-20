using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarRecorridoPagaTicketMunicipalTest
    {
        private ProcesadorModificarRecorridoPagaTicketMunicipal target;
        private Mock<IRepositorio> repositorio;
        private Mock<IConversor> conversor;
        private NullLogger log;
        private ModificarRecorridoPagaTicketMunicipal comando;
        private Guid instanceId;
        [SetUp]
        public void SetUp()
        {
            instanceId = Guid.NewGuid();
            repositorio = new Mock<IRepositorio>();
            conversor = new Mock<IConversor>();
            log = new NullLogger();
            comando = new ModificarRecorridoPagaTicketMunicipal{LogExceptuadosTicketMunicipalDto = new LogExceptuadosTicketMunicipalDto{InstanceId = instanceId , MaterialId = 1,Motivo = "Motivo",PagaTicketMunicipal = true}};
            repositorio.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                       .Returns(new Recorrido
                           {
                               InstanciaWorkflow = instanceId,
                               Centro = new Centro {Id = 1},
                               Material = new Material {Id = 1, Descripcion = "M"}
                           });
            target = new ProcesadorModificarRecorridoPagaTicketMunicipal(repositorio.Object,conversor.Object,log);
        }

        [Test]
        public void Ejecutar()
        {
            var result = target.Ejecutar(comando) as ResultadoCrear;

            Assert.NotNull(result);
            Assert.False(result.HayErrores);
            repositorio.Verify(s => s.GuardarCambios(),Times.Once());
        }

        [Test]
        public void EjecutarException()
        {
            repositorio.Setup(s => s.Agregar(It.IsAny<LogExceptuadosTicketMunicipal>())).Throws(new Exception("Error"));
            var result = target.Ejecutar(comando) as ResultadoCrear;

            Assert.NotNull(result);
            Assert.True(result.HayErrores);
            Assert.AreEqual(result.Errores.FirstOrDefault().Value,"Error");
            repositorio.Verify(s => s.GuardarCambios(),Times.Never());
        }
    }
}
