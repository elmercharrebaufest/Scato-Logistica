using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarPedidoDeTrasladoEnSAPTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<ZSDWS_SCATO> servSap;
        private WorkflowInvokerTest host;
        private VerificarPedidoDeTrasladoEnSAP target;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servSap = new Mock<ZSDWS_SCATO>();
            target = new VerificarPedidoDeTrasladoEnSAP();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servSap.Object);
            host.InArguments.CentroDestinoId = 1;
            host.InArguments.CentroEmisorId = 2;
            host.InArguments.MaterialId = 1;
            host.InArguments.TransportistaId = 1; 

            servRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>()))
                           .Returns(new TransportistaDto{Cuit = "1234-12341234"});
            servSap.Setup(s => s.VerifPedTrasladoRedespacho(It.IsAny<VerifPedTrasladoRedespachoRequest>()))
                   .Returns(new VerifPedTrasladoRedespachoResponse1{VerifPedTrasladoRedespachoResponse = new VerifPedTrasladoRedespachoResponse{Planificado = "X"}});
            servRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto{CodigoSAP = "M"});
            servRepositorio.Setup(s => s.ObtenerCentro(1)).Returns(new CentroDto {Descripcion = "C1", CodigoSAP = "C1"});
            servRepositorio.Setup(s => s.ObtenerCentro(2)).Returns(new CentroDto {Descripcion = "C2", CodigoSAP = "C2"});
            
        }

        [Test]
        public void Execute()
        {
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;
            Assert.NotNull(result);
            Assert.False(((Resultado)resultado).HayErrores);
        }


        [Test]
        public void ExecuteErrorTest()
        {
            servSap.Setup(s => s.VerifPedTrasladoRedespacho(It.IsAny<VerifPedTrasladoRedespachoRequest>()))
                   .Returns(new VerifPedTrasladoRedespachoResponse1 { VerifPedTrasladoRedespachoResponse = new VerifPedTrasladoRedespachoResponse { Planificado = "" } });

            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;
            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.Values.FirstOrDefault(), "No Existe Pedido de Traslado");
        }

        [Test]
        public void ExceptionTest()
        {
            servRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>()))
                           .Throws(new Exception("Error"));

            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.Values.FirstOrDefault(), "Error en el procedimiento del servicio SAP: Error");

        }
    }
}
