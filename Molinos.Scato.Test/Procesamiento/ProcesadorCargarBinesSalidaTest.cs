using System;
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
    public class ProcesadorCargarBinesSalidaTest
    {
        private ProcesadorCargarBinesSalida target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;

        private readonly Guid instanciaWf = new Guid("936DA01F-9ABD-4d9d-80C7-02AF85C822A8");
        private Material material100;
        private Material material200;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCargarBinesSalida(repositorioMock.Object, conversor, new NullLogger());

            material100 = new Material {Id = 100, Peso = 30};
            material200 = new Material {Id = 200, Peso = 40};
        }

        [Test]
        public void TestCargaBinesOK()
        {
            var comando = new CargarBinesSalida
            {
                Bines = new[]
                        {
                            new CargaDeBinesDto {CantidadBines = 20, RemitoBodegaUvaId = 1, TipoId = 100},
                            new CargaDeBinesDto {CantidadBines = 30, RemitoBodegaUvaId = 1, TipoId = 200}
                        },
                InstanciaWorkflow = instanciaWf,
                Observacion = "una observacion"
            };

            var remito = new RemitoBodegaUva
            {
                Id = 1,
                DescargasDeBines = new[]
                        {
                            new DescargaDeBines {CantidadBines = 10, Tipo = material100},
                            new DescargaDeBines {CantidadBines = 20, Tipo = material200}
                        },
                Recorrido = new Recorrido { Id = 99, InstanciaWorkflow = instanciaWf, PesoBruto = 25000, PesoTara = 10000 }
            };

            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>())).Returns(remito);
            repositorioMock.Setup(r => r.Obtener<Material>(100)).Returns(material100);
            repositorioMock.Setup(r => r.Obtener<Material>(200)).Returns(material200);

            var resultado = target.Ejecutar(comando) as ResultadoCargarBinesSalida;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(remito.CargasDeBines, Is.Not.Null);
            Assert.That(remito.CargasDeBines[0].CantidadBines, Is.EqualTo(20));
            Assert.That(remito.CargasDeBines[0].Tipo, Is.EqualTo(material100));
            Assert.That(remito.CargasDeBines[1].CantidadBines, Is.EqualTo(30));
            Assert.That(remito.CargasDeBines[1].Tipo, Is.EqualTo(material200));
            Assert.That(remito.Observacion, Is.EqualTo("una observacion"));
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }

        [Test]
        public void TestMismoPesoBinesEntradaSalida()
        {
            var comando = new CargarBinesSalida
                {
                    Bines = new[]
                        {
                            new CargaDeBinesDto {CantidadBines = 10, RemitoBodegaUvaId = 1, TipoId = 100},
                            new CargaDeBinesDto {CantidadBines = 20, RemitoBodegaUvaId = 1, TipoId = 200}
                        },
                    InstanciaWorkflow = instanciaWf,
                    Observacion = "una observacion"
                };

            var remito = new RemitoBodegaUva
                {
                    Id = 1,
                    DescargasDeBines = new[]
                        {
                            new DescargaDeBines {CantidadBines = 10, Tipo = material100},
                            new DescargaDeBines {CantidadBines = 20, Tipo = material200}
                        },
                    Recorrido = new Recorrido {Id = 99, InstanciaWorkflow = instanciaWf, PesoBruto = 25000, PesoTara = 10000}
                };

            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>())).Returns(remito);
            repositorioMock.Setup(r => r.Obtener<Material>(100)).Returns(material100);
            repositorioMock.Setup(r => r.Obtener<Material>(200)).Returns(material200);

            var resultado = target.Ejecutar(comando) as ResultadoCargarBinesSalida;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.PesoTaraBodega, Is.EqualTo(10000));
            Assert.That(remito.Recorrido.PesoTaraBodega, Is.EqualTo(10000));
        }

        [Test]
        public void TestPesoBinesEntradaMayorPesoBinesSalida()
        {
            var comando = new CargarBinesSalida
            {
                Bines = new[]
                        {
                            new CargaDeBinesDto {CantidadBines = 20, RemitoBodegaUvaId = 1, TipoId = 100},
                            new CargaDeBinesDto {CantidadBines = 15, RemitoBodegaUvaId = 1, TipoId = 200}
                        },
                InstanciaWorkflow = instanciaWf,
                Observacion = "una observacion"
            };

            var remito = new RemitoBodegaUva
            {
                Id = 1,
                DescargasDeBines = new[]
                        {
                            new DescargaDeBines {CantidadBines = 10, Tipo = material100},
                            new DescargaDeBines {CantidadBines = 20, Tipo = material200}
                        },
                Recorrido = new Recorrido { Id = 99, InstanciaWorkflow = instanciaWf, PesoBruto = 25000, PesoTara = 10000 }
            };

            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>())).Returns(remito);
            repositorioMock.Setup(r => r.Obtener<Material>(100)).Returns(material100);
            repositorioMock.Setup(r => r.Obtener<Material>(200)).Returns(material200);

            var resultado = target.Ejecutar(comando) as ResultadoCargarBinesSalida;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.PesoTaraBodega, Is.EqualTo(10000 + (1100 - 1200)));
            Assert.That(remito.Recorrido.PesoTaraBodega, Is.EqualTo(resultado.PesoTaraBodega));
        }

        [Test]
        public void TestPesoBinesEntradaMenorPesoBinesSalida()
        {
            var comando = new CargarBinesSalida
            {
                Bines = new[]
                        {
                            new CargaDeBinesDto {CantidadBines = 10, RemitoBodegaUvaId = 1, TipoId = 100},
                            new CargaDeBinesDto {CantidadBines = 20, RemitoBodegaUvaId = 1, TipoId = 200}
                        },
                InstanciaWorkflow = instanciaWf,
                Observacion = "una observacion"
            };

            var remito = new RemitoBodegaUva
            {
                Id = 1,
                DescargasDeBines = new[]
                        {
                            new DescargaDeBines {CantidadBines = 20, Tipo = material100},
                            new DescargaDeBines {CantidadBines = 15, Tipo = material200}
                        },
                Recorrido = new Recorrido { Id = 99, InstanciaWorkflow = instanciaWf, PesoBruto = 25000, PesoTara = 10000 }
            };

            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>())).Returns(remito);
            repositorioMock.Setup(r => r.Obtener<Material>(100)).Returns(material100);
            repositorioMock.Setup(r => r.Obtener<Material>(200)).Returns(material200);

            var resultado = target.Ejecutar(comando) as ResultadoCargarBinesSalida;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.PesoTaraBodega, Is.EqualTo(10000 + (1200 - 1100)));
            Assert.That(remito.Recorrido.PesoTaraBodega, Is.EqualTo(resultado.PesoTaraBodega));
        }

        [Test]
        public void TestSinBinesSalida()
        {
            var comando = new CargarBinesSalida
            {
                Bines = new CargaDeBinesDto[0],
                InstanciaWorkflow = instanciaWf,
                Observacion = "una observacion"
            };

            var remito = new RemitoBodegaUva
            {
                Id = 1,
                DescargasDeBines = new[]
                        {
                            new DescargaDeBines {CantidadBines = 20, Tipo = material100},
                            new DescargaDeBines {CantidadBines = 15, Tipo = material200}
                        },
                Recorrido = new Recorrido { Id = 99, InstanciaWorkflow = instanciaWf, PesoBruto = 25000, PesoTara = 10000 }
            };

            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>())).Returns(remito);
            repositorioMock.Setup(r => r.Obtener<Material>(100)).Returns(material100);
            repositorioMock.Setup(r => r.Obtener<Material>(200)).Returns(material200);

            var resultado = target.Ejecutar(comando) as ResultadoCargarBinesSalida;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.PesoTaraBodega, Is.EqualTo(10000 + 1200));
            Assert.That(remito.Recorrido.PesoTaraBodega, Is.EqualTo(resultado.PesoTaraBodega));
            Assert.That(remito.CargasDeBines, Is.Null.Or.Empty);
        }

        [Test]
        public void TestSinBinesEntrada()
        {
            var comando = new CargarBinesSalida
            {
                Bines = new[]
                        {
                            new CargaDeBinesDto {CantidadBines = 10, RemitoBodegaUvaId = 1, TipoId = 100},
                            new CargaDeBinesDto {CantidadBines = 20, RemitoBodegaUvaId = 1, TipoId = 200}
                        },
                InstanciaWorkflow = instanciaWf,
                Observacion = "una observacion"
            };

            var remito = new RemitoBodegaUva
            {
                Id = 1,
                DescargasDeBines = new DescargaDeBines[0],
                Recorrido = new Recorrido { Id = 99, InstanciaWorkflow = instanciaWf, PesoBruto = 25000, PesoTara = 10000 }
            };

            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>())).Returns(remito);
            repositorioMock.Setup(r => r.Obtener<Material>(100)).Returns(material100);
            repositorioMock.Setup(r => r.Obtener<Material>(200)).Returns(material200);

            var resultado = target.Ejecutar(comando) as ResultadoCargarBinesSalida;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.PesoTaraBodega, Is.EqualTo(10000 + (-1100)));
            Assert.That(remito.Recorrido.PesoTaraBodega, Is.EqualTo(resultado.PesoTaraBodega));
        }

        [Test]
        public void TestSinBines()
        {
            var comando = new CargarBinesSalida
            {
                Bines = new CargaDeBinesDto[0],
                InstanciaWorkflow = instanciaWf,
                Observacion = "una observacion"
            };

            var remito = new RemitoBodegaUva
            {
                Id = 1,
                DescargasDeBines = new DescargaDeBines[0],
                Recorrido = new Recorrido { Id = 99, InstanciaWorkflow = instanciaWf, PesoBruto = 25000, PesoTara = 10000 }
            };

            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>())).Returns(remito);
            repositorioMock.Setup(r => r.Obtener<Material>(100)).Returns(material100);
            repositorioMock.Setup(r => r.Obtener<Material>(200)).Returns(material200);

            var resultado = target.Ejecutar(comando) as ResultadoCargarBinesSalida;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.PesoTaraBodega, Is.EqualTo(10000));
            Assert.That(remito.Recorrido.PesoTaraBodega, Is.EqualTo(resultado.PesoTaraBodega));
        }


        //[Test]
        //public void TestCrear()
        //{
        //    var remitoBodegaUva = new RemitoBodegaUva(){ Id = 1};
        //    var comando = new CargarBinesSalida {Dto = dto, Observacion = "asdasd", InstanciaWorkflowId = new Guid("00000000-0000-0000-0000-000000000000") };

        //    repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>()))
        //            .Returns<Expression<Func<RemitoBodegaUva, bool>>>(q => remitoBodegaUva);
            
        //    var resultado = target.Ejecutar(comando);
        //    repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
        //    Assert.That(resultado, Is.Not.Null);
        //    Assert.That(resultado.HayErrores, Is.EqualTo(false));
        //}

        //[Test]
        //public void TestExcepción()
        //{
        //    var comando = new CrearIngresarBinSalida { Dto = dto, Observacion = "asdasd", InstanciaWorkflowId = new Guid("00000000-0000-0000-0000-000000000000") };

        //    repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>()))
        //                   .Throws(new IOException("Error"));

        //    var resultado = target.Ejecutar(comando);
        //    repositorioMock.Verify(s => s.Agregar(It.IsAny<Recorrido>()), Times.Never());
        //    repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
        //    Assert.That(resultado, Is.Not.Null);
        //    Assert.That(resultado.HayErrores, Is.EqualTo(true));
        //    Assert.That(resultado.Errores.First().Value.Contains("Error"), Is.EqualTo(true));
        //}
    }
}
