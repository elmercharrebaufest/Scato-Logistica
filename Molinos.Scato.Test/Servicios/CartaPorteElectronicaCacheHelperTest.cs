using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Helpers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Servicios
{
    [TestFixture]
    public class CartaPorteElectronicaCacheHelperTest
    {
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();

            // Simula el comportamiento real de AutoMapper (Mapper.Map(entrada, destino)) según
            // CartaPorteElectronicaMappingProfile: copia por reflection todas las propiedades de
            // entrada hacia destino, EXCEPTO Id y Pdf (ambas con .Ignore() en el profile real —
            // Pdf se resuelve aparte porque AFIP no siempre lo devuelve en cada consulta).
            var propiedadesIgnoradasPorElProfile = new[] { nameof(CartaPorteElectronica.Id), nameof(CartaPorteElectronica.Pdf) };

            conversorMock
                .Setup(c => c.Convertir(It.IsAny<CartaPorteElectronica>(), It.IsAny<CartaPorteElectronica>()))
                .Returns<CartaPorteElectronica, CartaPorteElectronica>((origen, destino) =>
                {
                    foreach (var propiedad in typeof(CartaPorteElectronica).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (propiedadesIgnoradasPorElProfile.Contains(propiedad.Name))
                            continue;

                        propiedad.SetValue(destino, propiedad.GetValue(origen));
                    }

                    return destino;
                });
        }

        [Test]
        public void CuandoNoExisteCPECacheada_Inserta()
        {
            repositorioMock.Setup(r => r.Obtener<CartaPorteElectronica>(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns((CartaPorteElectronica)null);

            var cpe = new CartaPorteElectronica { NroCTG = 111, Estado = "AC", FechaCacheado = DateTime.Now };

            CartaPorteElectronicaCacheHelper.Registrar(repositorioMock.Object, conversorMock.Object, cpe);

            repositorioMock.Verify(r => r.Agregar(cpe), Times.Exactly(1));
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void CuandoExisteCPECacheadaYNoCambioNingunValor_ConservaLaFechaCacheadoAnterior()
        {
            var fechaCacheadoOriginal = DateTime.Now.AddDays(-3);
            var cpeCacheada = new CartaPorteElectronica
            {
                Id = 1,
                NroCTG = 111,
                Estado = "AC",
                Dominio = "AAA111",
                PesoBruto = 30000,
                FechaCacheado = fechaCacheadoOriginal
            };

            var cpeDesdeAfip = new CartaPorteElectronica
            {
                NroCTG = 111,
                Estado = "AC",
                Dominio = "AAA111",
                PesoBruto = 30000
            };

            repositorioMock.Setup(r => r.Obtener<CartaPorteElectronica>(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns(cpeCacheada);

            CartaPorteElectronicaCacheHelper.Registrar(repositorioMock.Object, conversorMock.Object, cpeDesdeAfip);

            Assert.That(cpeCacheada.FechaCacheado, Is.EqualTo(fechaCacheadoOriginal));
        }

        [Test]
        public void CuandoExisteCPECacheadaYCambioElEstado_ActualizaLaFechaCacheado()
        {
            var fechaCacheadoOriginal = DateTime.Now.AddDays(-3);
            var cpeCacheada = new CartaPorteElectronica
            {
                Id = 1,
                NroCTG = 111,
                Estado = "AC",
                Dominio = "AAA111",
                PesoBruto = 30000,
                FechaCacheado = fechaCacheadoOriginal
            };

            var cpeDesdeAfip = new CartaPorteElectronica
            {
                NroCTG = 111,
                Estado = "RE",
                Dominio = "AAA111",
                PesoBruto = 30000
            };

            repositorioMock.Setup(r => r.Obtener<CartaPorteElectronica>(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns(cpeCacheada);

            CartaPorteElectronicaCacheHelper.Registrar(repositorioMock.Object, conversorMock.Object, cpeDesdeAfip);

            Assert.That(cpeCacheada.FechaCacheado, Is.GreaterThan(fechaCacheadoOriginal));
        }

        [Test]
        public void CuandoExisteCPECacheadaYCambioUnValorDistintoDelEstado_ActualizaLaFechaCacheado()
        {
            var fechaCacheadoOriginal = DateTime.Now.AddDays(-3);
            var cpeCacheada = new CartaPorteElectronica
            {
                Id = 1,
                NroCTG = 111,
                Estado = "AC",
                Dominio = "AAA111",
                PesoBruto = 30000,
                FechaCacheado = fechaCacheadoOriginal
            };

            var cpeDesdeAfip = new CartaPorteElectronica
            {
                NroCTG = 111,
                Estado = "AC",
                Dominio = "AAA111",
                PesoBruto = 32000 // cambio el peso bruto informado por AFIP, el Estado se mantuvo igual
            };

            repositorioMock.Setup(r => r.Obtener<CartaPorteElectronica>(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns(cpeCacheada);

            CartaPorteElectronicaCacheHelper.Registrar(repositorioMock.Object, conversorMock.Object, cpeDesdeAfip);

            Assert.That(cpeCacheada.FechaCacheado, Is.GreaterThan(fechaCacheadoOriginal));
        }

        [Test]
        public void CuandoAfipDevuelveUnPdfNuevo_DebeActualizarElPdfEnElCache()
        {
            // Regresión PR 5099: se perdió el `if (pdfNuevo != null) cartaPorte.Pdf = pdfNuevo`
            // al extraer este mapeo desde ProcesadorConsultarCPDigital.ActualizarCartaPorteElectronica.
            var pdfViejo = new byte[] { 1, 2, 3 };
            var pdfNuevo = new byte[] { 9, 8, 7 };

            var cpeCacheada = new CartaPorteElectronica { Id = 1, NroCTG = 111, Estado = "AC", Pdf = pdfViejo };
            var cpeDesdeAfip = new CartaPorteElectronica { NroCTG = 111, Estado = "AC", Pdf = pdfNuevo };

            repositorioMock.Setup(r => r.Obtener<CartaPorteElectronica>(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns(cpeCacheada);

            CartaPorteElectronicaCacheHelper.Registrar(repositorioMock.Object, conversorMock.Object, cpeDesdeAfip);

            Assert.That(cpeCacheada.Pdf, Is.EqualTo(pdfNuevo), "El Pdf cacheado debe refrescarse cuando AFIP devuelve un Pdf nuevo");
        }

        [Test]
        public void CuandoAfipNoDevuelvePdf_DebePreservarElPdfCacheado()
        {
            var pdfViejo = new byte[] { 1, 2, 3 };

            var cpeCacheada = new CartaPorteElectronica { Id = 1, NroCTG = 111, Estado = "AC", Pdf = pdfViejo };
            var cpeDesdeAfip = new CartaPorteElectronica { NroCTG = 111, Estado = "AC", Pdf = null };

            repositorioMock.Setup(r => r.Obtener<CartaPorteElectronica>(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns(cpeCacheada);

            CartaPorteElectronicaCacheHelper.Registrar(repositorioMock.Object, conversorMock.Object, cpeDesdeAfip);

            Assert.That(cpeCacheada.Pdf, Is.EqualTo(pdfViejo), "Si la respuesta de AFIP no trae Pdf, debe preservarse el Pdf ya cacheado");
        }

        [Test]
        public void CuandoExisteCPECacheadaYNoCambioNingunValor_ActualizaSiempreLaFechaUltimaActualizacion()
        {
            var cpeCacheada = new CartaPorteElectronica
            {
                Id = 1,
                NroCTG = 111,
                Estado = "AC",
                FechaUltimaActualizacion = DateTime.Now.AddDays(-3)
            };

            var cpeDesdeAfip = new CartaPorteElectronica
            {
                NroCTG = 111,
                Estado = "AC"
            };

            repositorioMock.Setup(r => r.Obtener<CartaPorteElectronica>(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns(cpeCacheada);

            CartaPorteElectronicaCacheHelper.Registrar(repositorioMock.Object, conversorMock.Object, cpeDesdeAfip);

            Assert.That(cpeCacheada.FechaUltimaActualizacion, Is.Not.Null);
            Assert.That(cpeCacheada.FechaUltimaActualizacion.Value, Is.GreaterThan(DateTime.Now.AddMinutes(-1)));
        }
    }
}
