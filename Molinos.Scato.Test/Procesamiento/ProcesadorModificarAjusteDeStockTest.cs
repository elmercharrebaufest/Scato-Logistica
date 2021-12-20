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
    public class ProcesadorModificarAjusteDeStockTest
    {
        private ProcesadorModificarAjusteDeStock target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private AjusteDeStockDto tipoDto;
        private AjusteDeStock tipo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarAjusteDeStock(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new AjusteDeStockDto()
                {
                    Id = 1,
                    CentroId = 1,
                    Fecha = DateTime.Now,
                    MaterialId = 2,
                    PesoBrutoIngreso = 0,
                    PesoNetoEgreso = 0,
                    PesoNetoIngreso = 1,
                    TipoComprobanteOnccaId = 1,
                    NumeroDocumentoIngreso = "1234-12345678"
                };
            tipo = new AjusteDeStock()
                {
                    Id = 1,
                    Centro = new Centro(){Id = 1, Descripcion = "Centro1"},
                    Fecha = DateTime.Now,
                    Material = new Material(){Id = 2, Descripcion = "Material1"},
                    PesoBrutoIngreso = 0,
                    PesoNetoEgreso = 0,
                    PesoNetoIngreso = 1,
                    TipoComprobanteOncca = new TipoComprobanteOncca(){Id = 1, Descripcion = "Comprobante1"},
                    NumeroDocumentoIngreso = "1234-12345678"
                };
        }

        [Test]
        public void TestModificarEntidad()
        {
            var tiposExistentesDoc = new List<Material>
                {
                    new Material() {Id = 1, Descripcion = "Material1"},
                };
            var tiposExistentes = new List<AjusteDeStock>
                {
                    new AjusteDeStock
                        {
                            Id = 1,
                            Centro = new Centro(){Id = 1, Descripcion = "Centro1"},
                            Fecha = DateTime.Now,
                            Material = new Material(){Id = 1, Descripcion = "Material1"},
                            PesoBrutoIngreso = 0,
                            PesoNetoEgreso = 0,
                            PesoNetoIngreso = 1,
                            TipoComprobanteOncca = new TipoComprobanteOncca(){Id = 1, Descripcion = "Comprobante1"},
                            NumeroDocumentoIngreso = "1234-12345678"
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<AjusteDeStock, bool>>>()))
                    .Returns<Expression<Func<AjusteDeStock, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                    .Returns<Expression<Func<Material, bool>>>(q => tiposExistentesDoc.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<AjusteDeStock>(It.IsAny<int>())).Returns(tipo);
            
            var comando = new ModificarAjusteDeStock() {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}