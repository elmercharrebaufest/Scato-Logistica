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
    public class ProcesadorModificarClienteTest
    {
        private ProcesadorModificarCliente target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ClienteDto tipoDto;
        private Cliente tipo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarCliente(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ClienteDto
            {
                Id = 1,
                Descripcion = "test",
                Activo = true,
                Bloqueado = false,
                CodigoSap = "202020",
                Cuit = "20-20202020-8",
                Direccion = "",
                Localidad = "",
                Provincia = ""

            };
            tipo = new Cliente
            {
                Id = 1,
                Descripcion = "test",
                Activo = true,
                Bloqueado = true,
                CodigoSap = "202020",
                Cuit = "20-20202020-8",
                Direccion = "",
                Localidad = "",
                Provincia = ""
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            var tiposExistentes = new List<Cliente>
                {
                    new Cliente
                        {
                            Id = 1,
                            Descripcion = "test",
                            Activo = true,
                            Bloqueado = true,
                            CodigoSap = "202020",
                            Cuit = "20-20202020-8",
                            Direccion = "",
                            Localidad = "",
                            Provincia = ""
                        },
                };

            repositorioMock.Setup(s => s.Obtener<Cliente>(It.IsAny<int>())).Returns(tipo);


            var comando = new ModificarCliente { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
