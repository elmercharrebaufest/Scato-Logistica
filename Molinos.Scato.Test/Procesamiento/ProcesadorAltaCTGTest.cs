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
    public class ProcesadorAltaCTGTest
    {
        private ProcesadorAltaCTG target;
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
            accesoWsCtg = new Mock<AccesoWsCtg>(servComandosMock.Object,repositorioMock.Object, loginCMS.Object, servicioRepMock.Object);
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorAltaCTG(repositorioMock.Object, conversor, new NullLogger(), cTgServicePortType.Object, accesoWsCtg.Object);
            tipoDto = new CartaPorteDto
                {
                    Id = 1,
                    DestinoCuit = "12312",
                    DestinoLocalidadCodigoSap = "3123",
                    Cosecha = "14-15"
                };
        }

        [Test]
        public void TestCrearEntidadOkRespuestaDeAfip()
        {
            var ticket = new loginCmsResponse {};
            var response = new solicitarCTGInicialResponse{response = new solicitarCTGReturnType{arrayErrores = new string[0],datosSolicitarCTGResponse = new datosSolicitarCTGResponseType{datosSolicitarCTG = new datosSolicitarCTGType{ctg = 3344434234234,fechaEmision = "2015-11-11",tarifaReferencia = 11.34m}}}};
            var authType = new authType{};
            var recorrido = new Recorrido
                {
                    Centro = new Centro
                        {
                            Cuit = "11-11111111-1",Localidad = new Localidad{CodigoAfip = "2323"}
                        }
                };
            var transportista = new Transportista {Cuit = "11-1548796-4"};
            cTgServicePortType.Setup(s => s.solicitarCTGInicial(It.IsAny<solicitarCTGInicialRequest>())).Returns(response);
            loginCMS.Setup(s => s.loginCms(It.IsAny<loginCmsRequest>())).Returns(ticket);
            repositorioMock.Setup(s => s.Listar<TicketAccesoAfip>(It.IsAny<Expression<Func<TicketAccesoAfip, bool>>>()))
                           .Returns(new List<TicketAccesoAfip>());
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>()))
                           .Returns(transportista);
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(recorrido);
            accesoWsCtg.Setup(s => s.ObtenerAuthType(It.IsAny<string>(),It.IsAny<Resultado>()))
                           .Returns(authType);
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                           .Returns(new Centro(){ Cuit = "1111111111",Localidad = new Localidad{CodigoAfip = "2323"}});
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>()))
                           .Returns(new Material());
            repositorioMock.Setup(s => s.Obtener<Proveedor>(It.IsAny<int>()))
                           .Returns(new Proveedor{Cuil = "3123123"});
            repositorioMock.Setup(s => s.Obtener<CartaPorte>(It.IsAny<int>()))
                           .Returns(new CartaPorte());
            repositorioMock.Setup(s => s.Obtener<BocaDestino>(It.IsAny<int>()))
                           .Returns(new BocaDestino{Localidad = new Localidad{CodigoAfip = "312323"},Proveedor = new Proveedor{Cuil = "123123"}});
            var comando = new DarDeAltaCTG { Dto = tipoDto ,Vehiculo = new VehiculoDto{PesoNetoOrigen = 30000}};
            var resultado = target.Ejecutar(comando);
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));

            repositorioMock.Verify(s => s.Listar<TicketAccesoAfip>(It.IsAny<Expression<Func<TicketAccesoAfip, bool>>>()),Times.Exactly(0));
            repositorioMock.Verify(s => s.Obtener<Transportista>(It.IsAny<int>()), Times.Exactly(1));
            accesoWsCtg.Verify(s => s.ObtenerAuthType(It.IsAny<string>(), It.IsAny<Resultado>()));
            repositorioMock.Verify(s => s.Obtener<Centro>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Material>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<CartaPorte>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<BocaDestino>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<ControlRecorrido>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<AltaCTG>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));

            cTgServicePortType.Verify(s => s.solicitarCTGInicial(It.IsAny<solicitarCTGInicialRequest>()), Times.Exactly(1));
            loginCMS.Verify(s => s.loginCms(It.IsAny<loginCmsRequest>()), Times.Exactly(0));
        }

        [Test]
        public void TestCrearEntidadFallaPorFaltaDeRespuestaDeAfip()
        {
            var ticket = new loginCmsResponse { };
            var response = new solicitarCTGInicialResponse { response = new solicitarCTGReturnType { arrayErrores = new string[0], datosSolicitarCTGResponse = new datosSolicitarCTGResponseType { arrayControles = new controlType[1]{new controlType{descripcion = "Error1",tipo = "Tipo1"}} } } };
            var authType = new authType { };
            var recorrido = new Recorrido
            {
                Centro = new Centro
                {
                    Cuit = "11-11111111-1",
                    Localidad = new Localidad { CodigoAfip = "2323" }
                }
            };
            var transportista = new Transportista { Cuit = "11-1548796-4" };
            cTgServicePortType.Setup(s => s.solicitarCTGInicial(It.IsAny<solicitarCTGInicialRequest>())).Returns(response);
            loginCMS.Setup(s => s.loginCms(It.IsAny<loginCmsRequest>())).Returns(ticket);
            repositorioMock.Setup(s => s.Listar<TicketAccesoAfip>(It.IsAny<Expression<Func<TicketAccesoAfip, bool>>>()))
                           .Returns(new List<TicketAccesoAfip>());
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>()))
                           .Returns(transportista);
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(recorrido);
            accesoWsCtg.Setup(s => s.ObtenerAuthType(It.IsAny<string>(), It.IsAny<Resultado>()))
                           .Returns(authType);
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                           .Returns(new Centro() { Cuit = "1111111111", Localidad = new Localidad { CodigoAfip = "2323" } });
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>()))
                           .Returns(new Material());
            repositorioMock.Setup(s => s.Obtener<Proveedor>(It.IsAny<int>()))
                           .Returns(new Proveedor { Cuil = "3123123" });
            repositorioMock.Setup(s => s.Obtener<CartaPorte>(It.IsAny<int>()))
                           .Returns(new CartaPorte());
            repositorioMock.Setup(s => s.Obtener<BocaDestino>(It.IsAny<int>()))
                           .Returns(new BocaDestino { Localidad = new Localidad { CodigoAfip = "312323" }, Proveedor = new Proveedor { Cuil = "123123" } });
            var comando = new DarDeAltaCTG { Dto = tipoDto, Vehiculo = new VehiculoDto { PesoNetoOrigen = 30000 } };
            var resultado = target.Ejecutar(comando);
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));

            repositorioMock.Verify(s => s.Listar<TicketAccesoAfip>(It.IsAny<Expression<Func<TicketAccesoAfip, bool>>>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.Obtener<Transportista>(It.IsAny<int>()), Times.Exactly(1));
            accesoWsCtg.Verify(s => s.ObtenerAuthType(It.IsAny<string>(), It.IsAny<Resultado>()));
            repositorioMock.Verify(s => s.Obtener<Centro>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Material>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Proveedor>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<CartaPorte>(It.IsAny<int>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.Obtener<BocaDestino>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<ControlRecorrido>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<AltaCTG>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));

            cTgServicePortType.Verify(s => s.solicitarCTGInicial(It.IsAny<solicitarCTGInicialRequest>()), Times.Exactly(1));
            loginCMS.Verify(s => s.loginCms(It.IsAny<loginCmsRequest>()), Times.Exactly(0));
        }
    }
}