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
    public class ProcesadorBajaCTGDefinitivoTest
    {
        private ProcesadorBajaCTGDefinitivo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IServicioRepositorio> servicioRepMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<LoginCMS> loginCMS;
        private Mock<CTGServicePortType> cTgServicePortType;
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
            target = new ProcesadorBajaCTGDefinitivo(repositorioMock.Object, conversor, new NullLogger(), cTgServicePortType.Object, accesoWsCtg.Object);
            tipoDto = new CartaPorteDto
                {
                    Id = 1,
                    NroCartaPorte = "1111",
                    CTG = "1111"

                };
        }

        [Test]
        public void TestCrearEntidadFallaPorFaltaDeRespuestaDeAfip()
        {
            var ticket = new loginCmsResponse {};
            var response = new confirmarDefinitivoResponse { };
            var authType = new authType{};
            var centro = new Centro
                {
                    Cuit = "11-11111111-1"
                };
            
            var transportista = new Transportista {Cuit = "11-1548796-4"};
            cTgServicePortType.Setup(s => s.confirmarDefinitivo(It.IsAny<confirmarDefinitivoRequest>())).Returns(new confirmarDefinitivoResponse{response = new confirmarDefinitivoReturnType{ arrayErrores = new string[]{"error"}}});
            loginCMS.Setup(s => s.loginCms(It.IsAny<loginCmsRequest>())).Returns(ticket);
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<TicketAccesoAfip, bool>>>()))
                           .Returns(new List<TicketAccesoAfip>());
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>()))
                           .Returns(transportista);
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Centro, bool>>>()))
                           .Returns(centro);
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new Recorrido { PesoBruto = 1000, PesoTara = 500, Vehiculo = new Vehiculo { CartaPorte = new CartaPorte { Cosecha = "14-15" } }, Material = new Material { CodigoEspecie = 14 } });
            accesoWsCtg.Setup(s => s.ObtenerAuthType(It.IsAny<string>(),It.IsAny<Resultado>()))
                           .Returns(authType);
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                           .Returns(new Centro() { Cuit = "1111111111" });
            var comando = new DarDeBajaCTGDefinitivo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            cTgServicePortType.Verify(v => v.confirmarDefinitivo(It.Is<confirmarDefinitivoRequest>(s => s.request.datosConfirmarDefinitivo.pesoNeto == 500)));
        }

    }
}