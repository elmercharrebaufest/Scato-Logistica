using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorRechazarRomaneoItemTest
    {
        private ProcesadorRechazarRomaneoItem target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private Romaneo romaneo;
        private RomaneoItemDto itemDto;
        private Mock<ZSDWS_SCATO> servicioSapMock;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            servicioSapMock = new Mock<ZSDWS_SCATO>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorRechazarRomaneoItem(repositorioMock.Object, conversor, new NullLogger(), servicioSapMock.Object);
            romaneo = new Romaneo
                {
                    Id = 1,
                    RomaneoItemsPedidos = new List<RomaneoItemPedido>{new RomaneoItemPedido{ Material = new Material{Id = 1}}}
                };

            itemDto = new RomaneoItemDto
                {
                    Id = 1,
                    ItemNro = 1,
                    Fecha = new DateTime(),
                    PesoBruto = 100,
                    PesoTara = 200,
                    LoteProveedor = "lote",
                    MaterialId = 1
                };


        }

        [Test]
        public void TestEjecutar()
        {
            servicioSapMock.Setup(x => x.AnulaContabilizacion(It.IsAny<AnulaContabilizacionRequest>()))
                          .Returns(new AnulaContabilizacionResponse1(new AnulaContabilizacionResponse { DocMaterial = "asd", EjercicioDocMaterial = "qwe" }));
            repositorioMock.Setup(s => s.Obtener<RomaneoItem>(It.IsAny<int>())).Returns(new RomaneoItem{ DocMaterial = "asd", EjercicioDocMaterial = "qwe"});
            repositorioMock.Setup(s => s.Obtener<Romaneo>(It.IsAny<int>())).Returns(romaneo);
            var comando = new RechazarRomaneoItem { Dto = new List<RomaneoItemDto> { itemDto } };
            var resultado = target.Ejecutar(comando);
            
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
