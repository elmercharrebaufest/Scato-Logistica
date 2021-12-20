using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServicioImpresion;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorImprimirMuestraAuditoriaTest
    {
        private ProcesadorImprimirMuestraAuditoria target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<IFirmaProvider> firmaProvider;
        private Mock<IServicioImpresorFactory> servicioImpresion;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            firmaProvider = new Mock<IFirmaProvider>();
            servicioImpresion = new Mock<IServicioImpresorFactory>();

            target = new ProcesadorImprimirMuestraAuditoria(repositorioMock.Object, conversorMock.Object, new NullLogger(),firmaProvider.Object, servicioImpresion.Object);

            conversorMock.Setup(
                s =>
                s.Convertir<ImpIdentificacionMuestraAuditoriaDto, ImpIdentificacionMuestraAuditoria>(
                    It.IsAny<ImpIdentificacionMuestraAuditoriaDto>())).Returns(new ImpIdentificacionMuestraAuditoria
                    {
                        Centro = "San Lorenzo",
                        Id = 1,
                        Codigo = "MuestraAuditoria",
                        NroMuestra = "1234",
                        NumeroDeOrden = "12345",
                        Impresora = "Imp1"
                    });
        }

        [Test]
        public void TestEjecutar()
        {
            var res =
                target.Ejecutar(new ImprimirMuestraAuditoria
                    {
                        Dto =
                            new ImpIdentificacionMuestraAuditoriaDto
                                {
                                    Centro = "San Lorenzo",
                                    Id = 1,
                                    Codigo = "MuestraAuditoria",
                                    NroMuestra = "1234",
                                    NumeroDeOrden = "12345",
                                    Impresora = "Imp1"
                                }
                    });

            Assert.NotNull(res);
            Assert.False(res.HayErrores);
        }
    }
}
