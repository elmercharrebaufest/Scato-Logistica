using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
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
    public class ProcesadorEliminarTransaccionSAPTest
    {
        private ProcesadorEliminarTransaccionSAP target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TransaccionSAPDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarTransaccionSAP(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TransaccionSAPDto
            {
                Id = 1,
                DescripcionCorta = "Tr 1",
                CentroOrigenId = "1",
                CentroOrigenDesc = "Centro Origen",
                MaterialId = 1,
                TipoComercialId = 1,
                FuncionSAP = FuncionSAP.SalidaDeOrigenEnRedespachos
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarTransaccionSAP { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<TransaccionSAP>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
