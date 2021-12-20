using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ImpresionGenericaTest
    {
        private ImpresionGenerica target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomando;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            target = new ImpresionGenerica();

            servRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                           .Returns(new CentroDto
                               {
                                   Descripcion = "Centro1",
                                   Id = 1,
                                   NumeroINV = "1",
                                   Cuit = "12",
                                   IngresosBrutos = "1",
                                   RazonSocial = "Centro1",
                                   LocalidadDesc = "DescLoc",
                                   ProvinciaDesc = "DescProv",
                                   CodigoPostal = "1",
                                   Direccion = "Dir"
                               });
            servRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>()))
                           .Returns(new MaterialDto {Descripcion = "M", Id = 1});
            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>()))
                           .Returns(new RecorridoDto
                               {
                                   Id = 1,
                                   TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                                   Workflow = new WorkflowDto {TipoDeWorkflow = TipoDeWorkflow.Ingreso},
                                   EsSustentable = false
                               });
            servRepositorio.Setup(s => s.ObtenerBocaDestino(It.IsAny<int>()))
                           .Returns(new BocaDestinoDto
                               {
                                   ProveedorId = 1,
                                   Domicilio = "Dom",
                                   Provincia = "Prov",
                                   Localidad = "Loc",
                                   NombreBocaDeDestino = "Boca"
                               });
            servRepositorio.Setup(s => s.ObtenerObservacion(It.IsAny<Guid>())).Returns(new ObservacionDto{Observaciones = "Obs"});
            servRepositorio.Setup(s => s.ObtenerVariedadPorMaterial(It.IsAny<int>())).Returns(new VariedadDto{Descripcion = "Var", NumeroINV = "1"});
            servRepositorio.Setup(s => s.ObtenerTenorAzucarino(It.IsAny<Guid>()));
            servRepositorio.Setup(s => s.ObtenerFormatoDeImpresion(It.IsAny<int>()))
                           .Returns(new FormatoDeImpresionDto());
            servRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>()))
                           .Returns(new TransportistaDto { Id = 1, Cuit = "30-12345678-1" });
            servRepositorio.Setup(s => s.ObtenerProveedor(It.IsAny<int>()))
                           .Returns(new ProveedorDto { Id = 1, Activo = true, Descripcion = "P1", Cuil = "123456"});
            servRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new DocumentoDeImpresionPorCentroDto{FormatoDeImpresionId = 1});

            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirDocumentoDeImpresion>())).Returns(new ResultadoCrear());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Returns(new ResultadoCrear());

            host = WorkflowInvokerTest.Create(target);

            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servcomando.Object);

            host.InArguments.CentroId = 1;
            host.InArguments.CantCopias = 1;
            host.InArguments.PesoNeto = "15000";
            host.InArguments.WorkflowId = Guid.NewGuid();
            host.InArguments.Patente = "AAA001";
            host.InArguments.PuestoDeTrabajoId = 1;
            host.InArguments.EsDestinoCliente = true;
            host.InArguments.CodigoDeImpresion = "CartaPorte";
        }

        [Test]
        public void ExecuteTest()
        {
            host.InArguments.EsUva = true;
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.False(((Resultado)resultado).HayErrores);
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirDocumentoDeImpresion>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        [Test]
        public void ExecuteExceptionsTest()
        {
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ImprimirDocumentoDeImpresion>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception("Error"));
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "2").Value, "Ocurrió un error en la finalización de la actividad");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "Error");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirDocumentoDeImpresion>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        [Test]
        public void ExecuteExceptionsDocumentoNullTest()
        {
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception());
            servRepositorio.Setup(s => s.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                           .Returns((DocumentoDeImpresionPorCentroDto)null);

            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "2").Value, "Ocurrió un error en la finalización de la actividad");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "1").Value, "El Documento no posee formato de impresión");
            Assert.AreEqual(((Resultado)resultado).Errores.First(x => x.Key == "").Value, "Error en la carga del log. El mismo no fue almacenado.");
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ImprimirDocumentoDeImpresion>()), Times.Never());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }
    }
}
