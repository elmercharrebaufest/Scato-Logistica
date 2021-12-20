using System;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
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
    public class ProcesadorModificarRecorridoBalanzaBrutoTest
    {
        private ProcesadorModificarRecorridoBalanzaBruto target;
        private Mock<IRepositorio> servRepositorio;
        private Mock<IConversor> conversor;
        private NullLogger log;
        private Recorrido recorrido;
        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IRepositorio>();
            conversor = new Mock<IConversor>();
            log = new NullLogger();
            target = new ProcesadorModificarRecorridoBalanzaBruto(servRepositorio.Object, conversor.Object, log);

            recorrido = new Recorrido{BalanzaBruto = new Balanza()};
            servRepositorio.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(recorrido);
        }

        [Test]
        public void Execute()
        {
            var resultado = target.Ejecutar(new ModificarRecorridoBalanzaBruto());

            Assert.NotNull(resultado);
            Assert.Null(recorrido.BalanzaBruto);
        }

        [Test]
        public void ExecuteException()
        {
            servRepositorio.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Throws(new Exception("Excepcion"));

            var resultado = target.Ejecutar(new ModificarRecorridoBalanzaBruto());

            Assert.NotNull(resultado);
            Assert.True(resultado.HayErrores);
            Assert.AreEqual(resultado.Errores.Values.FirstOrDefault(), "Ha ocurrido un error. Por favor intente nuevamente más tarde.");
        }

    }
}
