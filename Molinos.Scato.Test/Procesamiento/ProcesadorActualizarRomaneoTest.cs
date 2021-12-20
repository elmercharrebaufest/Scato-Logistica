using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    public class ProcesadorActualizarRomaneoTest
    {
        private ProcesadorActualizarRomaneo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ZSDWS_SCATO> servicioSapMock;
        private IConversor conversor;
        private Romaneo romaneo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            servicioSapMock = new Mock<ZSDWS_SCATO>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorActualizarRomaneo(repositorioMock.Object, conversor, new NullLogger(), servicioSapMock.Object);
            romaneo = new Romaneo
                {
                    Id = 1
                };
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Material, bool>>>())).Returns(new List<Material> { new Material { CodigoSAP = "123" } });
        }

        [Test]
        public void TestEjecutarModificacion()
        {
            servicioSapMock.Setup(x => x.ConsultaPedido(It.IsAny<ConsultaPedidoRequest>()))
                          .Returns(new ConsultaPedidoResponse1(new ConsultaPedidoResponse { Detalles = new[] { new ZMMBALANZA2 { MATNR = "123", CANT_PEDIDO = "1", CANT_RECIBIDA = "0", CHARG = "1", EBELP = "00001", FEC_ENTREGA = DateTime.Now.ToString(), MAKTX = "" } } }));

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Romaneo, bool>>>())).Returns(romaneo);
            var comando = new ActualizarRomaneo { Dto = new RomaneoDto() };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Romaneo>(r => r.Id == romaneo.Id)), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestEjecutarAlta()
        {
            servicioSapMock.Setup(x => x.ConsultaPedido(It.IsAny<ConsultaPedidoRequest>()))
                          .Returns(new ConsultaPedidoResponse1(new ConsultaPedidoResponse { Proveedor = "asd",Detalles = new[] { new ZMMBALANZA2 { MATNR = "123", CANT_PEDIDO = "1", CANT_RECIBIDA = "0", CHARG = "1", EBELP = "00001", FEC_ENTREGA = DateTime.Now.ToString(), MAKTX = "" } } }));

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Romaneo, bool>>>())).Returns((Romaneo)null);
            repositorioMock.Setup(s => s.Obtener<Proveedor>(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor());
            var comando = new ActualizarRomaneo { Dto = new RomaneoDto() };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Romaneo>(r => r.Id == 0)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        
    }
}
