using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class MicromuestraControllerTest
    {
        private MicromuestraController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private NullLogger logger;
        private List<CasilleroDto> casilleros;
        private List<MicroMuestrasPorCasilleroDto> microMuestras;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            logger = new NullLogger();
            target = new MicromuestraController(logger, servRepositorioMock.Object, servComandosMock.Object);
            servRepositorioMock.Setup(x => x.ListarImpresoras(It.IsAny<int>())).Returns(new List<ImpresoraDto>());
            servRepositorioMock.Setup(x => x.ObtenerImpresora(It.IsAny<int>())).Returns(new ImpresoraDto());

            casilleros = new List<CasilleroDto>
                {
                    new CasilleroDto
                    {
                        Id = 1,
                        Numero = "1111-000001",
                        Capacidad = 3,
                        CentroId = 1
                    }
                };

            microMuestras = new List<MicroMuestrasPorCasilleroDto>
                {
                    new MicroMuestrasPorCasilleroDto
                    {
                        Id = 1,
                        MuestraId = 1,
                        CasilleroId = 1
                    }
                };

        }

        [Test]
        public void TestIndex()
        {
            var result = target.Index(new DatosUsuario()) as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestIndexPost()
        {
            var caracteristicas = new List<CaracteristicaDeCalidadDto>
                {
                    new CaracteristicaDeCalidadDto
                        {
                            Id = 1,
                            TipoCaracteristica = CaracteristicasCalidad.EsHumedad,
                            Descripcion = "C1"
                        },
                    new CaracteristicaDeCalidadDto
                        {
                            Id = 2,
                            TipoCaracteristica = CaracteristicasCalidad.Ninguno,
                            Descripcion = "C2"
                        }
                };

            var muestra = new MuestraEnvioACamaraDto
                {
                    Id = 1,
                    CentroId = 1,
                    CaracteristicasDeCalidad = caracteristicas,
                    Proveedor = "Perez",
                    Patente = "AAA111"
                };

            const decimal valorHumedad = 80;

            var app = new NameValueCollection { { "Impresora", "Snagit 10" } };
            Comando aux = null;
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>()))
                           .Callback<Comando>(c => aux = c).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ObtenerMuestraEnvioACamaraPorNumero(It.IsAny<int>(),It.IsAny<string>())).Returns(muestra);
            servRepositorioMock.Setup(s => s.ListarAnalisisYCaladoPorId(It.IsAny<int>()))
                               .Returns(new List<AnalisisPorCaracteristicaDto>()
                                   {
                                       new AnalisisPorCaracteristicaDto
                                           {
                                               CaracteristicaId =
                                                   caracteristicas[0].Id,
                                               ValorCalado = 50, ValorAnalisis = valorHumedad, EsHumedad = true
                                           }
                                   });

            servRepositorioMock.Setup(s => s.ListarCasillerosPorCentro(1)).Returns(casilleros);
            servRepositorioMock.Setup(s => s.ListarMicroMuestrasPorCasilleroPorCentro(1)).Returns(microMuestras);

            var resultado = target.Index(new DatosUsuario { CentroId = 1}, new MicromuestraModel {CentroId = 1, CodigoBarras = "3333"}) as ViewResult;
            var imprimirMuestra = aux as ImprimirMicromuestra;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ImprimirMicromuestra>()), Times.Exactly(4));
            Assert.That(resultado.ViewName, Is.Null.Or.Empty);
            Assert.That(imprimirMuestra.Dto.Humedad, Is.EqualTo(valorHumedad.Formatted() + " %"));     
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Exito));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.Micromuestra_Ok));
        }

        [Test]
        public void TestIndexSinHumedadPost()
        {
            var caracteristicas = new List<CaracteristicaDeCalidadDto>
                {
                    new CaracteristicaDeCalidadDto
                        {
                            Id = 1,
                            TipoCaracteristica = CaracteristicasCalidad.Ninguno,
                            Descripcion = "C1"
                        },
                    new CaracteristicaDeCalidadDto
                        {
                            Id = 2,
                            TipoCaracteristica = CaracteristicasCalidad.Ninguno,
                            Descripcion = "C2"
                        }
                };

            var muestra = new MuestraEnvioACamaraDto
            {
                Id = 1,
                CentroId = 1,
                CaracteristicasDeCalidad = caracteristicas,
                Proveedor = "Perez",
                Patente = "AAA111",
                TieneAnalisisInterno = true
            };

            var app = new NameValueCollection { { "Impresora", "Snagit 10" } };
            Comando aux = null;
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>()))
                           .Callback<Comando>(c => aux = c).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ObtenerMuestraEnvioACamaraPorNumero(It.IsAny<int>(),It.IsAny<string>())).Returns(muestra);
            servRepositorioMock.Setup(s => s.ListarAnalisisYCaladoPorId(It.IsAny<int>()))
                               .Returns(new List<AnalisisPorCaracteristicaDto>()
                                   {
                                       new AnalisisPorCaracteristicaDto
                                           {
                                               CaracteristicaId =
                                                   caracteristicas[0].Id,
                                               ValorCalado = 50, ValorAnalisis = 80
                                           }
                                   });

            servRepositorioMock.Setup(s => s.ListarCasillerosPorCentro(1)).Returns(casilleros);
            servRepositorioMock.Setup(s => s.ListarMicroMuestrasPorCasilleroPorCentro(1)).Returns(microMuestras);

            var resultado = target.Index(new DatosUsuario { CentroId = 1 }, new MicromuestraModel { CentroId = 1, CodigoBarras = "3333" }) as ViewResult;
            var imprimirMuestra = aux as ImprimirMicromuestra;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ImprimirMicromuestra>()), Times.Exactly(5));
            Assert.That(resultado.ViewName, Is.Null.Or.Empty);
            Assert.That(imprimirMuestra.Dto.Humedad, Is.EqualTo(string.Empty));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Exito));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.Micromuestra_Ok));
        }

        [Test]
        public void TestMuestraNoExiste()
        {
            var caracteristicas = new List<CaracteristicaDeCalidadDto>
                {
                    new CaracteristicaDeCalidadDto
                        {
                            Id = 1,
                            TipoCaracteristica = CaracteristicasCalidad.Ninguno,
                            Descripcion = "C1"
                        },
                    new CaracteristicaDeCalidadDto
                        {
                            Id = 2,
                            TipoCaracteristica = CaracteristicasCalidad.Ninguno,
                            Descripcion = "C2"
                        }
                };

            var app = new NameValueCollection { { "Impresora", "Snagit 10" } };
            Comando aux = null;
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>()))
                           .Callback<Comando>(c => aux = c).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ObtenerMuestraEnvioACamaraPorNumero(It.IsAny<int>(),It.IsAny<string>())).Returns((MuestraEnvioACamaraDto)null);
            servRepositorioMock.Setup(s => s.ListarAnalisisYCaladoPorId(It.IsAny<int>()))
                               .Returns(new List<AnalisisPorCaracteristicaDto>()
                                   {
                                       new AnalisisPorCaracteristicaDto
                                           {
                                               CaracteristicaId =
                                                   caracteristicas[0].Id,
                                               ValorCalado = 50, ValorAnalisis = 80
                                           }
                                   });

            servRepositorioMock.Setup(s => s.ListarCasillerosPorCentro(1)).Returns(casilleros);
            servRepositorioMock.Setup(s => s.ListarMicroMuestrasPorCasilleroPorCentro(1)).Returns(microMuestras);

            var resultado = target.Index(new DatosUsuario { CentroId = 1 }, new MicromuestraModel { CentroId = 1, CodigoBarras = "3333" }) as ViewResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ImprimirMicromuestra>()), Times.Never());
            Assert.That(resultado.ViewName, Is.Null.Or.Empty);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
        }
    }
}
