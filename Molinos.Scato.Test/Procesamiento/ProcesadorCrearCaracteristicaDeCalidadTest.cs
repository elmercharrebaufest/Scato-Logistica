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
    public class ProcesadorCrearCaracteristicaDeCalidadTest
    {
        private ProcesadorCrearCaracteristicaDeCalidad target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private CaracteristicaDeCalidadDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearCaracteristicaDeCalidad(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new CaracteristicaDeCalidadDto
                {
                    Id = 1,
                    Analisis = TipoAnalisis.Calado,
                    CaladoMaximo = 10,
                    CaladoMinimo = 10,
                    CargaEnCalado = false,
                    CodigoSAP = "sd",
                    Descripcion = "f",
                    EsModificable = false,
                    NoObservableEnCalado = false,
                    InspeccionDeCamionesVacios = false,
                    InternoPorObservados = false,
                    DescuentoEnPorcentaje = FormulaDescuento.SinDescuento,
                    Ensayo = "a",
                    MaterialId = 1,
                    UnidadDeMedida = "TON"
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
         
            var comando = new CrearCaracteristicaDeCalidad {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<CaracteristicaDeCalidad>(o => o.Analisis == tipoDto.Analisis)), Times.Exactly(1));
            //repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        
    }
}
