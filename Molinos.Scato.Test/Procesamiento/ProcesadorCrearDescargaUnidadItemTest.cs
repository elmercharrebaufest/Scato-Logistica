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
    public class ProcesadorCrearDescargaUnidadItemTest
    {
        private ProcesadorCrearDescargaUnidadItem target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private DescargaUnidad descargaUnidad;
        private DescargaUnidadItemDto itemDto;
        private Mock<ZSDWS_SCATO> servicioSapMock;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            servicioSapMock = new Mock<ZSDWS_SCATO>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearDescargaUnidadItem(repositorioMock.Object, conversor, new NullLogger(), servicioSapMock.Object);
            descargaUnidad = new DescargaUnidad()
                {
                    Id = 1,
                    DescargaUnidadItemPedidos = new List<DescargaUnidadItemPedido>{new DescargaUnidadItemPedido(){ Material = new Material{Id = 1}}}
                };

            itemDto = new DescargaUnidadItemDto()
                {
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
            servicioSapMock.Setup(x => x.ContabilizarIngresos(It.IsAny<ContabilizarIngresosRequest>()))
                          .Returns(new ContabilizarIngresosResponse1(new ContabilizarIngresosResponse { DocMaterial = "asd", EjercicioDocMaterial = "qwe"}));

            repositorioMock.Setup(s => s.Obtener<DescargaUnidad>(It.IsAny<int>())).Returns(descargaUnidad);
            var comando = new CrearDescargaUnidadItem() { Dto = itemDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<DescargaUnidadItem>(r => r.ItemNro == itemDto.ItemNro)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
