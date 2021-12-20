using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ImpresionEnvioMuestraAuditoriaTest
    {
        private ImpresionEnvioMuestraAuditoria target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;
        private ScatoPersistenceParticipant participant;

        [SetUp]
        public void SetUp()
        {
            target = new ImpresionEnvioMuestraAuditoria();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
            participant = new ScatoPersistenceParticipant();
            host.Extensions.Add(participant);
            host.InArguments.ProveedorCuit = "12-123456789-1";
            host.InArguments.UsuarioCalado = "UserCalado";
            host.InArguments.EntregadorCuit = "12-123456789-2";
            host.InArguments.Procedencia = "P";
            host.InArguments.Corredor = "C1";
            host.InArguments.CorredorCuit = "12-123456798-7";
            host.InArguments.Entregador = "entregadr 1";
            host.InArguments.PatenteAcoplado = "BBB222";
        }

        //[Test]
        //public void TestImprimeInforme()
        //{
        //    srvRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new DocumentoDeImpresionPorCentroDto() { Id = 1, CentroId = 1 });
        //    srvRepositorio.Setup(s => s.ObtenerMaterialPorCentroPorInstanceId(It.IsAny<Guid>())).Returns(new MaterialPorCentroDto {PorcentajeMuestraAuditoria = 20});
        //    srvRepositorio.Setup(s => s.ObtenerNumeroAleatorio()).Returns(10);
        //    srvRepositorio.Setup(s => s.ObtenerNumeroMuestraAuditoriaGenerado()).Returns(2564);

        //    srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
        //    host.InArguments.CentroId = 2;
        //    host.InArguments.CodigoDeImpresion = "IdentificacionMuestraAuditoria";
        //    host.InArguments.WorkflowId = new Guid();
        //    host.InArguments.Calado = new CaladoDto{FechaCreacion = new DateTime(2015,6,6)};
        //    host.InArguments.NumeroDeOrden = "1";
        //    host.InArguments.Patente = "AAA111";
        //    host.InArguments.Proveedor = "proveedor 1";
        //    host.InArguments.PuestoDeTrabajoId = 1;

        //    var result = host.TestActivity();

        //    Assert.NotNull(result);
        //    srvComandos.Verify(v => v.Ejecutar(It.IsAny<ImprimirMuestraAuditoria>()), Times.Once());
        //}

        [Test]
        public void TestNoImprimeInforme()
        {
            srvRepositorio.Setup(s => s.ObtenerMaterialPorCentroPorInstanceId(It.IsAny<Guid>())).Returns(new MaterialPorCentroDto { PorcentajeMuestraAuditoria = 20 });
            srvRepositorio.Setup(s => s.ObtenerNumeroAleatorio()).Returns(50);

            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            host.InArguments.CentroId = 2;
            host.InArguments.CodigoDeImpresion = "IdentificacionMuestraAuditoria";
            host.InArguments.WorkflowId = new Guid();
            host.InArguments.Calado = new CaladoDto();
            host.InArguments.NumeroDeOrden = "1";
            host.InArguments.Patente = "AAA111";
            host.InArguments.Proveedor = "proveedor 1";
            host.InArguments.PuestoDeTrabajoId = 1;

            var result = host.TestActivity();

            srvComandos.Verify(v => v.Ejecutar(It.IsAny<ImprimirMuestraAuditoria>()), Times.Never());
        }

    }
}
