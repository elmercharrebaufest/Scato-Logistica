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
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    class ProcesadorModificarAjusteStockBinesTest
    {
        private ProcesadorModificarAjusteStockBines target;
        private Mock<IRepositorio> servRepositorio;
        private IConversor conversor;
        private AjusteStockBinesDto dto;
        private AjusteStockBines tipo;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarAjusteStockBines(servRepositorio.Object, conversor, new NullLogger());
            dto = new AjusteStockBinesDto
            {
                Id = 1,
                CentroId = 1,
                CentroDescripcion = "C",
                Stock = 2,
                Fecha = new DateTime(2015, 5, 5),
                MaterialId = 2,
                Observaciones = "Obs",
                ProveedorId = 0
            };

            tipo = new AjusteStockBines
            {
                Id = 1,
                Centro =  new Centro{Id = 1,Descripcion = "C"},
                Stock = 2,
                Fecha = new DateTime(2015, 5, 5),
                Material = new Material{Id = 1},
                Observaciones = "Obs",
                Proveedor = new Proveedor{Id = 0}
                
            };

            servRepositorio.Setup(s => s.Obtener<Centro>(It.Is<int>(i => i == dto.CentroId))).Returns(new Centro { Id = (int)(dto.CentroId) });
            servRepositorio.Setup(s => s.Obtener<Proveedor>(It.Is<int>(i => i == dto.ProveedorId)))
                           .Returns(new Proveedor { Id = 0 });
            servRepositorio.Setup(s => s.Obtener<Material>(It.Is<int>(i => i == dto.MaterialId))).Returns(new Material { Id = (dto.MaterialId) });
            servRepositorio.Setup(s => s.Existe<Material>(material => material.Id != It.IsAny<int>())).Returns(true);
        }

        [Test]
        public void TestModificarEntidad()
        {
            var tiposExistentesDoc = new List<Material>
                {
                    new Material {Id = 1, Descripcion = "Material1"},
                };

            var tiposExistentes = new List<AjusteStockBines>
                {
                    new AjusteStockBines
                        {
                            Id = 1,
                            Centro = new Centro{Id = 1, Descripcion = "Centro1"},
                            Fecha = DateTime.Now,
                            Material = new Material{Id = 1, Descripcion = "Material1"},
                        },
                };

            servRepositorio.Setup(s => s.Existe(It.IsAny<Expression<Func<AjusteStockBines, bool>>>()))
                    .Returns<Expression<Func<AjusteStockBines, bool>>>(q => tiposExistentes.Any((q.Compile())));
            servRepositorio.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                    .Returns<Expression<Func<Material, bool>>>(q => tiposExistentesDoc.Any((q.Compile())));
            servRepositorio.Setup(s => s.Obtener<AjusteStockBines>(It.IsAny<int>())).Returns(tipo);
            

            var comando = new ModificarAjusteStockBines { Dto = dto };
            var resultado = target.Ejecutar(comando);
            servRepositorio.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
