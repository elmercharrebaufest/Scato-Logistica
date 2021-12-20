using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearReasignacionDeTarjetaTest
    {
        private ProcesadorCrearReasignacionDeTarjeta target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private ReasignacionDeTarjetaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearReasignacionDeTarjeta(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new ReasignacionDeTarjetaDto()
                {
                    Id = 1,
                    Fecha = DateTime.Today,
                    NroTarjetaRfidAsignada = "1234567890",
                    NroTarjetaRfidNueva = "9876543210",
                    TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                    UsuarioNombre = "lvillar",
                    NumeroDocumentoIngreso = "1234567890"
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearReasignacionDeTarjeta() {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
