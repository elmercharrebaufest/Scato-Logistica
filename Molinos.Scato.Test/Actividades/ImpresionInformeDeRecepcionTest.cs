using System;
using System.Collections.Generic;
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
    public class ImpresionInformeDeRecepcionTest
    {
        private ImpresionInformeDeRecepcion target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;
        private ScatoPersistenceParticipant participant;

        [SetUp]
        public void SetUp()
        {
            target = new ImpresionInformeDeRecepcion();
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
            srvRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new DocumentoDeImpresionPorCentroDto() { Id = 1, CentroId = 1, CentroDescripcion = "CentDesc", DocumentoDeImpresionDescripcion = "DocImpDes", DocumentoDeImpresionId = 3, FormatoDeImpresionDescripcion = "Format1", FormatoDeImpresionId = 1, ImpresoraDescripcion = "impresora1", ImpresoraDireccion = "direc1", ImpresoraId = 33 });
            srvRepositorio.Setup(s => s.ObtenerImpresora(It.IsAny<int>())).Returns(new ImpresoraDto{CentroId = 1,Descripcion = "Impre1",Direccion = "direc",Id = 3});
            srvRepositorio.Setup(s => s.ObtenerRomaneosPorGuid(It.IsAny<Guid>())).Returns(new List<RomaneoDto> { new RomaneoDto { Estado = EstadoRomaneo.EnProceso, FechaCierre = new DateTime(), FechaInicio = new DateTime(), Imprimir = true, NroPedido = "3123", Numero = 3, Observaciones = "Observente", ProveedorDescripcion = "ProvDesc", ProveedorDistinto = false, ProveedorId = 3, WorkflowInstanceId = new Guid(), RomaneoItems = new List<RomaneoItemDto> { new RomaneoItemDto { Almacen = "almac", AlmacenId = 2, BalanzaId = 3, DocMaterial = "doc1", EjercicioDocMaterial = "asd", EjercicioDocMaterialAnulacion = "ss", Fecha = new DateTime(), FechaFabricacion = new DateTime(), FechaRemito = new DateTime() ,Id = 2,ItemNro = 23,LoteProveedor = "loteprov",Material = "3123",MaterialCodigoSap = "56456",MaterialId = 2,ModalidadBalanza = Modalidad.Automática,PesoBruto = 40000,PesoTara = 10000,Rechazado = false,RemitoNro = "54535345",RomaneoId = 2,TaraRomaneoId = 1} } } });
            srvRepositorio.Setup(s => s.ObtenerItemRomaneosPorRomaneoId(It.IsAny<int>())).Returns(new List<RomaneoItemDto> { new RomaneoItemDto { Almacen = "almac", AlmacenId = 2, BalanzaId = 3, DocMaterial = "doc1", EjercicioDocMaterial = "asd", EjercicioDocMaterialAnulacion = "ss", Fecha = new DateTime(), FechaFabricacion = new DateTime(), FechaRemito = new DateTime() ,Id = 2,ItemNro = 23,LoteProveedor = "loteprov",Material = "3123",MaterialCodigoSap = "56456",MaterialId = 2,ModalidadBalanza = Modalidad.Automática,PesoBruto = 40000,PesoTara = 10000,Rechazado = false,RemitoNro = "54535345",RomaneoId = 2,TaraRomaneoId = 1}});
            srvRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto {CodigoSAP = "2131", UnidadDeMedidad = "%"});
            srvRepositorio.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto());
            srvRepositorio.Setup(s => s.ObtenerNumeroInformeGenerado()).Returns(1);
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            host.InArguments.CentroId = 2;
            host.InArguments.CodigoDeImpresion = "InformeDeRecepcion";
            host.InArguments.WorkflowId = new Guid();
            host.InArguments.PuestoDeTrabajoId = 1;

            var result = host.TestActivity();

            srvRepositorio.Verify(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerImpresora(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerRomaneosPorGuid(It.IsAny<Guid>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerItemRomaneosPorRomaneoId(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerMaterial(It.IsAny<int>()), Times.Exactly(2));
            srvRepositorio.Verify(s => s.ObtenerProveedor(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerNumeroInformeGenerado(), Times.Exactly(1));


            Assert.That(result,Is.Not.Null);          
        }
    }
}
