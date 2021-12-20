using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ImpresionEtiquetaIntactaTest
    {
        private ImpresionEtiquetaIntacta target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;
        private ScatoPersistenceParticipant participant;

        [SetUp]
        public void SetUp()
        {
            target = new ImpresionEtiquetaIntacta();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
            participant = new ScatoPersistenceParticipant();
            host.Extensions.Add(participant);
        }

        [Test]
        public void TestImprimeInforme()
        {
            srvRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new DocumentoDeImpresionPorCentroDto() { Id = 1, CentroId = 1 });
            srvRepositorio.Setup(s => s.RequiereTecnologia(It.IsAny<Guid>())).Returns(true);
            srvRepositorio.Setup(s => s.ObtenerCartaDePorteRegistradaServicioMonsanto(It.IsAny<Guid>(), It.IsAny<TipoVehiculo>())).Returns(new CartaDePorteRegistradaServicioMonsantoDto());
            srvRepositorio.Setup(s => s.ObtenerCamaraPorMaterialPorCentro(It.IsAny<Guid>())).Returns(new CamaraDto());
            srvRepositorio.Setup(s => s.ObtenerVehiculoPorGuid(It.IsAny<Guid>())).Returns(new VehiculoDto());

            

            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            host.InArguments.CentroId = 2;
            host.InArguments.CodigoDeImpresion = "IdentificacionMuestraAuditoria";
            host.InArguments.WorkflowId = new Guid();
            host.InArguments.NumeroCartaPorte = "000540805405";
            host.InArguments.Patente = "AAA111";
            host.InArguments.NombreUsuario = "proveedor 1";
            host.InArguments.Material = "material 1";
            host.InArguments.PuestoDeTrabajoId = 1;

            host.InArguments.TipoVehiculo = TipoVehiculo.Camión;
            var result = host.TestActivity();

            srvComandos.Verify(v => v.Ejecutar(It.IsAny<ImprimirEtiquetaIntacta>()), Times.Once());
        }

    }
}
