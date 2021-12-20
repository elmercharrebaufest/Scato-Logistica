using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarRecorridoPesoNetoBodegaEnLitrosTest
    {
        private ProcesadorModificarRecorridoPesoNetoBodegaEnLitros target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;
        private Recorrido recorrido;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            log = new NullLogger();
            target = new ProcesadorModificarRecorridoPesoNetoBodegaEnLitros(repositorioMock.Object, conversorMock.Object, log);

            recorrido = new Recorrido
            {
                Id = 1,
                Almacen = new Almacen { Id = 1, Centro = new Centro { Id = 1 }, Descripcion = "Almacen 1" },
                Centro = new Centro { Id = 1 },
                Chofer = new Chofer { Id = 1, Nombre = "Chofer" },
                DatosProximaActividad = "Tara",
                InstanciaWorkflow = new Guid("25892e17-80f6-415f-9c65-7395632f0223"),
                Material = new Material { Id = 1, Descripcion = "MaterialDesc 1",FactorConversion = 2},
                NumeroDocumentoIngreso = "1111",
                Patente = "AAA111",
                TipoComercial = new TipoComercial { Id = 1, Descripcion = "Tipo 1" },
                TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                Workflow = new Workflow { Id = 1, Descripcion = "EgresoMaterialNoProductivo" }
            };


        }

        [Test]
        public void TestModificarPesoNetoBodega()
        {
            var comando = new ModificarRecorridoPesoNetoBodegaEnLitros { WorkflowId = new Guid("25892e17-80f6-415f-9c65-7395632f0223"), MaterialId = 1, PesoNetoBodega = 1000 };
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido,bool>>>())).Returns(recorrido);
            var resultado = target.Ejecutar(comando) as ResultadoModificarRecorridoPesoNetoBodegaEnLitros;
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(recorrido.PesoNetoBodegaEnLitros, Is.EqualTo(2000));
        }

        [Test]
        public void TestModificarError()
        {
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<int>())).Throws(new Exception("error"));
            var resultado = target.Ejecutar(new ModificarRecorridoPesoNetoBodegaEnLitros { WorkflowId = new Guid("25892e17-80f6-415f-9c65-7395632f0223"), MaterialId = 1, PesoNetoBodega = 1000 });
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Never());
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Values.Contains(Textos.Error_Generico), Is.EqualTo(true));
        }

    }
}