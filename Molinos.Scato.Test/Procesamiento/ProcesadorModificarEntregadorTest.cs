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
    public class ProcesadorModificarEntregadorTest
    {
        private ProcesadorModificarEntregador target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Entregador entregador;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarEntregador(repositorioMock.Object, conversorMock.Object, new NullLogger());
            entregador = new Entregador
                {
                    Id = 1,
                    CodigoSAPCondicionFiscal = "1",
                    DescripcionCorta = "Codigo Cero",
                    Cuil = "20-30591238-8",
                    Domicilio = "Domicilio",
                    TipoEntregador = "Tipo",
                    RazonSocial = "Emilio",
                    Tratamiento = "Tratamiento",
                };

            repositorioMock.Setup(s => s.Obtener<Pais>(It.IsAny<int>())).Returns(new Pais());
            repositorioMock.Setup(s => s.Obtener<Provincia>(It.IsAny<int>())).Returns(new Provincia());
            repositorioMock.Setup(s => s.Obtener<Localidad>(It.IsAny<int>())).Returns(new Localidad());
            repositorioMock.Setup(s => s.Obtener<Entregador>(It.IsAny<int>())).Returns(entregador);
        }

        [Test]
        public void TestModificarEntidad()
        {

            var entregadorDto = new EntregadorDto
                {
                    Id = 1,
                    CodigoSAPCondicionFiscal = "1",
                    DescripcionCorta = "Codigo Cero",
                    Cuil = "20-30591238-8",
                    Domicilio = "Domicilio",
                    TipoEntregador = "Tipo",
                    RazonSocial = "Emilio",
                    Tratamiento = "Tratamiento"
                };


            var comando = new ModificarEntregador {Dto = entregadorDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalido()
        {
            var tiposExistentes = new List<Entregador>
                {
                    new Entregador
                        {
                            Id = 1,
                            CodigoSAPCondicionFiscal = "1",
                            DescripcionCorta = "Codigo Cero",
                            Cuil = "20-30591238-8",
                            Domicilio = "Domicilio",
                            TipoEntregador = "Tipo",
                            RazonSocial = "Emilio",
                            Tratamiento = "Tratamiento"
                        },
                    new Entregador
                        {
                            Id = 2,
                            CodigoSAPCondicionFiscal = "2",
                            DescripcionCorta = "Codigo Uno",
                            Cuil = "20-30591238-8",
                            Domicilio = "Domicilio",
                            TipoEntregador = "Tipo",
                            RazonSocial = "Guille",
                            Tratamiento = "Tratamiento"
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Entregador, bool>>>()))
                    .Returns<Expression<Func<Entregador, bool>>>(q => tiposExistentes.Any((q.Compile())));

            var comando = new ModificarEntregador { Dto = new EntregadorDto { DescripcionCorta = tiposExistentes[0].DescripcionCorta, Cuil = tiposExistentes[0].Cuil, RazonSocial = tiposExistentes[0].RazonSocial } };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(3));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("DescripcionCorta"));
        }
    }
}
