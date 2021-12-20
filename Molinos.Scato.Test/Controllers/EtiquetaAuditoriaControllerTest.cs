using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class EtiquetaAuditoriaControllerTest
    {
        private EtiquetaAuditoriaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IConfiguracionProvider> configuracionMock;
        private NullLogger logger;
        private CartaPorteDto cp;
        private RecorridoDto recorrido;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            configuracionMock = new Mock<IConfiguracionProvider>();
            logger = new NullLogger();
            target = new EtiquetaAuditoriaController(logger, servRepositorioMock.Object, servComandosMock.Object);
            servRepositorioMock.Setup(x => x.ListarImpresoras(It.IsAny<int>())).Returns(new List<ImpresoraDto>());
            servRepositorioMock.Setup(x => x.ObtenerImpresora(It.IsAny<int>())).Returns(new ImpresoraDto());
            cp = new CartaPorteDto
                {
                    Vehiculos = new Collection<VehiculoDto>{new VehiculoDto{Patente = "AAA111"}},
                    Entregador = "Entregador 1",
                    EntregadorCuit = "20-36401843-9"
                };

            recorrido = new RecorridoDto
                {
                    PesoBruto = 40000,
                    PesoTara = 5000,
                    BalanzaBrutoId = 1,
                    BalanzaTaraId = 1,
                    PesoBrutoUsuario = "Usuario 1",
                    PesoTaraUsuario = "Usuario 2",
                    PesoBrutoFecha = DateTime.Today,
                    PesoTaraFecha = DateTime.Today.AddDays(-1)
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
            var model = new EtiquetaAuditoriaModel()
            {
                Centro = "Centro 1",
                CentroId = 1,
                ImpresoraId = 1,
                NumeroCartaPorte = "001111111111"
            };

            var app = new NameValueCollection { { "Impresora", "Snagit 10" } };
            configuracionMock.Setup(s => s.AppSettings).Returns(app);
            Comando aux = null;
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>()))
                           .Callback<Comando>(c => aux = c).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ObtenerCartaPortePorCentroYNumero(It.IsAny<string>(), It.IsAny<int>())).Returns(cp);
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto { Direccion = "Benielli 398", LocalidadDesc = "San Lorenzo", ProvinciaDesc = "Santa Fe" });
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto { Nombre = "Balanza 1" });
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorDocumentoPatenteYCentro(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(recorrido);

            var resultado = target.Index(new DatosUsuario { CentroId = 1 }, model) as ViewResult;
            var imprimirMuestra = aux as ImprimirEtiquetaAuditoria;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ImprimirEtiquetaAuditoria>()), Times.Exactly(1));
            Assert.That(resultado.ViewName, Is.Null.Or.Empty);
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Exito));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.EtiquetaAuditoria_Ok));
        }
    }
}
