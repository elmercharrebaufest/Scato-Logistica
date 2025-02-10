using Molinos.Scato.Dominio.Dto;
using NUnit.Framework;
using Molinos.Scato.Dominio.Validations;
using Molinos.Scato.Dominio.Enums;
using System;
using FluentValidation.TestHelper;

namespace Molinos.Scato.Test.Helpers
{
    [TestFixture]
    public class ValidacionCrearOrdenInternaFasonTests
    {
        private OrdenCargaInternaFasonValidator _validator;
        private OrdenCargaInternaFasonDto _ordenCargaValida;

        [SetUp]
        public void Setup()
        {
            _validator = new OrdenCargaInternaFasonValidator();

            _ordenCargaValida = new OrdenCargaInternaFasonDto
            {
                NumeroOrden = "12345678",
                PatenteCamion = "ABC123",
                FechaEmision = DateTime.Now,
                MaterialId = 1,
                ClienteId = 1,
                Cliente = "Cliente Test",
                TipoComercialId = 1,
                NumeroOrdenExterno = "123",
                TipoVehiculo = TipoVehiculo.Camión
            };

        }

        [Test]
        public void CuandoOrdenCargaEsValida_DebeRetornarErrores()
        {

            var result = _validator.Validate(_ordenCargaValida);


            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void CuandoNumeroOrdenEsNullVacio_NoDebeRetornarErrores(string numeroOrden) 
        {
            _ordenCargaValida.NumeroOrden = numeroOrden;

            var result = _validator.Validate(_ordenCargaValida);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        [TestCase("123456789")]
        public void CuandoNumeroOrdenTieneMasDe8Caracteres_DebeRetornarError(string numeroOrden)
        {
            _ordenCargaValida.NumeroOrden = numeroOrden;

            var result = _validator.Validate(_ordenCargaValida);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [Test]
        [TestCase("ERROR")]
        public void CuandoNumeroOrdenTieneCaracteres_DebeRetornarErrorEsNumericoRequerido(string numeroOrden) 
        {
            _ordenCargaValida.NumeroOrden = numeroOrden;

            var result = _validator.Validate(_ordenCargaValida);
                
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void CuandoPatenteCamionEsNuloOVacio_DebeRetornarError(string patente)
        {
            _ordenCargaValida.PatenteCamion = patente;

            var result = _validator.Validate(_ordenCargaValida);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [Test]
        [TestCase("12345678901")] // 11 caracteres, excede el máximo de 10
        public void CuandoPatenteCamionExcedeLongitudMaxima_DebeRetornarError(string patente)
        {
            _ordenCargaValida.PatenteCamion = patente;

            var result = _validator.Validate(_ordenCargaValida);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }


        [Test]
        [TestCase("1234")] // 4 dígitos maximo, válido
        [TestCase("")] // Vacío, válido porque es opcional
        [TestCase(null)] // Nulo, válido porque es opcional
        public void CuandoKmARecorrerEsValidoOpcional_NoDebeRetornarError(string km)
        {
            _ordenCargaValida.KmARecorrer = km;

            var result = _validator.Validate(_ordenCargaValida);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        [TestCase("12345")] // 5 caracteres, excede el máximo de 4
        [TestCase("ABC1")] // No es numérico
        public void CuandoKmARecorrerNoEsValido_DebeRetornarError(string km)
        {
            _ordenCargaValida.KmARecorrer = km;

            var result = _validator.Validate(_ordenCargaValida);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [Test]
        public void CuandoFechaEmisionEsNula_DebeRetornarError()
        {
            _ordenCargaValida.FechaEmision = default;

            var result = _validator.Validate(_ordenCargaValida);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

    }
}
