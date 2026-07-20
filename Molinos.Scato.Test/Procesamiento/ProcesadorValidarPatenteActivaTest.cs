using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Validaciones;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorValidarPatenteActivaTest
    {
        private ProcesadorValidarPatenteActiva target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            conversorMock = new Mock<IConversor>();
            repositorioMock = new Mock<IRepositorio>();
            log = new NullLogger();
            target = new ProcesadorValidarPatenteActiva(repositorioMock.Object, conversorMock.Object, log);
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        private void ConfigurarMaxSustituciones(int valor)
        {
            repositorioMock
                .Setup(r => r.ObtenerPrimero<ConfiguracionGeneral>(It.IsAny<Expression<Func<ConfiguracionGeneral, bool>>>()))
                .Returns(new ConfiguracionGeneral { Valor = valor.ToString() });
        }

        private void ConfigurarSinConfiguracion()
        {
            repositorioMock
                .Setup(r => r.ObtenerPrimero<ConfiguracionGeneral>(It.IsAny<Expression<Func<ConfiguracionGeneral, bool>>>()))
                .Returns((ConfiguracionGeneral)null);
        }

        private void ConfigurarPatentesActivas(IList<string> patentes)
        {
            repositorioMock
                .Setup(r => r.Listar<Recorrido, string>(
                    It.IsAny<Expression<Func<Recorrido, string>>>(),
                    It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns(patentes);
        }

        // ─── Tests: cortocircuito ─────────────────────────────────────────────────

        [Test]
        public void TestListaVacia_RetornaSinResolucion()
        {
            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string>() });

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.PatenteActiva, Is.Null);
            repositorioMock.Verify(
                r => r.Listar<Recorrido, string>(
                    It.IsAny<Expression<Func<Recorrido, string>>>()
                    , It.IsAny<Expression<Func<Recorrido, bool>>>()),
                Times.Exactly(0));
        }

        [Test]
        public void TestPatenteNull_RetornaSinResolucion()
        {
            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = null });

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.PatenteActiva, Is.Null);
        }

        [Test]
        public void TestSinConfiguracion_SinCoincidenciaExacta_NoResuelve()
        {
            ConfigurarSinConfiguracion();

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123" } });

            Assert.That(resultado.PatenteActiva, Is.Null);
        }

        [Test]
        public void TestMaxSustitucionesEnCero_SinCoincidenciaExacta_NoResuelve()
        {
            ConfigurarMaxSustituciones(0);

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123" } });

            Assert.That(resultado.PatenteActiva, Is.Null);
        }

        // ─── Tests: sin coincidencias ─────────────────────────────────────────────

        [Test]
        public void TestSinCoincidencias_NoResuelve()
        {
            ConfigurarMaxSustituciones(2);
            ConfigurarPatentesActivas(new List<string> { "XYZ999" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123" } });

            Assert.That(resultado.PatenteActiva, Is.Null);
        }

        [Test]
        public void TestDiferenciaFueraDeTolerancia_NoResuelve()
        {
            ConfigurarMaxSustituciones(1);
            ConfigurarPatentesActivas(new List<string> { "AAA123" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123" } });

            Assert.That(resultado.PatenteActiva, Is.Null);
        }

        [Test]
        public void TestLecturaExacta_DevuelvePatenteConDiferenciaEnCero()
        {
            ConfigurarPatentesActivas(new List<string> { "ABC123" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123" } });

            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.PatenteActiva, Is.EqualTo("ABC123"));
            Assert.That(resultado.Diferencia, Is.EqualTo(0));
        }

        [Test]
        public void TestMultiplesLecturasExactas_MatchenDistintasActivas_DevuelveErrorDeAmbiguedad()
        {
            ConfigurarPatentesActivas(new List<string> { "ABC123", "XYZ999" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123", "XYZ999" } });

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.PatenteActiva, Is.Null);
        }

        [Test]
        public void TestSinRecorridosActivos_NoResuelve()
        {
            ConfigurarMaxSustituciones(2);
            ConfigurarPatentesActivas(new List<string>());

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123" } });

            Assert.That(resultado.PatenteActiva, Is.Null);
        }

        // ─── Tests: resolución única ──────────────────────────────────────────────

        [Test]
        public void TestUnaSolaLectura_CandidatoUnico_Resuelve()
        {
            ConfigurarMaxSustituciones(1);
            ConfigurarPatentesActivas(new List<string> { "AAC123" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123" } });

            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.PatenteActiva, Is.EqualTo("AAC123"));
            Assert.That(resultado.Diferencia, Is.EqualTo(1));
        }

        [Test]
        public void TestDosCandidatosDistintaDiferencia_DevuelveErrorDeAmbiguedad()
        {
            ConfigurarMaxSustituciones(2);
            ConfigurarPatentesActivas(new List<string> { "AAA123", "AAC123" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123" } });

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.PatenteActiva, Is.Null);
        }

        [Test]
        public void TestDosCandidatosUnoFueraDeTolerancia_ResuelveElValido()
        {
            ConfigurarMaxSustituciones(1);
            ConfigurarPatentesActivas(new List<string> { "AAC123", "AAA123" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123" } });

            Assert.That(resultado.PatenteActiva, Is.EqualTo("AAC123"));
        }

        [Test]
        public void TestDosCandidatosConMismoScore_DevuelveErrorDeAmbiguedad()
        {
            ConfigurarMaxSustituciones(1);
            ConfigurarPatentesActivas(new List<string> { "AAC123", "ABA123" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123" } });

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.PatenteActiva, Is.Null);
        }

        // ─── Tests: escenario multi-cámara ────────────────────────────────────────

        [Test]
        public void TestMultiplesCamaras_TodasApuntanAlMismoCandidato()
        {
            ConfigurarMaxSustituciones(2);
            ConfigurarPatentesActivas(new List<string> { "AAC123" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123", "AXC123" } });

            Assert.That(resultado.PatenteActiva, Is.EqualTo("AAC123"));
        }

        [Test]
        public void TestMultiplesCamaras_UnaCamaraLeeExacto_DevuelveCoincidenciaExacta()
        {
            ConfigurarPatentesActivas(new List<string> { "AAC123" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "AAC123", "ABC123" } });

            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.PatenteActiva, Is.EqualTo("AAC123"));
            Assert.That(resultado.Diferencia, Is.EqualTo(0));
        }

        [Test]
        public void TestMultiplesCamaras_DosCandidatosIgualScore_DevuelveErrorDeAmbiguedad()
        {
            ConfigurarMaxSustituciones(2);
            ConfigurarPatentesActivas(new List<string> { "AAC123", "ABD124" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123", "ABD123" } });

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.PatenteActiva, Is.Null);
        }

        [Test]
        public void TestMultiplesCamaras_UnaSolaCamaraBuenaResuelve()
        {
            ConfigurarMaxSustituciones(1);
            ConfigurarPatentesActivas(new List<string> { "AAC123" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "XAC123", "ABC123" } });

            Assert.That(resultado.PatenteActiva, Is.EqualTo("AAC123"));
            Assert.That(resultado.Diferencia, Is.EqualTo(1));
        }

        // ─── Tests: entradas con null ─────────────────────────────────────────────

        [Test]
        public void TestPatenteActivaNullEnLista_SeOmiteEnLaComparacion()
        {
            ConfigurarMaxSustituciones(1);
            ConfigurarPatentesActivas(new List<string> { null, "AAC123" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { "ABC123" } });

            Assert.That(resultado.PatenteActiva, Is.EqualTo("AAC123"));
        }

        [Test]
        public void TestLecturaNullEnComando_SeOmite()
        {
            ConfigurarMaxSustituciones(1);
            ConfigurarPatentesActivas(new List<string> { "AAC123" });

            var resultado = (ResultadoValidarPatenteActiva)target.Ejecutar(
                new ValidarPatenteActiva { Patentes = new List<string> { null, "ABC123" } });

            Assert.That(resultado.PatenteActiva, Is.EqualTo("AAC123"));
        }

        // ─── Tests: CalcularDiferencia ────────────────────────────────────────────

        [Test]
        public void TestCalcularDiferencia_IgualesRetornaCero()
        {
            Assert.That(target.CalcularDiferencia("ABC123", "ABC123"), Is.EqualTo(0));
        }

        [Test]
        public void TestCalcularDiferencia_UnCaracterDistinto()
        {
            Assert.That(target.CalcularDiferencia("ABC123", "AAC123"), Is.EqualTo(1));
        }

        [Test]
        public void TestCalcularDiferencia_DosCaracteresDistintos()
        {
            Assert.That(target.CalcularDiferencia("ABC123", "AAA123"), Is.EqualTo(2));
        }

        [Test]
        public void TestCalcularDiferencia_LongitudDistintaCuentaExtra()
        {
            // El algoritmo es posicional: "ABC123" vs "AB123" desalinea desde la posición 2;
            // difieren en las posiciones 2, 3, 4 y tiene un caracter sobrante -> diff = 4.
            Assert.That(target.CalcularDiferencia("ABC123", "AB123"), Is.EqualTo(4));
        }

        [Test]
        public void TestCalcularDiferencia_AmbosVacios()
        {
            Assert.That(target.CalcularDiferencia(string.Empty, string.Empty), Is.EqualTo(0));
        }

        [Test]
        public void TestCalcularDiferencia_UnVacioUnCaracter()
        {
            Assert.That(target.CalcularDiferencia("A", string.Empty), Is.EqualTo(1));
        }

        [Test]
        public void TestCalcularDiferencia_EjemploDelDiagrama_AAC113()
        {
            Assert.That(target.CalcularDiferencia("ABC123", "AAC113"), Is.EqualTo(2));
        }
    }
}
