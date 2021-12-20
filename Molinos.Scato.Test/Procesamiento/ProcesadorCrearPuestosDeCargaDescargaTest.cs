using System;
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
    public class ProcesadorCrearPuestosDeCargaDescargaTest
    {
        private ProcesadorCrearPuestosDeCargaDescarga target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private PuestosDeCargaDescargaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearPuestosDeCargaDescarga(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new PuestosDeCargaDescargaDto()
            {
                Id = 1,
                CentroId = 1,
                Codigo = "H1",
                Nombre = "Hidraulica 1",
                PuestoDeTrabajoId = 1
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearPuestosDeCargaDescarga() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<PuestosDeCargaDescarga>(o => o.Codigo == tipoDto.Codigo)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorCodigo()
        {
            var tiposExistentes = new List<PuestosDeCargaDescarga>
                {
                    new PuestosDeCargaDescarga() {Id = 5, Codigo = "H1", Centro = new Centro(){Id = 1}},
                    new PuestosDeCargaDescarga() {Id = 6, Codigo = "H2", Centro = new Centro(){Id = 1}}
                };
            var puestos = new List<PuestoDeTrabajo>
                {
                    new PuestoDeTrabajo() {Id = 5, NombrePuesto= "PT1"},
                    new PuestoDeTrabajo() {Id = 6, NombrePuesto= "PT2"}
                };
            var lectoras = new List<Lector>
                {
                    new Lector() {Id = 5, Descripcion= "L1"},
                    new Lector() {Id = 6, Descripcion= "L2"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<PuestosDeCargaDescarga, bool>>>()))
                    .Returns<Expression<Func<PuestosDeCargaDescarga, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                    .Returns<Expression<Func<PuestoDeTrabajo, bool>>>(q => puestos.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Lector, bool>>>()))
                    .Returns<Expression<Func<Lector, bool>>>(q => lectoras.Any((q.Compile())));

            tipoDto.Codigo = "H1";
            tipoDto.CentroId = 1;
            var comando = new CrearPuestosDeCargaDescarga() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<PuestosDeCargaDescarga>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Codigo"));
        }
    }
}
