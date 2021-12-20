using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    class ProcesadorCrearCentroTest
    {
        private ProcesadorCrearCentro target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private CentroDto dto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;

            target = new ProcesadorCrearCentro(repositorioMock.Object, conversorMock, new NullLogger());
            
        }

        [Test]
        public void TestCrearEntidad()
        {
            dto = new CentroDto
            {
                Id = 4,
                Descripcion = "Centro 1",
                CodigoDeChamico = 2,
                CodigoSAP = "sap1",
                CamaraId = 3,
                ProvinciaId = 2,
                LocalidadId = 1
            };

            var provincia = new Provincia(){ Id = 2 };
            var camara = new Camara(){ Id = 3};
            var localidad = new Localidad(){ Id = 1 };
            var usuario = new List<Usuario>() { new Usuario() {NombreUsuario = "User1"} };

            var comando = new CrearCentro() { Dto = dto, NombreUsuario = "User1", Usuario = "User1"};

            repositorioMock.Setup(s => s.Obtener<Camara>(3)).Returns(camara);
            repositorioMock.Setup(s => s.Obtener<Provincia>(2)).Returns(provincia);
            repositorioMock.Setup(s => s.Obtener<Localidad>(1)).Returns(localidad);
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuario);
            
            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.Obtener<Camara>(comando.Dto.CamaraId), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Localidad>(comando.Dto.LocalidadId), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Provincia>(comando.Dto.ProvinciaId), Times.Exactly(1));
            repositorioMock.Verify(s => s.Listar(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Exactly(1));

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalido()
        {
            dto = new CentroDto
            {
                Id = 4,
                Descripcion = "Centro 1",
                CodigoDeChamico = 2,
                CodigoSAP = "sap1",
                CamaraId = 3,
                ProvinciaId = 2,
                LocalidadId = 1
            };

            var provincia = new Provincia() { Id = 2 };
            var camara = new Camara() { Id = 3 };
            var localidad = new Localidad() { Id = 1 };
            var usuario = new List<Usuario>() { new Usuario() { NombreUsuario = "User1" } };

            var comando = new CrearCentro() { Dto = dto, NombreUsuario = "User1" };

            repositorioMock.Setup(s => s.Obtener<Camara>(3)).Returns(camara);
            repositorioMock.Setup(s => s.Obtener<Provincia>(2)).Returns(provincia);
            repositorioMock.Setup(s => s.Obtener<Localidad>(1)).Returns(localidad);
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuario);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(true);

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.Obtener<Camara>(comando.Dto.CamaraId), Times.Exactly(0));
            repositorioMock.Verify(s => s.Obtener<Localidad>(comando.Dto.LocalidadId), Times.Exactly(0));
            repositorioMock.Verify(s => s.Obtener<Provincia>(comando.Dto.ProvinciaId), Times.Exactly(0));
            repositorioMock.Verify(s => s.Listar(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.Existe(It.IsAny<Expression<Func<Centro, bool>>>()), Times.Exactly(1));

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}