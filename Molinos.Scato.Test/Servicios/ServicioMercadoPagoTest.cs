using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Impl;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;
using Molinos.Scato.Test.Mock;
using System.ComponentModel;
using Molinos.Scato.Dominio.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Molinos.Scato.Dominio.Dto;
using System.Configuration;

namespace Molinos.Scato.Test.Servicios
{
    [TestFixture]
    class ServicioMercadoPagoTest
    {
        private ServicioMercadoPago target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<FakeHttpMessageHandler> _fakeHttpMessageHandler;
        private HttpClient _httpClient;


        CredencialMercadoPago listaDeCredencialesEnBbdd;
        CredencialMercadoPagoDto responseCredencialesNuevas;
        ListaCajaDto responseCajas;
        DetalleDePagoDto responseDetallePago201Exitoso;
        DetalleDePagoDto responseDetallePago201Rechazado;
        ErrorMercadoPagoDto responseError404MercadoPago;

        [SetUp]
        public void Setup()
        {
            ConfigurationManager.AppSettings["UrlSolicitarCredenciales"] = "https://api.mercadopago.com/test/token";
            ConfigurationManager.AppSettings["UrlPagoMercadoPago"] = "https://api.mercadopago.com/test/instore/qr/buyer/collectors/{0}/stores/SLO/pos/{1}/orders?access_token=";
            ConfigurationManager.AppSettings["UrlDevolucionMercadoPago"] = "https://api.mercadopago.com/test/v1/payments/{0}/refunds?access_token=";
            ConfigurationManager.AppSettings["UrlCaja"] = "https://api.mercadopago.com/test/pos?access_token=";


            repositorioMock = new Mock<IRepositorio>();
            _fakeHttpMessageHandler = new Mock<FakeHttpMessageHandler> { CallBase = true };
            _httpClient = new HttpClient(_fakeHttpMessageHandler.Object);
            target = new ServicioMercadoPago(repositorioMock.Object, new NullLogger(), _httpClient);


            listaDeCredencialesEnBbdd = new CredencialMercadoPago
            {
                AppId = 123123123123,
                SecretKey = "TestSecretKey",
                RedirectUri = "www.redirectUriTest",
                FechaVencimientoPermisos = DateTime.Now.AddDays(32),
                AccessToken = "Test-Access-Token",
                GaritasDeSalida = new List<GaritaDeSalida>
               {
                   new GaritaDeSalida
                   {
                       PuestoDeTrabajo = new PuestoDeTrabajo()
                   }
               }
            };
            responseCredencialesNuevas = new CredencialMercadoPagoDto
            {
                AccessToken = "TEST-access_token",
                PublicKey = "TEST-public_key",
                TokenActualizarPermiso = "TEST-refresh_token",
                UsuarioVendedorId = 000000001,
                TiempoVencimientoPermiso = 000000001
            };
            responseCajas = new ListaCajaDto
            {
                ListaDeCajas = new List<CajaDto>
                {
                    new CajaDto
                    {
                        CajaId="00001"
                    }
                }
            };
            responseDetallePago201Exitoso = new DetalleDePagoDto
            {
                PagosRealizados = new List<EstadoPagoDto>
                {
                    new EstadoPagoDto
                    {
                        DetalleDeEstado="TEST-DETALLE-DEPAGO"
                    }
                }
            };
            responseDetallePago201Rechazado = new DetalleDePagoDto
            {
                PagosRealizados = new List<EstadoPagoDto>
                {
                    new EstadoPagoDto
                    {
                        DetalleDeEstado="TEST-DETALLE-DEPAGO"
                    }
                }
            };
            responseError404MercadoPago = new ErrorMercadoPagoDto
            {
                Error = "test",
                Estado = 001,
                Mensaje = "MensajeError",
                CausaDeError = new List<CausaDeErrorMercadoPagoDto>
                {
                    new CausaDeErrorMercadoPagoDto
                    {
                        Codigo=123,
                        Dato="",
                        Descripcion=""
                    }
                }
            };

        }

        [Test]
        public void pagarConMercadoPagoOk()
        {
            repositorioMock.Setup(s => s.ObtenerMayor<CredencialMercadoPago, int>(It.IsAny<Expression<Func<CredencialMercadoPago, bool>>>(), It.IsAny<Expression<Func<CredencialMercadoPago, int>>>()))
                    .Returns(listaDeCredencialesEnBbdd);
            var PagoExitosoRespuestaMercPag = new StringContent(JsonConvert.SerializeObject(responseDetallePago201Exitoso), Encoding.UTF8, "application/json");
            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<string>(), HttpMethod.Post, It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = PagoExitosoRespuestaMercPag });

            var CajasRespuestaMercPag = new StringContent(JsonConvert.SerializeObject(responseCajas), Encoding.UTF8, "application/json");
            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = CajasRespuestaMercPag });

            repositorioMock.Setup(s => s.ObtenerMayor<CredencialMercadoPago, int>(It.IsAny<Expression<Func<CredencialMercadoPago, bool>>>(), It.IsAny<Expression<Func<CredencialMercadoPago, int>>>()))
                    .Returns(listaDeCredencialesEnBbdd);


            var resultado = target.Pagar(100.3, "TokkendePAGO", "idemp", "", 0);

            Assert.NotNull(resultado);
            repositorioMock.Verify(v => v.ObtenerMayor<CredencialMercadoPago, int>(It.IsAny<Expression<Func<CredencialMercadoPago, bool>>>(), It.IsAny<Expression<Func<CredencialMercadoPago, int>>>()), Times.Exactly(1));
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Exactly(0));
        }
        [Test]
        public void SolicitarCredencialesVendedorOk()
        {
            var respuestaMercPag = new StringContent(JsonConvert.SerializeObject(responseCredencialesNuevas), Encoding.UTF8, "application/json");
            var PagoExitosoRespuestaMercPag = new StringContent(JsonConvert.SerializeObject(responseDetallePago201Exitoso), Encoding.UTF8, "application/json");

            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<string>(), HttpMethod.Post, It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = respuestaMercPag });

            var cajasCero = new ListaCajaDto { ListaDeCajas = new List<CajaDto> { new CajaDto { CajaId = "0001" } } };
            var CajasRespuestaMercPag = new StringContent(JsonConvert.SerializeObject(cajasCero), Encoding.UTF8, "application/json");
            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = CajasRespuestaMercPag });

            repositorioMock.Setup(s => s.ObtenerMayor<CredencialMercadoPago, int>(It.IsAny<Expression<Func<CredencialMercadoPago, bool>>>(), It.IsAny<Expression<Func<CredencialMercadoPago, int>>>()))
                    .Returns(listaDeCredencialesEnBbdd);
            repositorioMock.Setup(s => s.Listar<CredencialMercadoPago>(It.IsAny<Expression<Func<CredencialMercadoPago, bool>>>()))
                    .Returns(new List<CredencialMercadoPago> { listaDeCredencialesEnBbdd });

            var response = target.SolicitarCredencialesVendedor("Codigo-de-TEST");

            Assert.NotNull(response);
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
        }
        [Test]
        public void guardarCredencialMarketPlaceOVendedorExistenteOk()
        {
            repositorioMock.Setup(s => s.ObtenerMayor<CredencialMercadoPago, int>(It.IsAny<Expression<Func<CredencialMercadoPago, bool>>>(), It.IsAny<Expression<Func<CredencialMercadoPago, int>>>()))
                    .Returns(listaDeCredencialesEnBbdd);
            repositorioMock.Setup(s => s.Listar<CredencialMercadoPago>(It.IsAny<Expression<Func<CredencialMercadoPago, bool>>>()))
                    .Returns(new List<CredencialMercadoPago> { listaDeCredencialesEnBbdd });
            var CredencialesAGuardar = new CredencialMercadoPagoDto
            {
                AppId = 123123123123,
                SecretKey = "MODIFICADOTestSecretKey"
            };


            var response = target.GuardarCredencial(CredencialesAGuardar);

            Assert.NotNull(response);
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
        }
        [Test]
        public void guardarCredencialMarketPlaceOVendedorNuevoOk()
        {
            repositorioMock.Setup(s => s.Listar<CredencialMercadoPago>(It.IsAny<Expression<Func<CredencialMercadoPago, bool>>>()))
                    .Returns(new List<CredencialMercadoPago>());
            repositorioMock.Setup(x => x.Agregar(It.IsAny<CredencialMercadoPago>())).Returns(listaDeCredencialesEnBbdd);
            var CredencialesAGuardar = new CredencialMercadoPagoDto
            {
                AppId = null,
                SecretKey = "MODIFICADOTestSecretKey"
            };

            var response = target.GuardarCredencial(CredencialesAGuardar);

            Assert.NotNull(response);
            Assert.IsTrue(response);
            repositorioMock.Verify(v => v.Agregar(It.IsAny<CredencialMercadoPago>()), Times.Once());
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
        }
        [Test]
        public void pagarConCredencialesDeVendedorPorVencerOk()
        {
            var credencial = new CredencialMercadoPago
            {
                UsuarioVendedorId = 001,
                FechaVencimientoPermisos = DateTime.Now.AddDays(10),
                GaritasDeSalida = new List<GaritaDeSalida>
               {
                   new GaritaDeSalida
                   {
                       PuestoDeTrabajo = new PuestoDeTrabajo()
                   }
               }
            };
            repositorioMock.Setup(x => x.Agregar(It.IsAny<CredencialMercadoPago>())).Returns(listaDeCredencialesEnBbdd);
            repositorioMock.Setup(s => s.ObtenerMayor<CredencialMercadoPago, int>(It.IsAny<Expression<Func<CredencialMercadoPago, bool>>>(), It.IsAny<Expression<Func<CredencialMercadoPago, int>>>()))
           .Returns(credencial);
            repositorioMock.Setup(s => s.Listar<CredencialMercadoPago>(It.IsAny<Expression<Func<CredencialMercadoPago, bool>>>()))
                   .Returns(new List<CredencialMercadoPago>());
            var PagoExitosoRespuestaMercPag = new StringContent(JsonConvert.SerializeObject(responseDetallePago201Exitoso), Encoding.UTF8, "application/json");
            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<string>(), HttpMethod.Post, It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = PagoExitosoRespuestaMercPag });

            var CajasRespuestaMercPag = new StringContent(JsonConvert.SerializeObject(responseCajas), Encoding.UTF8, "application/json");
            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = CajasRespuestaMercPag });

            
            var resultado = target.Pagar(100.3, "TokkendePAGO","idemp", "", 0);

            Assert.NotNull(resultado);
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Exactly(1));
        }
        [Test]
        public void solicitarCredencialesVendedorSinCajasExistentesyAgregarCajaOk()
        {

            var responseCajaNuevaDeMercPago = new CajaDto
            {
                CajaId = "50",
                Nombre = "CajaNuevaTest"
            };

            var respuestaMercPag = new StringContent(JsonConvert.SerializeObject(responseCredencialesNuevas), Encoding.UTF8, "application/json");

            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<string>(), HttpMethod.Post, It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = respuestaMercPag });

            var NuevaCajaRespuestaMercPag = new StringContent(JsonConvert.SerializeObject(responseCajaNuevaDeMercPago), Encoding.UTF8, "application/json");
            _fakeHttpMessageHandler.Setup(f => f.Send(It.Is<string>(x => x.StartsWith("https://api.mercadopago.com/pos")), HttpMethod.Post, It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.Created, Content = NuevaCajaRespuestaMercPag });
            repositorioMock.Setup(s => s.Listar<CredencialMercadoPago>(It.IsAny<Expression<Func<CredencialMercadoPago, bool>>>()))
                   .Returns(new List<CredencialMercadoPago>());
            repositorioMock.Setup(s => s.ObtenerMayor<CredencialMercadoPago, int>(It.IsAny<Expression<Func<CredencialMercadoPago, bool>>>(), It.IsAny<Expression<Func<CredencialMercadoPago, int>>>()))
                    .Returns(listaDeCredencialesEnBbdd);
            repositorioMock.Setup(x => x.Agregar(It.IsAny<CredencialMercadoPago>())).Returns(listaDeCredencialesEnBbdd);

            var cajasCero = new ListaCajaDto { ListaDeCajas = new List<CajaDto> { } };
            var CajasRespuestaMercPag = new StringContent(JsonConvert.SerializeObject(cajasCero), Encoding.UTF8, "application/json");
            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = CajasRespuestaMercPag });



            var resultado = target.SolicitarCredencialesVendedor("token-test");

            Assert.NotNull(resultado);
        }


        [Test]
        public void TestOk()
        {
            var nombrePuesto = "NOMBRE ";

            var numPuestoDeTrabajo = "00001";
            var asdsd = string.Join("", nombrePuesto.ToCharArray().Where(Char.IsDigit));


            numPuestoDeTrabajo = "1" + numPuestoDeTrabajo.Substring(1);

            var asd = Convert.ToDecimal(100);
            //CodigoDeErrorMercadoPago asd = CodigoDeErrorMercadoPago.ErrorMonto;


            //var asdddd = asd.GetType().GetMember(asd.ToString())
            //   .First()
            //   .GetCustomAttribute<DisplayAttribute>()
            //   .GetName();
            var errorTexto = "";

            CodigoDeErrorMercadoPago error;

            Enum.TryParse("400017", true, out error);

            var errorTexto1 = error.GetType().GetMember(error.ToString())
                                    .First()
                                    .GetCustomAttribute<DisplayAttribute>()
                                    ;

            var addd = errorTexto1.GetName();



            Console.WriteLine(errorTexto);
        }


    }
















}

