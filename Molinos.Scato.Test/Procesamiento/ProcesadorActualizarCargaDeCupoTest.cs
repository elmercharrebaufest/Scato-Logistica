using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorActualizarCargaDeCupoTest
    {
        private ProcesadorActualizarCargaDeCupo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;
        private Recorrido recorrido;
        private Mock<IConfiguracionProvider> config;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            log = new NullLogger();
            config = new Mock<IConfiguracionProvider>();
            target = new ProcesadorActualizarCargaDeCupo(repositorioMock.Object, conversorMock.Object, log, config.Object);
            config.Setup(x => x.AppSettings.Get(It.IsAny<string>())).Returns("1");
            recorrido = new Recorrido
            {
                Id = 1,
                Almacen = new Almacen { Id = 1, Centro = new Centro { Id = 1 }, Descripcion = "Almacen 1" },
                Calle = new Calle { Id = 1, CentroId = 1 },
                PuestosDeCargaDescargas = new Collection<PuestosDeCargaDescarga> { new PuestosDeCargaDescarga { Id = 1, Centro = new Centro { Id = 1 } } },
                BalanzaBruto = new Balanza { Id = 1, Centro = new Centro { Id = 1 } },
                BalanzaTara = new Balanza { Id = 1, Centro = new Centro { Id = 1 } },
                Centro = new Centro { Id = 1, RequiereCupo = true },
                Chofer = new Chofer { Id = 1, Nombre = "Chofer" },
                DatosProximaActividad = "Tara",
                InstanciaWorkflow = new Guid("25892e17-80f6-415f-9c65-7395632f0223"),
                Material = new Material { Id = 1, Descripcion = "MaterialDesc 1" },
                NumeroDocumentoIngreso = "1111",
                Patente = "AAA111",
                TipoComercial = new TipoComercial { Id = 1, Descripcion = "Tipo 1" },
                TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                Workflow = new Workflow { Id = 1, Codigo = "a", Descripcion = "EgresoMaterialNoProductivo" },
                WorkflowDefinicion = new WorkflowDefinicion() { Id = 1 },
                Vehiculo = new Vehiculo { Id = 1, CartaPorte = new CartaPorte { Id = 1, Cupo = "MOV1" } },

            };
        }

        [Test]
        public void TestActualizarCargaDeCupoNoRequiereCupo()
        {
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido
            {
                Id = 1,
                Centro = new Centro
                {
                    Id = 1,
                    RequiereCupo = false,
                },
                InstanciaWorkflow = It.IsAny<Guid>(),
                Vehiculo = new Vehiculo()
            });

            var resultado = target.Ejecutar(new ActualizarCargaDeCupo { InstanceId = new Guid("25892e17-80f6-415f-9c65-7395632f0223"), Numero = "1" });

            Assert.That(resultado.HayErrores, Is.False);
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Never());

        }

        [Test]
        public void TestActualizarCargaDeCupoConCupoYTarjetaDeGarita()
        {
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(recorrido);
            var cupo = new CargaDeCupo
            {
                Numero = "1",
                Recorrido = null,
                Centro = new Centro { Id = 1 },
                Cupo = "MOV1",
                Material = null
            };
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<CargaDeCupo, bool>>>())).Returns(new List<CargaDeCupo> { cupo });
            var resultado = target.Ejecutar(new ActualizarCargaDeCupo { InstanceId = new Guid("25892e17-80f6-415f-9c65-7395632f0223"), Numero = "1" });
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(cupo.Material.Id, Is.EqualTo(1));

            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
        }

        [Test]
        public void TestActualizarCupoPorCartaDePorte()
        {
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(recorrido);
            var cupo = new CargaDeCupo
            {
                Numero = "1",
                Recorrido = null,
                Centro = new Centro { Id = 1 },
                Cupo = null,
                Material = null
            };
            var cargaDecupoPorCartaDePorte = new CargaDeCupo
            {
                Numero = null,
                Cupo = "MOV1", 
                Recorrido = null,
                Material = new Material { Id = 1 },

            };
            repositorioMock.SetupSequence(s => s.Listar(It.IsAny<Expression<Func<CargaDeCupo, bool>>>()))
                .Returns(new List<CargaDeCupo> { cupo })
                .Returns(new List<CargaDeCupo> { cargaDecupoPorCartaDePorte });
            var resultado = target.Ejecutar(new ActualizarCargaDeCupo { InstanceId = new Guid("25892e17-80f6-415f-9c65-7395632f0223"), Numero = "1" });

            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(cargaDecupoPorCartaDePorte.Recorrido.Id, Is.EqualTo(1));
            Assert.That(cargaDecupoPorCartaDePorte.Numero, Is.EqualTo("1"));
            Assert.That(cargaDecupoPorCartaDePorte.Cupo, Is.EqualTo(recorrido.Vehiculo.CartaPorte.Cupo));

            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            repositorioMock.Verify(v => v.Remover(cupo), Times.Once());
        }

        [Test]
        public void TestActualizarCargaDeCupoConCupoGenericoYConTarjetaEquivocada()
        {
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(recorrido);
            var cupo = new CargaDeCupo
            {
                SinCupo = true,
                Numero = "1",
                Recorrido = null,
                Centro = new Centro { Id = 1 },
                Cupo = "MOL1",
                Material = null
            };
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<CargaDeCupo, bool>>>())).Returns(new List<CargaDeCupo> { cupo });               
            var resultado = target.Ejecutar(new ActualizarCargaDeCupo { InstanceId = new Guid("25892e17-80f6-415f-9c65-7395632f0223"), Numero = "1" });

            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(cupo.Material.Id, Is.EqualTo(1));
            Assert.That(cupo.Numero, Is.EqualTo("1"));
            Assert.That(cupo.Recorrido, Is.EqualTo(recorrido));

            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
        }       
    }
}