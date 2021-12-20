using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.AfipCTGWebService;
using Molinos.Scato.Servicios.AfipWebService;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorConsultarDestinatarioCTGTest
    {
        private ProcesadorConsultarDestinatarioCTG target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<LoginCMS> loginCMS;
        private Mock<CTGServicePortType> cTgServicePortType;
        private Mock<IServicioRepositorio> servicioRepMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<AccesoWsCtg> accesoWsCtg;
        private IConversor conversor;
        private CartaPorteDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            loginCMS = new Mock<LoginCMS>();
            cTgServicePortType = new Mock<CTGServicePortType>();
            servicioRepMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            accesoWsCtg = new Mock<AccesoWsCtg>(servComandosMock.Object, repositorioMock.Object, loginCMS.Object, servicioRepMock.Object);
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorConsultarDestinatarioCTG(repositorioMock.Object, conversor, new NullLogger(), loginCMS.Object, cTgServicePortType.Object, accesoWsCtg.Object);
            tipoDto = new CartaPorteDto
                {
                    Id = 1,
                    NroCartaPorte = "1111",
                    CTG = "500"
                };
        }

        [Test]
        public void TestCrearEntidadFallaPorRespuestaErrorDeAfip()
        {
            var ticket = new loginCmsResponse {};
            var response = new consultarDetalleCTGResponse { };
            var authType = new authType{};
            var centro = new Centro
                {
                    Cuit = "11-11111111-1"
                };
            
            cTgServicePortType.Setup(s => s.consultarDetalleCTG(It.IsAny<consultarDetalleCTGRequest>())).Returns(new consultarDetalleCTGResponse { response = new consultarDetalleCTGReturnType { arrayErrores = new string[] { "error" } } });      
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(centro);
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { PesoBruto = 1000, PesoTara = 500, Vehiculo = new Vehiculo { CartaPorte = new CartaPorte { Cosecha = "14-15" } }, Material = new Material { CodigoEspecie = 14 } });
            accesoWsCtg.Setup(s => s.ObtenerAuthType(It.IsAny<string>(),It.IsAny<Resultado>())).Returns(authType);
            var comando = new ConsultarDestinatarioCTG { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            cTgServicePortType.Verify(s => s.consultarDetalleCTG(It.Is<consultarDetalleCTGRequest>(a => a.request.ctg == 500)));
            repositorioMock.Verify(s => s.Obtener<Centro>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()), Times.Exactly(1));
            accesoWsCtg.Verify(s => s.ObtenerAuthType(It.IsAny<string>(), It.IsAny<Resultado>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(),Times.Exactly(0));
        }

        [Test]
        public void TestCrearEntidadPorRespuestaDeAfipOk()
        {
            var ticket = new loginCmsResponse { };
            var response = new consultarDetalleCTGResponse { };
            var authType = new authType { };
            var centro = new Centro
            {
                Cuit = "11-11111111-1"
            };

            cTgServicePortType.Setup(s => s.consultarDetalleCTG(It.IsAny<consultarDetalleCTGRequest>())).Returns(new consultarDetalleCTGResponse { response = new consultarDetalleCTGReturnType { consultarDetalleCTGDatos = new consultarDetalleCTGDatosReturnType { cuitDestinatario = "30111111115" } } });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(centro);
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { PesoBruto = 1000, PesoTara = 500, Vehiculo = new Vehiculo { CartaPorte = new CartaPorte { Cosecha = "14-15" } }, Material = new Material { CodigoEspecie = 14 } });
            accesoWsCtg.Setup(s => s.ObtenerAuthType(It.IsAny<string>(), It.IsAny<Resultado>())).Returns(authType);
            var comando = new ConsultarDestinatarioCTG { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            cTgServicePortType.Verify(s => s.consultarDetalleCTG(It.Is<consultarDetalleCTGRequest>(a => a.request.ctg == 500)));
            repositorioMock.Verify(s => s.Obtener<Centro>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()), Times.Exactly(1));
            accesoWsCtg.Verify(s => s.ObtenerAuthType(It.IsAny<string>(), It.IsAny<Resultado>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<DestinatarioCTG>()), Times.Exactly(1));
        }

    }
}