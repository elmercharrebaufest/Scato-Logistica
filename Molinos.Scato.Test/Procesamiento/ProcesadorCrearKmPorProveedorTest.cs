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
    public class ProcesadorCrearKmPorProveedorTest
    {
        private ProcesadorCrearKmPorProveedor target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private KmPorProveedorDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearKmPorProveedor(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new KmPorProveedorDto
            {
                Id = 1,
                CentroId = 1,
                ClienteId = 1,
                ClienteDescripcion = "Cliente 1",
                CentroDescripcion = "Centro 1",
                KmARecorrer = "400",
                LocalidadId = 1,
                ProvinciaId = 1,
                LocalidadDescripcion = "Localidad 1",
                ProvinciaDescripcion = "Provincia 1"
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Existe<KmPorProveedor>(x => x.Id == It.IsAny<int>()));
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(new Centro());
            repositorioMock.Setup(s => s.Obtener<Cliente>(It.IsAny<int>())).Returns(new Cliente());
            repositorioMock.Setup(s => s.Obtener<Localidad>(It.IsAny<int>())).Returns(new Localidad());

            var comando = new CrearKmPorProveedor { Dto = tipoDto, Usuario = "usuario 1" };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<KmPorProveedor>(x => x.Id == 0)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}