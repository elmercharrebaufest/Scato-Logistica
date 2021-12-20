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
    public class ProcesadorCrearTransaccionSAPTest
    {
        private ProcesadorCrearTransaccionSAP target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TransaccionSAPDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearTransaccionSAP(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TransaccionSAPDto
            {
                Id = 1,
                DescripcionCorta = "Tr 1",
                CentroOrigenId = "1",
                CentroOrigenDesc = "Centro Origen",
                MaterialId = 1,
                TipoComercialId = 1,
                FuncionSAP = FuncionSAP.SalidaDeOrigenEnRedespachos
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearTransaccionSAP { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TransaccionSAP>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalido()
        {
            var tiposExistentes = new List<TransaccionSAP>
                {
                    new TransaccionSAP {Id = 5, CentroOrigen = new Centro{Id = 1}, Material = new Material{Id = 1}, TipoComercial = new TipoComercial{Id = 1}, FuncionSAP = FuncionSAP.SalidaDeOrigenEnRedespachos},
                    new TransaccionSAP {Id = 6, CentroOrigen = new Centro{Id = 3}, Material = new Material{Id = 2}, TipoComercial = new TipoComercial{Id = 2}},
               };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TransaccionSAP, bool>>>()))
                    .Returns<Expression<Func<TransaccionSAP, bool>>>(q => tiposExistentes.Any((q.Compile())));

            var comando = new CrearTransaccionSAP { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TransaccionSAP>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo(""));
        }
    }
}
