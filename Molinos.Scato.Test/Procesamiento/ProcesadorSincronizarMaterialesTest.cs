using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
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
    public class ProcesadorSincronizarMaterialesTest
    {
        private ProcesadorSincronizarMateriales target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<ZSDWS_SCATO> servicioSapMock;
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            servicioSapMock = new Mock<ZSDWS_SCATO>();
            log = new NullLogger();
            target = new ProcesadorSincronizarMateriales(repositorioMock.Object, conversorMock.Object, servicioSapMock.Object, log);
            repositorioMock.Setup(
                s =>
                s.ObtenerMasReciente(It.IsAny<Expression<Func<LogSincronizacion, bool>>>(),
                                     It.IsAny<Expression<Func<LogSincronizacion, DateTime>>>()))
                           .Returns(new LogSincronizacion { FechaEjecucion = new DateTime(2014, 1, 1) });

        }

        [Test]
        public void TestSincronizarCompleta()
        {
            var materialesSap =
                new DatosMaterialesResponse1(new DatosMaterialesResponse
                    { Materiales = new[] {
                        new ZSDES5211 {DESCRIPCION = "Material 1", ID_SAP = "001", MEINS = "KG", WERKS = "1001"},
                        new ZSDES5211 {DESCRIPCION = "Material 2", ID_SAP = "002", MEINS = "KG", WERKS = "1001"},
                        }});

            var materialesSap2 =
                  new DatosMaterialesResponse1(new DatosMaterialesResponse
                  {
                      Materiales = new[] {
                                    new ZSDES5211 {DESCRIPCION = "Material 1", ID_SAP = "001", MEINS = "KG", WERKS = "1002"},
                                    }
                  });
            servicioSapMock.Setup(s => s.DatosMateriales(It.Is<DatosMaterialesRequest>(d => d.DatosMateriales.Centro == "1001")))
                .Returns(materialesSap);
            servicioSapMock.Setup(s => s.DatosMateriales(It.Is<DatosMaterialesRequest>(d => d.DatosMateriales.Centro == "1002")))
                .Returns(materialesSap2);

            LogSincronizacion logS = null;
            repositorioMock.Setup(s => s.Agregar(It.IsAny<LogSincronizacion>()))
                           .Returns<LogSincronizacion>(x => logS = x);
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Centro, bool>>>()))
                           .Returns(new List<Centro> { 
                               new Centro { CodigoSAP = "1001", Descripcion = "Centro 1" }, 
                               new Centro { CodigoSAP = "1002", Descripcion = "Centro 1" } 
                           });

            var resultado = target.Ejecutar(new SincronizarMateriales { CargaMasiva = true, RetornarResultado = false}) as ResultadoSincronizarMateriales;

            repositorioMock.Verify(v => v.Agregar(It.IsAny<Material>()), Times.Exactly(2));
            repositorioMock.Verify(v => v.Agregar(It.IsAny<MaterialPorCentro>()), Times.Exactly(3));

            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.Cantidad, Is.EqualTo(2));
            Assert.That(resultado.Items.Count, Is.EqualTo(0));
            Assert.That(logS.NombreInterface, Is.EqualTo("Materiales"));
            Assert.That(logS.Completa, Is.True);
            Assert.That(logS.Correcta, Is.True);
        }

        [Test]
        public void TestSincronizarSoloMaterialPorCentro()
        {
            var materialesSap =
                new DatosMaterialesResponse1(new DatosMaterialesResponse
                {
                    Materiales = new[] {
                        new ZSDES5211 {DESCRIPCION = "Material 1", ID_SAP = "001", MEINS = "KG", WERKS = "1001"},
                        new ZSDES5211 {DESCRIPCION = "Material 2", ID_SAP = "002", MEINS = "KG", WERKS = "1001"},
                        }
                });

            var materialesSap2 =
                  new DatosMaterialesResponse1(new DatosMaterialesResponse
                  {
                      Materiales = new[] {
                                    new ZSDES5211 {DESCRIPCION = "Material 1", ID_SAP = "001", MEINS = "KG", WERKS = "1002"},
                                    }
                  });
            servicioSapMock.Setup(s => s.DatosMateriales(It.Is<DatosMaterialesRequest>(d => d.DatosMateriales.Centro == "1001")))
                .Returns(materialesSap);
            servicioSapMock.Setup(s => s.DatosMateriales(It.Is<DatosMaterialesRequest>(d => d.DatosMateriales.Centro == "1002")))
                .Returns(materialesSap2);
            LogSincronizacion logS = null;
            repositorioMock.Setup(s => s.Agregar(It.IsAny<LogSincronizacion>()))
                           .Returns<LogSincronizacion>(x => logS = x);

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Material, bool>>>()))
                           .Returns(new Material{Activo = true, CodigoSAP = "001", Descripcion = "Material 1"});
            //repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Centro, bool>>>()))
            //               .Returns(new Centro { CodigoSAP = "1001", Descripcion = "Centro 1" });

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Centro, bool>>>()))
               .Returns(new List<Centro> { 
                               new Centro { CodigoSAP = "1001", Descripcion = "Centro 1" }, 
                               new Centro { CodigoSAP = "1002", Descripcion = "Centro 1" } 
                           });

            var resultado = target.Ejecutar(new SincronizarMateriales() { CargaMasiva = true, RetornarResultado = true }) as ResultadoSincronizarMateriales;

            repositorioMock.Verify(v => v.Agregar(It.IsAny<Material>()), Times.Never());
            repositorioMock.Verify(v => v.Agregar(It.IsAny<MaterialPorCentro>()), Times.Exactly(3));

            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.Cantidad, Is.EqualTo(2));
            Assert.That(resultado.Items.Count, Is.EqualTo(2));
            Assert.That(logS.NombreInterface, Is.EqualTo("Materiales"));
            Assert.That(logS.Completa, Is.True);
            Assert.That(logS.Correcta, Is.True);
        }

        [Test]
        public void TestSincronizarError()
        {
            const string mensajeError = "Error test";
            servicioSapMock.Setup(s => s.DatosMateriales(It.IsAny<DatosMaterialesRequest>()))
                           .Throws(new Exception(mensajeError));
            LogSincronizacion logS = null;
            repositorioMock.Setup(s => s.Agregar(It.IsAny<LogSincronizacion>()))
                           .Returns<LogSincronizacion>(x => logS = x);
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Centro, bool>>>()))
               .Returns(new List<Centro> { 
                               new Centro { CodigoSAP = "1001", Descripcion = "Centro 1" }, 
                               new Centro { CodigoSAP = "1002", Descripcion = "Centro 1" } 
                           });
            var resultado = target.Ejecutar(new SincronizarMateriales { CargaMasiva = true, RetornarResultado = false }) as ResultadoSincronizarMateriales;

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(logS.Correcta, Is.False);
            Assert.That(logS.NombreInterface, Is.EqualTo("Materiales"));
            Assert.That(logS.Mensaje, Is.EqualTo(mensajeError));
        }

    }
}