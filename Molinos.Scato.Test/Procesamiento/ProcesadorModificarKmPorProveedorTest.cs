using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class ProcesadorModificarKmPorProveedorTest
    {
        private ProcesadorModificarKmPorProveedor target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private KmPorProveedorDto tipoDto;
        private KmPorProveedor tipo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarKmPorProveedor(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipo = new KmPorProveedor
            {
                Id = 1,
                KmARecorrer = "400",
                Centro = new Centro
                {
                    Id = 1,
                    Descripcion = "Centro 1"
                },
                Cliente = new Cliente
                {
                    Id = 1,
                    Descripcion = "Cliente 1"
                },
                Localidad = new Localidad
                {
                    Id = 1,
                    Descripcion = "Localidad 1",
                    Provincia = new Provincia
                    {
                        Id = 1,
                        Descripcion = "Provincia 1"
                    }
                }
            };
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
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Existe<Cliente>(x => x.Id == It.IsAny<int>()))
                           .Returns(false);
            repositorioMock.Setup(s => s.Existe<Localidad>(x => x.Id == It.IsAny<int>()))
                           .Returns(false);
            repositorioMock.Setup(s => s.Existe<Provincia>(x => x.Id == It.IsAny<int>()))
                           .Returns(false);
            repositorioMock.Setup(s => s.Existe<Centro>(x => x.Id == It.IsAny<int>()))
                           .Returns(false);
            repositorioMock.Setup(s => s.Obtener<KmPorProveedor>(1))
                           .Returns(tipo);

            var comando = new ModificarKmPorProveedor { Dto = tipoDto, Usuario = "User 1" };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
