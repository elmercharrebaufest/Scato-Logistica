using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
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
    public class ProcesadorCrearSugerenciaTest
    {
        private ProcesadorCrearSugerencia target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversorMock;
        private SugerenciaDto dto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearSugerencia(repositorioMock.Object, conversorMock, new NullLogger());
            dto = new SugerenciaDto()
            {
                NombreUsuario = "User 1",
                CentroId = 1,
                Fecha = DateTime.Now,
                TextoSugerencia = "Sugerencia 1",
                Url = "localhost/Index"
            };
        }
        
        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearSugerencia() { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Sugerencia>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearInvalido()
        {
            var comando = new CrearSugerencia {Dto = dto};
            var resultado = target.Ejecutar(comando);
            resultado.Errores.Add("Error1", "Error1");
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Sugerencia>()), Times.Once());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Once());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Error1"));
        }
    }
}