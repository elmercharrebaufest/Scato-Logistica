using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarCartaPorteTest
    {
        private ProcesadorModificarCartaPorte target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private CartaPorteDto dto;
        private CartaPorte cartaPorte;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarCartaPorte(repositorioMock.Object, conversor, new NullLogger());
            dto = new CartaPorteDto
            {
                TipoVehiculo = TipoVehiculo.Camión,
                NroCartaPorte = "77777777",
                CTG = "77777777",
                FechaCP = DateTime.Now.AddDays(2),
                TipoComercial = "Tipo Comercial",
                CEE = "77777777",
                FechaEmision = DateTime.Now,
                FechaVto = DateTime.Now.AddDays(10),
                TitularCartaPorte = "Test",
                Destinatario = "Test",
                Transportista = "Test",
                Chofer = new ChoferDto { Id = 1, Nombre = "Bruce", Apellido = "Wayne", NumeroDeDocumento = "123"},
                Cosecha = "12-13",
                Procedencia = "Laurencena",
                OrigenVehiculo = OrigenVehiculo.Argentina,
                KmRecorrer = 798,
                TarifaTonelada = 12,
                FleteAPagar = true,
                Destino = "4",
                AgenteComprasId = 1,
                BocaDestinoId = 1,
                Vehiculos = new List<VehiculoDto>{ new VehiculoDto{Id = 1,Patente = "222",PatenteAcoplado = "666",PesoBrutoOrigen = 40000,PesoNetoOrigen = 30000,PesoTaraOrigen = 10000}}
            };

            cartaPorte = new CartaPorte
                {
                    TipoVehiculo = TipoVehiculo.Camión,
                    NroCartaPorte = "77777777",
                    CTG = "77777777",
                    FechaCP = DateTime.Now.AddDays(2),
                    TipoComercial = new TipoComercial { Descripcion = "Tipo Comercial" },
                    CEE = "77777777",
                    FechaEmision = DateTime.Now,
                    FechaVto = DateTime.Now.AddDays(10),
                    TitularCartaPorte = new Proveedor { RazonSocial = "Test" },
                    Destinatario = new Proveedor { RazonSocial = "Test" },
                    Transportista = new Transportista { RazonSocial = "Test" },
                    Chofer = new Chofer { Id = 1, Nombre = "Bruce", Apellido = "Wayne", NumeroDeDocumento = "123" },
                    Material = new Material{Descripcion = "Tomate"},
                    Cosecha = "14-15",
                    Procedencia = new Localidad { Descripcion = "Laurencena" },
                    KmRecorrer = 799,
                    TarifaTonelada = 13,
                    FleteAPagar = true,
                    CentroDestino = new Centro{Id = 4},
                    AgenteCompras = new Proveedor { RazonSocial = "Test" },
                    Vehiculos = new List<Vehiculo> { new Vehiculo{Id = 1,Patente = "345"} },
                    Intermediario = new Proveedor(),
                    RtteComercial = new Proveedor(),
                    Entregador = new Entregador(),
                    Corredor = new Proveedor(),
                    TarifaReferencia = 11m,
                    CodigoAnexo = "1223",
                    Cupo = "CUPO"

                };

            

        }

        [Test]
        public void TestCrear()
        {
            var workflows = new List<Workflow> { new Workflow { Id = 1, Codigo = "W1", Descripcion = "D1" } };
            var comando = new ModificarCartaPorte { Orden = dto};

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<LogModificacionDocumentoIngreso, bool>>>())).Returns(new LogModificacionDocumentoIngreso());
            repositorioMock.Setup(s => s.Obtener<CartaPorte>(It.IsAny<int>())).Returns(cartaPorte);
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new List<Recorrido>{new Recorrido { Id = 1,Vehiculo = new Vehiculo{Patente = "12345",PesoBrutoOrigen = 35000,PesoTaraOrigen = 12000}}});
            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>())).Returns(new Chofer {Id = dto.Chofer.Id});
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(new Material {Id = dto.MaterialId});
            repositorioMock.Setup(s => s.Obtener<Centro>(It.Is<int>(i => i == dto.DestinoId))).Returns(new Centro { Id = dto.DestinoId });
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial {Id = dto.TipoComercialId});
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(new Transportista {Id = dto.TransportistaId ?? 0});
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                    .Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());
            comando.Orden.TipoDeWorkflow = TipoDeWorkflow.Egreso;

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Transportista, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CartaPorte, bool>>>())).Returns(false);

            var resultado = target.Ejecutar(comando);
             repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<LogModificacionDocumentoIngreso>()), Times.Never());

        }

        [Test]
        public void TestCrear2()
        {
            dto.EsClienteDestinatario = true;
            var workflows = new List<Workflow> { new Workflow { Id = 1, Codigo = "W1", Descripcion = "D1" } };
            var comando = new ModificarCartaPorte { Orden = dto };

            repositorioMock.Setup(s => s.Obtener<CartaPorte>(It.IsAny<int>())).Returns(cartaPorte);
            repositorioMock.Setup(s => s.Obtener<Cliente>(It.IsAny<int>())).Returns(new Cliente());
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new List<Recorrido> { new Recorrido { Id = 1 } });
            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>())).Returns(new Chofer { Id = dto.Chofer.Id });
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(new Material { Id = dto.MaterialId });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.Is<int>(i => i == dto.DestinoId))).Returns(new Centro { Id = dto.DestinoId });
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial { Id = dto.TipoComercialId });
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(new Transportista { Id = dto.TransportistaId ?? 0 });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>())).Returns<Expression<Func<Workflow, bool>>>(q => workflows.Where((q.Compile())).SingleOrDefault());
            comando.Orden.TipoDeWorkflow = TipoDeWorkflow.Egreso;

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Transportista, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CartaPorte, bool>>>())).Returns(false);

            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<LogModificacionDocumentoIngreso>()), Times.Once());

        }

    }
}
