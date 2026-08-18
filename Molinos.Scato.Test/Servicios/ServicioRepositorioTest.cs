using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Servicios
{
    [TestFixture]
    public class ServicioRepositorioTest
    {
        private ServicioRepositorio target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IFirmaProvider> firmaMock;
        private Mock<ICalculadoraDescuento> calculadora;
        private Mock<IServicioOrquestador> orquestador;
        private AdministradorDeCalles administrador;
        private IConversor conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            firmaMock = new Mock<IFirmaProvider>();
            var config = new Mock<IConfiguracionProvider>();
            calculadora = new Mock<ICalculadoraDescuento>();
            orquestador = new Mock<IServicioOrquestador>();
            administrador = new AdministradorDeCalles(repositorioMock.Object);

            conversor = FactoryConversor.ConversorAutoMapper;
            config.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "TiempoDeDemoraExportaciones", "60" } });
            target = new ServicioRepositorio(repositorioMock.Object, conversor, new NullLogger(), firmaMock.Object, calculadora.Object, config.Object, orquestador.Object, administrador, null);
        }

        [Test]
        public void TestObtenerTipoDocumentoIdentidad()
        {
            var tipo = new TipoDocumentoIdentidad
            {
                Id = 5,
                Descripcion = "Libreta Cívica",
                DescripcionCorta = "LC"
            };

            repositorioMock.Setup(s => s.Obtener<TipoDocumentoIdentidad>(It.IsAny<int>())).Returns(tipo);
            var resultado = target.ObtenerTipoDocumentoIdentidad(tipo.Id);
            Assert.AreEqual(resultado.Id, tipo.Id);
            Assert.AreEqual(resultado.Descripcion, tipo.Descripcion);
            Assert.AreEqual(resultado.DescripcionCorta, tipo.DescripcionCorta);
        }

        [Test]
        public void TestListarTiposDocumentoIdentidadPaginado()
        {
            var tipo = new TipoDocumentoIdentidad
            {
                Id = 5,
                Descripcion = "Libreta Cívica",
                DescripcionCorta = "LC"
            };

            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<TipoDocumentoIdentidad>(null, paginacion)).Returns(new ListaPaginada<TipoDocumentoIdentidad>(new List<TipoDocumentoIdentidad> { tipo, new TipoDocumentoIdentidad { Id = 999, Descripcion = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoTiposDocumentoIdentidad(paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Descripcion, tipo.Descripcion);
        }

        [Test]
        public void TestListarTiposDocumentoIdentidad()
        {
            var tipo = new TipoDocumentoIdentidad
            {
                Id = 5,
                Descripcion = "Libreta Cívica",
                DescripcionCorta = "LC"
            };

            repositorioMock.Setup(s => s.Listar<TipoDocumentoIdentidad>(null)).Returns(new List<TipoDocumentoIdentidad> { tipo, new TipoDocumentoIdentidad { Id = 999, Descripcion = "ZZZ" } });
            var resultado = target.ListarTiposDocumentoIdentidad();
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Descripcion, tipo.Descripcion);
        }

        [Test]
        public void TestObtenerTipoComercial()
        {
            var tipo = new TipoComercial
            {
                Id = 5,
                Descripcion = "T1",
                UsaBinPallet = true,
                PesoEsperado = 3,
                Sentido = "E",
                ToleranciaDifPesoE = 4,
                ValidaPatente = false
            };

            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(tipo);
            var resultado = target.ObtenerTipoComercial(tipo.Id);
            Assert.AreEqual(resultado.Id, tipo.Id);
            Assert.AreEqual(resultado.Descripcion, tipo.Descripcion);
            Assert.AreEqual(resultado.PesoEsperado, tipo.PesoEsperado);
            Assert.AreEqual(resultado.UsaBinPallet, tipo.UsaBinPallet);
            Assert.AreEqual(resultado.ValidaPatente, tipo.ValidaPatente);
            Assert.AreEqual(resultado.ToleranciaDifPesoE, tipo.ToleranciaDifPesoE);
        }

        [Test]
        public void TestListarTiposComerciales()
        {
            var tipo = new TipoComercial
            {
                Id = 5,
                Descripcion = "T1"
            };

            repositorioMock.Setup(s => s.Listar<TipoComercial>(null)).Returns(new List<TipoComercial> { tipo, new TipoComercial { Id = 999, Descripcion = "ZZZ" } });
            var resultado = target.ListarTiposComerciales();
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Descripcion, tipo.Descripcion);
        }

        [Test]
        public void ListarTiposComercialesPorWf()
        {
            repositorioMock.Setup(s => s.Listar<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>())).Returns(new List<Workflow> { new Workflow { Id = 1 } });

            var tipo = new TipoComercialPorWfDto { WorkflowId = 1, TipoComercialId = 1, WorkflowDescripcion = "W1", TipoComercialDescripcion = "T1" };

            var paginacion = new Paginacion(ordenarPor: "WorkflowDescripcion");
            repositorioMock.Setup(s => s.ListarConsultaPaginada(It.IsAny<TipoComercialPorWfConsulta>())).Returns(new ListaPaginada<TipoComercialPorWfDto>(new List<TipoComercialPorWfDto> { tipo, new TipoComercialPorWfDto { WorkflowId = 1, TipoComercialId = 2, WorkflowDescripcion = "W1", TipoComercialDescripcion = "T2" } }, 1, 10, 2));
            var resultado = target.ListarTiposComercialesPorWf(It.IsAny<string>(), It.IsAny<int>(), paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].WorkflowId, tipo.WorkflowId);
            Assert.AreEqual(resultado.Items[0].WorkflowDescripcion, tipo.WorkflowDescripcion);
            Assert.AreEqual(resultado.Items[0].TipoComercialId, tipo.TipoComercialId);
            Assert.AreEqual(resultado.Items[0].TipoComercialDescripcion, tipo.TipoComercialDescripcion);
        }

        [Test]
        public void ListarTiposComercialesPorWfCodigo()
        {
            var workflow = new Workflow { Id = 1, Codigo = "C1", Descripcion = "W1" };
            var tiposComerciales = new List<TipoComercial>
                {
                    new TipoComercial {Id = 1, Descripcion = "D1", WorkflowsAsociados = new List<Workflow>{workflow}},
                    new TipoComercial {Id = 2, Descripcion = "D2", WorkflowsAsociados = new List<Workflow>()}
                };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<TipoComercial, bool>>>())).Returns<Expression<Func<TipoComercial, bool>>>(q => tiposComerciales.Where((q.Compile())).ToList());

            var resultado = target.ListarTiposComercialesPorWfCodigo(workflow.Codigo);
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.First().Id, tiposComerciales[0].Id);
            Assert.AreEqual(resultado.First().Descripcion, tiposComerciales[0].Descripcion);
        }

        [Test]
        public void TestObtenerMaterial()
        {
            var tipo = new Material
            {
                Id = 5,
                Descripcion = "MaterialDesc 1",
                DescripcionCorta = "mat 1"
            };

            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(tipo);
            var resultado = target.ObtenerMaterial(tipo.Id);

            Assert.AreEqual(resultado.Id, tipo.Id);
            Assert.AreEqual(resultado.Descripcion, tipo.Descripcion);
            Assert.AreEqual(resultado.DescripcionCorta, tipo.DescripcionCorta);
        }

        [Test]
        public void TestListarMaterialesPaginado()
        {
            var tipo = new Material
            {
                Id = 5,
                Descripcion = "MaterialDesc 1",
                DescripcionCorta = "mat 1"
            };

            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<MaterialPorCentro>(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>(), paginacion)).Returns(new ListaPaginada<MaterialPorCentro>(new List<MaterialPorCentro> { new MaterialPorCentro { Id = 999, Material = new Material { Descripcion = "MaterialDesc 1" } } }, pagina: 1, itemsPorPagina: 10, itemsTotales: 1));
            var resultado = target.ListarPaginadoMateriales(null, 1, paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 1);
            Assert.AreEqual(resultado.Items[0].Descripcion, tipo.Descripcion);
        }

        //[Test]
        //public void TestObtenerMaterialesPorAlmacen()
        //{
        //    var tipo = new MaterialDesc
        //    {
        //        Id = 5,
        //        Descripcion = "MaterialDesc 1",
        //        DescripcionCorta = "mat 1"
        //    };

        //    var listaMateriales = new List<MaterialDesc> {tipo, new MaterialDesc {Id = 999, Descripcion = "ZZZ"}};
        //    repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<MaterialDesc, bool>>>())).Returns(listaMateriales);

        //    var resultado = target.ObtenerMaterialesPorAlmacen(1);
        //    Assert.IsNotNull(resultado);
        //    Assert.AreEqual(resultado[0].Descripcion, tipo.Descripcion);
        //    Assert.AreEqual(resultado[1].Descripcion, "ZZZ");
        //}

        [Test]
        public void TestObtenerAlmacen()
        {
            var tipo = new Almacen
            {
                Id = 5,
                Descripcion = "Almacen 1",
                DescripcionCorta = "alm 1"
            };

            repositorioMock.Setup(s => s.Obtener<Almacen>(It.IsAny<int>())).Returns(tipo);
            var resultado = target.ObtenerAlmacen(tipo.Id);

            Assert.AreEqual(resultado.Id, tipo.Id);
            Assert.AreEqual(resultado.Descripcion, tipo.Descripcion);
            Assert.AreEqual(resultado.DescripcionCorta, tipo.DescripcionCorta);
        }

        [Test]
        public void TestObtenerTaraRomaneo()
        {
            var tipo = new TaraRomaneo
            {
                Id = 5,
                Descripcion = "tara 1",
                Peso = 50,
                Codigo = 2002
            };

            repositorioMock.Setup(s => s.Obtener<TaraRomaneo>(It.IsAny<int>())).Returns(tipo);
            var resultado = target.ObtenerTaraRomaneo(tipo.Id);

            Assert.AreEqual(resultado.Id, tipo.Id);
            Assert.AreEqual(resultado.Descripcion, tipo.Descripcion);
            Assert.AreEqual(resultado.Peso, tipo.Peso);
            Assert.AreEqual(resultado.Codigo, tipo.Codigo);
        }

        [Test]
        public void TestObtenerTalonarios()
        {
            var tipo = new Talonario
            {
                Id = 5,
                Descripcion = "Talonario 1",
                Sucursal = 50,
                PrimerNumero = 2002,
                UltimoNumero = 3000,
                ProximoNumero = 2003

            };

            repositorioMock.Setup(s => s.Obtener<Talonario>(It.IsAny<int>())).Returns(tipo);
            var resultado = target.ObtenerTalonario(tipo.Id);

            Assert.AreEqual(resultado.Id, tipo.Id);
            Assert.AreEqual(resultado.Descripcion, tipo.Descripcion);
            Assert.AreEqual(resultado.Sucursal, tipo.Sucursal);
            Assert.AreEqual(resultado.UltimoNumero, tipo.UltimoNumero);
            Assert.AreEqual(resultado.ProximoNumero, tipo.ProximoNumero);
        }


        [Test]
        public void TestListarAlmacenesPaginado()
        {
            var tipo = new Almacen
            {
                Id = 5,
                Descripcion = "Almacen 1",
                DescripcionCorta = "alm 1"
            };

            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<Almacen>(It.IsAny<Expression<Func<Almacen, bool>>>(), paginacion)).Returns(new ListaPaginada<Almacen>(new List<Almacen> { tipo, new Almacen { Id = 999, Descripcion = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoAlmacenes(null, paginacion, It.IsAny<int>());
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Descripcion, tipo.Descripcion);
        }

        [Test]
        public void TestListarTaraRomaneoPaginado()
        {
            var tipo = new TaraRomaneo
            {
                Id = 5,
                Descripcion = "tara 1",
                Peso = 55,
                Codigo = 2002
            };

            var paginacion = new Paginacion(ordenarPor: "Codigo");
            repositorioMock.Setup(s => s.Listar<TaraRomaneo>(It.IsAny<Expression<Func<TaraRomaneo, bool>>>(), paginacion)).Returns(new ListaPaginada<TaraRomaneo>(new List<TaraRomaneo> { tipo, new TaraRomaneo() { Id = 999, Codigo = 222 } }, 1, 10, 2));
            var resultado = target.ListarPaginadoTaraRomaneo(null, paginacion, It.IsAny<int>());
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Codigo, tipo.Codigo);
        }

        [Test]
        public void TestListarTalonarioPaginado()
        {
            var tipo = new Talonario
            {
                Id = 5,
                Descripcion = "talonario 1",
                Sucursal = 55,
                PrimerNumero = 2002,
                UltimoNumero = 3000,
                ProximoNumero = 2003
            };

            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<Talonario>(It.IsAny<Expression<Func<Talonario, bool>>>(), paginacion)).Returns(new ListaPaginada<Talonario>(new List<Talonario> { tipo, new Talonario { Id = 999, Descripcion = "talonario" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoTalonario(null, paginacion, It.IsAny<int>());
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Descripcion, tipo.Descripcion);
        }
        [Test]
        public void TestListaAlmacenes()
        {
            var tipo = new Almacen
            {
                Id = 5,
                Descripcion = "Almacen 1",
                DescripcionCorta = "alm 1"
            };

            repositorioMock.Setup(s => s.Listar<Almacen>(null)).Returns(new List<Almacen> { tipo, new Almacen { Id = 999, Descripcion = "ZZZ" } });
            var resultado = target.ListarAlmacenes();
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Descripcion, tipo.Descripcion);
        }

        [Test]
        public void TestObtenerAlmacenesPorCentro()
        {
            var tipo = new Almacen
            {
                Id = 5,
                Descripcion = "Almacen 1",
                DescripcionCorta = "alm 1",
                Centro = new Centro { Id = 1 }
            };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Almacen, bool>>>())).Returns(new List<Almacen> { tipo });

            var resultado = target.ObtenerAlmacenesPorCentro(1);
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado[0].Descripcion, tipo.Descripcion);
        }

        [Test]
        public void TestObtenerCentro()
        {
            var tipo = new Centro
            {
                Id = 5,
                Descripcion = "Centro 1",
            };

            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(tipo);
            var resultado = target.ObtenerCentro(tipo.Id);

            Assert.AreEqual(resultado.Id, tipo.Id);
            Assert.AreEqual(resultado.Descripcion, tipo.Descripcion);
        }


        [Test]
        public void TestObtenerPrecinto()
        {
            var tipo = new Precinto
            {
                Id = 5,
                NumeroPrecinto = "1",
            };

            repositorioMock.Setup(s => s.Obtener<Precinto>(It.IsAny<int>())).Returns(tipo);
            var resultado = target.ObtenerPrecinto(tipo.Id);

            Assert.AreEqual(resultado.Id, tipo.Id);
            Assert.AreEqual(resultado.NumeroPrecinto, tipo.NumeroPrecinto);
        }


        [Test]
        public void TestListaCentros()
        {
            var tipo = new Centro
            {
                Id = 5,
                Descripcion = "Centro 1",
            };

            repositorioMock.Setup(s => s.Listar<Centro>(null)).Returns(new List<Centro> { tipo, new Centro { Id = 999, Descripcion = "ZZZ" } });
            var resultado = target.ListarCentros();
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Descripcion, tipo.Descripcion);
        }

        [Test]
        public void TestListaCentrosPaginado()
        {
            var centro = new Centro
            {
                Id = 5,
                Descripcion = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<Centro>(null, paginacion)).Returns(new ListaPaginada<Centro>(new List<Centro> { centro, new Centro { Id = 999, Descripcion = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoCentros(paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Descripcion, centro.Descripcion);
        }

        [Test]
        public void TestListaProvincias()
        {
            var tipo = new Provincia
            {
                Id = 5,
                Descripcion = "Buenos Aires",
            };

            repositorioMock.Setup(s => s.Listar<Provincia>(null)).Returns(new List<Provincia> { tipo, new Provincia { Id = 999, Descripcion = "Mendoza" } });
            var resultado = target.ListarProvincias();
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Descripcion, tipo.Descripcion);
        }
        [Test]
        public void TestListaLocalidades()
        {
            var tipo = new Localidad
            {
                Id = 5,
                Descripcion = "Capital Federal",
            };

            repositorioMock.Setup(s => s.Listar<Localidad>(null)).Returns(new List<Localidad> { tipo, new Localidad { Id = 999, Descripcion = "Mendoza" } });
            var resultado = target.ListarLocalidades();
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Descripcion, tipo.Descripcion);
        }

        [Test]
        public void TestListarPrecintos()
        {
            var tipo = new Precinto
            {
                Id = 5,
                NumeroPrecinto = "1"
            };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Precinto, bool>>>())).Returns(new List<Precinto> { tipo });

            var resultado = target.ListarPrecintos(It.IsAny<Guid>());
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado[0].NumeroPrecinto, tipo.NumeroPrecinto);
        }

        [Test]
        public void TestObtenerObservacion()
        {
            var tipo = new Observacion
            {
                Id = 1,
                WorkflowInstanceId = new Guid(),
                Observaciones = "Observaciones 1"
            };

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Observacion, bool>>>())).Returns(tipo);

            var resultado = target.ObtenerObservacion(new Guid());
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.Observaciones, tipo.Observaciones);
        }


        [Test]
        public void TestObtenerEntregador()
        {
            var entregador = new Entregador
            {
                Id = 1,
                DescripcionCorta = "Entregador"
            };

            repositorioMock.Setup(s => s.Obtener<Entregador>(It.IsAny<int>())).Returns(entregador);

            var resultado = target.ObtenerEntregador(It.IsAny<int>());
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.DescripcionCorta, entregador.DescripcionCorta);
        }

        [Test]
        public void TestListarEntregadores()
        {
            var entregador = new Entregador
            {
                Id = 1,
                DescripcionCorta = "Entregador"
            };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Entregador, bool>>>(), It.IsAny<Paginacion>())).Returns(new ListaPaginada<Entregador>(new List<Entregador> { entregador }, 1, 10, 1));

            var resultado = target.ListarEntregadores(It.IsAny<string>(), It.IsAny<Paginacion>());
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.First().DescripcionCorta, entregador.DescripcionCorta);
        }

        [Test]
        public void ListarTransmisionesASapTest()
        {
            var tipo = new TransmisionASapDto
            {
                Id = 1,
                Estado = EstadoTransmisionASap.Correcto,
                InstanciaWorkflow = new Guid(),
                FuncionSap = FuncionSAP.AjusteDeDiferencias,
                Fecha = new DateTime(2010, 1, 1),
                Patente = "AAA222",
                TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                NumeroDocumento = "1234"
            };

            var paginacion = new Paginacion(ordenarPor: "Fecha");
            repositorioMock.Setup(s => s.ListarConsultaPaginada(It.IsAny<TransmisionASapConsulta>())).Returns(new ListaPaginada<TransmisionASapDto>(new List<TransmisionASapDto> { tipo, new TransmisionASapDto() }, 1, 10, 2));
            var resultado = target.ListarTransmisionesASap(new FiltroPanelDeTransaccionesSapDto(), paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Estado, tipo.Estado);
            Assert.AreEqual(resultado.Items[0].Fecha, tipo.Fecha);
        }

        [Test]
        public void TestBuscarProcedencia()
        {
            var localidades = new List<Localidad>
                {
                    new Localidad {Id = 10, CodigoAfip = "101", Descripcion = "Una Localidad"},
                    new Localidad {Id = 20, CodigoAfip = "102", Descripcion = "Otra Localidad"},
                    new Localidad {Id = 30, CodigoAfip = "103", Descripcion = "Otra Mas"}
                };

            repositorioMock
                .Setup(r => r.Listar(It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<int>()))
                .Returns<Expression<Func<Localidad, bool>>, int>(
                               (cond, max) => localidades.Where(cond.Compile()).ToList());

            var resultado = target.BuscarProcedencias("Una");
            Assert.That(resultado, Has.Count.EqualTo(1));
            Assert.That(resultado[0].Id, Is.EqualTo(10));
            Assert.That(resultado[0].Descripcion, Is.EqualTo("Una Localidad"));

            resultado = target.BuscarProcedencias("Localidad");
            Assert.That(resultado, Has.Count.EqualTo(2));
            Assert.That(resultado[0].Id, Is.EqualTo(10));
            Assert.That(resultado[0].Descripcion, Is.EqualTo("Una Localidad"));
            Assert.That(resultado[1].Id, Is.EqualTo(20));
            Assert.That(resultado[1].Descripcion, Is.EqualTo("Otra Localidad"));

            resultado = target.BuscarProcedencias("30");
            Assert.That(resultado, Is.Empty);

            resultado = target.BuscarProcedencias("3");
            Assert.That(resultado, Has.Count.EqualTo(1));
            Assert.That(resultado[0].Id, Is.EqualTo(30));
            Assert.That(resultado[0].Descripcion, Is.EqualTo("Otra Mas"));
        }

        [Test]
        public void TestListarAnalisisYCaladoPorId()
        {
            var caracteristicasDeCalidad = new List<CaracteristicaDeCalidad>
                {
                    new CaracteristicaDeCalidad
                        {
                            Id = 1,
                            EsHumedad = false,
                            CaracteristicaDeCalidadMaestro = new CaracteristicaDeCalidadMaestro{ Descripcion = "Car1" }
                        },
                    new CaracteristicaDeCalidad
                        {
                            Id = 2,
                            EsHumedad = true,
                            CaracteristicaDeCalidadMaestro = new CaracteristicaDeCalidadMaestro{ Descripcion = "Hum" }
                        }
                };

            var caladosPorCaracteristicas = new List<CaladoPorCaracteristica>
                {
                    new CaladoPorCaracteristica
                        {
                            Id = 1,
                            CaracteristicaDeCalidad = caracteristicasDeCalidad[0],
                            ValorCalado = 40,
                            AnalisisPreliminar = false
                        },
                    new CaladoPorCaracteristica
                        {
                            Id = 2,
                            CaracteristicaDeCalidad = caracteristicasDeCalidad[1],
                            ValorCalado = 40,
                            AnalisisPreliminar = true
                        }
                };

            var analisisPorCaracteristicas = new List<AnalisisPorCaracteristica>
                {
                    new AnalisisPorCaracteristica
                        {
                            Id = 1,
                            CaracteristicaDeCalidad = caracteristicasDeCalidad[1],
                            ValorAnalisis = 80
                        }
                };

            var calado = new Calado
            {
                CaladosPorCaracteristica = caladosPorCaracteristicas,
                CicloDeCalado = 1,
            };

            var analisis = new List<Dominio.Entidades.AnalisisDeCalidad>
                {
                    new Dominio.Entidades.AnalisisDeCalidad
                        {
                            CaracteristicasAnalizadas = analisisPorCaracteristicas,
                            Calado = calado,
                            Id = 1
                        }
                };

            repositorioMock.Setup(s => s.Obtener<Calado>(It.IsAny<int>())).Returns(calado);
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, bool>>>()))
                .Returns<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, bool>>>(q => analisis.Where(q.Compile()).ToList());

            var resultado = target.ListarAnalisisYCaladoPorId(It.IsAny<int>());

            Assert.That(resultado.Count, Is.EqualTo(2));
            Assert.That(resultado.First().ValorCalado, Is.EqualTo(40));
            Assert.That(resultado.Last().ValorAnalisis, Is.EqualTo(80));
        }

        [Test]
        public void TestObtenerOrdenDeDescargaPorInstanceId()
        {
            var ordenDeDescarga = new OrdenDeDescarga { Id = 1 };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>())).Returns(ordenDeDescarga);
            var resultado = target.ObtenerOrdenDeDescargaPorInstanceId(It.IsAny<Guid>());
            Assert.AreEqual(resultado.Id, ordenDeDescarga.Id);
        }

        [Test]
        public void TestObtenerOrdenDeDescargaPorInstanceIdSinResultado()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns((string)null);
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>())).Returns((OrdenDeDescarga)null);
            var resultado = target.ObtenerOrdenDeDescargaPorInstanceId(It.IsAny<Guid>());
            Assert.AreEqual(resultado, null);
        }

        [Test]
        public void TestListarCasillerosPorCentro()
        {
            var listaCasilleros = new List<Casillero>
                {
                    new Casillero
                        {
                            Id = 1,
                            Numero = "1111-000001",
                            Capacidad = 3,
                            Centro = new Centro {Id = 1}
                        },
                    new Casillero
                        {
                            Id = 2,
                            Numero = "1111-000002",
                            Capacidad = 3,
                            Centro = new Centro {Id = 1}
                        }
                };

            repositorioMock.Setup(s => s.Listar<Casillero>(It.IsAny<Expression<Func<Casillero, bool>>>())).Returns(listaCasilleros);

            var resultado = target.ListarCasillerosPorCentro(1);
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Numero, "1111-000001");
        }

        [Test]
        public void TestListarMicroMuestrasPorCasilleroPorCentro()
        {
            var listaMicroMuestrasPorCasillero = new List<MicroMuestrasPorCasillero>
                {
                    new MicroMuestrasPorCasillero
                        {
                            Id = 1,
                            Muestra = new MuestraEnvioACamara { Id = 1 },
                            Casillero = new Casillero { Id = 1, Centro = new Centro {Id = 1} }
                        },
                    new MicroMuestrasPorCasillero
                        {
                            Id = 2,
                            Muestra = new MuestraEnvioACamara { Id = 1 },
                            Casillero = new Casillero { Id = 1, Centro = new Centro {Id = 1}}
                        }
                };

            repositorioMock.Setup(s => s.Listar<MicroMuestrasPorCasillero>(It.IsAny<Expression<Func<MicroMuestrasPorCasillero, bool>>>())).Returns(listaMicroMuestrasPorCasillero);
            var resultado = target.ListarMicroMuestrasPorCasilleroPorCentro(1);
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].MuestraId, 1);
        }

        [Test]
        public void TestListarLiberacionDeCasilleros()
        {
            var listaLiberacionDeCasilleros = new List<LiberacionDeCasillerosDto>
                {
                    new LiberacionDeCasillerosDto
                        {
                            Id = 1,
                            NDeCasillero = "1111-000001",
                            TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte.ToString(),
                            NumeroDocumento = "122412412412",
                            Patente = "LKF450"
                        },
                    new LiberacionDeCasillerosDto
                        {
                            Id = 2,
                            NDeCasillero = "1111-000002",
                            TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte.ToString(),
                            NumeroDocumento = "567892412412",
                            Patente = "FGH321"
                        }
                };

            var paginacion = new Paginacion(ordenarPor: "NDeCasillero");
            repositorioMock.Setup(s => s.ListarConsultaPaginada(It.IsAny<LiberacionDeCasillerosConsulta>())).Returns(new ListaPaginada<LiberacionDeCasillerosDto>(listaLiberacionDeCasilleros, 1, 10, 2));
            var resultado = target.ListarLiberacionDeCasilleros(new LiberacionDeCasillerosDto(), paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].NDeCasillero, "1111-000001");
            Assert.AreEqual(resultado.Items[0].NumeroDocumento, "122412412412");
        }

        [Test]
        public void TestListarConsultaCasillerosPorAntiguedad()
        {
            var listaConsultaCasillerosPorAntiguedad = new List<ConsultaCasilleroAntiguedadDto>
                {
                    new ConsultaCasilleroAntiguedadDto
                        {
                            Centro = "Chivilcoy",
                            OcupadosMas = "1(33%)",
                            OcupadosMenos = "1(33%)",
                            Libres = "1"
                        },
                    new ConsultaCasilleroAntiguedadDto
                        {
                            Centro = "Luccheti",
                            OcupadosMas = "2(100%)",
                            OcupadosMenos = "0",
                            Libres = "0"
                        }
                };

            var paginacion = new Paginacion(ordenarPor: "Centro");
            repositorioMock.Setup(s => s.ListarConsultaPaginada(It.IsAny<CasillerosPorAntiguedadConsulta>())).Returns(new ListaPaginada<ConsultaCasilleroAntiguedadDto>(listaConsultaCasillerosPorAntiguedad, 1, 10, 2));
            var resultado = target.ListarConsultaCasillerosPorAntiguedad(new ConsultaCasilleroAntiguedadDto(), paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Centro, "Chivilcoy");
            Assert.AreEqual(resultado.Items[0].Libres, "1");
        }

        [Test]
        public void TestListarConsultaCasillerosPorCasillero()
        {
            var consultaCasilleros = new List<ConsultaCasilleroDto>
                {
                    new ConsultaCasilleroDto
                        {
                            NDeCasillero = "1111-000001",
                            CantMuestra = "2",
                            TipoDocumento = TipoDocumentoIngreso.CartaPorte.ToString(),
                            NumeroDocumento = "412412412412",
                            Patente = "JRK412"
                        },
                    new ConsultaCasilleroDto
                        {
                            NDeCasillero = "1111-000002",
                            CantMuestra = "2",
                            TipoDocumento = TipoDocumentoIngreso.Remito.ToString(),
                            NumeroDocumento = "122412412412",
                            Patente = "LKF450"
                        }
                };

            var paginacion = new Paginacion(ordenarPor: "NDeCasillero");
            repositorioMock.Setup(s => s.ListarConsultaPaginada(It.IsAny<CasillerosPorCasilleroConsulta>())).Returns(new ListaPaginada<ConsultaCasilleroDto>(consultaCasilleros, 1, 10, 2));
            var resultado = target.ListarConsultaCasillerosPorCasillero(new ConsultaCasilleroDto(), paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].NDeCasillero, "1111-000001");
            Assert.AreEqual(resultado.Items[0].Patente, "JRK412");
        }

        [Test]
        public void TestListarConsultaCasillerosPorMuestra()
        {
            var consultaCasillerosMuestra = new List<ConsultaCasilleroMuestraDto>
                {
                    new ConsultaCasilleroMuestraDto
                        {
                            NumeroDocumento = "123456789123",
                            Patente = "PHJ890",
                            NDeCasillero = "1111-000001",
                            CantMuestra = "2"
                        },
                    new ConsultaCasilleroMuestraDto
                        {
                            NumeroDocumento = "765456789123",
                            Patente = "JRK412",
                            NDeCasillero = "1111-000001",
                            CantMuestra = "2"
                        }
                };

            var paginacion = new Paginacion(ordenarPor: "NumeroDocumento");
            repositorioMock.Setup(s => s.ListarConsultaPaginada(It.IsAny<CasillerosPorMuestraConsulta>())).Returns(new ListaPaginada<ConsultaCasilleroMuestraDto>(consultaCasillerosMuestra, 1, 10, 2));
            var resultado = target.ListarConsultaCasillerosPorMuestra(new ConsultaCasilleroMuestraDto(), paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].NumeroDocumento, "123456789123");
            Assert.AreEqual(resultado.Items[0].Patente, "PHJ890");
        }

        [Test]
        public void ObtenerEmpresa()
        {
            repositorioMock.Setup(s => s.Obtener<Empresa>(It.IsAny<int>())).Returns(new Empresa { Nombre = "M" });

            var result = target.ObtenerEmpresa(1);
            Assert.AreEqual(result.Nombre, "M");
        }

        [Test]
        public void ObtenerTecnologia()
        {
            repositorioMock.Setup(s => s.Obtener<Tecnologia>(It.IsAny<int>())).Returns(new Tecnologia { Codigo = "T" });

            var result = target.ObtenerTecnologia(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Codigo, "T");
        }

        [Test]
        public void RequiereTecnologia()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new Recorrido());

            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>(),
                                    It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Returns(true);

            var result = target.RequiereTecnologia(Guid.NewGuid());
            Assert.True(result);
        }

        [Test]
        public void CorrespondeRegistrarCartadePorteTren()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, int>>>())).Returns(1);
            var guid = Guid.NewGuid();
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, Guid>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Guid> { guid });
            repositorioMock.Setup(
                s =>
                s.ObtenerMenor(It.IsAny<Expression<Func<LogActividad, bool>>>(),
                               It.IsAny<Expression<Func<LogActividad, int>>>(),
                               It.IsAny<Expression<Func<LogActividad, Guid>>>())).Returns(guid);

            var result = target.CorrespondeRegistrarCartadePorteTren(guid);

            Assert.NotNull(result);
            Assert.True(result);
        }

        [Test]
        public void ObtenerRecorridoGuidPorNumeroCiu()
        {
            var guid = new Guid();
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, Guid>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new List<Guid> { guid });

            var resultado = target.ObtenerRecorridoGuidPorNumeroCiu("111");
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, guid);
        }

        [Test]
        public void ObtenerCantidadVagones()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, int>>>())).Returns(1);
            repositorioMock.Setup(s => s.Contar<Vehiculo>(It.IsAny<Expression<Func<Vehiculo, bool>>>())).Returns(1);
            var result = target.ObtenerCantidadVagones(Guid.NewGuid());
            Assert.NotNull(result);
            Assert.AreEqual(result, 1);
        }

        [Test]
        public void ObtenerRecorridoGuidPorNumeroCiuNull()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, Guid>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new List<Guid> { });

            var resultado = target.ObtenerRecorridoGuidPorNumeroCiu("111");
            Assert.AreEqual(resultado, Guid.Empty);
        }

        [Test]
        public void ValidarProximaActividadPorPuesto()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, Guid>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new List<Guid> { });

            repositorioMock.Setup(
                s =>
                s.ObtenerConsultaEscalar(It.IsAny<IConsultaEscalar<bool>>())).Returns(true);

            repositorioMock.Setup(
                s =>
                s.Existe(It.IsAny<Expression<Func<ActividadPorDispositivo, bool>>>())).Returns(true);

            repositorioMock.Setup(
                x => x.Listar(It.IsAny<Expression<Func<ActividadPorDispositivo, int>>>(),
                               It.IsAny<Expression<Func<ActividadPorDispositivo, bool>>>())).Returns(new List<int>() { 1 });
            ICollection<VideoCamara> camaras = new List<VideoCamara>() { new VideoCamara { Codigo = "a", Directorio = "b" } };
            repositorioMock.Setup(
                s =>
                s.Obtener<PuestoDeTrabajo>(It.IsAny<int>())).Returns(new PuestoDeTrabajo() { VideoCamaras = camaras, Entrada = "c" });
            var recorrido = new DatosRecorridoDto
            {
                InstanciaWorkflow = new Guid(),
                TarjetaDeAcceso = "1111",
                WorkflowDefinicionId = 0,
                CentroCodigoSap = "1234"
            };
            var resultado = target.ValidarProximaActividadPorPuesto(recorrido, "Actividad", new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, Lectura = "1111" } }, "usuario");
            Assert.AreEqual(resultado.VideoCamaras.First().Codigo, "a");
            Assert.AreEqual(resultado.VideoCamaras.First().Directorio, "b");
        }

        [Test]
        public void ObtenerTransmisionASapPorIdyFuncionSap()
        {
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>()))
                       .Returns(new TransmisionASap
                       {
                           Id = 1,
                           Estado = EstadoTransmisionASap.Correcto,
                           FuncionSap = FuncionSAP.AjusteDeDiferencias
                       });
            var result = target.ObtenerTransmisionASapPorIdyFuncionSap(1);
            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
            string val;
            Assert.True(result.Campos.TryGetValue("FuncionSap", out val));
            Assert.NotNull(val);
            Assert.AreEqual(val, FuncionSAP.AjusteDeDiferencias.ToString());
            Assert.True(result.Campos.TryGetValue("Estado", out val));
            Assert.NotNull(val);
            Assert.AreEqual(val, EstadoTransmisionASap.Correcto.ToString());
        }

        [Test]
        public void ListarSubZonasPorZona()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<SubZona, bool>>>()))
                           .Returns(new List<SubZona> { new SubZona { Descripcion = "SZ", Id = 1, Zona = new Zona { Descripcion = "Z", Id = 1 } } });
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<SubZona, SubZonaDto>>>(), It.IsAny<Expression<Func<SubZona, bool>>>()))
                           .Returns(new List<SubZonaDto>
                               {
                                   new SubZonaDto
                                       {
                                           Descripcion = "SZ",
                                           Id = 1,
                                           Zona = new ZonaDto {Descripcion = "Z", Id = 1}
                                       }
                               });

            var result = target.ListarSubZonasPorZona(1);
            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Descripcion, "SZ");
            Assert.AreEqual(result.FirstOrDefault().Id, 1);
            Assert.AreEqual(result.FirstOrDefault().Zona.Descripcion, "Z");
            Assert.AreEqual(result.FirstOrDefault().Zona.Id, 1);

        }

        [Test]
        public void ListarZonas()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Zona, bool>>>()))
                           .Returns(new List<Zona> { new Zona() { Descripcion = "Z", Id = 1 } });
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Zona, ZonaDto>>>(),
                                        It.IsAny<Expression<Func<Zona, bool>>>()))
                           .Returns(new List<ZonaDto> { new ZonaDto { Descripcion = "Z", Id = 1 } });
            var result = target.ListarZonas();
            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Descripcion, "Z");
            Assert.AreEqual(result.FirstOrDefault().Id, 1);
        }

        [Test]
        public void ObtenerTransmisionesCuposIdsConError()
        {
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<TransmisionASap, int>>>(),
                         It.IsAny<Expression<Func<TransmisionASap, bool>>>())).Returns(new List<int> { 1, 2 });

            var result = target.ObtenerTransmisionesCuposIdsConError();
            Assert.NotNull(result);
            Assert.AreEqual(result[0], 1);
            Assert.AreEqual(result[1], 2);

        }

        [Test]
        public void ObtenerAnalisisPorVagones()
        {
            var guid = Guid.NewGuid();
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                                    It.IsAny<Expression<Func<Recorrido, int>>>())).Returns(1);
            repositorioMock.Setup(s => s.Listar<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Recorrido>
                               {
                                   new Recorrido
                                       {
                                           Id = 1,
                                           InstanciaWorkflow = guid,
                                           AnalisisDeCalidad =
                                               new Dominio.Entidades.AnalisisDeCalidad {Id = 1, NumeroOrden = "123"},
                                           Calado = new Calado {NumeroOrden = "1234"},
                                           Vehiculo = new Vehiculo {NumeroVehiculo = 12}
                                       }
                               });
            var result = target.ObtenerAnalisisPorVagones(guid);


            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().AnalisisDeCalidad.NumeroOrden, "123");
        }

        [Test]
        public void CorrespondeRegistrarMuestreoYPesajeTren()
        {
            var guid = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, int>>>())).Returns(1);
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, Guid>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Guid> { guid, guid2 });
            repositorioMock.Setup(
                s =>
                s.ObtenerMayor(It.IsAny<Expression<Func<LogActividad, bool>>>(),
                                                        It.IsAny<Expression<Func<LogActividad, int>>>(),
                                                        It.IsAny<Expression<Func<LogActividad, Guid>>>())).Returns(guid);
            repositorioMock.Setup(s => s.Contar(It.IsAny<Expression<Func<LogActividad, bool>>>()))
                           .Returns(2);

            var result = target.CorrespondeRegistrarMuestreoYPesajeTren(guid);
            Assert.NotNull(result);
            Assert.True(result);
        }

        [Test]
        public void ObtenerCartaDePorteRegistradaServicioMonsantoVehiculoCamion()
        {
            var convert = new Mock<IConversor>();
            convert.Setup(
                s =>
                s.Convertir<CartaDePorteRegistradaServicioMonsanto, CartaDePorteRegistradaServicioMonsantoDto>(
                    It.IsAny<CartaDePorteRegistradaServicioMonsanto>()))
                   .Returns(new CartaDePorteRegistradaServicioMonsantoDto { Id = 1, LaboratorioRazonSocial = "Lab" });

            var guid = Guid.NewGuid();
            repositorioMock.Setup(
                s =>
                s.Obtener<CartaDePorteRegistradaServicioMonsanto>(
                    It.IsAny<Expression<Func<CartaDePorteRegistradaServicioMonsanto, bool>>>()))
                           .Returns(new CartaDePorteRegistradaServicioMonsanto
                           {
                               Id = 1,
                               LaboratorioRazonSocial = "Lab"
                           });
            target = new ServicioRepositorio(repositorioMock.Object, conversor, new NullLogger(), firmaMock.Object, calculadora.Object, null, orquestador.Object, administrador, null);
            var result = target.ObtenerCartaDePorteRegistradaServicioMonsanto(guid, TipoVehiculo.Camión);

            Assert.NotNull(result);
            Assert.AreEqual(result.LaboratorioRazonSocial, "Lab");
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerCartaDePorteRegistradaServicioMonsantoVehiculoOtro()
        {
            var convert = new Mock<IConversor>();
            convert.Setup(
                s =>
                s.Convertir<CartaDePorteRegistradaServicioMonsanto, CartaDePorteRegistradaServicioMonsantoDto>(
                    It.IsAny<CartaDePorteRegistradaServicioMonsanto>()))
                   .Returns(new CartaDePorteRegistradaServicioMonsantoDto { Id = 1, LaboratorioRazonSocial = "Lab" });

            var guid = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, int>>>())).Returns(1);
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, Guid>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Guid> { guid, guid2 });

            repositorioMock.Setup(
                s =>
                s.ObtenerMayor(It.IsAny<Expression<Func<CartaDePorteRegistradaServicioMonsanto, bool>>>(),
                               It.IsAny<Expression<Func<CartaDePorteRegistradaServicioMonsanto, int>>>()))
                           .Returns(new CartaDePorteRegistradaServicioMonsanto());

            target = new ServicioRepositorio(repositorioMock.Object, convert.Object, new NullLogger(), firmaMock.Object, null, null, null, administrador, null);
            var result = target.ObtenerCartaDePorteRegistradaServicioMonsanto(guid, TipoVehiculo.Tren);

            Assert.NotNull(result);
            Assert.AreEqual(result.LaboratorioRazonSocial, "Lab");
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerRecorridoInstanceIdPorRecorridoId()
        {
            var guid = Guid.NewGuid();
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, Guid>>>())).Returns(guid);

            var result = target.ObtenerRecorridoInstanceIdPorRecorridoId(1);
            Assert.NotNull(result);
            Assert.AreEqual(result, guid);
        }

        [Test]
        public void TienePermiso()
        {
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<IConsultaEscalar<bool>>())).Returns(true);

            var result = target.TienePermiso("s", PermisosScato.AbmActividadConCargaAutomatica);
            Assert.True(result);
        }

        [Test]
        public void CartaPorteTieneEntregador()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<CartaPorte, bool>>>(),
                                    It.IsAny<Expression<Func<CartaPorte, bool>>>())).Returns(true);

            var result = target.CartaPorteTieneEntregador(1);
            Assert.True(result);
        }

        [Test]
        public void ObtenerHumedimetroPorNombrePc()
        {
            var convert = new Mock<IConversor>();
            convert.Setup(s => s.Convertir<Humedimetro, HumedimetroDto>(It.IsAny<Humedimetro>()))
                   .Returns(new HumedimetroDto { CentroId = 1, Codigo = "H" });
            repositorioMock.Setup(s => s.Obtener<Humedimetro>(It.IsAny<Expression<Func<Humedimetro, bool>>>()))
                           .Returns(new Humedimetro { Centro = new Centro { Id = 1 } , Codigo = "H" });
            target = new ServicioRepositorio(repositorioMock.Object, conversor, new NullLogger(), firmaMock.Object, calculadora.Object, null, null, administrador, null);
            var result = target.ObtenerHumedimetroPorNombrePc(1, "P");
            Assert.NotNull(result);
            Assert.AreEqual(result.CentroId, 1);
            Assert.AreEqual(result.Codigo, "H");
        }

        [Test]
        public void EsProveedorExcluidoIntacta()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<ProveedorExcluidoIntacta, bool>>>())).Returns(true);

            var result = target.EsProveedorExcluidoIntacta(1);

            Assert.True(result);
        }

        [Test]
        public void ObtenerUltimaImpresionPorInstanceIdYCodigoNotNull()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Impresion, bool>>>()))
                           .Returns(new ImpReciboMunicipal { Id = 1 });

            var result = target.ObtenerUltimaImpresionPorInstanceIdYCodigo(Guid.NewGuid(), "C");
            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerUltimaImpresionPorInstanceIdYCodigoNull()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Impresion, bool>>>()))
                           .Returns((ImpReciboMunicipal)null);

            var result = target.ObtenerUltimaImpresionPorInstanceIdYCodigo(Guid.NewGuid(), "C");
            Assert.Null(result);
        }

        [Test]
        public void ConsultaControlRecorridoLogActividad()
        {
            repositorioMock.Setup(s => s.ListarConsulta(It.IsAny<ControlRecorridoLogActividadConsulta>()))
                           .Returns(new List<ControlRecorridoLogActividadConsultaDto>
                               {
                                   new ControlRecorridoLogActividadConsultaDto
                                       {
                                           Actividad = "actividad",
                                           Comentario = "comentario",
                                           Fecha = new DateTime(2015, 8, 8),
                                           Tabla = "LogActividad",
                                           Usuario = ""
                                       },
                                   new ControlRecorridoLogActividadConsultaDto
                                       {
                                           Actividad = "actividad",
                                           Comentario = "comentario",
                                           Fecha = new DateTime(2015, 8, 8),
                                           Tabla = "ControlRecorrido",
                                           Usuario = ""
                                       }
                               });

            var result = target.ConsultaControlRecorridoLogActividad(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result[0].Actividad, "actividad");
            Assert.AreEqual(result[0].Tabla, "LogActividad");
        }

        [Test]
        public void ListarRecorridosEnPlayaExternaPorCentro()
        {
            var instance = Guid.NewGuid();
            var listapg =
                new ListaPaginada<InstanciaWorkflowDto>(
                    new List<InstanciaWorkflowDto>
                        {
                            new InstanciaWorkflowDto {CentroId = 1, Id = instance, Patente = "aaa111"}
                        }, 1, 1, 1);

            repositorioMock.Setup(s => s.ListarConsultaPaginada(It.IsAny<ListarRecorridosEnPlayaExterna>()))
                           .Returns(listapg);
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Returns(new List<Workflow> { new Workflow { Id = 1, Descripcion = "w", Activo = true } });

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Workflow, WorkflowDto>>>(),
                         It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Returns(new List<WorkflowDto> { new WorkflowDto { Id = 1, Descripcion = "w", Activo = true } });

            var result = target.ListarRecorridosEnPlayaExternaPorCentro(new FiltroListaDeWorkflowsDto(),
                                                                        new Paginacion());

            Assert.NotNull(result);
            Assert.AreEqual(result.Workflows.Items, listapg);
        }

        [Test]
        public void ListarPaginadoProveedorExcluidoIntacta()
        {
            var listapg = new ListaPaginada<ProveedorExcluidoIntactaDto>(new List<ProveedorExcluidoIntactaDto>
                {
                    new ProveedorExcluidoIntactaDto
                        {
                            CodigoSap = "ppp",
                            Cuil = "1234",
                            Id = 1,
                            Proveedor = "p",
                            RazonSocial = "PEx",
                            ProveedorId = 1
                        }
                }, 1, 1, 1);


            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<ProveedorExcluidoIntacta, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<ProveedorExcluidoIntacta>(new List<ProveedorExcluidoIntacta>
                               {
                                   new ProveedorExcluidoIntacta
                                       {
                                           Id = 1,
                                           Proveedor = new Proveedor
                                               {
                                                   CodigoSap = "ppp",
                                                   Cuil = "1234",
                                                   Id = 1,
                                                   RazonSocial = "PEx"
                                               }
                                       }
                               }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<ProveedorExcluidoIntacta, ProveedorExcluidoIntactaDto>>>(),
                         It.IsAny<Expression<Func<ProveedorExcluidoIntacta, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(listapg);

            var result = target.ListarPaginadoProveedorExcluidoIntacta("a", new Paginacion());
            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().Cuil, "1234");
        }


        [Test]
        public void ListarPaginadoTecnologia()
        {
            var listapg = new ListaPaginada<TecnologiaDto>(new List<TecnologiaDto>
                {
                    new TecnologiaDto
                        {
                            Id = 1,
                            Codigo = "c",
                            Nombre = "Tec"
                        }
                }, 1, 1, 1);


            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Tecnologia, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<Tecnologia>(new List<Tecnologia>
                               {
                                   new Tecnologia
                                       {
                                           Id = 1,
                                           Codigo = "c",
                                           Nombre = "Tec"
                                       }
                               }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Tecnologia, TecnologiaDto>>>(),
                         It.IsAny<Expression<Func<Tecnologia, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(listapg);

            var result = target.ListarPaginadoTecnologia("a", new Paginacion());
            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().Nombre, "Tec");
        }


        [Test]
        public void ListarPaginadoEmpresa()
        {
            var listapg = new ListaPaginada<EmpresaDto>(new List<EmpresaDto>
                {
                    new EmpresaDto
                        {
                            Id = 1,
                            Nombre = "Emp"
                        }
                }, 1, 1, 1);


            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Empresa, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<Empresa>(new List<Empresa>
                               {
                                   new Empresa
                                       {
                                           Id = 1,
                                           Nombre = "Emp"
                                       }
                               }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Empresa, EmpresaDto>>>(),
                         It.IsAny<Expression<Func<Empresa, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(listapg);

            var result = target.ListarPaginadoEmpresa("a", new Paginacion());
            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().Nombre, "Emp");
        }

        [Test]
        public void ListarEmpresas()
        {
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Empresa, bool>>>(), It.IsAny<int>()))
                           .Returns(new List<Empresa> { new Empresa { Id = 1, Nombre = "Emp" } });

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Empresa, EmpresaDto>>>(),
                         It.IsAny<Expression<Func<Empresa, bool>>>(), It.IsAny<int>()))
                           .Returns(new List<EmpresaDto> { new EmpresaDto { Id = 1, Nombre = "Emp" } });

            var result = target.ListarEmpresas();

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Nombre, "Emp");
        }

        [Test]
        public void ListarTecnologias()
        {
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Tecnologia, bool>>>(), It.IsAny<int>()))
                           .Returns(new List<Tecnologia> { new Tecnologia { Id = 1, Nombre = "Tec" } });

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Tecnologia, TecnologiaDto>>>(),
                         It.IsAny<Expression<Func<Tecnologia, bool>>>(), It.IsAny<int>()))
                           .Returns(new List<TecnologiaDto> { new TecnologiaDto { Id = 1, Nombre = "Emp" } });

            var result = target.ListarTecnologias();

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Nombre, "Tec");
        }

        [Test]
        public void RecorridoPagaTicketMunicipal()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, bool?>>>())).Returns(true);

            var result = target.RecorridoPagaTicketMunicipal(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.True((bool)result);
        }

        [Test]
        public void VerificarPuestodeCargaDescarga()
        {
            repositorioMock.Setup(
                s =>
                s.Existe(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            var result = target.VerificarPuestodeCargaDescarga(Guid.NewGuid(), 1);

            Assert.NotNull(result);
            Assert.True(result);
        }

        [Test]
        public void ObtenerPuestosIdPorPC()
        {
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<PuestoDeTrabajo, int>>>(),
                         It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new List<int> { 1, 2, 3 });

            var result = target.ObtenerPuestosIdPorPC("Pc1");

            Assert.NotNull(result);
            Assert.AreEqual(result, new List<int> { 1, 2, 3 });
        }

        [Test]
        public void ObtenerTiempoEntreActividades()
        {
            var fecha = new DateTime(2015, 6, 6);
            repositorioMock.Setup(
                s =>
                s.ObtenerMayor(It.IsAny<Expression<Func<LogActividad, bool>>>(), It.IsAny<Expression<Func<LogActividad, int>>>(), It.IsAny<Expression<Func<LogActividad, DateTime>>>())).Returns(fecha);

            var result = target.ObtenerTiempoEntreActividades(Guid.NewGuid(), "A", "B");

            Assert.NotNull(result);
            Assert.AreEqual(result, new TimeSpan(0));
        }



        [Test]
        public void ObtenerAsignacionDeEstablecimiento()
        {
            var instance = Guid.NewGuid();

            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, Proveedor>>>()))
                           .Returns(new Proveedor { Id = 1, RazonSocial = "Proveedor" });

            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, DatosDeInstanciaDto>>>()))
                           .Returns(new DatosDeInstanciaDto { WorkflowId = 1, WorkflowCodigo = "w" });

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Establecimiento, bool>>>()))
                           .Returns(new List<Establecimiento> { new Establecimiento { NombreDeEstablecimiento = "Estab" } });
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Establecimiento, EstablecimientoDto>>>(),
                         It.IsAny<Expression<Func<Establecimiento, bool>>>()))
                           .Returns(new List<EstablecimientoDto>
                               {
                                   new EstablecimientoDto {NombreDeEstablecimiento = "Estab"}
                               });

            var result = target.ObtenerAsignacionDeEstablecimiento(instance);

            Assert.NotNull(result);
            Assert.AreEqual(result.InstanceId, instance);
            Assert.AreEqual(result.Establecimientos.FirstOrDefault().NombreDeEstablecimiento, "Estab");
        }


        [Test]
        public void EsProveedorSustentable()
        {
            repositorioMock.Setup(
                s =>
                s.Existe(It.IsAny<Expression<Func<Establecimiento, bool>>>())).Returns(true);

            var result = target.EsProveedorSustentable(1);

            Assert.NotNull(result);
            Assert.True(result);
        }

        [Test]
        public void ListarPaginadoEstablecimientos()
        {
            var listapg = new ListaPaginada<EstablecimientoDto>(new List<EstablecimientoDto>
                {
                    new EstablecimientoDto
                        {
                            Id = 1,
                            NombreDeEstablecimiento = "Estab"
                        }
                }, 1, 1, 1);


            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<Establecimiento>(new List<Establecimiento>
                               {
                                   new Establecimiento
                                       {
                                           Id = 1,
                                           NombreDeEstablecimiento = "Estab"
                                       }
                               }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Establecimiento, EstablecimientoDto>>>(),
                         It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(listapg);

            var result = target.ListarPaginadoEstablecimientos("a", new Paginacion());
            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().NombreDeEstablecimiento, "Estab");
        }

        [Test]
        public void ObtenerEstablecimiento()
        {
            repositorioMock.Setup(s => s.Obtener<EstablecimientoDto>(It.IsAny<int>()))
                           .Returns(new EstablecimientoDto { NombreDeEstablecimiento = "Estab" });
            repositorioMock.Setup(s => s.Obtener<Establecimiento>(It.IsAny<int>()))
                           .Returns(new Establecimiento { NombreDeEstablecimiento = "Estab" });
            var result = target.ObtenerEstablecimiento(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.NombreDeEstablecimiento, "Estab");
        }
        [Test]
        public void ObtenerRecorridoImpresionReciboMunicipal()
        {
            var instance = Guid.NewGuid();
            repositorioMock.Setup(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int>>>()))
                           .Returns(new Recorrido { InstanciaWorkflow = instance, Patente = "AAA111",Transportista=new Transportista { RazonSocial="TEsT" } });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<ReciboMunicipal, bool>>>()))
                           .Returns(new ReciboMunicipal());

            var result = target.ObtenerRecorridoImpresionReciboMunicipal(instance);

            Assert.NotNull(result);
            Assert.AreEqual(result.Patente, "AAA111");
        }

        [Test]
        public void ObtenerRecorridoMaterialCentroImpresionFormulario239()
        {
            var instance = Guid.NewGuid();
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, Formulario239RecorridoMaterialCentroDto>>>()))
                           .Returns(new Formulario239RecorridoMaterialCentroDto
                           {
                               CentroDescripcion = "Centro1"
                           });

            var result = target.ObtenerRecorridoMaterialCentroImpresionFormulario239(instance);

            Assert.NotNull(result);
            Assert.AreEqual(result.CentroDescripcion, "Centro1");
        }

        [Test]
        public void ObtenerBalanzasPorGuid()
        {
            var instance = Guid.NewGuid();
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, Balanza>>>()))
                           .Returns(new Balanza { Nombre = "Balan" });

            var result = target.ObtenerBalanzasPorGuid(instance);

            Assert.NotNull(result);
            Assert.AreEqual(result.BalanzaBruto.Nombre, "Balan");
        }

        [Test]
        public void VerificarCorrespondeDescargaTrue()
        {
            var instance = Guid.NewGuid();
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new Recorrido());
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>()))
                           .Returns(new MaterialPorCentro { CorrespondeDescarga = true });

            var result = target.VerificarCorrespondeDescarga(instance);

            Assert.NotNull(result);
            Assert.True(result);
        }
        [Test]
        public void VerificarCorrespondeDescargaFalse()
        {
            var instance = Guid.NewGuid();

            var result = target.VerificarCorrespondeDescarga(instance);

            Assert.NotNull(result);
            Assert.False(result);
        }

        [Test]
        public void VerificarCorrespondeControlDeBalanza()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            var result = target.VerificarCorrespondeControlDeBalanza(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.True(result);
        }

        [Test]
        public void ObtenerToleranciaOrigenAsignacionOK()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Recorrido> { new Recorrido(), new Recorrido { Establecimiento = new Establecimiento() } });
            repositorioMock.Setup(s => s.Obtener<Balanza>(It.IsAny<int>())).Returns(new Balanza { ToleranciaOrigen = 100 });
            var result = target.ObtenerToleranciaOrigen(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result, 100);
        }

        [Test]
        public void ObtenerToleranciaOrigenBalanzaNull()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Recorrido> { new Recorrido(), new Recorrido { Establecimiento = new Establecimiento() } });
            var result = target.ObtenerToleranciaOrigen(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result, 0);
        }

        [Test]
        public void ObtenerToleranciaRomaneoAsignacionOK()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Recorrido> { new Recorrido(), new Recorrido { Establecimiento = new Establecimiento() } });
            repositorioMock.Setup(s => s.Obtener<Balanza>(It.IsAny<int>())).Returns(new Balanza { ToleranciaRechazo = 100 });
            var result = target.ObtenerToleranciaRomaneo(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result, 100);
        }

        [Test]
        public void ObtenerToleranciaRomaneoBalanzaNull()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Recorrido> { new Recorrido(), new Recorrido { Establecimiento = new Establecimiento() } });
            var result = target.ObtenerToleranciaRomaneo(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result, 0);
        }


        [Test]
        public void LeerToleranciaRechazoAsignacionOK()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Recorrido> { new Recorrido(), new Recorrido { Establecimiento = new Establecimiento() } });
            repositorioMock.Setup(s => s.Obtener<Balanza>(It.IsAny<int>())).Returns(new Balanza { ToleranciaRechazo = 100 });
            var result = target.LeerToleranciaRechazo(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result, 100);
        }

        [Test]
        public void LeerToleranciaRechazoBalanzaNull()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Recorrido> { new Recorrido(), new Recorrido { Establecimiento = new Establecimiento() } });
            var result = target.LeerToleranciaRechazo(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result, 0);
        }

        [Test]
        public void LeerPesoMaximoAsignacionOK()
        {
            var result = target.LeerPesoMaximo(Guid.NewGuid(), TipoDeWorkflow.Egreso);

            Assert.NotNull(result);
            Assert.AreEqual(result, 0);
        }

        [Test]
        public void LeerPesoMaximoBalanzaNull()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Recorrido> { new Recorrido(), new Recorrido { Establecimiento = new Establecimiento() } });
            var result = target.LeerPesoMaximo(Guid.NewGuid(), TipoDeWorkflow.Ingreso);

            Assert.NotNull(result);
            Assert.AreEqual(result, 0);
        }

        [Test]
        public void ListarPermisosDeActividad()
        {
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Permiso, string>>>(), It.IsAny<Expression<Func<Permiso, bool>>>()))
                           .Returns(new List<string> { "P1" });

            var result = target.ListarPermisosDeActividad();

            Assert.AreEqual(result.FirstOrDefault(), "P1");
        }

        [Test]
        public void ActividadEsEjecutable()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<ActividadConCargaAutomatica, bool>>>()))
                           .Returns(true);
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<UsuarioTienePermisoParaActividadConsulta>()))
                           .Returns(true);

            var result = target.ActividadEsEjecutable("w", "A", "s");

            Assert.NotNull(result);
            Assert.True(result);
        }

        [Test]
        public void ObtenerActividadConCargaAutomatica()
        {
            repositorioMock.Setup(s => s.Obtener<ActividadConCargaAutomatica>(It.IsAny<int>()))
                           .Returns(new ActividadConCargaAutomatica { Actividad = "A" });

            var result = target.ObtenerActividadConCargaAutomatica(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Actividad, "A");
        }

        [Test]
        public void ListarPaginadoActividadesConCargaAutomatica()
        {
            var listapg = new ListaPaginada<ActividadConCargaAutomaticaDto>(new List<ActividadConCargaAutomaticaDto>
                {
                    new ActividadConCargaAutomaticaDto
                        {
                            Id = 1,
                            Actividad = "A"
                        }
                }, 1, 1, 1);


            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<ActividadConCargaAutomatica, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<ActividadConCargaAutomatica>(new List<ActividadConCargaAutomatica>
                               {
                                   new ActividadConCargaAutomatica
                                       {
                                           Id = 1,
                                           Actividad = "A"
                                       }
                               }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<ActividadConCargaAutomatica, ActividadConCargaAutomaticaDto>>>(),
                         It.IsAny<Expression<Func<ActividadConCargaAutomatica, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(listapg);

            var result = target.ListarPaginadoActividadesConCargaAutomatica(1, "a", new Paginacion());
            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().Actividad, "A");
        }

        [Test]
        public void EsActividadAutomatica()
        {
            var instance = Guid.NewGuid();
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<ActividadPorDispositivo, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(false);
            var result = target.EsActividadAutomatica(instance, "A", "A");

            Assert.NotNull(result);
            Assert.True(result);
        }

        [Test]
        public void BuscarAsignacionDeRecorridoPorInstanceIdNull()
        {
            var instance = Guid.NewGuid();

            var result = target.BuscarAsignacionDeRecorridoPorInstanceId(instance);

            Assert.Null(result);
        }

        [Test]
        public void BuscarAsignacionDeRecorridoPorInstanceIdOk()
        {
            var instance = Guid.NewGuid();
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new Recorrido { Calado = new Calado(), Centro = new Centro { Id = 1 }, Material = new Material() });

            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>(),
                                    It.IsAny<Expression<Func<MaterialPorCentro, int>>>())).Returns(1);
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<AsignacionDeRecorrido, bool>>>()))
                           .Returns(new AsignacionDeRecorrido { Id = 1, Centro = new Centro { Id = 1 }, MaterialPorCentro = new MaterialPorCentro(), PuestosDeCargaDescargas = new[] { new PuestosDeCargaDescarga() } });
            var result = target.BuscarAsignacionDeRecorridoPorInstanceId(instance);

            Assert.NotNull(result);
            Assert.AreEqual(result.CentroId, 1);
        }

        [Test]
        public void ListarArchivoINV()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>()))
                           .Returns(new List<RemitoBodegaUva>
                               {
                                   new RemitoBodegaUva
                                       {
                                           Patente = "AAA111",
                                           Material = new Material {Descripcion = "Mat"},
                                           Recorrido =
                                               new Recorrido
                                                   {
                                                       InstanciaWorkflow = Guid.NewGuid(),
                                                       TipoDocumentoIngreso = TipoDocumentoIngreso.RemitoBodegaUvaPropia,
                                                       Calado = new Calado
                                                           {
                                                               CaladosPorCaracteristica =
                                                                   new Collection<CaladoPorCaracteristica>
                                                                       {
                                                                           new CaladoPorCaracteristica
                                                                               {
                                                                                   CaracteristicaDeCalidad =
                                                                                       new CaracteristicaDeCalidad
                                                                                           {
                                                                                               EsTenorAzucarino = true,
                                                                                               DescuentoEnPorcentaje =
                                                                                                   FormulaDescuento
                                        .Acumulado,
                                                                                               CaladoMaximo = 50
                                                                                           }
                                                                               }
                                                                       }
                                                           }
                                                   }
                                       }
                               });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                           .Returns(new Centro
                           {
                               Id = 1,
                               Descripcion = "Centro",
                               NumeroINV = "111",
                               RazonSocial = "Centro Uva",
                               IngresosBrutos = "123"
                           });

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<CiuAnulado, bool>>>()))
                           .Returns(new List<CiuAnulado>
                               {
                                   new CiuAnulado {Id = 1, Numero = "123", Fecha = new DateTime(2015, 8, 8)}
                               });

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Observacion, bool>>>()))
                           .Returns(new Observacion());
            repositorioMock.Setup(
                s => s.Obtener(It.IsAny<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, bool>>>()))
                           .Returns(new Dominio.Entidades.AnalisisDeCalidad
                           {
                               CaracteristicasAnalizadas =
                                       new List<AnalisisPorCaracteristica>
                                           {
                                               new AnalisisPorCaracteristica
                                                   {
                                                       CaracteristicaDeCalidad =
                                                           new CaracteristicaDeCalidad
                                                               {
                                                                   EsTenorAzucarino = true,
                                                                   DescuentoEnPorcentaje = FormulaDescuento.Acumulado,
                                                                   CaladoMaximo = 50
                                                               }
                                                   }
                                           }
                           });

            var result = target.ListarArchivoINV(new DateTime(2015, 6, 6), new DateTime(2015, 9, 9), 1);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().NumeroINVBodega, "111");

        }

        [Test]
        public void NumeroHojaDeRutaYerbateraValido()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            var result = target.NumeroHojaDeRutaYerbateraValido("123", 1, "w");

            Assert.NotNull(result);
            Assert.AreEqual(result.Error, "La Hoja de Ruta  123 ya fue Ingresada");
        }

        [Test]
        public void ObtenerHojaDeRutaYerbateraVaciaOk()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Returns(new Workflow
                           {
                               Centro = new Centro { Id = 1, Descripcion = "Centro1" },
                               Activo = true,
                               TipoDeWorkflow = TipoDeWorkflow.Egreso,
                               Descripcion = "Workflow1"
                           });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                           .Returns(new Centro { Id = 1, Descripcion = "Centro1", Localidad = new Localidad { Descripcion = "Localidad1" } });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                           .Returns(new Proveedor { Descripcion = "Proveedor1", Activo = true });

            var result = target.ObtenerHojaDeRutaYerbateraVacia(1, "w", "1", "2");

            Assert.NotNull(result);
            Assert.AreEqual(result.TipoDeWorkflow, TipoDeWorkflow.Egreso);

        }

        [Test]
        public void ObtenerHojaDeRutaYerbateraVaciaOkDestinatarioIgualProveedor()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Returns(new Workflow
                           {
                               Centro = new Centro { Id = 1, Descripcion = "Centro1" },
                               Activo = true,
                               TipoDeWorkflow = TipoDeWorkflow.Egreso,
                               Descripcion = "Workflow1"
                           });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                           .Returns(new Centro { Id = 1, Descripcion = "Centro1", Localidad = new Localidad { Descripcion = "Localidad1" } });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                           .Returns(new Proveedor { Descripcion = "Proveedor1", Activo = true });

            var result = target.ObtenerHojaDeRutaYerbateraVacia(1, "w", "1", "1");

            Assert.NotNull(result);
            Assert.AreEqual(result.TipoDeWorkflow, TipoDeWorkflow.Egreso);

        }

        [Test]
        public void ObtenerHojaDeRutaYerbateraVaciaException()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Returns(new Workflow
                           {
                               Centro = new Centro { Id = 1, Descripcion = "Centro1" },
                               Activo = true,
                               TipoDeWorkflow = TipoDeWorkflow.Egreso,
                               Descripcion = "Workflow1"
                           });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                           .Returns(new Centro { Id = 1, Descripcion = "Centro1", Localidad = new Localidad { Descripcion = "Localidad1" } });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                           .Throws(new Exception("Error"));



            Assert.Throws<Exception>(() => target.ObtenerHojaDeRutaYerbateraVacia(1, "w", "1", "1"));
        }

        [Test]
        public void ObtenerHojaDeRutaYerbateraPorInstanceId()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<HojaDeRutaYerbatera, bool>>>()))
                           .Returns(new HojaDeRutaYerbatera { Id = 1, Patente = "AAA111" });

            var result = target.ObtenerHojaDeRutaYerbateraPorInstanceId(Guid.NewGuid());
            Assert.NotNull(result);
            Assert.AreEqual(result.Patente, "AAA111");
        }

        [Test]
        public void ObtenerHojaDeRutaYerbatera()
        {
            repositorioMock.Setup(s => s.Obtener<HojaDeRutaYerbatera>(It.IsAny<int>()))
                           .Returns(new HojaDeRutaYerbatera { Patente = "AAA111" });

            var result = target.ObtenerHojaDeRutaYerbatera(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Patente, "AAA111");
        }

        [Test]
        public void ListarCalidadesPorMaterialyCentro()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<CalidadMaterial, bool>>>()))
                           .Returns(new List<CalidadMaterial>
                               {
                                   new CalidadMaterial
                                       {
                                           MaterialPorCentro =
                                               new MaterialPorCentro
                                                   {
                                                       Centro = new Centro {Id = 1, Descripcion = "Centro1"},
                                                       Material = new Material {Descripcion = "Mat1"}
                                                   }
                                       }
                               });

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<CalidadMaterial, CalidadMaterialDto>>>(),
                         It.IsAny<Expression<Func<CalidadMaterial, bool>>>()))
                           .Returns(new List<CalidadMaterialDto>
                               {
                                   new CalidadMaterialDto {CentroId = 1, Material = "Mat1"}
                               });

            var result = target.ListarCalidadesPorMaterialyCentro(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Material, "Mat1");
        }

        [Test]
        public void ListarMaterialesPorCentro()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>()))
                           .Returns(new List<MaterialPorCentro>
                               {
                                   new MaterialPorCentro
                                       {
                                           Material = new Material {Descripcion = "Mat1"},
                                           Centro = new Centro {Id = 1, Descripcion = "Centro1"}
                                       }
                               });
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<MaterialPorCentro, MaterialPorCentroDto>>>(),
                         It.IsAny<Expression<Func<MaterialPorCentro, bool>>>()))
                           .Returns(new List<MaterialPorCentroDto>
                               {
                                   new MaterialPorCentroDto {CentroId = 1, MaterialDesc = "Mat1"}
                               });

            var result = target.ListarMaterialesPorCentro(1);
            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().MaterialDesc, "Mat1");
            Assert.AreEqual(result.FirstOrDefault().CentroId, 1);
        }

        [Test]
        public void ObtenerAsignacionDeRecorrido()
        {
            repositorioMock.Setup(s => s.Obtener<AsignacionDeRecorrido>(It.IsAny<int>()))
                           .Returns(new AsignacionDeRecorrido { BalanzaBruto = new Balanza { Nombre = "B1" }, PuestosDeCargaDescargas = new Collection<PuestosDeCargaDescarga>() });

            var result = target.ObtenerAsignacionDeRecorrido(1);
            Assert.NotNull(result);
            Assert.AreEqual(result.BalanzaBruto, "B1");
        }

        [Test]
        public void ListarPaginadoAsignacionesDeRecorrido()
        {
            repositorioMock.Setup(
                s => s.Listar(It.IsAny<Expression<Func<AsignacionDeRecorrido, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<AsignacionDeRecorrido>(new List<AsignacionDeRecorrido>
                               {
                                   new AsignacionDeRecorrido
                                       {
                                           PuestosDeCargaDescargas = new Collection<PuestosDeCargaDescarga>(),
                                           MaterialPorCentro =
                                               new MaterialPorCentro {Material = new Material {Descripcion = "Mat1"}}
                                       }
                               }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<AsignacionDeRecorrido, AsignacionDeRecorridoDto>>>(),
                         It.IsAny<Expression<Func<AsignacionDeRecorrido, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<AsignacionDeRecorridoDto>(
                                   new List<AsignacionDeRecorridoDto> { new AsignacionDeRecorridoDto { Material = "Mat1" } },
                                   1, 1, 1));

            var result = target.ListarPaginadoAsignacionesDeRecorrido(1, "", new Paginacion());

            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().Material, "Mat1");
        }

        [Test]
        public void ObtenerTenorAzucarinoNull()
        {
            var result = target.ObtenerTenorAzucarinoNumerico(Guid.NewGuid());

            Assert.Null(result);
        }

        [Test]
        public void ObtenerTenorAzucarinoAnalisis()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, Calado>>>())).Returns(new Calado());
            repositorioMock.Setup(
                s => s.Obtener(It.IsAny<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, bool>>>()))
                           .Returns(new Dominio.Entidades.AnalisisDeCalidad
                           {
                               CaracteristicasAnalizadas =
                                       new List<AnalisisPorCaracteristica>
                                           {
                                               new AnalisisPorCaracteristica
                                                   {
                                                       CaracteristicaDeCalidad =
                                                           new CaracteristicaDeCalidad {EsTenorAzucarino = true},
                                                           ValorAnalisis = 10
                                                   }
                                           }
                           });

            var result = target.ObtenerTenorAzucarino(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result, "10");
        }

        [Test]
        public void ObtenerTenorAzucarinoCalado()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, Calado>>>()))
                           .Returns(new Calado
                           {
                               CaladosPorCaracteristica =
                                       new Collection<CaladoPorCaracteristica>
                                           {
                                               new CaladoPorCaracteristica
                                                   {
                                                       CaracteristicaDeCalidad =
                                                           new CaracteristicaDeCalidad {EsTenorAzucarino = true},
                                                       ValorCalado = 12
                                                   }
                                           }
                           });

            var result = target.ObtenerTenorAzucarino(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result, "12");
        }

        [Test]
        public void ListarMaterialesPorWorkflowYCodigoSAP()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<MaterialPorWorkflow, bool>>>()))
                           .Returns(new List<MaterialPorWorkflow>
                               {
                                   new MaterialPorWorkflow {Material = new Material {Descripcion = "Mat1"}}
                               });

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<MaterialPorWorkflow, MaterialPorWorkflowDto>>>(),
                         It.IsAny<Expression<Func<MaterialPorWorkflow, bool>>>()))
                           .Returns(new List<MaterialPorWorkflowDto>
                               {
                                   new MaterialPorWorkflowDto {MaterialDesc = "Mat1"}
                               });
            var result = target.ListarMaterialesPorWorkflowYCodigoSAP(1, 1, new List<string> { "123" });

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().MaterialDesc, "Mat1");
        }

        [Test]
        public void ListarMaterialesPorWorkflowVinedoYCodigoSAP()
        {
            repositorioMock.Setup(s => s.Obtener<Vinedo>(It.IsAny<int>()))
                           .Returns(new VinedoPropio { VariedadesPorVinedo = new Collection<VariedadPorVinedo>() });

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<MaterialPorWorkflow, bool>>>()))
                           .Returns(new List<MaterialPorWorkflow>
                               {
                                   new MaterialPorWorkflow {Material = new Material {Descripcion = "Mat1"}}
                               });

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<MaterialPorWorkflow, MaterialPorWorkflowDto>>>(),
                         It.IsAny<Expression<Func<MaterialPorWorkflow, bool>>>()))
                           .Returns(new List<MaterialPorWorkflowDto>
                               {
                                   new MaterialPorWorkflowDto {MaterialDesc = "Mat1"}
                               });

            var result = target.ListarMaterialesPorWorkflowVinedoYCodigoSAP(1, 1, 1, new List<string> { "123" });

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().MaterialDesc, "Mat1");
        }

        [Test]
        public void ObtenerDistribucionDeAlmacenes()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<DistribucionDeAlmacenes, bool>>>()))
                           .Returns(new DistribucionDeAlmacenes
                           {
                               Recorrido = new Recorrido { Material = new Material { Id = 1 } },
                               NombreDeUsuario = "user"
                           });
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<DistribucionDeAlmacen, bool>>>()))
                           .Returns(new List<DistribucionDeAlmacen>());

            var result = target.ObtenerDistribucionDeAlmacenes(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result.NombreDeUsuario, "user");
        }

        [Test]
        public void ListarVinedoTerceros()
        {
            repositorioMock.Setup(
                s => s.Listar(It.IsAny<Expression<Func<VinedoTerceros, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<VinedoTerceros>(
                                   new List<VinedoTerceros> { new VinedoTerceros { Descripcion = "vin1" } }, 1, 1, 1));
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<VinedoTerceros, VinedoTercerosDto>>>(),
                         It.IsAny<Expression<Func<VinedoTerceros, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<VinedoTercerosDto>(
                                   new List<VinedoTercerosDto> { new VinedoTercerosDto { Descripcion = "vin1" } }, 1, 1, 1));

            var result = target.ListarVinedoTerceros("f", new Paginacion());

            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().Descripcion, "vin1");
        }

        [Test]
        public void ObtenerDescargasDeBinesPorRemitoBodegaUva()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<DescargaDeBines, bool>>>()))
                           .Returns(new List<DescargaDeBines>
                               {
                                   new DescargaDeBines {CantidadBines = 1, Tipo = new Material {Descripcion = "Mat1"}}
                               });
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<DescargaDeBines, DescargaDeBinesDto>>>(),
                         It.IsAny<Expression<Func<DescargaDeBines, bool>>>()))
                           .Returns(new List<DescargaDeBinesDto>
                               {
                                   new DescargaDeBinesDto {Tipo = "Mat1", CantidadBines = 1}
                               });


            var result = target.ObtenerDescargasDeBinesPorRemitoBodegaUva(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().CantidadBines, 1);
            Assert.AreEqual(result.FirstOrDefault().Tipo, "Mat1");
        }

        [Test]
        public void FiltrarCuartelesPorVinedo()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Cuartel, bool>>>()))
                           .Returns(new List<Cuartel> { new Cuartel { Codigo = "C1" } });
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Cuartel, CuartelDto>>>(), It.IsAny<Expression<Func<Cuartel, bool>>>()))
                           .Returns(new List<CuartelDto> { new CuartelDto { Codigo = "C1" } });

            var result = target.FiltrarCuartelesPorVinedo(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Codigo, "C1");
        }

        [Test]
        public void ObtenerHojaDeRutaPorInstanceId()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<HojaDeRuta, bool>>>()))
                           .Returns(new HojaDeRuta { PatenteCamion = "AAA111" });

            var result = target.ObtenerHojaDeRutaPorInstanceId(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerRemitoBodegaVinoPorInstanceId()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<RemitoBodegaVino, bool>>>()))
                           .Returns(new RemitoBodegaVino { Patente = "AAA111" });

            var result = target.ObtenerRemitoBodegaVinoPorInstanceId(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result.Patente, "AAA111");
        }

        [Test]
        public void ObtenerRemitoBodegaVino()
        {
            repositorioMock.Setup(s => s.Obtener<RemitoBodegaVino>(It.IsAny<int>()))
                           .Returns(new RemitoBodegaVino { Patente = "AAA111" });

            var result = target.ObtenerRemitoBodegaVino(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Patente, "AAA111");
        }

        [Test]
        public void ObtenerRemitoBodegaUvaPorInstanceId()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>()))
                           .Returns(new RemitoBodegaUva { Patente = "AAA111" });

            var result = target.ObtenerRemitoBodegaUvaPorInstanceId(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result.Patente, "AAA111");
        }

        [Test]
        public void ObtenerRemitoBodegaUva()
        {
            repositorioMock.Setup(s => s.Obtener<RemitoBodegaUva>(It.IsAny<int>()))
                           .Returns(new RemitoBodegaUva { Patente = "AAA111" });

            var result = target.ObtenerRemitoBodegaUva(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Patente, "AAA111");
        }

        [Test]
        public void ListarVariedades()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Variedad, bool>>>()))
                           .Returns(new List<Variedad> { new Variedad { Descripcion = "Var" } });
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Variedad, VariedadDto>>>(),
                         It.IsAny<Expression<Func<Variedad, bool>>>()))
                           .Returns(new List<VariedadDto> { new VariedadDto { Descripcion = "Var" } });

            var result = target.ListarVariedades();
            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Descripcion, "Var");
        }

        [Test]
        public void ObtenerRemitoBodegaUvaPorGuid()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>()))
                           .Returns(new RemitoBodegaUva { Material = new Material { Descripcion = "Mat1" } });

            var result = target.ObtenerRemitoBodegaUvaPorGuid(Guid.NewGuid());
            Assert.NotNull(result);
            Assert.AreEqual(result.Material, "Mat1");
        }

        [Test]
        public void ObtenerRemitoBodegaUvaIdPorGuid()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<RemitoBodegaUva, bool>>>(),
                                    It.IsAny<Expression<Func<RemitoBodegaUva, int>>>())).Returns(1);

            var result = target.ObtenerRemitoBodegaUvaIdPorGuid(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result, 1);
        }

        [Test]
        public void ObtenerKilosARecibirPorVinedoOk()
        {
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<KgRecibidosPorFincaViñedoAño>())).Returns(1);
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<VariedadPorVinedo, bool>>>()))
                           .Returns(new VariedadPorVinedo { TopeHectarea = 10, Hectareas = 10 });

            var result = target.ObtenerKilosARecibirPorVinedo(1, 1, "1");
            Assert.NotNull(result);
            Assert.False(result.Error);
            Assert.AreEqual(result.KilosARecibir, 99);
        }

        [Test]
        public void ObtenerKilosARecibirPorVinedoVariedadNull()
        {
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<KgRecibidosPorFincaViñedoAño>())).Returns(1);

            var result = target.ObtenerKilosARecibirPorVinedo(1, 1, "1");
            Assert.NotNull(result);
            Assert.True(result.Error);
            Assert.AreEqual(result.MensajeError, Dominio.Recursos.Textos.Error_VariedadPorVinedo);
        }

        [Test]
        public void ObtenerKilosRecibidosPorVinedo()
        {
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<KgRecibidosPorFincaViñedoAño>())).Returns(2);

            var result = target.ObtenerKilosRecibidosPorVinedo(1, 1, "1");
            Assert.NotNull(result);
            Assert.AreEqual(result, 2);
        }

        [Test]
        public void VerificarKilosDeclaradosPorVinedoSinMaterialError()
        {
            var result = target.VerificarKilosDeclaradosPorVinedo(1, 1, "1");

            Assert.NotNull(result);
            Assert.True(result.Error);
        }

        [Test]
        public void VerificarKilosDeclaradosPorVinedoSinVariedadError()
        {
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>()))
                           .Returns(new Material { Descripcion = "Mat1" });

            var result = target.VerificarKilosDeclaradosPorVinedo(1, 1, "1");

            Assert.NotNull(result);
            Assert.True(result.Error);
        }

        [Test]
        public void VerificarKilosDeclaradosPorVinedo()
        {
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>()))
                           .Returns(new Material { Descripcion = "Mat1" });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<VariedadPorVinedo, bool>>>()))
                           .Returns(new VariedadPorVinedo
                           {
                               Id = 1,
                               AvisoCorte = 200,
                               TopeHectarea = 10,
                               Hectareas = 10,
                               Vinedo = new VinedoPropio { Descripcion = "Vin1" },
                               Variedad = new Variedad { Descripcion = "Var1" }
                           });
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<KgRecibidosPorFincaViñedoAño>())).Returns(101);
            var result = target.VerificarKilosDeclaradosPorVinedo(1, 1, "1");

            Assert.NotNull(result);
            Assert.True(result.ExcedeKilosARecibir);
            Assert.AreEqual(result.KilosARecibir, 0);
        }

        [Test]
        public void VerificarKilosDeclaradosPorVinedoException()
        {
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>()))
                           .Throws(new Exception("Error"));

            var result = target.VerificarKilosDeclaradosPorVinedo(1, 1, "1");

            Assert.NotNull(result);
            Assert.True(result.Error);
        }

        [Test]
        public void EsCiuAnulado()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CiuAnulado, bool>>>())).Returns(true);

            var result = target.EsCiuAnulado("1");

            Assert.NotNull(result);
            Assert.True(result);
        }

        [Test]
        public void ListarPaginadoCiuAnulados()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<CiuAnulado, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<CiuAnulado>(
                                        new List<CiuAnulado> { new CiuAnulado { Numero = "123" } }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<CiuAnulado, CiuAnuladoDto>>>(),
                         It.IsAny<Expression<Func<CiuAnulado, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<CiuAnuladoDto>(
                                   new List<CiuAnuladoDto> { new CiuAnuladoDto { Numero = "123" } }, 1, 1, 1));

            var result = target.ListarPaginadoCiuAnulados(new Paginacion());

            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().Numero, "123");
        }

        [Test]
        public void ListarListadoDePesadas()
        {
            repositorioMock.Setup(s => s.ListarConsulta(It.IsAny<ListarListadoDePesadas>()))
                           .Returns(new List<ListadoDePesadasDto> { new ListadoDePesadasDto { BalanzaBruto = "B1" } });

            var result = target.ListarListadoDePesadas(new List<int> { 1 }, new DateTime(2015, 6, 6),
                                                       new DateTime(2015, 9, 9), new List<int> { 1 }, new List<int> { 1 },
                                                       true);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().BalanzaBruto, "B1");
        }

        [Test]
        public void ValidarProximaActividadPorPuestoSinPatente()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, Guid>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new List<Guid> { });

            repositorioMock.Setup(
                s =>
                s.ObtenerConsultaEscalar(It.IsAny<IConsultaEscalar<bool>>())).Returns(true);

            repositorioMock.Setup(
                s =>
                s.Existe(It.IsAny<Expression<Func<ActividadPorDispositivo, bool>>>())).Returns(true);

            repositorioMock.Setup(
                x => x.Listar(It.IsAny<Expression<Func<ActividadPorDispositivo, int>>>(),
                               It.IsAny<Expression<Func<ActividadPorDispositivo, bool>>>())).Returns(new List<int>() { 1 });
            ICollection<VideoCamara> camaras = new List<VideoCamara>() { new VideoCamara { Codigo = "a", Directorio = "b" } };
            repositorioMock.Setup(
                s =>
                s.Obtener<PuestoDeTrabajo>(It.IsAny<int>())).Returns(new PuestoDeTrabajo() { VideoCamaras = camaras, Entrada = "c" });
            var recorrido = new DatosRecorridoDto
            {
                InstanciaWorkflow = new Guid(),
                TarjetaDeAcceso = "1111",
                WorkflowDefinicionId = 0,
                CentroCodigoSap = "1234",
                Patente = "AAA111"
            };
            var resultado = target.ValidarProximaActividadPorPuestoSinPatente(recorrido, "Actividad", new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, Lectura = "1111" } });
            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.Patente, "AAA111");

        }

        [Test]
        public void ObtenerDatosRecorridoActivo()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, DatosRecorridoDto>>>()))
                           .Returns(new DatosRecorridoDto
                           {
                               Patente = "AAA111",
                               WorkflowId = 1,
                               NumeroDocumentoIngreso = "1234"
                           });

            var result = target.ObtenerDatosRecorridoActivo("aaa111", "1");

            Assert.NotNull(result);
            Assert.AreEqual(result.Patente, "AAA111");
            Assert.AreEqual(result.NumeroDocumentoIngreso, "1234");
        }

        [Test]
        public void ObtenerAjusteYStockBines()
        {
            repositorioMock.Setup(s => s.Obtener<AjusteStockBines>(It.IsAny<int>()))
                           .Returns(new AjusteStockBines { Material = new Material { Descripcion = "Mat1" } });

            var result = target.ObtenerAjusteYStockBines(1);
            Assert.NotNull(result);
            Assert.AreEqual(result.MaterialDescripcion, "Mat1");
        }

        [Test]
        public void ObtenerAjusteDeStock()
        {
            repositorioMock.Setup(s => s.Obtener<AjusteDeStock>(It.IsAny<int>()))
                           .Returns(new AjusteDeStock { Material = new Material { Descripcion = "Mat1" } });

            var result = target.ObtenerAjusteDeStock(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.MaterialDesc, "Mat1");
        }

        [Test]
        public void ListarPaginadoAjusteYStockBines()
        {
            repositorioMock.Setup(
                s => s.Listar(It.IsAny<Expression<Func<AjusteStockBines, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<AjusteStockBines>(new List<AjusteStockBines>
                               {
                                   new AjusteStockBines
                                       {
                                           Material = new Material{Descripcion = "Mat1"}
                                       }
                               }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<AjusteStockBines, AjusteStockBinesDto>>>(),
                         It.IsAny<Expression<Func<AjusteStockBines, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<AjusteStockBinesDto>(
                                   new List<AjusteStockBinesDto> { new AjusteStockBinesDto { MaterialDescripcion = "Mat1" } },
                                   1, 1, 1));

            var result = target.ListarPaginadoAjusteYStockBines("1", new Paginacion(), 1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().MaterialDescripcion, "Mat1");
        }

        [Test]
        public void ListarPaginadoAjusteDeStock()
        {
            repositorioMock.Setup(
                s => s.Listar(It.IsAny<Expression<Func<AjusteDeStock, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<AjusteDeStock>(new List<AjusteDeStock>
                               {
                                   new AjusteDeStock
                                       {
                                           Material = new Material{Descripcion = "Mat1"}
                                       }
                               }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<AjusteDeStock, AjusteDeStockDto>>>(),
                         It.IsAny<Expression<Func<AjusteDeStock, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<AjusteDeStockDto>(
                                   new List<AjusteDeStockDto> { new AjusteDeStockDto { MaterialDesc = "Mat1" } },
                                   1, 1, 1));

            var result = target.ListarPaginadoAjusteDeStock("1", new Paginacion(), 1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().MaterialDesc, "Mat1");
        }

        [Test]
        public void ListarCaracteristicasDeCalidadPorConfiguracionNoExisteElMaterialPorCentro()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Returns(false);
            var result = target.ListarCaracteristicasDeCalidadPorConfiguracion(1, 1, "");

            Assert.NotNull(result);
            Assert.AreEqual(result.Count, 0);
        }

        [Test]
        public void ListarCaracteristicasDeCalidadPorConfiguracion()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<CaracteristicaDeCalidad, bool>>>()))
                           .Returns(new List<CaracteristicaDeCalidad> { new CaracteristicaDeCalidad() });
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<CaracteristicaDeCalidad, CaracteristicaConfiguracionDeTablaDto>>>(),
                         It.IsAny<Expression<Func<CaracteristicaDeCalidad, bool>>>()))
                           .Returns(new List<CaracteristicaConfiguracionDeTablaDto>
                               {
                                   new CaracteristicaConfiguracionDeTablaDto
                                       {
                                           CaracteristicaDeCalidadDesc = "Car",
                                           CaracteristicaDeCalidadId = 1,
                                           Visible = true
                                       }
                               });

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<ConfiguracionDeTabla, bool>>>()))
                           .Returns(new ConfiguracionDeTabla
                           {
                               CaracteristicasDeCalidad =
                                       new Collection<CaracteristicaDeCalidad>
                                           {
                                               new CaracteristicaDeCalidad
                                                   {
                                                       Id = 1,
                                                       CaracteristicaDeCalidadMaestro =
                                                           new CaracteristicaDeCalidadMaestro {Descripcion = "Car"},
                                                       DescripcionCorta = "Car"
                                                   }
                                           }
                           });


            var result = target.ListarCaracteristicasDeCalidadPorConfiguracion(1, 1, "");

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().CaracteristicaDeCalidadDesc, "Car");
            Assert.True(result.FirstOrDefault().Visible);
        }

        [Test]
        public void WorkflowEstaAsignado()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new Recorrido { Almacen = new Almacen() });

            var result = target.WorkflowEstaAsignado(Guid.NewGuid());

            Assert.True(result);
        }

        [Test]
        public void ObtenerCalidadMaterialPorHumedadEInstanceId()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new Recorrido { Almacen = new Almacen(), Material = new Material() });

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<CalidadMaterial, bool>>>()))
                           .Returns(new CalidadMaterial { Descripcion = "CalMat" });


            var result = target.ObtenerCalidadMaterialPorHumedadEInstanceId(1, true, Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "CalMat");

        }

        [Test]
        public void ObtenerCalidadMaterial()
        {
            repositorioMock.Setup(s => s.Obtener<CalidadMaterial>(It.IsAny<int>()))
                .Returns(new CalidadMaterial { Descripcion = "CalMat" });

            var result = target.ObtenerCalidadMaterial(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "CalMat");
        }

        //[Test]
        //public void ListarWorkFlowsEnPuestoComando()
        //{
        //    repositorioMock.Setup(s => s.ListarConsultaPaginada(It.IsAny<ListarWorkFlowsConsulta>()))
        //                   .Returns(
        //                       new ListaPaginada<Recorrido>(
        //                           new List<Recorrido>
        //                               {
        //                                   new Recorrido
        //                                       {
        //                                           Patente = "AAA111",
        //                                           Almacen = new Almacen(),
        //                                           WorkflowDefinicion = new WorkflowDefinicion {Id = 1}
        //                                       }
        //                               }, 1,
        //                           1, 1));

        //    var result = target.ListarWorkFlowsEnPuestoComando();

        //    Assert.NotNull(result);
        //    Assert.AreEqual(result.FirstOrDefault().WorkflowDefinicionId,1);

        //}

        //[Test]
        //public void ListarWorkFlows()
        //{
        //    var instance = Guid.NewGuid();
        //    repositorioMock.Setup(s => s.ListarConsultaPaginada(It.IsAny<ListarWorkFlowsConsulta>()))
        //                   .Returns(
        //                       new ListaPaginada<Recorrido>(
        //                           new List<Recorrido>
        //                               {
        //                                   new Recorrido
        //                                       {
        //                                           Patente = "AAA111",
        //                                           Almacen = new Almacen(),
        //                                           WorkflowDefinicion = new WorkflowDefinicion {Id = 1},
        //                                           InstanciaWorkflow = instance,
        //                                           Calado =
        //                                               new Calado
        //                                                   {
        //                                                           CaladosPorCaracteristica =
        //                                                           new Collection<CaladoPorCaracteristica>
        //                                                               {
        //                                                                   new CaladoPorCaracteristica
        //                                                                       {
        //                                                                           CaracteristicaDeCalidad =
        //                                                                               new CaracteristicaDeCalidad
        //                                                                                   {
        //                                                                                       DescripcionCorta =
        //                                                                                           "CarDeCal",
        //                                                                                       Id = 1
        //                                                                                   },
        //                                                                           ValorCalado = 10
        //                                                                       }
        //                                                               }
        //                                                   },
        //                                           AnalisisDeCalidad =
        //                                               new Dominio.Entidades.AnalisisDeCalidad
        //                                                   {
        //                                                       CaracteristicasAnalizadas =
        //                                                           new List<AnalisisPorCaracteristica>
        //                                                               {
        //                                                                   new AnalisisPorCaracteristica
        //                                                                       {
        //                                                                           CaracteristicaDeCalidad =
        //                                                                               new CaracteristicaDeCalidad
        //                                                                                   {
        //                                                                                       Id = 1,
        //                                                                                       DescripcionCorta = "CarDeCal"
        //                                                                                   },
        //                                                                           ValorCalado = 10,
        //                                                                           ValorAnalisis = 9
        //                                                                       }
        //                                                               }
        //                                                   }
        //                                       }
        //                               }, 1,
        //                           1, 1));

        //    repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<CalidadMaterial, bool>>>()))
        //                   .Returns(new List<CalidadMaterial>
        //                       {
        //                           new CalidadMaterial
        //                               {
        //                                   Descripcion = "CalMat",
        //                                   MaterialPorCentro =
        //                                       new MaterialPorCentro {Id = 1, Material = new Material {Descripcion = "Mat"}}
        //                               }
        //                       });
        //    repositorioMock.Setup(
        //        s =>
        //        s.Listar(It.IsAny<Expression<Func<CalidadMaterial, CalidadMaterialDto>>>(),
        //                 It.IsAny<Expression<Func<CalidadMaterial, bool>>>()))
        //                   .Returns(new List<CalidadMaterialDto> {new CalidadMaterialDto {Descripcion = "CalMat"}});

        //    repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<ConfiguracionDeTabla, bool>>>()))
        //                   .Returns(new ConfiguracionDeTabla
        //                   {
        //                       CaracteristicasDeCalidad =
        //                           new Collection<CaracteristicaDeCalidad>
        //                                   {
        //                                       new CaracteristicaDeCalidad
        //                                           {
        //                                               Id = 1,
        //                                               CaracteristicaDeCalidadMaestro =
        //                                                   new CaracteristicaDeCalidadMaestro {Descripcion = "Car"},
        //                                               DescripcionCorta = "Car"
        //                                           }
        //                                   }
        //                   });

        //    repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Workflow, bool>>>()))
        //                   .Returns(new List<Workflow> {new Workflow {Descripcion = "W"}});
        //    repositorioMock.Setup(
        //        s =>
        //        s.Listar(It.IsAny<Expression<Func<Workflow, WorkflowDto>>>(),
        //                 It.IsAny<Expression<Func<Workflow, bool>>>()))
        //                   .Returns(new List<WorkflowDto> {new WorkflowDto {Descripcion = "W"}});

        //    repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<MuestraEnvioACamara, MuestraPuestoComandoDto>>>(), It.IsAny<Expression<Func<MuestraEnvioACamara, bool>>>()))
        //                   .Returns(new List<MuestraPuestoComandoDto>{ new MuestraPuestoComandoDto
        //                       {
        //                           CaladoId = 10,
        //                           EsHumedad = true,
        //                           EsGranosVerdes = false,
        //                           EsGranosDañados = false,
        //                           EsCuerposExtranos = false
        //                       }});
        //    var result = target.ListarWorkFlows(new Paginacion(), new FiltroListaDeWorkflowsDto {MaterialId = 1});
        //    Assert.NotNull(result);
        //    Assert.AreEqual(result.Workflows.FirstOrDefault().CaracteristicasDeCalidad.Values.FirstOrDefault(),"9");
        //}

        [Test]
        public void BalanzasObligatoriasEnPuestoComando()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Centro, bool>>>(),
                                    It.IsAny<Expression<Func<Centro, bool>>>())).Returns(true);


            var result = target.BalanzasObligatoriasEnPuestoComando(1);

            Assert.True(result);
        }

        [Test]
        public void ListarCalidadesPorCentro()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<CalidadMaterial, bool>>>()))
                           .Returns(new List<CalidadMaterial> { new CalidadMaterial { Descripcion = "CalMat" } });

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<CalidadMaterial, CalidadMaterialDto>>>(),
                         It.IsAny<Expression<Func<CalidadMaterial, bool>>>()))
                           .Returns(new List<CalidadMaterialDto> { new CalidadMaterialDto { Descripcion = "CalMat" } });


            var result = target.ListarCalidadesPorCentro(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Descripcion, "CalMat");
        }

        [Test]
        public void ObtenerTipoDocumentoIdentidad()
        {
            repositorioMock.Setup(s => s.Obtener<TipoDocumentoIdentidad>(It.IsAny<int>()))
                           .Returns(new TipoDocumentoIdentidad { Id = 3, Descripcion = "documentoDNI" });

            var result = target.ObtenerTipoDocumentoIdentidad(1);

            repositorioMock.Verify(s => s.Obtener<TipoDocumentoIdentidad>(It.IsAny<int>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 3, result.Descripcion);
            Assert.AreEqual(result.Descripcion, "documentoDNI");
        }

        [Test]
        public void ObtenerConversionCentro()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<ConversionCentro, bool>>>()))
                           .Returns(new ConversionCentro { Id = 3, Centro = new Centro { Descripcion = "AAA" } });

            var result = target.ObtenerConversionCentro(1, 2);

            repositorioMock.Verify(s => s.ObtenerPrimero(It.IsAny<Expression<Func<ConversionCentro, bool>>>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 3);
            Assert.AreEqual(result.CentroDesc, "AAA");
        }

        [Test]
        public void ObtenerVariedadPorVinedo()
        {
            repositorioMock.Setup(s => s.Obtener<VariedadPorVinedo>(It.IsAny<int>())).Returns(new VariedadPorVinedo { Id = 3, Cosecha = "CSE" });
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<IConsultaEscalar<decimal>>())).Returns(500);

            var result = target.ObtenerVariedadPorVinedo(1);

            repositorioMock.Verify(s => s.Obtener<VariedadPorVinedo>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.ObtenerConsultaEscalar(It.IsAny<IConsultaEscalar<decimal>>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 3);
            Assert.AreEqual(result.Cosecha, "CSE");
            Assert.AreEqual(result.KgRecibidos, 500);
        }

        [Test]
        public void ObtenerCodigoEstablecimientoPorGuid()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");

            var result = target.ObtenerCodigoEstablecimientoPorGuid(new Guid());

            repositorioMock.Verify(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result, "1234");
        }

        [Test]
        public void ListarPaginadoTiposDocumentoIdentidad()
        {
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<TipoDocumentoIdentidad, bool>>>(), paginacion)).Returns(new ListaPaginada<TipoDocumentoIdentidad>(new List<TipoDocumentoIdentidad> { new TipoDocumentoIdentidad { CodigoSap = "4455" } }, 1, 10, 1));

            var resultado = target.ListarPaginadoTiposDocumentoIdentidad(paginacion);

            repositorioMock.Verify(s => s.Listar(It.IsAny<Expression<Func<TipoDocumentoIdentidad, bool>>>(), paginacion), Times.Exactly(1));
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 1);
            Assert.AreEqual(resultado.Items[0].CodigoSap, "4455");
        }

        [Test]
        public void ListarPaginadoVariedadPorVinedo()
        {
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<VariedadPorVinedo, bool>>>(), paginacion)).Returns(new ListaPaginada<VariedadPorVinedo>(new List<VariedadPorVinedo> { new VariedadPorVinedo { Cosecha = "4455" } }, 1, 10, 1));
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<IConsultaEscalar<decimal>>())).Returns(500);

            var resultado = target.ListarPaginadoVariedadPorVinedo(3, paginacion);

            repositorioMock.Verify(s => s.Listar(It.IsAny<Expression<Func<VariedadPorVinedo, bool>>>(), paginacion), Times.Exactly(1));
            repositorioMock.Verify(s => s.ObtenerConsultaEscalar(It.IsAny<IConsultaEscalar<decimal>>()), Times.Exactly(1));
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 1);
            Assert.AreEqual(resultado.Items[0].Cosecha, "4455");
        }



        [Test]
        public void ObtenerCartaPortePorInstanceId()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>())).Returns(new CartaPorte { CTG = "Mat1" });

            var result = target.ObtenerCartaPortePorInstanceId(new Guid());

            Assert.NotNull(result);
            Assert.AreEqual(result.CTG, "Mat1");

        }
        [Test]
        public void ObtenerCartaPortePorInstanceIdNull()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>())).Returns((CartaPorte)null);

            var result = target.ObtenerCartaPortePorInstanceId(new Guid());

            Assert.Null(result);
        }

        [Test]
        public void ObtenerAnalisisDeCalidadPorCaladoId()
        {
            repositorioMock.Setup(s => s.ObtenerMayor<Dominio.Entidades.AnalisisDeCalidad, int>(It.IsAny<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, bool>>>(), It.IsAny<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, int>>>())).Returns(new Dominio.Entidades.AnalisisDeCalidad { Id = 2 });

            var result = target.ObtenerAnalisisDeCalidadPorCaladoId(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 2);

        }

        [Test]
        public void ObtenerAnalisisDeCalidadPorInstanceId()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, Dominio.Entidades.AnalisisDeCalidad>>>())).Returns(new Dominio.Entidades.AnalisisDeCalidad { Id = 2 });

            var result = target.ObtenerAnalisisDeCalidadPorInstanceId(new Guid());

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 2);

        }
        [Test]
        public void ObtenerAnalisisDeCalidadPorInstanceIdNull()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, Dominio.Entidades.AnalisisDeCalidad>>>())).Returns((Dominio.Entidades.AnalisisDeCalidad)null);

            var result = target.ObtenerAnalisisDeCalidadPorInstanceId(new Guid());

            Assert.Null(result);
        }

        [Test]
        public void ListarDescuentos()
        {
            var tipo = new Descuento
            {
                Id = 5,
            };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Descuento, bool>>>())).Returns(new List<Descuento> { tipo });
            var resultado = target.ListarDescuentos(It.IsAny<int>());
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado[0].Id, 5);
        }

        [Test]
        public void ListarPaginadoTransaccionesSapSinFiltro()
        {
            var tipo = new TransaccionSAP
            {
                Id = 5,
                DescripcionCorta = "LC"
            };

            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<TransaccionSAP>(It.IsAny<Expression<Func<TransaccionSAP, bool>>>(), paginacion)).Returns(new ListaPaginada<TransaccionSAP>(new List<TransaccionSAP> { tipo }, 1, 10, 2));
            var resultado = target.ListarPaginadoTransaccionesSAPPorCentro("", paginacion, 1);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 1);
            Assert.AreEqual(resultado.Items[0].DescripcionCorta, tipo.DescripcionCorta);
        }

        [Test]
        public void ListarPaginadoTransaccionesSapConFiltro()
        {
            var tipo = new TransaccionSAP
            {
                Id = 5,
                DescripcionCorta = "LC"
            };

            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<TransaccionSAP>(It.IsAny<Expression<Func<TransaccionSAP, bool>>>(), paginacion)).Returns(new ListaPaginada<TransaccionSAP>(new List<TransaccionSAP> { tipo }, 1, 10, 2));
            var resultado = target.ListarPaginadoTransaccionesSAPPorCentro("A", paginacion, 1);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 1);
            Assert.AreEqual(resultado.Items[0].DescripcionCorta, tipo.DescripcionCorta);
        }

        [Test]
        public void ListarPaginadoTransaccionesSapConFiltroNumerico()
        {
            var tipo = new TransaccionSAP
            {
                Id = 5,
                DescripcionCorta = "LC"
            };

            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<TransaccionSAP>(It.IsAny<Expression<Func<TransaccionSAP, bool>>>(), paginacion)).Returns(new ListaPaginada<TransaccionSAP>(new List<TransaccionSAP> { tipo }, 1, 10, 2));
            var resultado = target.ListarPaginadoTransaccionesSAPPorCentro("4", paginacion, 1);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 1);
            Assert.AreEqual(resultado.Items[0].DescripcionCorta, tipo.DescripcionCorta);
        }

        [Test]
        public void ListarPaginadoTransaccionesSapFiltradas()
        {
            var tipo = new TransaccionSAP
            {
                Id = 5,
                DescripcionCorta = "LC"
            };

            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<TransaccionSAP, bool>>>(), paginacion)).Returns(new ListaPaginada<TransaccionSAP>(new List<TransaccionSAP> { tipo }, 1, 10, 2));
            var resultado = target.ListarPaginadoTransaccionesSAPFiltradas(1, 1, 1, FuncionSAP.AjusteDeDiferencias, paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 1);
            Assert.AreEqual(resultado.Items[0].DescripcionCorta, tipo.DescripcionCorta);
        }

        [Test]
        public void ListarCartasDePortePorCentroFechaTiposComercialesYMaterial()
        {
            repositorioMock.Setup(s => s.ListarConsulta(It.IsAny<ListarListadoCamiones>())).Returns(new List<ListadoCamionesDto> { new ListadoCamionesDto { CuilDelChofer = "a" } });
            var resultado = target.ListarCartasDePortePorCentroFechaTiposComercialesYMaterial(new List<int>(), DateTime.Now, DateTime.Now, new List<int>(), new List<int>(), true);
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado[0].CuilDelChofer, "a");
        }

        [Test]
        public void ListarArchivoOncca()
        {
            repositorioMock.Setup(s => s.ListarConsulta(It.IsAny<ListarArchivoOncca>())).Returns(new List<OnccaEmitidasDto> { new OnccaEmitidasDto { Cosecha = "a" } });
            var resultado = target.ListarArchivoOncca(new List<int>(), DateTime.Now, DateTime.Now, TipoDeWorkflow.Egreso);
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado[0].Cosecha, "a");
        }

        [Test]
        public void ListarCartasDePortePorCentroYFecha()
        {
            var tipo = new CartaPorte
            {
                Id = 5,
            };

            repositorioMock.Setup(s => s.Listar<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, CartaPorte>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new List<CartaPorte> { tipo });
            var resultado = target.ListarCartasDePortePorCentroYFecha(1, DateTime.Now, DateTime.Now, TipoDeWorkflow.Egreso);
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado[0].Id, 5);
        }

        [Test]
        public void ListarAlmacenesPorMaterial()
        {
            var tipo = new Almacen
            {
                Id = 5,
            };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Almacen, bool>>>())).Returns(new List<Almacen> { tipo });
            var resultado = target.ListarAlmacenesPorMaterial(1, 1);
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado[0].Id, 5);
        }

        [Test]
        public void ListarPaginadoAnalisisYCaladoPorCaracteristica()
        {
            var c1 = new CaracteristicaDeCalidad() { DescripcionCorta = "c1" };
            var c2 = new CaracteristicaDeCalidad() { DescripcionCorta = "c2" };
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Calado, bool>>>())).Returns(new List<Calado> { new Calado() { CaladosPorCaracteristica = new Collection<CaladoPorCaracteristica> { new CaladoPorCaracteristica { CaracteristicaDeCalidad = c1 }, new CaladoPorCaracteristica { CaracteristicaDeCalidad = c2, AnalisisPreliminar = true } } } });
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, bool>>>())).Returns(new List<Dominio.Entidades.AnalisisDeCalidad> { new Dominio.Entidades.AnalisisDeCalidad { CaracteristicasAnalizadas = new List<AnalisisPorCaracteristica> { new AnalisisPorCaracteristica { CaracteristicaDeCalidad = c2 } } } });

            var resultado = target.ListarPaginadoAnalisisYCaladoPorCaracteristica(new Guid(), new Paginacion(pagina: 1, itemsPorPagina: 3));
            Assert.AreEqual(resultado.Items.Count, 2);
        }

        [Test]
        public void ListarAnalisisYCaladoPorCaracteristica()
        {
            var c1 = new CaracteristicaDeCalidad() { DescripcionCorta = "c1" };
            var c2 = new CaracteristicaDeCalidad() { DescripcionCorta = "c2", MaterialPorCentro = new MaterialPorCentro { Material = new Material() } };
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Calado, bool>>>())).Returns(new List<Calado> { new Calado() { CaladosPorCaracteristica = new Collection<CaladoPorCaracteristica> { new CaladoPorCaracteristica { CaracteristicaDeCalidad = c1 }, new CaladoPorCaracteristica { CaracteristicaDeCalidad = c2, AnalisisPreliminar = true } } } });
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, bool>>>())).Returns(new List<Dominio.Entidades.AnalisisDeCalidad> { new Dominio.Entidades.AnalisisDeCalidad { CaracteristicasAnalizadas = new List<AnalisisPorCaracteristica> { new AnalisisPorCaracteristica { CaracteristicaDeCalidad = c2 } } } });

            var resultado = target.ListarAnalisisYCaladoPorCaracteristica(new Guid());
            Assert.AreEqual(resultado.Count, 2);
        }

        [Test]
        public void ListarCaracteristicasParaAjustesDeCalidad()
        {
            var c1 = new CaracteristicaDeCalidad() { DescripcionCorta = "c1", CaracteristicaDeCalidadMaestro = new CaracteristicaDeCalidadMaestro(), MaterialPorCentro = new MaterialPorCentro { Material = new Material() } };
            var c2 = new CaracteristicaDeCalidad() { DescripcionCorta = "c2", CaracteristicaDeCalidadMaestro = new CaracteristicaDeCalidadMaestro(), MaterialPorCentro = new MaterialPorCentro { Material = new Material() } };
            repositorioMock.Setup(s => s.Obtener<Calado>(1)).Returns(new Calado() { CaladosPorCaracteristica = new Collection<CaladoPorCaracteristica> { new CaladoPorCaracteristica { CaracteristicaDeCalidad = c1 }, new CaladoPorCaracteristica { CaracteristicaDeCalidad = c2, AnalisisPreliminar = true } } });
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, bool>>>())).Returns(new List<Dominio.Entidades.AnalisisDeCalidad> { new Dominio.Entidades.AnalisisDeCalidad { CaracteristicasAnalizadas = new List<AnalisisPorCaracteristica> { new AnalisisPorCaracteristica { CaracteristicaDeCalidad = c2 } } } });

            var resultado = target.ListarCaracteristicasParaAjustesDeCalidad(1);
            Assert.AreEqual(resultado.Count, 2);
        }
        [Test]
        public void ListarAnalisisYCaladoPorCaracteristicaNoAceptables()
        {
            var c1 = new CaracteristicaDeCalidad() { DescripcionCorta = "c1", NoAceptarSiSeDefineUnValor = true, CaladoMaximo = 4 };
            var c2 = new CaracteristicaDeCalidad() { DescripcionCorta = "c2", MaterialPorCentro = new MaterialPorCentro { Material = new Material() } };
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Calado, bool>>>())).Returns(new List<Calado> { new Calado() { CaladosPorCaracteristica = new Collection<CaladoPorCaracteristica> { new CaladoPorCaracteristica { CaracteristicaDeCalidad = c1, ValorCalado = 10 }, new CaladoPorCaracteristica { CaracteristicaDeCalidad = c2, AnalisisPreliminar = true } } } });
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, bool>>>())).Returns(new List<Dominio.Entidades.AnalisisDeCalidad> { new Dominio.Entidades.AnalisisDeCalidad { CaracteristicasAnalizadas = new List<AnalisisPorCaracteristica> { new AnalisisPorCaracteristica { CaracteristicaDeCalidad = c2 } } } });

            var resultado = target.ListarAnalisisYCaladoPorCaracteristicaNoAceptables(new Guid());
            Assert.AreEqual(resultado.Count, 1);
        }
        [Test]
        public void ListarPuestosDeTrabajoPorNombrePc()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                           .Returns(new List<PuestoDeTrabajo>
                               {
                                   new PuestoDeTrabajo
                                       {
                                           Automatico = false,
                                           Centro = new Centro {Id = 1, Descripcion = "Centro1"}
                                       }
                               });
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<PuestoDeTrabajo, PuestoDeTrabajoDto>>>(),
                         It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                           .Returns(new List<PuestoDeTrabajoDto>
                               {
                                   new PuestoDeTrabajoDto {CentroId = 1, Automatico = false}
                               });

            var result = target.ListarPuestosDeTrabajoPorNombrePc("PC1", 1);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Automatico, false);
            Assert.AreEqual(result.FirstOrDefault().CentroId, 1);
        }

        [Test]
        public void ListarPuestosDeTrabajoPorCentro()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                           .Returns(new List<PuestoDeTrabajo>
                               {
                                   new PuestoDeTrabajo
                                       {
                                           Automatico = false,
                                           Centro = new Centro {Id = 1, Descripcion = "Centro1"}
                                       }
                               });
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<PuestoDeTrabajo, PuestoDeTrabajoDto>>>(),
                         It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                           .Returns(new List<PuestoDeTrabajoDto>
                               {
                                   new PuestoDeTrabajoDto {CentroId = 1, Automatico = false}
                               });

            var result = target.ListarPuestosDeTrabajoPorCentro(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Automatico, false);
            Assert.AreEqual(result.FirstOrDefault().CentroId, 1);
        }

        [Test]
        public void ObtenerMotivoQuiebreBarrera()
        {
            repositorioMock.Setup(s => s.Obtener<MotivoQuiebreBarrera>(It.IsAny<int>()))
                           .Returns(new MotivoQuiebreBarrera { Id = 1, Motivo = "Mot", Patente = "AAA111" });

            var result = target.ObtenerMotivoQuiebreBarrera(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
            Assert.AreEqual(result.Motivo, "Mot");
            Assert.AreEqual(result.Patente, "AAA111");
        }

        [Test]
        public void ListarPaginadoMotivoQuiebreBarreraPuesto0()
        {
            repositorioMock.Setup(
                s => s.Listar(It.IsAny<Expression<Func<MotivoQuiebreBarrera, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<MotivoQuiebreBarrera>(
                                   new List<MotivoQuiebreBarrera>
                                       {
                                           new MotivoQuiebreBarrera {Motivo = "Mot", Patente = "AAA111"}
                                       }, 1, 1, 1));
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<MotivoQuiebreBarrera, MotivoQuiebreBarreraDto>>>(),
                         It.IsAny<Expression<Func<MotivoQuiebreBarrera, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<MotivoQuiebreBarreraDto>(
                                   new List<MotivoQuiebreBarreraDto>
                                       {
                                           new MotivoQuiebreBarreraDto {Motivo = "Mot", Patente = "AAA111"}
                                       }, 1, 1, 1));

            var result = target.ListarPaginadoMotivoQuiebreBarrera(0, 1, new Paginacion());


            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().Motivo, "Mot");
            Assert.AreEqual(result.Items.FirstOrDefault().Patente, "AAA111");
        }

        [Test]
        public void ListarPaginadoMotivoQuiebreBarreraPuestoDistintoA0()
        {
            repositorioMock.Setup(
                s => s.Listar(It.IsAny<Expression<Func<MotivoQuiebreBarrera, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<MotivoQuiebreBarrera>(
                                   new List<MotivoQuiebreBarrera>
                                       {
                                           new MotivoQuiebreBarrera {Motivo = "Mot", Patente = "AAA111"}
                                       }, 1, 1, 1));
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<MotivoQuiebreBarrera, MotivoQuiebreBarreraDto>>>(),
                         It.IsAny<Expression<Func<MotivoQuiebreBarrera, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<MotivoQuiebreBarreraDto>(
                                   new List<MotivoQuiebreBarreraDto>
                                       {
                                           new MotivoQuiebreBarreraDto {Motivo = "Mot", Patente = "AAA111"}
                                       }, 1, 1, 1));

            var result = target.ListarPaginadoMotivoQuiebreBarrera(85, 1, new Paginacion());


            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().Motivo, "Mot");
            Assert.AreEqual(result.Items.FirstOrDefault().Patente, "AAA111");
        }

        [Test]
        public void ObtenerInstanceIdPorTipoYNumero()
        {
            var instance = Guid.NewGuid();
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new Recorrido { NumeroDocumentoIngreso = "1234", InstanciaWorkflow = instance });

            var result = target.ObtenerInstanceIdPorTipoYNumero(TipoDocumentoIngreso.HojaDeRutaYerbatera, "1234");

            Assert.NotNull(result);
            Assert.AreEqual(result, instance);
        }

        [Test]
        public void ObtenerTarjetaRFIDAsignada()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                    It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("001");

            var result = target.ObtenerTarjetaRFIDAsignada(TipoDocumentoIngreso.HojaDeRutaYerbatera, "123");

            Assert.NotNull(result);
            Assert.AreEqual(result, "001");
        }

        [Test]
        public void EsTarjetaEnRangoValido()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TarjetaRango, bool>>>())).Returns(true);

            var result = target.EsTarjetaEnRangoValido("123456", 1);

            Assert.NotNull(result);
            Assert.True(result);

        }

        [Test]
        public void EsTarjetaBloqueada()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TarjetaBloqueada, bool>>>())).Returns(true);

            var result = target.EsTarjetaBloqueada("123456", 1);

            Assert.NotNull(result);
            Assert.True(result);

        }

        [Test]
        public void ListarPaginadoActividadesPorBarreraSemaforo()
        {
            repositorioMock.Setup(
                s => s.Listar(It.IsAny<Expression<Func<ActividadPorDispositivo, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<ActividadPorDispositivo>(new List<ActividadPorDispositivo>
                               {
                                   new ActividadPorDispositivo {Salida = "Sal1", Actividad = "Act1", Id = 1}
                               }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<ActividadPorDispositivo, ActividadPorDispositivoDto>>>(),
                         It.IsAny<Expression<Func<ActividadPorDispositivo, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<ActividadPorDispositivoDto>(new List<ActividadPorDispositivoDto>
                               {
                                   new ActividadPorDispositivoDto {Salida = "Sal1", Actividad = "Act1", Id = 1}
                               }, 1, 1, 1));

            var result = target.ListarPaginadoActividadesPorBarreraSemaforo(1, new Paginacion());
            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().Actividad, "Act1");
            Assert.AreEqual(result.Items.FirstOrDefault().Salida, "Sal1");
        }

        [Test]
        public void ObtenerActividadPorDispositivo()
        {
            repositorioMock.Setup(s => s.Obtener<ActividadPorDispositivo>(It.IsAny<int>()))
                           .Returns(new ActividadPorDispositivo { Actividad = "Act1" });
            var result = target.ObtenerActividadPorDispositivo(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Actividad, "Act1");
        }

        [Test]
        public void ObtenerControlDeTiempo()
        {
            repositorioMock.Setup(s => s.Obtener<ControlDeTiempo>(It.IsAny<int>()))
                           .Returns(new ControlDeTiempo { CodigoControl = "Ctrl" });
            var result = target.ObtenerControlDeTiempo(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.CodigoControl, "Ctrl");
        }

        [Test]
        public void ObtenerUltimoLogActividad()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerMayor(It.IsAny<Expression<Func<LogActividad, bool>>>(),
                               It.IsAny<Expression<Func<LogActividad, int>>>()))
                           .Returns(new LogActividad { Actividad = "Act1", Id = 8 });

            var result = target.ObtenerUltimoLogActividad(Guid.NewGuid(), "Act1");

            Assert.NotNull(result);
            Assert.AreEqual(result.Actividad, "Act1");
            Assert.AreEqual(result.Id, 8);
        }

        [Test]
        public void ObtenerControlDeTiempoPorCodigoControlPorGuid()
        {
            var instance = Guid.NewGuid();
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new Recorrido { InstanciaWorkflow = instance, Workflow = new Workflow { Id = 1 } });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<ControlDeTiempo, bool>>>()))
                           .Returns(new ControlDeTiempo { CodigoControl = "Ctrl", Id = 1 });

            var result = target.ObtenerControlDeTiempoPorCodigoControlPorGuid("Ctrl", instance);

            Assert.NotNull(result);
            Assert.AreEqual(result.CodigoControl, "Ctrl");
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ListarPaginadoControlesDeTiempo()
        {
            repositorioMock.Setup(
                s => s.Listar(It.IsAny<Expression<Func<ControlDeTiempo, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<ControlDeTiempo>(
                                   new List<ControlDeTiempo> { new ControlDeTiempo { CodigoControl = "Ctrl" } }, 1, 1, 1));
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<ControlDeTiempo, ControlDeTiempoDto>>>(),
                         It.IsAny<Expression<Func<ControlDeTiempo, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<ControlDeTiempoDto>(
                                   new List<ControlDeTiempoDto> { new ControlDeTiempoDto { CodigoControl = "Ctrl" } }, 1, 1,
                                   1));

            var result = target.ListarPaginadoControlesDeTiempo(new Paginacion(), 1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().CodigoControl, "Ctrl");

        }

        [Test]
        public void ListarListadoDeMuestrasDeHumedad()
        {
            repositorioMock.Setup(s => s.ListarConsulta(It.IsAny<ListarMuestrasDeHumedad>()))
                           .Returns(new List<MuestraDeHumedadDto> { new MuestraDeHumedadDto { Humedimetro = "Hum1" } });

            var result = target.ListarListadoDeMuestrasDeHumedad(1, 1, new DateTime(2015, 6, 6),
                                                                 new DateTime(2016, 9, 9));

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Humedimetro, "Hum1");
        }

        [Test]
        public void ListarTiposVehiculoBodega()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<TipoVehiculoBodega, bool>>>()))
                           .Returns(new List<TipoVehiculoBodega> { new TipoVehiculoBodega { Id = 5, Descripcion = "Bodeg1" } });

            var result = target.ListarTiposVehiculoBodega();

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Id, 5);
            Assert.AreEqual(result.FirstOrDefault().Descripcion, "Bodeg1");
        }

        [Test]
        public void ObtenerMaterialPorCodigoSap()
        {
            var codigo = "0000445561";
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Material, bool>>>()))
                           .Returns(new Material { CodigoSAP = codigo, Id = 6 });

            var result = target.ObtenerMaterialPorCodigoSap(codigo);

            Assert.NotNull(result);
            Assert.AreEqual(result.CodigoSAP, codigo);
            Assert.AreEqual(result.Id, 6);
        }

        [Test]
        public void ObtenerAlmacenDescripcion()
        {
            var id = 5;
            repositorioMock.Setup(s => s.Obtener<Almacen>(id))
                           .Returns(new Almacen { Descripcion = "alm1", Id = 6 });

            var result = target.ObtenerAlmacenDescripcion(id);

            Assert.NotNull(result);
            Assert.AreEqual(result, "alm1");
        }


        [Test]
        public void ObtenerTiempoMaximoCentro()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, int?>>>())).Returns(5);

            var resultado = target.ObtenerTiempoMaximoCentro(5);
            Assert.AreEqual(resultado, 5);
        }

        [Test]
        public void ObtenerCentroCodigoSap()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, string>>>())).Returns("5566");

            var resultado = target.ObteneCodigoSapPorCentroId(5);
            Assert.AreEqual(resultado, "5566");
        }

        [Test]
        public void ObtenerCentroIdPorInstanceId()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int>>>())).Returns(5);

            var resultado = target.ObtenerCentroIdPorInstanceId(new Guid());
            Assert.AreEqual(resultado, 5);
        }

        [Test]
        public void TieneDescuentoPorHumedad()
        {
            var caracteristicaDeCalidad = new CaracteristicaDeCalidad
            {
                EsHumedad = true
            };
            var analisisPorCaracteristica = new AnalisisPorCaracteristica
            {
                CaracteristicaDeCalidad = caracteristicaDeCalidad,
                DescuentoEnPorcentaje = new decimal(9)
            };
            var caracteristicasAnalizadas = new List<AnalisisPorCaracteristica>
            {
                //analisisPorCaracteristica
            };

            var analisis = new Dominio.Entidades.AnalisisDeCalidad
            {
                CaracteristicasAnalizadas = caracteristicasAnalizadas
            };

            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, Dominio.Entidades.AnalisisDeCalidad>>>())).Returns(analisis);

            var caladoPorCaracteristica = new CaladoPorCaracteristica
            {
                CaracteristicaDeCalidad = caracteristicaDeCalidad,
                DescuentoEnPorcentaje = new decimal(9)
            };
            var caladosPorCaracteristica = new Collection<CaladoPorCaracteristica>
                {
                    caladoPorCaracteristica
                };
            var calado = new Calado
            {
                CaladosPorCaracteristica = caladosPorCaracteristica
            };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, Calado>>>())).Returns(calado);


            var resultado = target.TieneDescuentoPorHumedad(new Guid());

            repositorioMock.Verify(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, Dominio.Entidades.AnalisisDeCalidad>>>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, Calado>>>()), Times.Exactly(1));

            Assert.AreEqual(resultado, true);
        }

        [Test]
        public void TieneAnalisisDeCalidad()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            var resultado = target.TieneAnalisisDeCalidad(new Guid());
            Assert.AreEqual(resultado, true);
        }

        [Test]
        public void ObtenerBalanza()
        {
            var id = 5;
            repositorioMock.Setup(s => s.Obtener<Balanza>(id))
                           .Returns(new Balanza { Nombre = "Balanza1", Id = 5 });

            var result = target.ObtenerBalanza(id);

            Assert.NotNull(result);
            Assert.AreEqual(result.Nombre, "Balanza1");
            Assert.AreEqual(result.Id, 5);
        }

        [Test]
        public void ObtenerBalanzaNombre()
        {
            var id = 5;
            repositorioMock.Setup(s => s.Obtener<Balanza>(id))
                           .Returns(new Balanza { Nombre = "alm1", Id = 6 });

            var result = target.ObtenerBalanzaNombre(id);

            Assert.NotNull(result);
            Assert.AreEqual(result, "alm1");
        }

        [Test]
        public void ListarTodasLasBalanzas()
        {
            var centro = new Centro
            {
                Id = 1,
                Descripcion = "cent1"
            };
            var blz1 = new Balanza
            {
                Id = 5,
                Nombre = "blnza1",
                Centro = centro
            };
            var blz2 = new Balanza
            {
                Id = 6,
                Nombre = "blnza2",
                Centro = centro
            };

            repositorioMock.Setup(s => s.Listar<Balanza>(It.IsAny<Expression<Func<Balanza, bool>>>(), It.IsAny<int>())).Returns(new List<Balanza> { blz1, blz2 });
            var resultado = target.ListarTodasLasBalanzas(1);
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Nombre, blz1.Nombre);
            Assert.AreEqual(resultado[1].Id, blz2.Id);
        }

        [Test]
        public void ListarBalanzas()
        {
            var blz1 = new Balanza
            {
                Id = 5,
                Nombre = "blnza1"
            };
            var blz2 = new Balanza
            {
                Id = 6,
                Nombre = "blnza2"
            };

            repositorioMock.Setup(s => s.Listar<Balanza>(It.IsAny<Expression<Func<Balanza, bool>>>(), It.IsAny<int>())).Returns(new List<Balanza> { blz1, blz2 });
            var resultado = target.ListarBalanzasActivas(5, TipoVehiculo.Camión);
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Nombre, blz1.Nombre);
            Assert.AreEqual(resultado[1].Id, blz2.Id);
        }

        [Test]
        public void ListarBalanzasPorNombrePc()
        {
            var blz1 = new Balanza
            {
                Id = 5,
                Nombre = "blnza1"
            };

            repositorioMock.Setup(s => s.Listar<Balanza>(It.IsAny<Expression<Func<Balanza, bool>>>())).Returns(new List<Balanza> { blz1 });
            var resultado = target.ListarBalanzasActivasPorNombrePc(5, "PC1", TipoVehiculo.Camión);
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado[0].Nombre, blz1.Nombre);
        }

        [Test]
        public void BuscarCentros()
        {
            var blz1 = new CentroInfoDto
            {
                Id = 5,
                Descripcion = "blnza1"
            };

            var blz2 = new CentroInfoDto
            {
                Id = 6,
                Descripcion = "blnza2"
            };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Centro, CentroInfoDto>>>(), It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>())).Returns(new List<CentroInfoDto> { blz1, blz2 });
            var resultado = target.BuscarCentros("desc");
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Descripcion, blz1.Descripcion);
            Assert.AreEqual(resultado[1].Descripcion, blz2.Descripcion);
        }

        [Test]
        public void BuscarCentro()
        {
            var blz1 = new CentroInfoDto
            {
                Id = 5,
                Descripcion = "blnza1"
            };

            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, CentroInfoDto>>>())).Returns(blz1);
            var resultado = target.BuscarCentro("desc");
            Assert.AreEqual(resultado.Id, 5);
            Assert.AreEqual(resultado.Descripcion, blz1.Descripcion);
        }

        [Test]
        public void BuscarCentroBodega()
        {
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<Expression<Func<Centro, bool>>>())).Returns(new Centro { Descripcion = "alm1", Id = 6 });
            var result = target.BuscarCentroBodega("crit");

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "alm1");
            Assert.AreEqual(result.Id, 6);
        }



        [Test]
        public void ObtenerCartaPorte()
        {
            repositorioMock.Setup(s => s.Obtener<CartaPorte>(It.IsAny<int>()))
                           .Returns(new CartaPorte { Id = 1 });

            var result = target.ObtenerCartaPorte(1);

            repositorioMock.Verify(s => s.Obtener<CartaPorte>(1), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerCartaPortePorCentroYNumero()
        {
            repositorioMock.Setup(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DateTime>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()))
                           .Returns(new CartaPorte { Id = 3 });

            var result = target.ObtenerCartaPortePorCentroYNumero("123", 1);

            repositorioMock.Verify(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DateTime>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 3);
        }

        [Test]
        public void ObtenerCartaPorteAReutilizarPorNumero()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Returns(new Workflow { Id = 1 });
            repositorioMock.Setup(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DateTime>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()))
                           .Returns(new CartaPorte { Id = 3 });
            repositorioMock.Setup(s => s.Existe<MaterialPorWorkflow>(It.IsAny<Expression<Func<MaterialPorWorkflow, bool>>>()))
                           .Returns(true);
            var result = target.ObtenerCartaPorteAReutilizarPorNumero("123", 1, "workflow");

            repositorioMock.Verify(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DateTime>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.CartaPorte.Id, 3);
        }

        [Test]
        public void ObtenerCartaPorteAReutilizarPorNumeroSinCp()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Returns(new Workflow { Id = 1 });
            repositorioMock.Setup(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DateTime>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()))
                           .Returns((CartaPorte)null);
            repositorioMock.Setup(s => s.Existe<MaterialPorWorkflow>(It.IsAny<Expression<Func<MaterialPorWorkflow, bool>>>()))
                           .Returns(true);
            var result = target.ObtenerCartaPorteAReutilizarPorNumero("123", 1, "workflow");

            repositorioMock.Verify(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DateTime>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.CodigoDeError, 1);
        }

        [Test]
        public void ObtenerCartaPorteAReutilizarPorNumeroSinMaterial()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Returns(new Workflow { Id = 1 });
            repositorioMock.Setup(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DateTime>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()))
                           .Returns(new CartaPorte { Id = 3, Material = new Material() });
            repositorioMock.Setup(s => s.Existe<MaterialPorWorkflow>(It.IsAny<Expression<Func<MaterialPorWorkflow, bool>>>()))
                           .Returns(false);
            var result = target.ObtenerCartaPorteAReutilizarPorNumero("123", 1, "workflow");

            repositorioMock.Verify(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DateTime>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.CodigoDeError, 2);
        }

        [Test]
        public void ObtenerCartaPorteRedespachoPorNumero()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>())).Returns(new Workflow { Id = 1 });
            repositorioMock.Setup(s => s.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(false);

            repositorioMock.Setup(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DateTime>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>())).Returns(new CartaPorte { Id = 3 });

            repositorioMock.Setup(s => s.Existe<MaterialPorWorkflow>(It.IsAny<Expression<Func<MaterialPorWorkflow, bool>>>())).Returns(true);
            var result = target.ObtenerCartaPorteRedespachoPorNumero("123", 1, "workflow", 0, false, false);

            repositorioMock.Verify(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DateTime>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.CartaPorte.Id, 3);
        }

        [Test]
        public void ObtenerCartaPorteRedespachoPorNumeroMismoCentro()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>())).Returns(new Workflow { Id = 1 });
            repositorioMock.Setup(s => s.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            repositorioMock.Setup(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DateTime>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>())).Returns(new CartaPorte { Id = 3 });

            repositorioMock.Setup(s => s.Existe<MaterialPorWorkflow>(It.IsAny<Expression<Func<MaterialPorWorkflow, bool>>>())).Returns(true);
            var result = target.ObtenerCartaPorteRedespachoPorNumero("123", 1, "workflow", 0, false, false);

            Assert.NotNull(result);
            Assert.AreEqual(result.CodigoDeError, 4);
        }

        [Test]
        public void ObtenerCartaPorteRedespachoPorNumeroSinMaterial()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>())).Returns(new Workflow { Id = 1 });
            repositorioMock.Setup(s => s.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(false);

            repositorioMock.Setup(s => s.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DateTime>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>())).Returns(new CartaPorte { Id = 3, Material = new Material() });

            repositorioMock.Setup(s => s.Existe<MaterialPorWorkflow>(It.IsAny<Expression<Func<MaterialPorWorkflow, bool>>>())).Returns(false);
            var result = target.ObtenerCartaPorteRedespachoPorNumero("123", 1, "workflow", 0, false, false);

            Assert.NotNull(result);
            Assert.AreEqual(result.CodigoDeError, 2);
        }

        [Test]
        public void ObtenerCartaPorteVaciaEgreso()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>())).Returns(new Workflow { Id = 1, TipoDeWorkflow = TipoDeWorkflow.Egreso });
            repositorioMock.Setup(s => s.Obtener<Centro>(1)).Returns(new Centro { Id = 1, Localidad = new Localidad { Id = 5, }, CodigoEstablecimiento = "b" });
            repositorioMock.Setup(s => s.Obtener<Proveedor>(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { Id = 1 });

            var result = target.ObtenerCartaPorteVacia(1, "workflow", "a", "b");

            Assert.NotNull(result);
            Assert.AreEqual(result.TitularCartaPorteId, 1);
            Assert.AreEqual(result.DestinatarioId, 1);
            Assert.AreEqual(result.ProcedenciaId, 5);
            Assert.AreEqual(result.CodEstab, "b");
        }

        [Test]
        public void ObtenerCartaPorteVaciaIngreso()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>())).Returns(new Workflow { Id = 1, TipoDeWorkflow = TipoDeWorkflow.Ingreso });
            repositorioMock.Setup(s => s.Obtener<Centro>(1)).Returns(new Centro { Id = 1, Localidad = new Localidad { Id = 5, }, CodigoEstablecimiento = "b" });
            repositorioMock.Setup(s => s.Obtener<Proveedor>(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { Id = 1 });

            var result = target.ObtenerCartaPorteVacia(1, "workflow", "a", "b");

            Assert.NotNull(result);
            Assert.AreEqual(result.DestinoId, 1);
        }

        [Test]
        public void ObtenerCartaPorteVaciaFasonEgreso()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>())).Returns(new Workflow { Id = 1, TipoDeWorkflow = TipoDeWorkflow.Egreso });
            repositorioMock.Setup(s => s.Obtener<Centro>(1)).Returns(new Centro { Id = 1, Localidad = new Localidad { Id = 5, }, CodigoEstablecimiento = "b" });
            repositorioMock.Setup(s => s.Obtener<Proveedor>(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { Id = 1 });
            repositorioMock.Setup(s => s.Obtener<Cliente>(It.IsAny<Expression<Func<Cliente, bool>>>())).Returns(new Cliente { Id = 2 });

            var result = target.ObtenerCartaPorteVaciaFason(1, "workflow", "a", "b");

            Assert.NotNull(result);
            Assert.AreEqual(result.TitularCartaPorteId, 1);
            Assert.AreEqual(result.DestinatarioId, 2);
            Assert.AreEqual(result.ProcedenciaId, 5);
            Assert.AreEqual(result.CodEstab, "b");
        }

        [Test]
        public void ObtenerCartaPorteVaciaFasonIngreso()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>())).Returns(new Workflow { Id = 1, TipoDeWorkflow = TipoDeWorkflow.Ingreso });
            repositorioMock.Setup(s => s.Obtener<Centro>(1)).Returns(new Centro { Id = 1, Localidad = new Localidad { Id = 5, }, CodigoEstablecimiento = "b" });
            repositorioMock.Setup(s => s.Obtener<Proveedor>(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor { Id = 1 });

            var result = target.ObtenerCartaPorteVaciaFason(1, "workflow", "a", "b");

            Assert.NotNull(result);
            Assert.AreEqual(result.DestinoId, 1);
        }

        [Test]
        public void ListarPaginadoBalanza()
        {
            var centro = new Balanza
            {
                Id = 5,
                Nombre = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<Balanza>(It.IsAny<Expression<Func<Balanza, bool>>>(), paginacion)).Returns(new ListaPaginada<Balanza>(new List<Balanza> { centro, new Balanza { Id = 999, Nombre = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoBalanza("", 1, paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Nombre, centro.Nombre);
        }

        [Test]
        public void ListarPaginadoBocasDestino()
        {
            var centro = new BocaDestino
            {
                Id = 5,
                CodigoONCCA = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<BocaDestino>(It.IsAny<Expression<Func<BocaDestino, bool>>>(), paginacion)).Returns(new ListaPaginada<BocaDestino>(new List<BocaDestino> { centro, new BocaDestino { Id = 999, CodigoONCCA = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoBocasDestino("", paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].CodigoONCCA, centro.CodigoONCCA);
        }

        [Test]
        public void ListarPaginadoCaladoPorCaracteristica()
        {
            var centro = new CaladoPorCaracteristica
            {
                Id = 5,
                Rango = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<CaladoPorCaracteristica>(It.IsAny<Expression<Func<CaladoPorCaracteristica, bool>>>(), paginacion)).Returns(new ListaPaginada<CaladoPorCaracteristica>(new List<CaladoPorCaracteristica> { centro, new CaladoPorCaracteristica { Id = 999, Rango = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoCaladoPorCaracteristica(1, paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Rango, centro.Rango);
        }

        [Test]
        public void ListarPaginadoCalidadMaterial()
        {
            var centro = new CalidadMaterial
            {
                Id = 5,
                Descripcion = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<CalidadMaterial>(It.IsAny<Expression<Func<CalidadMaterial, bool>>>(), paginacion)).Returns(new ListaPaginada<CalidadMaterial>(new List<CalidadMaterial> { centro, new CalidadMaterial { Id = 999, Descripcion = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoCalidadMaterial(1, 1, paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Descripcion, centro.Descripcion);
        }

        [Test]
        public void ListarPaginadoCalle()
        {
            var centro = new Calle
            {
                Id = 5,
                Codigo = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<Calle>(It.IsAny<Expression<Func<Calle, bool>>>(), paginacion)).Returns(new ListaPaginada<Calle>(new List<Calle> { centro, new Calle { Id = 999, Codigo = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoCalle(1, paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Codigo, centro.Codigo);
        }

        [Test]
        public void ListarPaginadoCamaras()
        {
            var centro = new Camara
            {
                Id = 5,
                Descripcion = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<Camara>(It.IsAny<Expression<Func<Camara, bool>>>(), paginacion)).Returns(new ListaPaginada<Camara>(new List<Camara> { centro, new Camara { Id = 999, Descripcion = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoCamaras("", paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Descripcion, centro.Descripcion);
        }

        [Test]
        public void ListarPaginadoCasilleros()
        {
            var centro = new Casillero
            {
                Id = 5,
                Numero = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<Casillero>(It.IsAny<Expression<Func<Casillero, bool>>>(), paginacion)).Returns(new ListaPaginada<Casillero>(new List<Casillero> { centro, new Casillero { Id = 999, Numero = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoCasilleros(paginacion, 1);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Numero, centro.Numero);
        }

        [Test]
        public void ListarPaginadoCentrosPorUsuario()
        {
            var centro = new Centro
            {
                Id = 5,
                Descripcion = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<Centro>(It.IsAny<Expression<Func<Centro, bool>>>(), paginacion)).Returns(new ListaPaginada<Centro>(new List<Centro> { centro, new Centro { Id = 999, Descripcion = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoCentrosPorUsuario("", paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].Descripcion, centro.Descripcion);
        }

        [Test]
        public void ListarPaginadoConversionCentro()
        {
            var centro = new ConversionCentro
            {
                Id = 5,
                CodigoCamara = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<ConversionCentro>(It.IsAny<Expression<Func<ConversionCentro, bool>>>(), paginacion)).Returns(new ListaPaginada<ConversionCentro>(new List<ConversionCentro> { centro, new ConversionCentro { Id = 999, CodigoCamara = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoConversionCentro(1, paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].CodigoCamara, centro.CodigoCamara);
        }

        [Test]
        public void ListarPaginadoConversionGrupo()
        {
            var centro = new ConversionGrupo
            {
                Id = 5,
                CodigoSegunCamara = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<ConversionGrupo>(It.IsAny<Expression<Func<ConversionGrupo, bool>>>(), paginacion)).Returns(new ListaPaginada<ConversionGrupo>(new List<ConversionGrupo> { centro, new ConversionGrupo { Id = 999, CodigoSegunCamara = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoConversionGrupo(paginacion, 1);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].CodigoSegunCamara, centro.CodigoSegunCamara);
        }

        [Test]
        public void ListarPaginadoConversionMaterial()
        {
            var centro = new ConversionMaterial
            {
                Id = 5,
                CodigoCamara = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<ConversionMaterial>(It.IsAny<Expression<Func<ConversionMaterial, bool>>>(), paginacion)).Returns(new ListaPaginada<ConversionMaterial>(new List<ConversionMaterial> { centro, new ConversionMaterial { Id = 999, CodigoCamara = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoConversionMaterial(1, paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].CodigoCamara, centro.CodigoCamara);
        }

        [Test]
        public void ListarPaginadoConversionProcedencia()
        {
            var centro = new ConversionProcedencia
            {
                Id = 5,
                CodigoCamara = "Centro 1",
            };
            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar<ConversionProcedencia>(It.IsAny<Expression<Func<ConversionProcedencia, bool>>>(), paginacion)).Returns(new ListaPaginada<ConversionProcedencia>(new List<ConversionProcedencia> { centro, new ConversionProcedencia { Id = 999, CodigoCamara = "ZZZ" } }, 1, 10, 2));
            var resultado = target.ListarPaginadoConversionProcedencia(paginacion, 1);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 2);
            Assert.AreEqual(resultado.Items[0].CodigoCamara, centro.CodigoCamara);
        }
        
        [Test]
        public void ObtenerMaterialPorCentro()
        {
            var material = new Material
            {
                Id = 1,
                Descripcion = "mat1"
            };
            var centro = new Centro
            {
                Id = 1,
                Descripcion = "cent1"
            };
            var materialPorCentro = new MaterialPorCentro
            {
                Id = 5,
                Material = material,
                Centro = centro
            };

            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Returns(materialPorCentro);
            var resultado = target.ObtenerMaterialPorCentro(1, 1);
            Assert.AreEqual(resultado.Id, materialPorCentro.Id);
            Assert.AreEqual(resultado.CentroId, centro.Id);
            Assert.AreEqual(resultado.MaterialId, material.Id);
        }

        //[Test]
        //public void ObtenerMaterialPorCentroPorInstanceId()
        //{
        //    var material = new Material
        //        {
        //            Id = 55,
        //            Descripcion = "mat1"
        //        };
        //    var centro = new Centro
        //        {
        //            Id = 31,
        //            Descripcion = "cent1"
        //        };
        //    var materialPorCentro = new MaterialPorCentro
        //        {
        //            Id = 5,
        //            Material = material,
        //            Centro = centro
        //        };
        //    dynamic mXc = new { MaterialId = material.Id, CentroId = centro.Id };
        //    repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, dynamic>>>())).Returns(mXc);
        //    repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Returns(materialPorCentro);
        //    var resultado = target.ObtenerMaterialPorCentroPorInstanceId(new Guid());
        //    Assert.AreEqual(resultado, Is.Not.Null);
        //    Assert.AreEqual(resultado.CentroId, centro.Id);
        //    Assert.AreEqual(resultado.MaterialId, material.Id);
        //}

        [Test]
        public void BuscarMaterialesPorCentro()
        {
            var material = new Material
            {
                Id = 1,
                Descripcion = "mat1"
            };
            var centro = new Centro
            {
                Id = 1,
                Descripcion = "cent1"
            };
            var materialPorCentro1 = new MaterialPorCentro
            {
                Id = 5,
                Material = material,
                Centro = centro
            };
            var materialPorCentro2 = new MaterialPorCentro
            {
                Id = 6,
                Material = material,
                Centro = centro
            };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>(), It.IsAny<int>())).Returns(new List<MaterialPorCentro> { materialPorCentro1, materialPorCentro2 });
            var resultado = target.BuscarMaterialesPorCentro(1, "cent1");
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Id, 5);
            Assert.AreEqual(resultado[1].Id, 6);
        }

        [Test]
        public void ListarTaraRomaneos()
        {
            var tipo = new TaraRomaneo()
            {
                Id = 5,
                Descripcion = "Libreta Cívica",
            };

            var paginacion = new Paginacion(ordenarPor: "Descripcion");
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<TaraRomaneo, bool>>>(), paginacion)).Returns(new ListaPaginada<TaraRomaneo>(new List<TaraRomaneo> { tipo }, 1, 10, 2));
            var resultado = target.ListarTaraRomaneos("filt", paginacion);
            Assert.AreEqual(resultado.ItemsPorPagina, 10);
            Assert.AreEqual(resultado.Pagina, 1);
            Assert.AreEqual(resultado.Items.Count, 1);
            Assert.AreEqual(resultado.Items[0].Descripcion, tipo.Descripcion);
        }

        [Test]
        public void ListarListadoDeCalidades()
        {
            repositorioMock.Setup(s => s.ListarConsulta(It.IsAny<ListarListadoDeCalidades>()))
                           .Returns(new List<ListadoDeCalidadesDto> { new ListadoDeCalidadesDto { Centro = "Centro1" } });

            var result = target.ListarListadoDeCalidades(new List<int> { 1 }, new DateTime(2015, 6, 6),
                                                         new DateTime(2015, 7, 7), new List<int> { 1 }, 1);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Centro, "Centro1");
        }

        [Test]
        public void ObtenerAlmacenPredeterminado()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerMayor(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>(),
                               It.IsAny<Expression<Func<MaterialPorCentro, int>>>(),
                               It.IsAny<Expression<Func<MaterialPorCentro, Almacen>>>()))
                           .Returns(new Almacen { Descripcion = "Alm1" });

            var result = target.ObtenerAlmacenPredeterminado(1, 1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "Alm1");
        }

        [Test]
        public void ObtenerNumeroAleatorio()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroAleatorio()).Returns(1);

            var result = target.ObtenerNumeroAleatorio();
            Assert.AreEqual(result, 1);
        }

        [Test]
        public void ListarPaginadoPuestosDeTrabajo()
        {
            var listapg = new ListaPaginada<PuestoDeTrabajoDto>(new List<PuestoDeTrabajoDto>
                {
                    new PuestoDeTrabajoDto
                        {
                            Id = 1,
                            NombrePc = "PC1"
                        }
                }, 1, 1, 1);


            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<PuestoDeTrabajo>(new List<PuestoDeTrabajo>
                               {
                                   new PuestoDeTrabajo
                                       {
                                           Id = 1,
                                           NombrePc = "PC1"
                                       }
                               }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<PuestoDeTrabajo, PuestoDeTrabajoDto>>>(),
                         It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(listapg);

            var result = target.ListarPaginadoPuestosDeTrabajo("a", 1, new Paginacion());
            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().NombrePc, "PC1");

        }

        [Test]
        public void ListarPuestosDeTrabajo()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                           .Returns(new List<PuestoDeTrabajo>
                               {
                                   new PuestoDeTrabajo
                                       {
                                           Automatico = false,
                                           Centro = new Centro {Id = 1, Descripcion = "Centro1"}
                                       }
                               });
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<PuestoDeTrabajo, PuestoDeTrabajoDto>>>(),
                         It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                           .Returns(new List<PuestoDeTrabajoDto>
                               {
                                   new PuestoDeTrabajoDto {CentroId = 1, Automatico = false}
                               });

            var result = target.ListarPuestosDeTrabajo();

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Automatico, false);
            Assert.AreEqual(result.FirstOrDefault().CentroId, 1);
        }

        [Test]
        public void ObtenerPuestoDeTrabajoPorDispositivo()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                           .Returns(new PuestoDeTrabajo { NombrePc = "PC1" });

            var result = target.ObtenerPuestoDeTrabajoPorDispositivo("1");

            Assert.NotNull(result);
            Assert.AreEqual(result.NombrePc, "PC1");
        }

        [Test]
        public void RedireccionarAListaAutomatizada()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(true);

            var result = target.RedireccionarAListaAutomatizada("a", 1);

            Assert.True(result);
        }

        [Test]
        public void ObtenerPuestoDeTrabajoPorNombrePc()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                           .Returns(new PuestoDeTrabajo { NombrePc = "PC1" });

            var result = target.ObtenerPuestoDeTrabajoPorNombrePc("PC1", 1);

            Assert.NotNull(result);
            Assert.AreEqual(result.NombrePc, "PC1");
        }

        [Test]
        public void ObtenerPuestoDeTrabajo()
        {
            repositorioMock.Setup(s => s.Obtener<PuestoDeTrabajo>(It.IsAny<int>()))
                           .Returns(new PuestoDeTrabajo { NombrePc = "PC1" });

            var result = target.ObtenerPuestoDeTrabajo(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.NombrePc, "PC1");
        }

        [Test]
        public void ListarLectores()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Lector, bool>>>()))
                           .Returns(new List<Lector>
                               {
                                   new Lector
                                       {
                                           Descripcion = "L1",
                                           Id = 1
                                       }
                               });
            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<Lector, LectorDto>>>(),
                         It.IsAny<Expression<Func<Lector, bool>>>()))
                           .Returns(new List<LectorDto>
                               {
                                   new LectorDto {Descripcion = "L1",
                                           Id = 1}
                               });

            var result = target.ListarLectores();

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Descripcion, "L1");
            Assert.AreEqual(result.FirstOrDefault().Id, 1);
        }

        [Test]
        public void ListarPaginadoHidraulica()
        {
            var listapg = new ListaPaginada<PuestosDeCargaDescargaDto>(new List<PuestosDeCargaDescargaDto>
                {
                    new PuestosDeCargaDescargaDto
                        {
                            Id = 1,
                            Nombre = "PC1"
                        }
                }, 1, 1, 1);


            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<PuestosDeCargaDescarga, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<PuestosDeCargaDescarga>(new List<PuestosDeCargaDescarga>
                               {
                                   new PuestosDeCargaDescarga
                                       {
                                           Id = 1,
                                           Nombre = "PC1"
                                       }
                               }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<PuestosDeCargaDescarga, PuestosDeCargaDescargaDto>>>(),
                         It.IsAny<Expression<Func<PuestosDeCargaDescarga, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(listapg);

            var result = target.ListarPaginadoHidraulica(1, new Paginacion());
            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().Nombre, "PC1");
        }

        [Test]
        public void ObtenerReciboMunicipal()
        {
            repositorioMock.Setup(s => s.Obtener<ReciboMunicipal>(It.IsAny<int>()))
                           .Returns(new ReciboMunicipal { Ordenanza = "A" });

            var result = target.ObtenerReciboMunicipal(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Ordenanza, "A");
        }

        [Test]
        public void ObtenerTarjetaSupervisor()
        {
            repositorioMock.Setup(s => s.Obtener<TarjetaSupervisor>(It.IsAny<int>()))
                           .Returns(new TarjetaSupervisor { Descripcion = "T1" });

            var result = target.ObtenerTarjetaSupervisor(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "T1");
        }

        [Test]
        public void ObtenerStock()
        {
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar<int>(It.IsAny<IConsultaEscalar<int>>()))
                           .Returns(1);

            var result = target.ObtenerStock(TipoStockBines.Centro, new DateTime(2015, 6, 6), 1, 1);

            Assert.NotNull(result);
            Assert.AreEqual(result, 1);
        }

        [Test]
        public void ObtenerMovimientoDeBines()
        {
            repositorioMock.Setup(s => s.Obtener<MovimientoDeBines>(It.IsAny<int>()))
                           .Returns(new MovimientoDeBines
                           {
                               Cantidad = 1,
                           });

            var result = target.ObtenerMovimientoDeBines(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Cantidad, 1);
        }

        [Test]
        public void ListarMuestrasPorLoteBiotecnologia()
        {
            repositorioMock.Setup(
                s => s.ListarConsultaPaginada(It.IsAny<ListarMuestraEnvioACamaraIntactaParaImpresionConsulta>()))
                           .Returns(
                               new ListaPaginada<MuestraEnvioACamaraBiotecnoligiaDto>(
                                   new List<MuestraEnvioACamaraBiotecnoligiaDto>
                                       {
                                           new MuestraEnvioACamaraBiotecnoligiaDto {Actividad = "Act1"}
                                       }, 1, 1, 1));

            var result = target.ListarMuestrasPorLoteBiotecnologia(1, new Paginacion());

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Actividad, "Act1");
        }

        [Test]
        public void ObtenerLoteBiotecnologiaParaImpresion()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<LoteBiotecnologia, bool>>>(),
                                    It.IsAny<Expression<Func<LoteBiotecnologia, LoteBiotecnologiaDto>>>()))
                           .Returns(new LoteBiotecnologiaDto { CamaraDesc = "Cam1" });

            repositorioMock.Setup(
                s => s.ListarConsultaPaginada(It.IsAny<ListarMuestraEnvioACamaraIntactaParaImpresionConsulta>()))
                           .Returns(
                               new ListaPaginada<MuestraEnvioACamaraBiotecnoligiaDto>(
                                   new List<MuestraEnvioACamaraBiotecnoligiaDto>
                                       {
                                           new MuestraEnvioACamaraBiotecnoligiaDto {Actividad = "Act1"}
                                       }, 1, 1, 1));

            var result = target.ObtenerLoteBiotecnologiaParaImpresion(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.CamaraDesc, "Cam1");
            Assert.AreEqual(result.Muestras.FirstOrDefault().Actividad, "Act1");

        }

        [Test]
        public void ExistenMuestraEnvioACamaraIntactaPendientes()
        {
            repositorioMock.Setup(
                s => s.ObtenerConsultaEscalar(It.IsAny<ExistenMuestraEnvioACamaraIntactaPendientesConsulta>()))
                           .Returns(true);

            var result = target.ExistenMuestraEnvioACamaraIntactaPendientes(1, 1);
            Assert.True(result);
        }

        [Test]
        public void ObtenerLoteBiotecnologiaParaArchivo()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerProyeccion(It.IsAny<Expression<Func<LoteBiotecnologia, bool>>>(),
                                    It.IsAny<Expression<Func<LoteBiotecnologia, LoteBiotecnologiaDto>>>()))
                           .Returns(new LoteBiotecnologiaDto { CamaraDesc = "Cam1" });

            repositorioMock.Setup(
                s => s.ListarConsulta(It.IsAny<ListarMuestraEnvioACamaraIntactaParaArchivoConsulta>()))
                           .Returns(new List<MuestraEnvioACamaraBiotecnoligiaDto>
                                       {
                                           new MuestraEnvioACamaraBiotecnoligiaDto {Actividad = "Act1"}
                                       });
            firmaMock.Setup(x => x.ObtenerFirmaSinLogo()).Returns(new FirmaDto());
            var result = target.ObtenerLoteBiotecnologiaParaArchivo(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.CamaraDesc, "Cam1");
            Assert.AreEqual(result.Muestras.FirstOrDefault().Actividad, "Act1");
        }

        [Test]
        public void ListarPaginadoMovimientoDeBines()
        {
            repositorioMock.Setup(
                s => s.Listar(It.IsAny<Expression<Func<MovimientoDeBines, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<MovimientoDeBines>(new List<MovimientoDeBines>
                               {
                                   new MovimientoDeBines
                                       {
                                           Material = new Material{Descripcion = "Mat1"}
                                       }
                               }, 1, 1, 1));

            repositorioMock.Setup(
                s =>
                s.Listar(It.IsAny<Expression<Func<MovimientoDeBines, MovimientoDeBinesDto>>>(),
                         It.IsAny<Expression<Func<MovimientoDeBines, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<MovimientoDeBinesDto>(
                                   new List<MovimientoDeBinesDto> { new MovimientoDeBinesDto { MaterialDescripcion = "Mat1" } },
                                   1, 1, 1));

            var result = target.ListarPaginadoMovimientoDeBines("1", new Paginacion(), 1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Items.FirstOrDefault().MaterialDescripcion, "Mat1");
        }

        [Test]
        public void ListarPaginadoTarjetasSupervisor()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<TarjetaSupervisor, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<TarjetaSupervisor>(new List<TarjetaSupervisor>
                               {
                                   new TarjetaSupervisor
                                       {
                                           Descripcion = "TS1",
                                       }
                               }, 1, 1, 1));

            var result = target.ListarPaginadoTarjetasSupervisor("", new Paginacion(), 1);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Descripcion, "TS1");
        }

        [Test]
        public void ObtenerTarjetaRango()
        {
            repositorioMock.Setup(s => s.Obtener<TarjetaRango>(It.IsAny<int>()))
                           .Returns(new TarjetaRango { Codigo = "C" });

            var result = target.ObtenerTarjetaRango(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Codigo, "C");
        }

        [Test]
        public void ListarPaginadoTarjetasRango()
        {
            repositorioMock.Setup(
                s => s.Listar(It.IsAny<Expression<Func<TarjetaRango, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<TarjetaRango>(
                                   new List<TarjetaRango> { new TarjetaRango { Codigo = "C" } }, 1, 1, 1));

            var result = target.ListarPaginadoTarjetasRango("a", new Paginacion(), 1);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Codigo, "C");
        }

        [Test]
        public void ObtenerTarjetaBloqueada()
        {
            repositorioMock.Setup(s => s.Obtener<TarjetaBloqueada>(It.IsAny<int>()))
                           .Returns(new TarjetaBloqueada { Numero = "123" });

            var result = target.ObtenerTarjetaBloqueada(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Numero, "123");
        }

        [Test]
        public void ListarPaginadoTarjetasBloqueadas()
        {
            repositorioMock.Setup(
                s => s.Listar(It.IsAny<Expression<Func<TarjetaBloqueada, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<TarjetaBloqueada>(
                                   new List<TarjetaBloqueada> { new TarjetaBloqueada { Numero = "123" } }, 1, 1, 1));

            var result = target.ListarPaginadoTarjetasBloqueadas("a", new Paginacion(), 1);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Numero, "123");
        }

        [Test]
        public void ListarTaraContenedores()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<TaraContenedor, bool>>>()))
                           .Returns(new List<TaraContenedor> { new TaraContenedor { CodigoContenedor = "C" } });

            var result = target.ListarTaraContenedores();

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().CodigoContenedor, "C");
        }

        [Test]
        public void ObtenerTaraContenedor()
        {
            repositorioMock.Setup(s => s.Obtener<TaraContenedor>(It.IsAny<int>()))
                           .Returns(new TaraContenedor { CodigoContenedor = "C" });

            var result = target.ObtenerTaraContenedor(1);
            Assert.NotNull(result);
            Assert.AreEqual(result.CodigoContenedor, "C");
        }

        [Test]
        public void ListarPaginadoTaraContenedor()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<TaraContenedor, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<TaraContenedor>(new List<TaraContenedor> { new TaraContenedor { CodigoContenedor = "C" } }, 1, 1, 1));

            var result = target.ListarPaginadoTaraContenedor("a", new Paginacion());

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().CodigoContenedor, "C");
        }

        [Test]
        public void ObtenerOrdenDeCargaContenedorPorInstanceId()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeCargaContenedor, bool>>>()))
                           .Returns(new OrdenDeCargaContenedor { PatenteCamion = "AAA111" });


            var result = target.ObtenerOrdenDeCargaContenedorPorInstanceId(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerOrdenDeCargaContenedor()
        {
            repositorioMock.Setup(s => s.Obtener<OrdenDeCargaContenedor>(It.IsAny<int>()))
                           .Returns(new OrdenDeCargaContenedor { PatenteCamion = "AAA111" });

            var result = target.ObtenerOrdenDeCargaContenedor(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerOrdenDeCargaContenedorPorNumeroDeOrden()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeCargaContenedor, bool>>>()))
                           .Returns(new OrdenDeCargaContenedor { PatenteCamion = "AAA111" });

            var result = target.ObtenerOrdenDeCargaContenedorPorNumeroDeOrden("123");

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerNumeroOrdenDeCargaContenedorGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, string>>>())).Returns("123");
            repositorioMock.Setup(s => s.ObtenerNumeroOrdenDeCargaContenedorGenerado()).Returns(1);

            var result = target.ObtenerNumeroOrdenDeCargaContenedorGenerado(1);

            Assert.NotNull(result);
            Assert.AreEqual(result, "123-00000001");
        }

        [Test]
        public void ObtenerPesoNetoDescargaUnidad()
        {
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<PesoNetoDescargaUnidadConsulta>())).Returns(1);

            var result = target.ObtenerPesoNetoDescargaUnidad(Guid.NewGuid());

            Assert.AreEqual(1, result);
        }

        [Test]
        public void ObtenerDescargaUnidad()
        {
            repositorioMock.Setup(s => s.Obtener<DescargaUnidad>(It.IsAny<int>()))
                           .Returns(new DescargaUnidad { Observaciones = "obs" });

            var result = target.ObtenerDescargaUnidad(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Observaciones, "obs");
        }

        [Test]
        public void ObtenerDescargaUnidadPorGuid()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<DescargaUnidad, bool>>>()))
                           .Returns(new List<DescargaUnidad> { new DescargaUnidad { NroPedido = "123" } });

            var result = target.ObtenerDescargaUnidadPorGuid(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().NroPedido, "123");
        }

        [Test]
        public void ListarControlDeBalanza()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<ControlDeBalanzaPesada, bool>>>()))
                           .Returns(new List<ControlDeBalanzaPesada> { new ControlDeBalanzaPesada { Id = 2 } });

            var result = target.ListarControlDeBalanza(new DateTime(2015, 1, 1), new DateTime(2015, 6, 6));

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().Id, 2);
        }

        [Test]
        public void ObtenerControlDeBalanzaPorRecorrido()
        {
            repositorioMock.Setup(
                s =>
                s.ObtenerMayor(It.IsAny<Expression<Func<ControlDeBalanza, bool>>>(),
                               It.IsAny<Expression<Func<ControlDeBalanza, int>>>()))
                           .Returns(new ControlDeBalanza { Id = 1 });

            var result = target.ObtenerControlDeBalanzaPorRecorrido(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.ControlDeBalanzaId, 1);
        }

        [Test]
        public void ObtenerControlDeBalanza()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<ControlDeBalanza, bool>>>()))
                           .Returns(new ControlDeBalanza { Id = 1 });

            var result = target.ObtenerControlDeBalanza(1);
            Assert.NotNull(result);
            Assert.AreEqual(result.ControlDeBalanzaId, 1);
        }

        [Test]
        public void ObtenerProveedorPorCodigoSap()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                           .Returns(new Proveedor { Id = 1 });

            var result = target.ObtenerProveedorPorCodigoSap("123");

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);

        }

        [Test]
        public void ObtenerProveedorPorCuit()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                           .Returns(new Proveedor { Id = 1 });

            var result = target.ObtenerProveedorPorCuit("123", new TiposProveedor());

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);

        }

        [Test]
        public void ObtenerProveedor()
        {
            repositorioMock.Setup(s => s.Obtener<Proveedor>(It.IsAny<int>()))
                           .Returns(new Proveedor { Descripcion = "P1" });

            var result = target.ObtenerProveedor(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "P1");
        }

        [Test]
        public void ObtenerItemRomaneosPorRomaneoId()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<RomaneoItem, bool>>>()))
                           .Returns(new List<RomaneoItem> { new RomaneoItem { ItemNro = 1 } });

            var result = target.ObtenerItemRomaneosPorRomaneoId(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().ItemNro, 1);
        }

        [Test]
        public void ObtenerBocaDestino()
        {
            repositorioMock.Setup(s => s.Obtener<BocaDestino>(It.IsAny<int>()))
                           .Returns(new BocaDestino { NombreBocaDeDestino = "P1" });

            var result = target.ObtenerBocaDestino(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.NombreBocaDeDestino, "P1");
        }

        [Test]
        public void ObtenerCaladoAnterior()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero<Calado>(It.IsAny<Expression<Func<Calado, bool>>>()))
                           .Returns(new Calado { CicloDeCalado = 1 });

            var result = target.ObtenerCaladoAnterior(new Guid());

            Assert.NotNull(result);
            Assert.AreEqual(result.CicloDeCalado, 1);
        }


        [Test]
        public void TestObtenerCaladoPorGuid()
        {
            var calado = new Calado { CicloDeCalado = 1 };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, Calado>>>())).Returns(calado);
            var resultado = target.ObtenerCaladoPorGuid(It.IsAny<Guid>());
            Assert.AreEqual(resultado.CicloDeCalado, calado.CicloDeCalado);
        }

        [Test]
        public void ObtenerCamara()
        {
            repositorioMock.Setup(s => s.Obtener<Camara>(It.IsAny<int>()))
                           .Returns(new Camara { Descripcion = "P1" });

            var result = target.ObtenerCamara(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "P1");
        }

        [Test]
        public void ObtenerCantidadCalados()
        {
            repositorioMock.Setup(s => s.Contar<Calado>(It.IsAny<Expression<Func<Calado, bool>>>()))
                           .Returns(5);

            var result = target.ObtenerCantidadCalados(new Guid());

            Assert.NotNull(result);
            Assert.AreEqual(result, 5);
        }

        [Test]
        public void ObtenerCaracteristicaDeCalidad()
        {
            repositorioMock.Setup(s => s.Obtener<CaracteristicaDeCalidad>(It.IsAny<int>()))
                           .Returns(new CaracteristicaDeCalidad { DescripcionCorta = "P1" });

            var result = target.ObtenerCaracteristicaDeCalidad(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.DescripcionCorta, "P1");
        }

        [Test]
        public void ObtenerCaracteristicaDeCalidadHumedad()
        {
            repositorioMock.Setup(s => s.Obtener<CaracteristicaDeCalidad>(It.IsAny<Expression<Func<CaracteristicaDeCalidad, bool>>>()))
                           .Returns(new CaracteristicaDeCalidad { DescripcionCorta = "P1" });

            var result = target.ObtenerCaracteristicaDeCalidadHumedad(1, 2);

            Assert.NotNull(result);
            Assert.AreEqual(result.DescripcionCorta, "P1");
        }

        [Test]
        public void ObtenerCasillero()
        {
            repositorioMock.Setup(s => s.Obtener<Casillero>(It.IsAny<int>()))
                           .Returns(new Casillero { Id = 2 });

            var result = target.ObtenerCasillero(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 2);
        }

        [Test]
        public void ObtenerCasilleroPorNumeroYCentro()
        {
            repositorioMock.Setup(s => s.Obtener<Casillero>(It.IsAny<Expression<Func<Casillero, bool>>>()))
                           .Returns(new Casillero { Id = 2 });

            var result = target.ObtenerCasilleroPorNumeroYCentro("23", 2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 2);
        }

        [Test]
        public void ObtenerChofer()
        {
            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<int>()))
                           .Returns(new Chofer { Id = 2 });

            var result = target.ObtenerChofer(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 2);
        }

        [Test]
        public void ObtenerChoferPorCuit()
        {
            repositorioMock.Setup(s => s.Obtener<Chofer>(It.IsAny<Expression<Func<Chofer, bool>>>()))
                           .Returns(new Chofer { Id = 2 });

            var result = target.ObtenerChoferPorCuit("23");

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 2);
        }


        [Test]
        public void ObtenerCliente()
        {
            repositorioMock.Setup(s => s.Obtener<Cliente>(It.IsAny<int>()))
                           .Returns(new Cliente { Id = 2 });

            var result = target.ObtenerCliente(1);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 2);
        }

        [Test]
        public void ObtenerClientePorCodigoSap()
        {
            repositorioMock.Setup(s => s.Obtener<Cliente>(It.IsAny<Expression<Func<Cliente, bool>>>()))
                           .Returns(new Cliente { Id = 2 });

            var result = target.ObtenerClientePorCodigoSap("23");

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 2);
        }

        [Test]
        public void TestObtenerDatosDeInstanciaAltaCTGPorGuid()
        {
            var calado = new DatosDeInstanciaAltaCTGDto { WorkflowCodigo = "a" };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, DatosDeInstanciaAltaCTGDto>>>())).Returns(calado);
            var resultado = target.ObtenerDatosDeInstanciaAltaCTGPorGuid(It.IsAny<Guid>());
            Assert.AreEqual(resultado.WorkflowCodigo, calado.WorkflowCodigo);
        }

        [Test]
        public void ObtenerExcepcionAlContro()
        {
            repositorioMock.Setup(s => s.Obtener<ExcepcionAlControl>(It.IsAny<int>()))
                           .Returns(new ExcepcionAlControl { Id = 2 });

            var result = target.ObtenerExcepcionAlControl(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 2);
        }

        [Test]
        public void ObtenerHojaDeRuta()
        {
            repositorioMock.Setup(s => s.Obtener<HojaDeRuta>(It.IsAny<int>()))
                           .Returns(new HojaDeRuta { Id = 1 });

            var result = target.ObtenerHojaDeRuta(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerHojaDeRutaPorNumero()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<HojaDeRuta, bool>>>()))
                           .Returns(new HojaDeRuta { PatenteCamion = "AAA111" });

            var result = target.ObtenerHojaDeRutaPorNumero("a");

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerHumedimetro()
        {
            repositorioMock.Setup(s => s.Obtener<Humedimetro>(It.IsAny<int>()))
                           .Returns(new Humedimetro { Codigo = "a" });

            var result = target.ObtenerHumedimetro(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Codigo, "a");
        }

        [Test]
        public void ObtenerDocumentoDeImpresion()
        {
            repositorioMock.Setup(s => s.Obtener<DocumentoDeImpresion>(It.IsAny<int>()))
                           .Returns(new DocumentoDeImpresion { Descripcion = "a" });

            var result = target.ObtenerImpresiones(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerInhabilitacionCamion()
        {
            repositorioMock.Setup(s => s.Obtener<InhabilitacionCamion>(It.IsAny<int>()))
                           .Returns(new InhabilitacionCamion { Motivo = "a" });

            var result = target.ObtenerInhabilitacionCamion(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Motivo, "a");
        }

        [Test]
        public void ObteneInhabilitacionChofer()
        {
            repositorioMock.Setup(s => s.Obtener<InhabilitacionChofer>(It.IsAny<int>()))
                           .Returns(new InhabilitacionChofer { Motivo = "a" });

            var result = target.ObtenerInhabilitacionChofer(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Motivo, "a");
        }

        [Test]
        public void ObtenerLocalidad()
        {
            repositorioMock.Setup(s => s.Obtener<Localidad>(It.IsAny<int>()))
                           .Returns(new Localidad { Descripcion = "a" });

            var result = target.ObtenerLocalidad(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerMaterialPorWorkflow()
        {
            repositorioMock.Setup(s => s.Obtener<MaterialPorWorkflow>(It.IsAny<int>()))
                           .Returns(new MaterialPorWorkflow { Id = 1 });

            var result = target.ObtenerMaterialPorWorkflow(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerMotivo()
        {
            repositorioMock.Setup(s => s.Obtener<Motivo>(It.IsAny<int>()))
                           .Returns(new Motivo { Descripcion = "a" });

            var result = target.ObtenerMotivo(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerNumeroControlDeCargaGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroDeControlDeCargaGenerado())
                           .Returns(8);

            var result = target.ObtenerNumeroControlDeCargaGenerado();

            Assert.NotNull(result);
            Assert.AreEqual(result, 8);
        }

        [Test]
        public void ObtenerNumeroDeTicketGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroDeTicketGenerado(It.IsAny<int>(), It.IsAny<bool>()))
                           .Returns(8);

            var result = target.ObtenerNumeroDeTicketGenerado(It.IsAny<int>(), It.IsAny<bool>());

            Assert.NotNull(result);
            Assert.AreEqual(result, 8);
        }

        [Test]
        public void ObtenerNumeroDocumentoFasonGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroDocumentoFasonGenerado())
                           .Returns(8);

            var result = target.ObtenerNumeroDocumentoFasonGenerado();

            Assert.NotNull(result);
            Assert.AreEqual(result, 8);
        }

        [Test]
        public void ObtenerNumeroDocumentoGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroDocumentoGenerado())
                           .Returns(8);

            var result = target.ObtenerNumeroDocumentoGenerado();

            Assert.NotNull(result);
            Assert.AreEqual(result, 8);
        }

        [Test]
        public void ObtenerNumeroHojaDeRutaGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroHojaDeRutaGenerado())
                           .Returns(8);

            var result = target.ObtenerNumeroHojaDeRutaGenerado();

            Assert.NotNull(result);
            Assert.AreEqual(result, 8);
        }

        [Test]
        public void ObtenerNumeroInformeGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroOrdenDeDescargaGenerado())
                           .Returns(8);

            var result = target.ObtenerNumeroInformeGenerado();

            Assert.NotNull(result);
            Assert.AreEqual(result, 8);
        }

        [Test]
        public void ObtenerNumeroMuestraAuditoriaGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroMuestraAuditoriaGenerado())
                           .Returns(8);

            var result = target.ObtenerNumeroMuestraAuditoriaGenerado();

            Assert.NotNull(result);
            Assert.AreEqual(result, 8);
        }

        [Test]
        public void ObtenerNumeroOrdenDeDescargaFasonGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroOrdenDeDescargaFasonGenerado())
                           .Returns(8);

            var result = target.ObtenerNumeroOrdenDeDescargaFasonGenerado(It.IsAny<int>());

            Assert.NotNull(result);
            Assert.AreEqual(result, "-00000008");
        }

        [Test]
        public void ObtenerNumeroOrdenDeDescargaGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroOrdenDeDescargaGenerado())
                           .Returns(8);

            var result = target.ObtenerNumeroOrdenDeDescargaGenerado(It.IsAny<int>());

            Assert.NotNull(result);
            Assert.AreEqual(result, "-00000008");
        }

        [Test]
        public void ObtenerNumeroOrdenEntrePlantasGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroOrdenEntrePlantasGenerado())
                           .Returns(8);

            var result = target.ObtenerNumeroOrdenEntrePlantasGenerado(It.IsAny<int>());

            Assert.NotNull(result);
            Assert.AreEqual(result, "-00000008");
        }

        [Test]
        public void ObtenerNumeroRemitoGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroOrdenDeDescargaFasonGenerado())
                           .Returns(8);

            var result = target.ObtenerNumeroRemitoGenerado();

            Assert.NotNull(result);
            Assert.AreEqual(result, 8);
        }

        [Test]
        public void ObtenerOrdenCargaInterna()
        {
            repositorioMock.Setup(s => s.Obtener<OrdenCargaInterna>(It.IsAny<int>()))
                           .Returns(new OrdenCargaInterna { Id = 1 });

            var result = target.ObtenerOrdenCargaInterna(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerObtenerOrdenCargaInternaFason()
        {
            repositorioMock.Setup(s => s.Obtener<OrdenCargaInternaFason>(It.IsAny<int>()))
                           .Returns(new OrdenCargaInternaFason { Id = 1 });

            var result = target.ObtenerOrdenCargaInternaFason(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerOrdenCargaInternaFasonPorInstanceId()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<OrdenCargaInternaFason, bool>>>()))
                           .Returns(new OrdenCargaInternaFason { PatenteCamion = "AAA111" });

            var result = target.ObtenerOrdenCargaInternaFasonPorInstanceId(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerOrdenCargaInternaFasonPorNumero()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<OrdenCargaInternaFason, bool>>>()))
                           .Returns(new OrdenCargaInternaFason { PatenteCamion = "AAA111" });

            var result = target.ObtenerOrdenCargaInternaFasonPorNumero(It.IsAny<string>());

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerOrdenCargaInternaPorInstanceId()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<OrdenCargaInterna, bool>>>()))
                           .Returns(new OrdenCargaInterna { PatenteCamion = "AAA111" });

            var result = target.ObtenerOrdenCargaInternaPorInstanceId(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerOrdenDeDescarga()
        {
            repositorioMock.Setup(s => s.Obtener<OrdenDeDescarga>(It.IsAny<int>()))
                           .Returns(new OrdenDeDescarga { Id = 1 });

            var result = target.ObtenerOrdenDeDescarga(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerOrdenDeDescargaFason()
        {
            repositorioMock.Setup(s => s.Obtener<OrdenDeDescargaFason>(It.IsAny<int>()))
                           .Returns(new OrdenDeDescargaFason { Id = 1 });

            var result = target.ObtenerOrdenDeDescargaFason(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerOrdenDeDescargaFasonPorNumeroDeOrden()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescargaFason, bool>>>()))
                           .Returns(new OrdenDeDescargaFason { PatenteCamion = "AAA111" });

            var result = target.ObtenerOrdenDeDescargaFasonPorNumeroDeOrden(It.IsAny<string>());

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerOrdenDeDescargaFasonPorNumeroDeOrdenYCliente()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescargaFason, bool>>>()))
                           .Returns(new OrdenDeDescargaFason { PatenteCamion = "AAA111" });

            var result = target.ObtenerOrdenDeDescargaFasonPorNumeroDeOrdenYCliente(It.IsAny<string>(), It.IsAny<int>());

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerOrdenDeDescargaPorNumeroDeOrden()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>()))
                           .Returns(new OrdenDeDescarga { PatenteCamion = "AAA111" });

            var result = target.ObtenerOrdenDeDescargaPorNumeroDeOrden(It.IsAny<string>());

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerOrdenEntrePlantas()
        {
            repositorioMock.Setup(s => s.Obtener<OrdenEntrePlantas>(It.IsAny<int>()))
                           .Returns(new OrdenEntrePlantas { KmRecorrer = 15 });

            var result = target.ObtenerOrdenEntrePlantas(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.KmRecorrer, 15);
        }

        [Test]
        public void ObtenerOrdenEntrePlantasPorInstanceId()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenEntrePlantas, bool>>>()))
                           .Returns(new OrdenEntrePlantas { PatenteCamion = "AAA111" });

            var result = target.ObtenerOrdenEntrePlantasPorInstanceId(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerOrdenEntrePlantasPorNumeroDeOrden()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenEntrePlantas, bool>>>()))
                           .Returns(new OrdenEntrePlantas { PatenteCamion = "AAA111" });

            var result = target.ObtenerOrdenEntrePlantasPorNumeroDeOrden(It.IsAny<string>());

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteCamion, "AAA111");
        }

        [Test]
        public void ObtenerPermiso()
        {
            repositorioMock.Setup(s => s.Obtener<Permiso>(It.IsAny<int>()))
                           .Returns(new Permiso { Descripcion = "a" });

            var result = target.ObtenerPermiso(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerPesoMaximo()
        {
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<PesoMaximoPorTipoVehiculo, bool>>>())).Returns(new PesoMaximoPorTipoVehiculo { PesoMaxEgreso = 10, TipoVehiculo = 0 });

            var result = target.ObtenerPesoMaximo(0, 1);

            Assert.NotNull(result);
            Assert.AreEqual(result, 0);
        }


        [Test]
        public void ObtenerProveedorConBocaDestino()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                           .Returns(new Proveedor { Id = 1, AM = false, Activo = false, Analisis = false, CM = false, CodigoSap = "a", Cuil = "1", BocasDestino = new List<BocaDestino>(), Descripcion = "b", Domicilio = "c", EnvioAutomaticoMail = false, EsDestinatario = false, EsIntermediario = false, EsRemitenteComercial = false, EsTitularCP = false, Localidad = new Localidad { CodigoAfip = "a", Id = 1, Descripcion = "b", Provincia = new Provincia { CodigoAfip = 2, Descripcion = "b", Id = 1, Pais = new Pais { Descripcion = "a", Id = 1 } } }, Mail = "f", PR = false, Pais = new Pais(), Pesada = false, Provincia = new Provincia(), RazonSocial = "j", ToleranciaEnKg = 10000, ToleranciaEnPorcentaje = 1000, VM = false, });

            var result = target.ObtenerProveedorConBocaDestino(It.IsAny<string>());

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerProvincia()
        {
            repositorioMock.Setup(s => s.Obtener<Provincia>(It.IsAny<int>()))
                           .Returns(new Provincia { Descripcion = "a" });

            var result = target.ObtenerProvincia(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerRecorrido()
        {
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<int>()))
                           .Returns(new Recorrido { Patente = "a" });

            var result = target.ObtenerRecorrido(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Patente, "a");
        }

        [Test]
        public void ObtenerCalle()
        {
            repositorioMock.Setup(s => s.Obtener<Calle>(It.IsAny<int>()))
                           .Returns(new Calle { Nombre = "a" });

            var result = target.ObtenerCalle(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Nombre, "a");
        }

        [Test]
        public void ObtenerCalleNombre()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<CallePorRecorrido, bool>>>(), It.IsAny<Expression<Func<CallePorRecorrido, string>>>())).Returns("a");
            
            var result = target.ObtenerCalleNombre(2);

            Assert.NotNull(result);
            Assert.AreEqual(result, "a");
        }


        //[Test]
        //public void ObtenerCamaraPorMaterialPorCentro()
        //{
        //    var ordenDeDescarga = new OrdenDeDescarga { Id = 1 };
        //    var obj = new { MaterialId = 1, CentroId = 2 };
        //    repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, object>>>())).Returns((Recorrido x) => obj);
        //    repositorioMock.Setup(s => s.ObtenerProyeccion<MaterialPorCentro, Camara>(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>(), It.IsAny<Expression<Func<MaterialPorCentro, Camara>>>())).Returns(new Camara { Descripcion = "a" });
        //    var resultado = target.ObtenerCamaraPorMaterialPorCentro(It.IsAny<Guid>());
        //    Assert.AreEqual(resultado.Id, ordenDeDescarga.Id);
        //}



        [Test]
        public void ObtenerClientePorInstanceIdOrdenCargaFas()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion<OrdenCargaFas, Cliente>(It.IsAny<Expression<Func<OrdenCargaFas, bool>>>(), It.IsAny<Expression<Func<OrdenCargaFas, Cliente>>>())).Returns(new Cliente { Descripcion = "a" });


            var result = target.ObtenerClientePorInstanceId(new Guid(), TipoDocumentoIngreso.OrdenCargaFas);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerClientePorInstanceIdOrdenCargaInterna()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion<OrdenCargaInterna, Cliente>(It.IsAny<Expression<Func<OrdenCargaInterna, bool>>>(), It.IsAny<Expression<Func<OrdenCargaInterna, Cliente>>>())).Returns(new Cliente { Descripcion = "a" });


            var result = target.ObtenerClientePorInstanceId(new Guid(), TipoDocumentoIngreso.OrdenCargaInterna);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerClientePorInstanceIdOrdenCargaInternaFason()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion<OrdenCargaInternaFason, Cliente>(It.IsAny<Expression<Func<OrdenCargaInternaFason, bool>>>(), It.IsAny<Expression<Func<OrdenCargaInternaFason, Cliente>>>())).Returns(new Cliente { Descripcion = "a" });


            var result = target.ObtenerClientePorInstanceId(new Guid(), TipoDocumentoIngreso.OrdenCargaInternaFason);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerClientePorInstanceIdOrdenDeDescargaFason()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion<OrdenDeDescargaFason, Cliente>(It.IsAny<Expression<Func<OrdenDeDescargaFason, bool>>>(), It.IsAny<Expression<Func<OrdenDeDescargaFason, Cliente>>>())).Returns(new Cliente { Descripcion = "a" });


            var result = target.ObtenerClientePorInstanceId(new Guid(), TipoDocumentoIngreso.OrdenDeDescargaFason);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerClientePorInstanceIdOrdenDeCargaContenedor()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion<OrdenDeCargaContenedor, Cliente>(It.IsAny<Expression<Func<OrdenDeCargaContenedor, bool>>>(), It.IsAny<Expression<Func<OrdenDeCargaContenedor, Cliente>>>())).Returns(new Cliente { Descripcion = "a" });


            var result = target.ObtenerClientePorInstanceId(new Guid(), TipoDocumentoIngreso.OrdenDeCargaContenedor);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerControlRecorrido()
        {

            repositorioMock.Setup(
                s =>
                s.ObtenerMayor(It.IsAny<Expression<Func<ControlRecorrido, bool>>>(), It.IsAny<Expression<Func<ControlRecorrido, int>>>())).Returns(new ControlRecorrido { Actividad = "a" });

            var result = target.ObtenerControlRecorrido(Guid.NewGuid(), "a");

            Assert.NotNull(result);
            Assert.AreEqual(result.Actividad, "a");
        }

        [Test]
        public void ObtenerConversionCaracteristica()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<ConversionCaracteristica, bool>>>()))
                           .Returns(new ConversionCaracteristica { CodigoCamara = "AAA111" });

            var result = target.ObtenerConversionCaracteristica(1, 2);

            Assert.NotNull(result);
            Assert.AreEqual(result.CodigoCamara, "AAA111");
        }

        [Test]
        public void ObtenerConversionGrup()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<ConversionGrupo, bool>>>()))
                           .Returns(new ConversionGrupo { CodigoSegunCamara = "AAA111" });

            var result = target.ObtenerConversionGrupo(1, 2);

            Assert.NotNull(result);
            Assert.AreEqual(result.CodigoSegunCamara, "AAA111");
        }

        [Test]
        public void ObtenerConversionMaterial()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<ConversionMaterial, bool>>>()))
                           .Returns(new ConversionMaterial { CodigoCamara = "AAA111" });

            var result = target.ObtenerConversionMaterial(1, 2);

            Assert.NotNull(result);
            Assert.AreEqual(result.CodigoCamara, "AAA111");
        }

        [Test]
        public void ObtenerDescargaUnidadPorNroPedido()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<DescargaUnidad, bool>>>()))
                           .Returns(new List<DescargaUnidad> { new DescargaUnidad { NroPedido = "a" } });

            var result = target.ObtenerDescargaUnidadPorNroPedido("a");

            Assert.NotNull(result);
            Assert.AreEqual(result.NroPedido, "a");
        }


        [Test]
        public void ObtenerDescargaUnidadProveedor()
        {
            repositorioMock.Setup(s => s.Obtener<DescargaUnidad>(It.IsAny<int>())).Returns(new DescargaUnidad { NroPedido = "a" });

            var ordenDeDescarga = new OrdenDeDescarga { Id = 1 };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>())).Returns(ordenDeDescarga);
            var resultado = target.ObtenerDescargaUnidadProveedor(It.IsAny<int>());
            Assert.AreEqual(resultado.NroPedido, "a");
        }

        [Test]
        public void ObtenerDocumentoDeImpresionPorCentro()
        {
            repositorioMock.Setup(s => s.Obtener<DocumentoDeImpresionPorCentro>(It.IsAny<int>()))
                           .Returns(new DocumentoDeImpresionPorCentro { Id = 1 });

            var result = target.ObtenerDocumentoDeImpresionPorCentro(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerDocumentoDeImpresionPorCentroPorInstanceId()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<DocumentoDeImpresionPorCentro, bool>>>()))
                           .Returns(new DocumentoDeImpresionPorCentro { Id = 1 });

            var result = target.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo("", 1, 0);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerEstadoServidor()
        {

            var result = target.ObtenerEstadoServidor("s1");

            Assert.NotNull(result);
            Assert.AreEqual(result.ServidorNombre, "s1");
        }

        [Test]
        public void ObtenerFirma()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero<Firma>(It.IsAny<Expression<Func<Firma, bool>>>()))
                           .Returns(new Firma { Ciudad = "a", CodigoSAP = "a", Cuit = "a", Descripcion = "a", DescripcionCorta = "b", Direccion = "b", Favicon = new byte[0], FechaDeInicio = "a", Id = 1, IngBrutosConvMultilateral = "a", Logo = new byte[0], RazonSocial = "" });

            var result = target.ObtenerFirma();

            Assert.NotNull(result);
            Assert.AreEqual(result.Ciudad, "a");
        }

        [Test]
        public void ObtenerFormatoDeImpresion()
        {
            repositorioMock.Setup(s => s.Obtener<FormatoDeImpresion>(It.IsAny<int>()))
                           .Returns(new FormatoDeImpresion { Id = 1 });

            var result = target.ObtenerFormatoDeImpresion(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 1);
        }

        [Test]
        public void ObtenerPuestosDeCargaDescarga()
        {
            repositorioMock.Setup(s => s.Obtener<PuestosDeCargaDescarga>(It.IsAny<int>()))
                           .Returns(new PuestosDeCargaDescarga { Nombre = "a" });

            var result = target.ObtenerHidraulica(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Nombre, "a");
        }

        [Test]
        public void ObtenerHidraulicaNombre()
        {
            repositorioMock.Setup(s => s.Obtener<PuestosDeCargaDescarga>(It.IsAny<int>()))
                           .Returns(new PuestosDeCargaDescarga { Nombre = "a" });

            var result = target.ObtenerHidraulicaNombre(2);

            Assert.NotNull(result);
            Assert.AreEqual(result, "a");
        }

        [Test]
        public void ObtenerImpresora()
        {
            repositorioMock.Setup(s => s.Obtener<Impresora>(It.IsAny<int>()))
                           .Returns(new Impresora { Descripcion = "a" });

            var result = target.ObtenerImpresora(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerInstanceIdPorPatente()
        {
            var instance = new Guid();
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new Recorrido { Patente = "AAA111", InstanciaWorkflow = instance });

            var result = target.ObtenerInstanceIdPorPatente("");

            Assert.NotNull(result);
            Assert.AreEqual(result, instance);
        }

        [Test]
        public void ObtenerLoteParaArchivo()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Lote, bool>>>(), It.IsAny<Expression<Func<Lote, LoteDto>>>())).Returns(new LoteDto { CamaraDesc = "a" });
            repositorioMock.Setup(s => s.ListarConsulta(It.IsAny<ListarMuestraEnvioACamaraParaArchivoConsulta>())).Returns(new List<MuestraEnvioACamaraDto> { new MuestraEnvioACamaraDto() });
            firmaMock.Setup(x => x.ObtenerFirmaSinLogo()).Returns(new FirmaDto());

            var result = target.ObtenerLoteParaArchivo(It.IsAny<int>());
            Assert.NotNull(result);
            Assert.AreEqual(result.Muestras.Count, 1);
            Assert.AreEqual(result.CamaraDesc, "a");
        }

        [Test]
        public void TestObtenerAsignacionDePuestoComando()
        {
            repositorioMock.Setup(s => s.Listar<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Recorrido> { new Recorrido { Material = new Material { Id = 12 }, Centro = new Centro { Id = 2 }, Almacen = new Almacen { Id = 1 }, BalanzaBruto = new Balanza(), BalanzaTara = new Balanza(), Calle = new Calle(), PuestosDeCargaDescargas = new Collection<PuestosDeCargaDescarga> { new PuestosDeCargaDescarga() } } });

            var result = target.ObtenerAsignacionDePuestoComando("{3F2504E0-4F89-11D3-9A0C-0305E82C3301}");
            Assert.NotNull(result);
            Assert.AreEqual(result.AlmacenId, 1);
            Assert.AreEqual(result.SonSustentables, false);
            Assert.AreEqual(result.MaterialId, 12);
        }

        [Test]
        public void TestObtenerAsignacionDePuestoComandoSustentable()
        {
            repositorioMock.Setup(s => s.Listar<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Recorrido> { new Recorrido { Material = new Material { Id = 12 }, Centro = new Centro { Id = 2 }, Almacen = new Almacen { Id = 1 }, Establecimiento = new Establecimiento() } });

            var result = target.ObtenerAsignacionDePuestoComando("{3F2504E0-4F89-11D3-9A0C-0305E82C3301}");
            Assert.NotNull(result);
            Assert.AreEqual(result.AlmacenId, 1);
            Assert.AreEqual(result.SonSustentables, true);
            Assert.AreEqual(result.MaterialId, 12);
        }

        [Test]
        public void TestObtenerAsignacionDePuestoComandoErrorSustentable()
        {
            repositorioMock.Setup(s => s.Listar<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Recorrido> {
                                new Recorrido { Material = new Material { Id = 12 }, Establecimiento = new Establecimiento(), Centro = new Centro { Id= 5} },
                                new Recorrido { Material = new Material { Id = 12 }, Establecimiento = null } });

            var result = target.ObtenerAsignacionDePuestoComando("{3F2504E0-4F89-11D3-9A0C-0305E82C3301}");
            Assert.NotNull(result);
            Assert.AreEqual(result.AlmacenId, 0);
            Assert.AreEqual(result.SonSustentables, true);
            Assert.AreEqual(result.MaterialId, 12);
        }

        [Test]
        public void TestObtenerAsignacionDePuestoComandoErrorSustentable2()
        {
            repositorioMock.Setup(s => s.Listar<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new List<Recorrido> { new Recorrido { Material = new Material { Id = 12 }, Establecimiento = new Establecimiento() }, new Recorrido { Material = new Material { Id = 12 }, Establecimiento = null } });

            var result = target.ObtenerAsignacionDePuestoComando("");
            Assert.NotNull(result);
            Assert.AreEqual(result.AlmacenId, 0);
            Assert.AreEqual(result.SonSustentables, false);
            Assert.AreEqual(result.MaterialId, null);
        }

        [Test]
        public void ListarPermisosPorUsuario()
        {
            repositorioMock.Setup(s => s.ListarConsulta(It.IsAny<PermisosPorUsuarioConsulta>()))
                           .Returns(new List<Permiso> { new Permiso { ActividadWorkflow = "a", TipoPermiso = TipoPermiso.Abm, Codigo = PermisosScato.AbmActividadConCargaAutomatica, Descripcion = "permiso", RolesAsociados = new List<Rol>() } });

            var result = target.ListarPermisosPorUsuario("usuario");

            Assert.NotNull(result);
            Assert.AreEqual(result.First().ActividadWorkflow, "a");
            Assert.AreEqual(result.First().TipoPermiso, TipoPermiso.Abm);
            Assert.AreEqual(result.First().Codigo, PermisosScato.AbmActividadConCargaAutomatica);
            Assert.AreEqual(result.First().Descripcion, "permiso");
        }

        [Test]
        public void ObtenerGruposPorUsuario()
        {
            repositorioMock.Setup(s => s.ListarConsulta(It.IsAny<PermisosPorUsuarioConsulta>()))
                           .Returns(new List<Permiso> { new Permiso { ActividadWorkflow = "a", TipoPermiso = TipoPermiso.Grupo, Codigo = PermisosScato.AbmActividadConCargaAutomatica, Descripcion = "permiso", RolesAsociados = new List<Rol>() } });

            var result = target.ObtenerGruposPorUsuario("usuario");

            Assert.NotNull(result);
            Assert.AreEqual(result.First(), "permiso");
        }

        [Test]
        public void ObtenerMuestraEnvioACamaraPorCalado()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<MuestraEnvioACamara, bool>>>()))
                           .Returns(new MuestraEnvioACamara { Patente = "AAA111" });

            var result = target.ObtenerMuestraEnvioACamaraPorCalado(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Patente, "AAA111");
        }

        [Test]
        public void ObtenerMuestraEnvioACamaraPorNumero()
        {
            repositorioMock.Setup(s => s.ListarConsulta(It.IsAny<ListarMuestraEnvioACamaraConsulta>()))
                           .Returns(new List<MuestraEnvioACamaraDto> { new MuestraEnvioACamaraDto { Patente = "AAA111" } });

            var result = target.ObtenerMuestraEnvioACamaraPorNumero(2, "nnn");

            Assert.NotNull(result);
            Assert.AreEqual(result.Patente, "AAA111");
        }

        [Test]
        public void ObtenerNotificacionesSobre()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Notificacion, bool>>>(), It.IsAny<Paginacion>()))
                           .Returns(new ListaPaginada<Notificacion>(new List<Notificacion> { new Notificacion { Grupo = "AAA111" } }, 0, 0, 0));

            var result = target.ObtenerNotificaciones("", true, false, false);

            Assert.NotNull(result);
            Assert.AreEqual(result.NotificacionesSobre.First().Grupo, "AAA111");
        }

        [Test]
        public void ObtenerNotificacionesAlerta()
        {
            repositorioMock.Setup(s => s.Listar<Notificacion>(It.IsAny<Expression<Func<Notificacion, bool>>>(), It.IsAny<int>()))
                           .Returns(new List<Notificacion> { new Notificacion { Grupo = "AAA111" } });

            var result = target.ObtenerNotificaciones("", false, true, false);

            Assert.NotNull(result);
            Assert.AreEqual(result.AlertasNoLeidas.First().Grupo, "AAA111");
        }

        [Test]
        public void ObtenerNotificacionesContar()
        {
            repositorioMock.Setup(s => s.Contar(It.IsAny<Expression<Func<Notificacion, bool>>>()))
                           .Returns(5);

            var result = target.ObtenerNotificaciones("", false, false, true);

            Assert.NotNull(result);
            Assert.AreEqual(result.Cantidad, 5);
        }

        [Test]
        public void ObtenerNumeroLote()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Lote, bool>>>(), It.IsAny<Expression<Func<Lote, string>>>()))
                           .Returns("111");

            var result = target.ObtenerNumeroLote(4);

            Assert.NotNull(result);
            Assert.AreEqual(result, "111");
        }

        [Test]
        public void ObtenerNumeroLoteBiotecnologia()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<LoteBiotecnologia, bool>>>(), It.IsAny<Expression<Func<LoteBiotecnologia, string>>>()))
                           .Returns("111");

            var result = target.ObtenerNumeroLoteBiotecnologia(4);

            Assert.NotNull(result);
            Assert.AreEqual(result, "111");
        }

        [Test]
        public void ObtenerNumeroMuestraEnvioACamara()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<MuestraEnvioACamara, bool>>>(), It.IsAny<Expression<Func<MuestraEnvioACamara, string>>>()))
                           .Returns("111");

            var result = target.ObtenerNumeroMuestraEnvioACamara(4);

            Assert.NotNull(result);
            Assert.AreEqual(result, "111");
        }

        [Test]
        public void ObtenerOrdenCargaFas()
        {
            repositorioMock.Setup(s => s.Obtener<OrdenCargaFas>(It.IsAny<int>()))
                           .Returns(new OrdenCargaFas { Cliente = new Cliente { Descripcion = "a" } });

            var result = target.ObtenerOrdenCargaFas(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.ClienteDesc, "a");
        }

        [Test]
        public void ObtenerOrdenCargaFasPorInstanceId()
        {
            repositorioMock.Setup(s => s.Obtener<OrdenCargaFas>(It.IsAny<Expression<Func<OrdenCargaFas, bool>>>()))
                           .Returns(new OrdenCargaFas { Cliente = new Cliente { Descripcion = "a" } });

            var result = target.ObtenerOrdenCargaFasPorInstanceId(new Guid());

            Assert.NotNull(result);
            Assert.AreEqual(result.ClienteDesc, "a");
        }

        [Test]
        public void ObtenerOrdenDeDescargaFasonPorInstanceId()
        {
            var ordenDeDescarga = new OrdenDeDescargaFason { Id = 1 };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescargaFason, bool>>>())).Returns(ordenDeDescarga);
            var resultado = target.ObtenerOrdenDeDescargaFasonPorInstanceId(It.IsAny<Guid>());
            Assert.AreEqual(resultado.Id, ordenDeDescarga.Id);
        }

        [Test]
        public void ObtenerOrdenDeDescargaFasonPorNumero()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero<OrdenDeDescargaFason>(It.IsAny<Expression<Func<OrdenDeDescargaFason, bool>>>()))
                           .Returns(new OrdenDeDescargaFason { Cliente = new Cliente { Descripcion = "a" } });

            var result = target.ObtenerOrdenDeDescargaFasonPorNumero("");

            Assert.NotNull(result);
            Assert.AreEqual(result.Cliente, "a");
        }

        [Test]
        public void ObtenerOrdenDeDescargaPorNumero()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero<OrdenDeDescarga>(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>()))
                           .Returns(new OrdenDeDescarga { PatenteAcoplado = "a" });

            var result = target.ObtenerOrdenDeDescargaPorNumero("");

            Assert.NotNull(result);
            Assert.AreEqual(result.PatenteAcoplado, "a");
        }

        [Test]
        public void ObtenerPatentePorGuid()
        {
            var ordenDeDescarga = new OrdenDeDescargaFason { Id = 1 };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            var resultado = target.ObtenerPatentePorGuid(It.IsAny<Guid>());
            Assert.AreEqual(resultado, "1234");
        }

        [Test]
        public void ObtenerPesoNetoRomaneo()
        {
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<PesoNetoRomaneoConsulta>())).Returns(1234);
            var resultado = target.ObtenerPesoNetoRomaneo(It.IsAny<Guid>());
            Assert.AreEqual(resultado, 1234);
        }

        [Test]
        public void ObtenerProveedorPorNroPedidoEnDescargaUnidad()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<DescargaUnidad, bool>>>())).Returns(new List<DescargaUnidad> { new DescargaUnidad { Proveedor = new Proveedor { Descripcion = "a" } } });

            var resultado = target.ObtenerProveedorPorNroPedidoEnDescargaUnidad(It.IsAny<string>(), It.IsAny<Guid>());
            Assert.AreEqual(resultado, "a");
        }

        [Test]
        public void ObtenerProveedorPorNroPedidoEnDescargaUnidadConOrden()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<DescargaUnidad, bool>>>())).Returns(new List<DescargaUnidad> { new DescargaUnidad { Proveedor = new Proveedor { Descripcion = "a", Id = 2 } } });

            var ordenDeDescarga = new OrdenDeDescarga { Id = 1, Proveedor = new Proveedor { Id = 2 } };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>())).Returns(ordenDeDescarga);
            var resultado = target.ObtenerProveedorPorNroPedidoEnDescargaUnidad(It.IsAny<string>(), It.IsAny<Guid>());
            Assert.AreEqual(resultado, "a");
        }

        [Test]
        public void ObtenerProveedorPorNroPedidoEnDescargaUnidadConOrdenDistintos()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<DescargaUnidad, bool>>>())).Returns(new List<DescargaUnidad> { new DescargaUnidad { Proveedor = new Proveedor { Descripcion = "a", Id = 3 } } });

            var ordenDeDescarga = new OrdenDeDescarga { Id = 1, Proveedor = new Proveedor { Id = 2 } };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>())).Returns(ordenDeDescarga);
            var resultado = target.ObtenerProveedorPorNroPedidoEnDescargaUnidad(It.IsAny<string>(), It.IsAny<Guid>());
            Assert.AreEqual(resultado, "distintos^-a");
        }



        [Test]
        public void ObtenerProveedorPorNroPedidoEnRomaneo()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Romaneo, bool>>>())).Returns(new List<Romaneo> { new Romaneo { Proveedor = new Proveedor { Descripcion = "a" } } });

            var resultado = target.ObtenerProveedorPorNroPedidoEnRomaneo(It.IsAny<string>(), It.IsAny<Guid>());
            Assert.AreEqual(resultado, "a");
        }

        [Test]
        public void ObtenerProveedorPorNroPedidoEnRomaneoConOrden()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Romaneo, bool>>>())).Returns(new List<Romaneo> { new Romaneo { Proveedor = new Proveedor { Descripcion = "a", Id = 2 } } });

            var ordenDeDescarga = new OrdenDeDescarga { Id = 1, Proveedor = new Proveedor { Id = 2 } };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>())).Returns(ordenDeDescarga);
            var resultado = target.ObtenerProveedorPorNroPedidoEnRomaneo(It.IsAny<string>(), It.IsAny<Guid>());
            Assert.AreEqual(resultado, "a");
        }

        [Test]
        public void ObtenerProveedorPorNroPedidoEnRomaneoOrdenDistintos()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Romaneo, bool>>>())).Returns(new List<Romaneo> { new Romaneo { Proveedor = new Proveedor { Descripcion = "a", Id = 3 } } });

            var ordenDeDescarga = new OrdenDeDescarga { Id = 1, Proveedor = new Proveedor { Id = 2 } };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>())).Returns(ordenDeDescarga);
            var resultado = target.ObtenerProveedorPorNroPedidoEnRomaneo(It.IsAny<string>(), It.IsAny<Guid>());
            Assert.AreEqual(resultado, "distintos^-a");
        }

        [Test]
        public void ObtenerRecorridoIdPorGuid()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int>>>())).Returns(1234);
            var resultado = target.ObtenerRecorridoIdPorGuid(It.IsAny<Guid>());
            Assert.AreEqual(resultado, 1234);
        }

        [Test]
        public void ObtenerRecorridoInstanceIdPorTarjetaDeAcceso()
        {
            var guid = new Guid("3F2504E0-4F89-11D3-9A0C-0305E82C3301");
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new List<Recorrido> { new Recorrido { InstanciaWorkflow = guid } });
            var resultado = target.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), 2);
            Assert.AreEqual(resultado, guid);
        }

        [Test]
        public void ObtenerRecorridoInstanceIdPorTarjetaDeAccesoNull()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new List<Recorrido>());
            var resultado = target.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), 2);
            Assert.AreEqual(resultado, new Guid());
        }

        [Test]
        public void ObtenerRecorridoNoRechazadoPorNumeroDocumento()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new List<Recorrido> { new Recorrido { Id = 1 } });
            var resultado = target.ObtenerRecorridoNoRechazadoPorNumeroDocumento(It.IsAny<string>());
            Assert.AreEqual(resultado.First().Id, 1);
        }

        [Test]
        public void ObtenerRecorridoPorDocumentoPatenteYCentro()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { Id = 1 });
            var resultado = target.ObtenerRecorridoPorDocumentoPatenteYCentro("CartaPorte", "123", "AAA111", 2);
            Assert.AreEqual(resultado.Id, 1);
        }

        [Test]
        public void ObtenerRecorridoPorGuid()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { Id = 1 });
            var resultado = target.ObtenerRecorridoPorGuid(new Guid());
            Assert.AreEqual(resultado.Id, 1);
        }

        [Test]
        public void ObtenerRecorridoPorNumeroCiu()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { Id = 1 });
            var resultado = target.ObtenerRecorridoPorNumeroCiu("123");
            Assert.AreEqual(resultado.Id, 1);
        }

        [Test]
        public void ObtenerRecorridoPorNumeroDocumento()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new List<Recorrido> { new Recorrido { Id = 1 } });
            var resultado = target.ObtenerRecorridoPorNumeroDocumento(It.IsAny<string>());
            Assert.AreEqual(resultado.First().Id, 1);
        }

        [Test]
        public void ObtenerRecorridoPorNumeroDocumentoSap()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { Id = 1 });
            var resultado = target.ObtenerRecorridoPorNumeroDocumentoSap("123");
            Assert.AreEqual(resultado.Id, 1);
        }

        [Test]
        public void ObtenerRecorridoValoresSapPorGuid()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, ValoresSapDto>>>())).Returns(new ValoresSapDto { DocumentoInternoSap = "1" });
            var resultado = target.ObtenerRecorridoValoresSapPorGuid(new Guid());
            Assert.AreEqual(resultado.DocumentoInternoSap, "1");
        }

        [Test]
        public void ObtenerRemito()
        {
            repositorioMock.Setup(s => s.Obtener<Remito>(It.IsAny<int>()))
                           .Returns(new Remito { KmRecorrer = 123 });

            var result = target.ObtenerRemito(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.KmRecorrer, 123);
        }

        [Test]
        public void TestObtenerRemitoPorInstanceId()
        {
            var Remito = new Remito { Id = 1 };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<Remito, bool>>>())).Returns(Remito);
            var resultado = target.ObtenerRemitoPorInstanceId(It.IsAny<Guid>());
            Assert.AreEqual(resultado.Id, Remito.Id);
        }

        [Test]
        public void ObtenerRemitoPorNroRemito()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<Remito, bool>>>()))
                           .Returns(new Remito { KmRecorrer = 123 });

            var result = target.ObtenerRemitoPorNroRemito("a");

            Assert.NotNull(result);
            Assert.AreEqual(result.KmRecorrer, 123);
        }

        [Test]
        public void ObtenerRemitoPorOrdenDeDescarga()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<Remito, bool>>>()))
                           .Returns(new Remito { KmRecorrer = 123 });

            var result = target.ObtenerRemitoPorOrdenDeDescarga("a");

            Assert.NotNull(result);
            Assert.AreEqual(result.KmRecorrer, 123);
        }

        [Test]
        public void ObtenerRol()
        {
            repositorioMock.Setup(s => s.Obtener<Rol>(It.IsAny<int>()))
                           .Returns(new Rol { Descripcion = "a" });

            var result = target.ObtenerRol(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerRomaneo()
        {
            repositorioMock.Setup(s => s.Obtener<Romaneo>(It.IsAny<int>()))
                           .Returns(new Romaneo { NroPedido = "a" });

            var result = target.ObtenerRomaneo(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.NroPedido, "a");
        }

        [Test]
        public void ObtenerRomaneoPorNroPedido()
        {
            repositorioMock.Setup(s => s.Listar<Romaneo>(It.IsAny<Expression<Func<Romaneo, bool>>>()))
                           .Returns(new List<Romaneo> { new Romaneo { NroPedido = "a" } });

            var result = target.ObtenerRomaneoPorNroPedido("2");

            Assert.NotNull(result);
            Assert.AreEqual(result.NroPedido, "a");
        }

        [Test]
        public void ObtenerRomaneoProveedor()
        {
            repositorioMock.Setup(s => s.Obtener<Romaneo>(It.IsAny<int>()))
                           .Returns(new Romaneo { NroPedido = "a" });

            var result = target.ObtenerRomaneoProveedor(2);

            Assert.Null(result);
        }

        [Test]
        public void ObtenerRomaneoProveedorConProveedorDistinto()
        {
            repositorioMock.Setup(s => s.Obtener<Romaneo>(It.IsAny<int>()))
                           .Returns(new Romaneo { NroPedido = "a", Proveedor = new Proveedor { Id = 2, Descripcion = "b" } });

            var ordenDeDescarga = new OrdenDeDescarga { Id = 1, Proveedor = new Proveedor { Id = 3 } };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>())).Returns(ordenDeDescarga);

            var result = target.ObtenerRomaneoProveedor(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.NroPedido, "a");
            Assert.AreEqual(result.ProveedorDescripcion, "distintos^-b");
        }

        [Test]
        public void ObtenerRomaneoProveedorConProveedorIgual()
        {
            repositorioMock.Setup(s => s.Obtener<Romaneo>(It.IsAny<int>()))
                           .Returns(new Romaneo { NroPedido = "a", Proveedor = new Proveedor { Id = 2, Descripcion = "b" } });

            var ordenDeDescarga = new OrdenDeDescarga { Id = 1, Proveedor = new Proveedor { Id = 2 } };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>())).Returns(ordenDeDescarga);

            var result = target.ObtenerRomaneoProveedor(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.NroPedido, "a");
            Assert.AreEqual(result.ProveedorDescripcion, "b");
        }

        [Test]
        public void ObtenerRomaneosPorGuid()
        {
            repositorioMock.Setup(s => s.Listar<Romaneo>(It.IsAny<Expression<Func<Romaneo, bool>>>()))
                           .Returns(new List<Romaneo> { new Romaneo { NroPedido = "a" } });

            var result = target.ObtenerRomaneosPorGuid(new Guid());

            Assert.NotNull(result);
            Assert.AreEqual(result.First().NroPedido, "a");
        }

        [Test]
        public void ObtenerSuplencia()
        {
            repositorioMock.Setup(s => s.Obtener<Suplencia>(It.IsAny<int>()))
                           .Returns(new Suplencia { UsuarioSuplente = new Usuario { NombreUsuario = "a" } });

            var result = target.ObtenerSuplencia(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.UsuarioSuplenteNombreUsuario, "a");
        }

        [Test]
        public void ObtenerTicketAccesoAfip()
        {
            repositorioMock.Setup(s => s.Listar<TicketAccesoAfip>(It.IsAny<Expression<Func<TicketAccesoAfip, bool>>>()))
                           .Returns(new List<TicketAccesoAfip> { new TicketAccesoAfip { CuitRepresentado = "a" } });

            var result = target.ObtenerTicketAccesoAfip();

            Assert.NotNull(result);
            Assert.AreEqual(result.CuitRepresentado, "a");
        }

        [Test]
        public void ObtenerTipoDeWorkflowPorGuid()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, TipoDeWorkflow>>>()))
                           .Returns(TipoDeWorkflow.Egreso);

            var result = target.ObtenerTipoDeWorkflowPorGuid(new Guid());

            Assert.NotNull(result);
            Assert.AreEqual(result, TipoDeWorkflow.Egreso);
        }

        [Test]
        public void ObtenerTransmisionASap()
        {
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>()))
                           .Returns(new TransmisionASap { MensajeError = "a" });

            var result = target.ObtenerTransmisionASap(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.MensajeError, "a");
        }

        [Test]
        public void ObtenerUsuario()
        {
            repositorioMock.Setup(s => s.Obtener<Usuario>(It.IsAny<int>()))
                           .Returns(new Usuario { Apellido = "a" });

            var result = target.ObtenerUsuario(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Apellido, "a");
        }

        [Test]
        public void ObtenerWorkflow()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<int>()))
                           .Returns(new Workflow { Descripcion = "a" });

            var result = target.ObtenerWorkflow(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerUltimaWorkflowDefinicionPorCordigo()
        {
            repositorioMock.Setup(s => s.ObtenerMayor<WorkflowDefinicion, int, int>(It.IsAny<Expression<Func<WorkflowDefinicion, bool>>>(), It.IsAny<Expression<Func<WorkflowDefinicion, int>>>(), It.IsAny<Expression<Func<WorkflowDefinicion, int>>>()))
                           .Returns(4);

            var result = target.ObtenerUltimaWorkflowDefinicionPorCordigo("a");

            Assert.NotNull(result);
            Assert.AreEqual(result, 4);
        }




        [Test]
        public void ObtenerUltimoRomaneoPorGuid()
        {
            repositorioMock.Setup(s => s.Listar<Romaneo>(It.IsAny<Expression<Func<Romaneo, bool>>>())).Returns(new List<Romaneo> { new Romaneo { NroPedido = "a" } });

            var result = target.ObtenerUltimoRomaneoPorGuid(new Guid());

            Assert.Null(result);
        }

        [Test]
        public void ObtenerUltimoRomaneoPorGuidConProveedorDistinto()
        {
            repositorioMock.Setup(s => s.Listar<Romaneo>(It.IsAny<Expression<Func<Romaneo, bool>>>())).
                Returns(new List<Romaneo> { new Romaneo { NroPedido = "a", Proveedor = new Proveedor { Id = 2, Descripcion = "b" } } });

            var ordenDeDescarga = new OrdenDeDescarga { Id = 1, Proveedor = new Proveedor { Id = 3 } };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>())).Returns(ordenDeDescarga);

            var result = target.ObtenerUltimoRomaneoPorGuid(new Guid());

            Assert.NotNull(result);
            Assert.AreEqual(result.NroPedido, "a");
            Assert.AreEqual(result.ProveedorDescripcion, "distintos^-b");
        }

        [Test]
        public void ObtenerUltimoRomaneoPorGuidConProveedorIgual()
        {
            repositorioMock.Setup(s => s.Listar<Romaneo>(It.IsAny<Expression<Func<Romaneo, bool>>>())).
                Returns(new List<Romaneo> { new Romaneo { NroPedido = "a", Proveedor = new Proveedor { Id = 2, Descripcion = "b" } } });

            var ordenDeDescarga = new OrdenDeDescarga { Id = 1, Proveedor = new Proveedor { Id = 2 } };
            repositorioMock.Setup(s => s.ObtenerProyeccion(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, string>>>())).Returns("1234");
            repositorioMock.Setup(s => s.ObtenerPrimero(It.IsAny<Expression<Func<OrdenDeDescarga, bool>>>())).Returns(ordenDeDescarga);

            var result = target.ObtenerUltimoRomaneoPorGuid(new Guid());

            Assert.NotNull(result);
            Assert.AreEqual(result.NroPedido, "a");
            Assert.AreEqual(result.ProveedorDescripcion, "b");
        }

        [Test]
        public void ObtenerUsuarioId()
        {
            repositorioMock.Setup(s => s.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>()))
                           .Returns(new Usuario { Apellido = "a" });

            var result = target.ObtenerUsuarioId("a");

            Assert.NotNull(result);
            Assert.AreEqual(result.Apellido, "a");
        }

        [Test]
        public void ObtenerVehiculoPorGuid()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion<Recorrido, Vehiculo>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, Vehiculo>>>())).Returns(new Vehiculo { Patente = "a" });

            var result = target.ObtenerVehiculoPorGuid(new Guid());

            Assert.NotNull(result);
            Assert.AreEqual(result.Patente, "a");
        }

        [Test]
        public void ObtenerWorkflowPorCodigo()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<Expression<Func<Workflow, bool>>>()))
                           .Returns(new Workflow { Descripcion = "a" });

            var result = target.ObtenerWorkflowPorCodigo("a");

            Assert.NotNull(result);
            Assert.AreEqual(result.Descripcion, "a");
        }

        [Test]
        public void ObtenerWorkflowDefinicion()
        {
            repositorioMock.Setup(s => s.Obtener<WorkflowDefinicion>(It.IsAny<int>()))
                           .Returns(new WorkflowDefinicion { NombreUsuario = "a" });

            var result = target.ObtenerWorkflowDefinicion(2);

            Assert.NotNull(result);
            Assert.AreEqual(result.NombreUsuario, "a");
        }

        [Test]
        public void ProcedenciaYCodigoValido()
        {
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                           .Returns(new Centro { Id = 1, Localidad = new Localidad { Id = 1 }, CodigoEstablecimiento = "a" });

            var result = target.ProcedenciaYCodigoValido(1, "a", 1);

            Assert.NotNull(result);
            Assert.AreEqual(result, true);
        }

        [Test]
        public void ProcedenciaYCodigoInvalido1()
        {
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                           .Returns(new Centro { Id = 1, Localidad = new Localidad { Id = 1 }, CodigoEstablecimiento = "b" });

            var result = target.ProcedenciaYCodigoValido(1, "a", 1);

            Assert.NotNull(result);
            Assert.AreEqual(result, false);
        }

        [Test]
        public void ProcedenciaYCodigoInvalido2()
        {
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                           .Returns(new Centro { Id = 1, Localidad = new Localidad { Id = 2 }, CodigoEstablecimiento = "a" });

            var result = target.ProcedenciaYCodigoValido(1, "a", 1);

            Assert.NotNull(result);
            Assert.AreEqual(result, false);
        }

        [Test]
        public void ValidacionCtgEsAutomatica()
        {
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                           .Returns(new Centro { Id = 1, SolicitaConfirmarCTG = true });

            var result = target.ValidacionCtgEsAutomatica(1);

            Assert.NotNull(result);
            Assert.AreEqual(result, false);
        }

        [Test]
        public void ValidacionCtgEsAutomaticafalse()
        {
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                           .Returns(new Centro { Id = 1, SolicitaConfirmarCTG = false });

            var result = target.ValidacionCtgEsAutomatica(1);

            Assert.NotNull(result);
            Assert.AreEqual(result, true);
        }

        [Test]
        public void ConsultarColaImpresion()
        {
            repositorioMock.Setup(s => s.Obtener<Impresora>(It.IsAny<int>())).Returns(new Impresora { Id = 1, Direccion = @"\\imp1\imp2" });

            IList<ColaImpresionDto> result = null;
            try
            {
                result = target.ConsultarColaImpresion(1, "123");
            }
            catch (Exception)
            {

            }
            Assert.AreEqual(result, null);

        }

        [Test]
        public void AccionJobImpresion()
        {
            repositorioMock.Setup(s => s.Obtener<Impresora>(It.IsAny<int>())).Returns(new Impresora { Id = 1, Direccion = @"\\imp1\imp2" });
            try
            {
                target.AccionJobImpresion(1, 2, AccionColaImpresion.Pausar);
            }
            catch (Exception)
            {
                Assert.True(true);
            }
        }

        [Test]
        public void BuscarExcepcionAlControl()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<ExcepcionAlControl, bool>>>())).Returns(true);
            try
            {
                target.BuscarExcepcionAlControl(1, 1, 1, DateTime.Now, null, 3);
            }
            catch (Exception)
            {
                Assert.True(true);
            }
        }

        [Test]
        public void ObtenerPesoNetoConDescuento()
        {
            var caladosPorCaracteristicas = new List<CaladoPorCaracteristica>
                {
                    new CaladoPorCaracteristica
                        {
                            Id = 1,
                            DescuentoEnKg = 1500,
                            CaracteristicaDeCalidad = new CaracteristicaDeCalidad{CodigoSAP = "sap1"},
                        },
                    new CaladoPorCaracteristica
                        {
                            Id = 2,
                            DescuentoEnKg = 3000,
                            CaracteristicaDeCalidad = new CaracteristicaDeCalidad{CodigoSAP = "sap2"},
                        }
                };

            var recorrido = new Recorrido
            {
                Id = 1,
                PesoBruto = 40000,
                PesoTara = 10000,
                Calado = new Calado { CaladosPorCaracteristica = caladosPorCaracteristicas }
            };
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(recorrido);
            var netoDescontado = target.ObtenerPesoNetoConDescuento(new Guid());
            Assert.That(netoDescontado.HasValue, Is.EqualTo(true));
            Assert.That(netoDescontado.Value, Is.EqualTo(25500));
        }

        [Test]
        public void ObtenerPesoNetoConDescuento2()
        {
            var caladosPorCaracteristicas = new List<CaladoPorCaracteristica>
                {
                    new CaladoPorCaracteristica
                        {
                            Id = 1,
                            DescuentoEnKg = 1500,
                            CaracteristicaDeCalidad = new CaracteristicaDeCalidad{CodigoSAP = "sap1"},
                        },
                    new CaladoPorCaracteristica
                        {
                            Id = 2,
                            DescuentoEnKg = 3000,
                            CaracteristicaDeCalidad = new CaracteristicaDeCalidad{CodigoSAP = "sap2"},
                        }
                };
            var analisis = new Dominio.Entidades.AnalisisDeCalidad
            {
                CaracteristicasAnalizadas =
                new List<AnalisisPorCaracteristica>
                    {
                            new AnalisisPorCaracteristica
                            {
                                Id = 1,
                                CaracteristicaDeCalidad = new CaracteristicaDeCalidad{CodigoSAP = "sap1"},
                                DescuentoEnKg = 4500
                            }
                    }
            };
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { Id = 1, PesoBruto = 40000, PesoTara = 10000, Calado = new Calado { CaladosPorCaracteristica = caladosPorCaracteristicas } });
            repositorioMock.Setup(s => s.Obtener<Dominio.Entidades.AnalisisDeCalidad>(It.IsAny<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, bool>>>())).Returns(analisis);
            var netoDescontado = target.ObtenerPesoNetoConDescuento(new Guid());
            Assert.That(netoDescontado.HasValue, Is.EqualTo(true));
            Assert.That(netoDescontado.Value, Is.EqualTo(25500m));
        }

        [Test]
        public void ObtenerPesoNetoConDescuento3()
        {
            var caladosPorCaracteristicas = new List<CaladoPorCaracteristica>
                {
                    new CaladoPorCaracteristica
                        {
                            Id = 1,
                            DescuentoEnPorcentaje = 0,
                            CaracteristicaDeCalidad = new CaracteristicaDeCalidad{CodigoSAP = "sap1"},
                        },
                    new CaladoPorCaracteristica
                        {
                            Id = 2,
                            DescuentoEnPorcentaje = 0,
                            CaracteristicaDeCalidad = new CaracteristicaDeCalidad{CodigoSAP = "sap2"},
                        }
                };
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { Id = 1, PesoBruto = 40000, PesoTara = 10000, Calado = new Calado { CaladosPorCaracteristica = caladosPorCaracteristicas } });
            var netoDescontado = target.ObtenerPesoNetoConDescuento(new Guid());
            Assert.That(netoDescontado.HasValue.Equals(true));
            Assert.That(netoDescontado.Value.Equals(30000));
        }

        [Test]
        public void ObtenerPesoNetoConDescuentoSinCalado()
        {

            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { Id = 1, PesoBruto = 40000, PesoTara = 10000 });
            var netoDescontado = target.ObtenerPesoNetoConDescuento(new Guid());
            Assert.That(netoDescontado.HasValue.Equals(true));
            Assert.That(netoDescontado.Value.Equals(30000));

            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Guid>())).Returns(new Recorrido { Id = 1, PesoBruto = 40000 });
            var netoDescontado2 = target.ObtenerPesoNetoConDescuento(new Guid());
            Assert.That(netoDescontado2.HasValue.Equals(true));
            Assert.That(netoDescontado2, Is.EqualTo(30000));
        }

        [Test]
        public void ObtenerVariedadPorMaterial()
        {
            var variedad = new Variedad()
            {
                Id = 1,
                Descripcion = "Variedad1",
                NumeroINV = "4"
            };
            repositorioMock.Setup(s => s.ObtenerProyeccion<Material, Variedad>(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, Variedad>>>())).Returns(variedad);

            var result = target.ObtenerVariedadPorMaterial(1);

            Assert.That(result, Is.TypeOf<VariedadDto>());
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Descripcion, Is.EqualTo("Variedad1"));
            Assert.That(result.NumeroINV, Is.EqualTo("4"));
        }

        [Test]
        public void ObtenerVariedadPorMaterialNull()
        {
            repositorioMock.Setup(s => s.ObtenerProyeccion<Material, Variedad>(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, Variedad>>>()));

            var result = target.ObtenerVariedadPorMaterial(1);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void BuscarMaterial()
        {
            var materialPorCentro = new MaterialPorCentro()
            {
                Material = new Material()
                {
                    Descripcion = "Semilla de Soja",
                    Id = 4
                }
            };
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Returns(materialPorCentro);
            var result = target.BuscarMaterial(1, "Semilla");

            Assert.That(result, Is.TypeOf<MaterialDto>());
            Assert.That(result.Descripcion, Is.EqualTo("Semilla de Soja"));
            Assert.That(result.Id, Is.EqualTo(4));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarMaterialNull()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Throws(new Exception());
            var result = target.BuscarMaterial(1, "Semilla");

            Assert.That(result, Is.Null);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarMaterialTodosLosCentros()
        {
            var material = new Material()
            {
                Id = 1,
                Activo = true,
                Descripcion = "Semilla de Soja"
            };
            repositorioMock.Setup(x => x.Obtener<Material>(It.IsAny<Expression<Func<Material, bool>>>()))
                .Returns(material);

            var result = target.BuscarMaterialTodosLosCentros("Semilla de Soja");

            Assert.That(result, Is.TypeOf<MaterialDto>());
            Assert.That(result.Descripcion, Is.EqualTo("Semilla de Soja"));
            Assert.That(result.Id, Is.EqualTo(1));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarMaterialTodosLosCentrosNull()
        {
            repositorioMock.Setup(x => x.Obtener<Material>(It.IsAny<Expression<Func<Material, bool>>>())).Throws(new Exception());

            var result = target.BuscarMaterialTodosLosCentros("Semilla de Soja");

            Assert.That(result, Is.Null);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarMaterialPorCentro()
        {
            var materialPorCentro = new MaterialPorCentro()
            {
                Material = new Material()
                {
                    Descripcion = "Semilla de Soja",
                    Id = 4
                }
            };
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Returns(materialPorCentro);
            var result = target.BuscarMaterialPorCentro(1, "Semilla");

            Assert.That(result, Is.TypeOf<MaterialPorCentroDto>());
            Assert.That(result.MaterialDesc, Is.EqualTo("Semilla de Soja"));
            Assert.That(result.MaterialId, Is.EqualTo(4));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarVinedo()
        {
            Vinedo vinedo = new VinedoPropio()
            {
                Id = 1,
                Descripcion = "Vinedo"
            };
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Vinedo, bool>>>())).Returns(vinedo);
            var result = target.BuscarVinedo("Semilla de Soja");

            Assert.That(result, Is.TypeOf<VinedoDto>());
            Assert.That(result.Descripcion, Is.EqualTo("Vinedo"));
            Assert.That(result.Id, Is.EqualTo(1));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Vinedo, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarVinedoNull()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Vinedo, bool>>>())).Throws(new Exception());
            var result = target.BuscarVinedo("Semilla de Soja");

            Assert.That(result, Is.Null);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Vinedo, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarTransportista()
        {
            var proveedor = new TransportistaInfoDto()
            {
                Cuit = "Cuit",
                Id = 1,
                RazonSocial = "RazonSocial"
            };
            repositorioMock.Setup(s => s.ObtenerProyeccion<Transportista, TransportistaInfoDto>(It.IsAny<Expression<Func<Transportista, bool>>>(), It.IsAny<Expression<Func<Transportista, TransportistaInfoDto>>>())).Returns(proveedor);

            var result = target.BuscarTransportista("Cuit");
            Assert.That(result, Is.TypeOf<TransportistaInfoDto>());
            Assert.That(result.Cuit, Is.EqualTo("Cuit"));
            Assert.That(result.RazonSocial, Is.EqualTo("RazonSocial"));
            Assert.That(result.Id, Is.EqualTo(1));
            repositorioMock.Verify(s => s.ObtenerProyeccion<Transportista, TransportistaInfoDto>(It.IsAny<Expression<Func<Transportista, bool>>>(), It.IsAny<Expression<Func<Transportista, TransportistaInfoDto>>>()), Times.Once());
        }

        [Test]
        public void BuscarTransportistaNull()
        {
            var proveedor = new TransportistaInfoDto()
            {
                Cuit = "Cuit",
                Id = 1,
                RazonSocial = "RazonSocial"
            };
            repositorioMock.Setup(s => s.ObtenerProyeccion<Transportista, TransportistaInfoDto>(It.IsAny<Expression<Func<Transportista, bool>>>(), It.IsAny<Expression<Func<Transportista, TransportistaInfoDto>>>())).Throws(new Exception());
            var result = target.BuscarTransportista("Cuit");

            Assert.That(result, Is.Null);
            repositorioMock.Verify(s => s.ObtenerProyeccion<Transportista, TransportistaInfoDto>(It.IsAny<Expression<Func<Transportista, bool>>>(), It.IsAny<Expression<Func<Transportista, TransportistaInfoDto>>>()), Times.Once());
        }

        [Test]
        public void ObtenerPesoNetoMaximo()
        {
            repositorioMock.Setup(x => x.ObtenerProyeccion<PesoMaximoPorTipoVehiculo, int?>(It.IsAny<Expression<Func<PesoMaximoPorTipoVehiculo, bool>>>(), It.IsAny<Expression<Func<PesoMaximoPorTipoVehiculo, int?>>>())).Returns(101);

            var result = target.ObtenerPesoNetoMaximo(TipoVehiculo.Camión, 1);

            Assert.That(result, Is.Not.Null);
            repositorioMock.Verify(x => x.ObtenerProyeccion<PesoMaximoPorTipoVehiculo, int?>(It.IsAny<Expression<Func<PesoMaximoPorTipoVehiculo, bool>>>(), It.IsAny<Expression<Func<PesoMaximoPorTipoVehiculo, int?>>>()), Times.Once());
        }

        [Test]
        public void ObtenerStockDeEstablecimiento()
        {
            repositorioMock.Setup(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<int>())).Returns(new StockDeEstablecimiento());
            repositorioMock.Setup(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(61000);

            var result = target.ObtenerStockDeEstablecimiento(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StockUtilizado, Is.EqualTo(61000));
            repositorioMock.Verify(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<int>()), Times.Once());
            repositorioMock.Verify(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>()), Times.Once());
        }

        [Test]
        public void ListarPaginadoStockDeEstablecimientos()
        {
            repositorioMock.Setup(s => s.Listar<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>(), It.IsAny<Paginacion>())).Returns(new ListaPaginada<StockDeEstablecimiento>(new List<StockDeEstablecimiento> { new StockDeEstablecimiento { Id = 999, CodigoEstablecimiento = "15", Cosecha = "14-15", FechaDesde = new DateTime(), FechaHasta = new DateTime(), StockDeclarado = 5001 } }, 1, 10, 2));
            repositorioMock.Setup(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(61000);

            var result = target.ListarPaginadoStockDeEstablecimientos("filtro1", new Paginacion());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            repositorioMock.Verify(s => s.Listar<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>(), It.IsAny<Paginacion>()), Times.Once());
            repositorioMock.Verify(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>()), Times.Once());
        }

        [Test]
        public void ValidarStockEstablecimiento()
        {
            var stock = new StockDeEstablecimiento
            {
                CodigoEstablecimiento = "130",
                Cosecha = "15-16",
                FechaDesde = DateTime.Now.AddDays(-1),
                FechaHasta = DateTime.Now.AddDays(1),
                StockDeclarado = 91000.13m
            };
            repositorioMock.Setup(x => x.ObtenerProyeccion<Establecimiento, string>(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Expression<Func<Establecimiento, string>>>())).Returns("130");
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>())).Returns(new CartaPorte { Cosecha = "15-16" });
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, int?>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(30000);
            repositorioMock.Setup(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>())).Returns(stock);
            repositorioMock.Setup(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(61000);
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, bool>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            var result = target.ValidarStockEstablecimiento(5, new Guid());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
            repositorioMock.Verify(x => x.ObtenerProyeccion<Establecimiento, string>(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Expression<Func<Establecimiento, string>>>()), Times.Once());
            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()), Times.Once());
            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, int?>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>()), Times.Once());
            repositorioMock.Verify(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>()), Times.Once());
            repositorioMock.Verify(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>()), Times.Once());
        }

        [Test]
        public void ValidarStockEstablecimientoError1()
        {
            repositorioMock.Setup(x => x.ObtenerProyeccion<Establecimiento, string>(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Expression<Func<Establecimiento, string>>>())).Returns("130");
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>())).Returns(new CartaPorte { Cosecha = "15-16" });
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, int?>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(30000);
            repositorioMock.Setup(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>())).Returns((StockDeEstablecimiento)null);
            repositorioMock.Setup(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(61000);
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, bool>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            var result = target.ValidarStockEstablecimiento(5, new Guid());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(true));
            Assert.That(result.Errores.FirstOrDefault().Value, Is.EqualTo(Textos.AsignacionEstablecimientoError_SinStockDeEstablecimiento));
            repositorioMock.Verify(x => x.ObtenerProyeccion<Establecimiento, string>(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Expression<Func<Establecimiento, string>>>()), Times.Once());
            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()), Times.Once());
            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, int?>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>()), Times.Once());
            repositorioMock.Verify(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>()), Times.Once());
            repositorioMock.Verify(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>()), Times.Exactly(0));
        }

        [Test]
        public void ValidarStockEstablecimientoError2()
        {
            var stock = new StockDeEstablecimiento
            {
                CodigoEstablecimiento = "130",
                Cosecha = "15-16",
                FechaDesde = DateTime.Now.AddDays(-10),
                FechaHasta = DateTime.Now.AddDays(-5),
                StockDeclarado = 91000.13m
            };
            repositorioMock.Setup(x => x.ObtenerProyeccion<Establecimiento, string>(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Expression<Func<Establecimiento, string>>>())).Returns("130");
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>())).Returns(new CartaPorte { Cosecha = "15-16" });
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, int?>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(30000);
            repositorioMock.Setup(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>())).Returns(stock);
            repositorioMock.Setup(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(61000);
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, bool>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            var result = target.ValidarStockEstablecimiento(5, new Guid());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(true));
            Assert.That(result.Errores.FirstOrDefault().Value, Is.EqualTo(Textos.AsignacionEstablecimientoError_FueraVigencia));

            repositorioMock.Verify(x => x.ObtenerProyeccion<Establecimiento, string>(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Expression<Func<Establecimiento, string>>>()), Times.Once());
            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()), Times.Once());
            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, int?>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>()), Times.Once());
            repositorioMock.Verify(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>()), Times.Once());
            repositorioMock.Verify(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>()), Times.Exactly(0));
        }

        [Test]
        public void ValidarStockEstablecimientoError3()
        {
            var stock = new StockDeEstablecimiento
            {
                CodigoEstablecimiento = "130",
                Cosecha = "15-16",
                FechaDesde = DateTime.Now.AddDays(-1),
                FechaHasta = DateTime.Now.AddDays(1),
                StockDeclarado = 50000
            };
            repositorioMock.Setup(x => x.ObtenerProyeccion<Establecimiento, string>(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Expression<Func<Establecimiento, string>>>())).Returns("130");
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>())).Returns(new CartaPorte { Cosecha = "15-16" });
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, int?>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns(30000);
            repositorioMock.Setup(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>())).Returns(stock);
            repositorioMock.Setup(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(61000);
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, bool>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            var result = target.ValidarStockEstablecimiento(5, new Guid());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(true));
            Assert.That(result.Errores.FirstOrDefault().Value, Is.EqualTo(Textos.AsignacionEstablecimientoError_LimiteStockSuperado));

            repositorioMock.Verify(x => x.ObtenerProyeccion<Establecimiento, string>(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Expression<Func<Establecimiento, string>>>()), Times.Once());
            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()), Times.Once());
            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, int?>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>()), Times.Once());
            repositorioMock.Verify(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>()), Times.Once());
            repositorioMock.Verify(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>()), Times.Once());
        }

        [Test]
        public void ValidarStockEstablecimientoError4()
        {
            var stock = new StockDeEstablecimiento
            {
                CodigoEstablecimiento = "130",
                Cosecha = "15-16",
                FechaDesde = DateTime.Now.AddDays(-1),
                FechaHasta = DateTime.Now.AddDays(1),
                StockDeclarado = 91000.13m
            };
            repositorioMock.Setup(x => x.ObtenerProyeccion<Establecimiento, string>(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Expression<Func<Establecimiento, string>>>())).Returns("130");
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>())).Returns(new CartaPorte { Cosecha = "15-16" });
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, int?>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns((int?)null);
            repositorioMock.Setup(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>())).Returns(stock);
            repositorioMock.Setup(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(61000);
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, bool>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);

            var result = target.ValidarStockEstablecimiento(5, new Guid());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(true));
            Assert.That(result.Errores.FirstOrDefault().Value, Is.EqualTo(Textos.AsignacionEstablecimientoError_SinNetoOrigen));

            repositorioMock.Verify(x => x.ObtenerProyeccion<Establecimiento, string>(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Expression<Func<Establecimiento, string>>>()), Times.Once());
            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()), Times.Once());
            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, int?>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>()), Times.Once());
            repositorioMock.Verify(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>()), Times.Once());
            repositorioMock.Verify(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>()), Times.Exactly(0));
        }

        [Test]
        public void ValidarStockEstablecimientoNoValidaStock()
        {
            var stock = new StockDeEstablecimiento
            {
                CodigoEstablecimiento = "130",
                Cosecha = "15-16",
                FechaDesde = DateTime.Now.AddDays(-1),
                FechaHasta = DateTime.Now.AddDays(1),
                StockDeclarado = 91000.13m
            };
            repositorioMock.Setup(x => x.ObtenerProyeccion<Establecimiento, string>(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Expression<Func<Establecimiento, string>>>())).Returns("130");
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>())).Returns(new CartaPorte { Cosecha = "15-16" });
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, int?>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>())).Returns((int?)null);
            repositorioMock.Setup(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>())).Returns(stock);
            repositorioMock.Setup(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(61000);
            repositorioMock.Setup(x => x.ObtenerProyeccion<Recorrido, bool>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(false);

            var result = target.ValidarStockEstablecimiento(5, new Guid());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));

            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, bool>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>()), Times.Exactly(1));
            repositorioMock.Verify(x => x.ObtenerProyeccion<Establecimiento, string>(It.IsAny<Expression<Func<Establecimiento, bool>>>(), It.IsAny<Expression<Func<Establecimiento, string>>>()), Times.Exactly(0));
            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, CartaPorte>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, CartaPorte>>>()), Times.Exactly(0));
            repositorioMock.Verify(x => x.ObtenerProyeccion<Recorrido, int?>(It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Expression<Func<Recorrido, int?>>>()), Times.Exactly(0));
            repositorioMock.Verify(x => x.Obtener<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>()), Times.Exactly(0));
            repositorioMock.Verify(x => x.Sumar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>()), Times.Exactly(0));
        }

        [Test]
        public void BuscarProcedencia()
        {
            var localidad = new Localidad()
            {
                Id = 1,
                CodigoAfip = "CodigoAfip",
                Descripcion = "Descripcion",
                Provincia = new Provincia()
                {
                    CodigoAfip = 1,
                    Descripcion = "Santa Fe",
                    Id = 1,
                    Pais = new Pais()
                    {
                        Id = 1,
                        Descripcion = "Argentina"
                    }
                }
            };
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(localidad);

            var result = target.BuscarProcedencia("CodigoAfip");

            Assert.That(result.CodigoAfip, Is.EqualTo("CodigoAfip"));
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Descripcion, Is.EqualTo("Descripcion"));
            Assert.That(result.ProvinciaDesc, Is.EqualTo("Santa Fe"));
            Assert.That(result.ProvinciaId, Is.EqualTo(1));

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarProcedenciaCatch()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>())).Throws(new Exception());
            var result = target.BuscarProcedencia("CodigoAfip");
            Assert.That(result, Is.Null);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarProcedenciaNull()
        {
            var result = target.BuscarProcedencia("CodigoAfip");
            Assert.That(result, Is.Null);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarVinedoPropio()
        {
            var vinedo = new VinedoPropio()
            {
                Id = 1,
                Calidad = "Calidad",
                CentroOperativo = "CentroOperativo",
                Cuarteles = new List<Cuartel>()
                {
                    new Cuartel()
                    {
                        Activo = true,
                        Codigo = "CodigoCuartel",
                        Id = 1
                    }
                },
                Descripcion = "Descripcion",
                Proveedor = new Proveedor()
                {
                    Activo = true,
                    Descripcion = "Proveedor",
                    Id = 1
                },
                NumeroINV = "100"
            };
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<VinedoPropio, bool>>>())).Returns(vinedo);

            var result = target.BuscarVinedoPropio("Descripcion");

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Descripcion, Is.EqualTo("Descripcion"));
            Assert.That(result.NumeroINV, Is.EqualTo("100"));

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<VinedoPropio, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarVinedoPropioCatch()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<VinedoPropio, bool>>>())).Throws(new Exception());
            var result = target.BuscarVinedoPropio("Descripcion");
            Assert.That(result, Is.Null);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<VinedoPropio, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarVinedoPropioNull()
        {
            var result = target.BuscarVinedoPropio("Descripcion");
            Assert.That(result, Is.Null);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<VinedoPropio, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarCliente()
        {
            var cliente = new Cliente()
            {
                Id = 1,
                Activo = true,
                Descripcion = "Descripcion",
                Direccion = "Direccion",
                CodigoSap = "1024",
                Cuit = "20-11111111-2",
                Localidad = "San Lorenzo",
                Provincia = "Santa Fe"
            };
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Cliente, bool>>>())).Returns(cliente);

            var result = target.BuscarCliente("Descripcion");

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Activo, Is.True);
            Assert.That(result.Descripcion, Is.EqualTo("Descripcion"));
            Assert.That(result.Direccion, Is.EqualTo("Direccion"));
            Assert.That(result.CodigoSap, Is.EqualTo("1024"));
            Assert.That(result.Cuit, Is.EqualTo("20-11111111-2"));
            Assert.That(result.Localidad, Is.EqualTo("San Lorenzo"));
            Assert.That(result.Provincia, Is.EqualTo("Santa Fe"));

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Cliente, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarClienteCatch()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Cliente, bool>>>())).Throws(new Exception());
            var result = target.BuscarCliente("Descripcion");
            Assert.That(result, Is.Null);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Cliente, bool>>>()), Times.Once());
        }

        [Test]
        public void BuscarClienteNull()
        {
            var result = target.BuscarCliente("Descripcion");
            Assert.That(result, Is.Null);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Cliente, bool>>>()), Times.Once());
        }

        [Test]
        public void ListarPaises()
        {
            var paises = new List<Pais>()
            {
                new Pais()
                {
                    Id = 1,
                    Descripcion = "Argentina"
                },
                new Pais()
                {
                    Id = 2,
                    Descripcion = "Bolivia"
                }
            };
            repositorioMock.Setup(s => s.Listar<Pais>(null)).Returns(paises);
            var resultado = target.ListarPaises();
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Id, paises[0].Id);
            Assert.AreEqual(resultado[1].Id, paises[1].Id);
            Assert.AreEqual(resultado[0].Descripcion, paises[0].Descripcion);
            Assert.AreEqual(resultado[1].Descripcion, paises[1].Descripcion);

            Assert.That(resultado[0].Id, Is.EqualTo(1));
            Assert.That(resultado[0].Descripcion, Is.EqualTo("Argentina"));
            Assert.That(resultado[1].Id, Is.EqualTo(2));
            Assert.That(resultado[1].Descripcion, Is.EqualTo("Bolivia"));
            repositorioMock.Verify(x => x.Listar<Pais>(null), Times.Once());
        }

        [Test]
        public void ListarProvinciasPorPais()
        {
            var pais = new Pais()
            {
                Id = 1,
                Descripcion = "Argentina"
            };
            var provincias = new List<Provincia>()
            {
                new Provincia()
                {
                    Id = 1,
                    Descripcion = "Córdoba",
                    CodigoAfip = 1,
                    Pais = pais
                },
                new Provincia()
                {
                    Id = 2,
                    Descripcion = "Santa Fe",
                    CodigoAfip = 1,
                    Pais = pais
                }
            };
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<Provincia, bool>>>())).Returns(provincias);
            var resultado = target.ListarProvinciasPorPais(1);
            Assert.AreEqual(resultado.Count, 2);
            Assert.AreEqual(resultado[0].Id, provincias[0].Id);
            Assert.AreEqual(resultado[1].Id, provincias[1].Id);
            Assert.AreEqual(resultado[0].Descripcion, provincias[0].Descripcion);
            Assert.AreEqual(resultado[1].Descripcion, provincias[1].Descripcion);

            Assert.That(resultado[0].Id, Is.EqualTo(1));
            Assert.That(resultado[0].Descripcion, Is.EqualTo("Córdoba"));
            Assert.That(resultado[1].Id, Is.EqualTo(2));
            Assert.That(resultado[1].Descripcion, Is.EqualTo("Santa Fe"));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Provincia, bool>>>()), Times.Once());
        }

        [Test]
        public void ListarReglaDeAnalisisObligatorioActivas()
        {

            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<ReglaDeAnalisisObligatorio, bool>>>()))
                .Returns(new List<ReglaDeAnalisisObligatorio>() { new ReglaDeAnalisisObligatorio { Id = 1, CantidadAnalisis = 2, Centro = new Centro { Id = 1 } } });

            var resultado = target.ListarReglaDeAnalisisObligatorioActivas();
            Assert.AreEqual(resultado.Count, 1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ReglaDeAnalisisObligatorio, bool>>>()), Times.Once());
        }

        [Test]
        public void ListarMaterialesPorCamara()
        {

            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<MaterialPorCentro, Material>>>(),
                    It.IsAny<Expression<Func<MaterialPorCentro, bool>>>()))
                .Returns(new List<Material>() { new Material { Id = 1, Descripcion = "Poroto de Soja" }, new Material { Id = 2, Descripcion = "Trigo Pan" } });

            var resultado = target.ListarMaterialesPorCamara(It.IsAny<int>());
            Assert.AreEqual(resultado.Count, 2);

        }


        [Test]
        public void ListarGruposPorCamara()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<ConversionGrupo, bool>>>()))
                .Returns(new List<ConversionGrupo>() { new ConversionGrupo { Id = 1 }, new ConversionGrupo { Id = 2 } });
            var resultado = target.ListarGruposPorCamara(It.IsAny<int>());
            Assert.AreEqual(resultado.Count, 2);
        }
        [Test]
        public void ListarAlmacenesPorCentroYesSustentable()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<Almacen, bool>>>()))
                .Returns(new List<Almacen>() { new Almacen { Id = 1 }, new Almacen { Id = 2 } });

            var resultado = target.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>());
            Assert.AreEqual(resultado.Count, 2);
        }
        [Test]
        public void ListarAlmacenesPorCentro()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<Almacen, bool>>>()))
                .Returns(new List<Almacen>() { new Almacen { Id = 1 }, new Almacen { Id = 2 } });

            var resultado = target.ListarAlmacenesPorCentro(It.IsAny<int>());
            Assert.AreEqual(resultado.Count, 2);
        }

        [Test]
        public void ListarAlmacenesPorMaterialYCentro()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<Almacen, bool>>>()))
                .Returns(new List<Almacen>() { new Almacen { Id = 1 }, new Almacen { Id = 2 } });

            var resultado = target.ListarAlmacenesPorMaterialYCentro(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>());
            Assert.AreEqual(resultado.Count, 2);
        }
        [Test]
        public void ListarCentrosPorUsuario()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<Centro, bool>>>()))
                .Returns(new List<Centro>() { new Centro { Id = 1 } });

            var resultado = target.ListarCentrosPorUsuario(It.IsAny<string>());
            Assert.AreEqual(resultado.Count, 1);
        }
        [Test]
        public void ListarCamaras()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<Camara, bool>>>()))
                .Returns(new List<Camara>() { new Camara { Id = 1 } });

            var resultado = target.ListarCamaras();
            Assert.AreEqual(resultado.Count, 1);
        }

        [Test]
        public void ListarPaginadoCategoria()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<Categoria, bool>>>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<Categoria>(new List<Categoria> { new Categoria { Id = 1 } }, 1, 2, 3));

            var resultado = target.ListarPaginadoCategoria(null, It.IsAny<Paginacion>());
            Assert.AreEqual(resultado.Items.Count, 1);
        }
        [Test]
        public void ObtenerCategoria()
        {
            repositorioMock.Setup(s => s.Obtener<Categoria>(It.IsAny<int>()))
                .Returns(new Categoria { Id = 1 });

            var resultado = target.ObtenerCategoria(It.IsAny<int>());
            Assert.IsNotNull(resultado);
        }

        [Test]
        public void ObtenerUltimaMuestraEnvioACamaraPorCaladoId()
        {
            repositorioMock.Setup(s => s.ObtenerMayor(It.IsAny<Expression<Func<MuestraEnvioACamara, bool>>>(), It.IsAny<Expression<Func<MuestraEnvioACamara, int>>>()))
                .Returns(new MuestraEnvioACamara { Id = 4 });

            var resultado = target.ObtenerUltimaMuestraEnvioACamaraPorCaladoId(It.IsAny<int>());
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.Id, 4);
        }

        [Test]
        public void ListarMuestrasPorLote()
        {
            repositorioMock.Setup(s => s.ListarConsulta(It.IsAny<ListarMuestraEnvioACamaraConsulta>())).Returns(new List<MuestraEnvioACamaraDto> { new MuestraEnvioACamaraDto { Id = 1 } });

            repositorioMock.Setup(s => s.ObtenerMayor(It.IsAny<Expression<Func<MuestraEnvioACamara, bool>>>(), It.IsAny<Expression<Func<MuestraEnvioACamara, int>>>()))
                .Returns(new MuestraEnvioACamara { Id = 4 });

            var resultado = target.ListarMuestrasPorLote(It.IsAny<int>(), It.IsAny<Paginacion>());
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.ItemsTotales, 1);
        }

        [Test]
        public void ObtenerNumeroDeTicketImportacionGenerado()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroDeTicketImportacionGenerado()).Returns(1);

            var resultado = target.ObtenerNumeroDeTicketImportacionGenerado();
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, 1);
        }
        [Test]
        public void ObtenerSecuenciaEnvioACamara()
        {
            repositorioMock.Setup(s => s.ObtenerNumeroDocumentoGenerado()).Returns(1);

            var resultado = target.ObtenerSecuenciaEnvioACamara();
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, 1);
        }

        [Test]
        public void ListarLocalidadesPorProvincia()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<Localidad, bool>>>()))
                .Returns(new List<Localidad>() { new Localidad { Id = 1 } });

            var resultado = target.ListarLocalidadesPorProvincia(It.IsAny<int>());
            Assert.AreEqual(resultado.Count, 1);
        }
        [Test]
        public void ListarChoferes()
        {
            repositorioMock.Setup(s => s.Listar(
                  It.IsAny<Expression<Func<Chofer, bool>>>(), It.IsAny<Paginacion>()))
              .Returns(new ListaPaginada<Chofer>(new List<Chofer> { new Chofer { Id = 1 } }, 1, 2, 3));

            var resultado = target.ListarChoferes(It.IsAny<string>(), It.IsAny<Paginacion>());
            Assert.AreEqual(resultado.ItemsTotales, 3);
        }

        [Test]
        public void ListarKmPorProveedor()
        {
            repositorioMock.Setup(s => s.Listar(
                  It.IsAny<Expression<Func<KmPorProveedor, bool>>>(), It.IsAny<Paginacion>()))
              .Returns(new ListaPaginada<KmPorProveedor>(new List<KmPorProveedor> { new KmPorProveedor { Id = 1 } }, 1, 2, 3));

            var resultado = target.ListarKmPorProveedor(It.IsAny<string>(), It.IsAny<Paginacion>());
            Assert.AreEqual(resultado.ItemsTotales, 3);
            Assert.AreEqual(resultado.Items.Count, 1);
        }


        [Test]
        public void ListarVinedoPropio()
        {
            repositorioMock.Setup(s => s.Listar(
                  It.IsAny<Expression<Func<VinedoPropio, bool>>>(), It.IsAny<Paginacion>()))
              .Returns(new ListaPaginada<VinedoPropio>(new List<VinedoPropio> { new VinedoPropio { Id = 1 } }, 1, 2, 3));

            var resultado = target.ListarVinedoPropio(It.IsAny<string>(), It.IsAny<Paginacion>());
            Assert.AreEqual(resultado.ItemsTotales, 3);
            Assert.AreEqual(resultado.Items.Count, 1);
        }

        [Test]
        public void ListarVinedosPropios()
        {
            repositorioMock.Setup(s => s.Listar(
                       It.IsAny<Expression<Func<VinedoPropio, bool>>>()))
                   .Returns(new List<VinedoPropio>() { new VinedoPropio { Id = 1 } });

            var resultado = target.ListarVinedosPropios();
            Assert.IsNotNull(resultado);
        }

        [Test]
        public void ListarVinedosTerceros()
        {
            repositorioMock.Setup(s => s.Listar(
                       It.IsAny<Expression<Func<VinedoTerceros, bool>>>()))
                   .Returns(new List<VinedoTerceros>() { new VinedoTerceros { Id = 1 } });

            var resultado = target.ListarVinedosTerceros(It.IsAny<int>());
            Assert.IsNotNull(resultado);
        }

        [Test]
        public void BuscarChoferes()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<Chofer, bool>>>(), It.IsAny<int>()))
                .Returns(new List<Chofer>() { new Chofer { Id = 1 } });

            var filtro = new ChoferFiltro { Apellido = "Perez", Nombre = "Jose", Cuil = "00-0000_0000", Documento = "35888888" };
            var resultado = target.BuscarChoferes(filtro);
            Assert.AreEqual(resultado.Count, 1);
        }
        [Test]
        public void BuscarChofer()
        {
            repositorioMock.Setup(s => s.Obtener(
                    It.IsAny<Expression<Func<Chofer, bool>>>()))
                .Returns(new Chofer { Id = 1, Apellido = "Perez" });

            var filtro = new ChoferFiltro { Apellido = "Perez", Nombre = "Jose", Cuil = "00-0000_0000", Documento = "35888888" };
            var resultado = target.BuscarChofer(filtro);
            Assert.AreEqual(resultado.Apellido, "Perez");
        }

        [Test]
        public void BuscarChoferesGeneral()
        {
            repositorioMock.Setup(s => s.Listar(
                       It.IsAny<Expression<Func<Chofer, bool>>>()))
                   .Returns(new List<Chofer>() { new Chofer { Id = 1 } });

            var resultado = target.BuscarChoferesGeneral(It.IsAny<string>());
            Assert.IsNotNull(resultado);
        }

        [Test]
        public void ObtenerKmPorProveedor()
        {
            repositorioMock.Setup(s => s.Obtener<KmPorProveedor>(It.IsAny<int>()))
                .Returns(new KmPorProveedor { Id = 1, KmARecorrer = "20" });

            var resultado = target.ObtenerKmPorProveedor(It.IsAny<int>());
            Assert.AreEqual(resultado.KmARecorrer, "20");
        }

        [Test]
        public void ObtenerGraficoDePlanta()
        {
            repositorioMock.Setup(s => s.Obtener(
                    It.IsAny<Expression<Func<GraficoDePlanta, bool>>>()))
                .Returns(new GraficoDePlanta { Id = 1, Centro = new Centro { Id = 5, Descripcion = "San Lorenzo" } });

            var resultado = target.ObtenerGraficoDePlanta(It.IsAny<string>(), It.IsAny<int>());
            Assert.AreEqual(resultado.CentroId, 5);
        }

        [Test]
        public void ListarGraficosDePlanta()
        {
            repositorioMock.Setup(s => s.Listar(
                       It.IsAny<Expression<Func<GraficoDePlanta, bool>>>()))
                   .Returns(new List<GraficoDePlanta>() { new GraficoDePlanta { Id = 1 } });

            var resultado = target.ListarGraficosDePlanta(It.IsAny<int>());
            Assert.AreEqual(resultado.Count(), 1);
        }


        [Test]
        public void BuscarTransportistasPorCuit()
        {
            repositorioMock.Setup(s => s.Listar<Transportista, TransportistaInfoDto>(
                It.IsAny<Expression<Func<Transportista, TransportistaInfoDto>>>(),
                       It.IsAny<Expression<Func<Transportista, bool>>>()))
                   .Returns(new List<TransportistaInfoDto>() { new TransportistaInfoDto { Id = 1 } });

            var resultado = target.BuscarTransportistasPorCuit(It.IsAny<string>());
            Assert.AreEqual(resultado.Count(), 1);
        }

        [Test]
        public void ListarTransportistas()
        {
            repositorioMock.Setup(s => s.Listar(
                       It.IsAny<Expression<Func<Transportista, bool>>>()))
                   .Returns(new List<Transportista>() { new Transportista { Id = 1 } });

            var resultado = target.ListarTransportistas();
            Assert.AreEqual(resultado.Count(), 1);
        }

        [Test]
        public void ListarPaginadoTransportistas()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<Transportista, bool>>>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<Transportista>(new List<Transportista> { new Transportista { Id = 1 } }, 1, 2, 3));

            var resultado = target.ListarPaginadoTransportistas("test", It.IsAny<Paginacion>());
            Assert.AreEqual(resultado.Items.Count, 1);
        }

        [Test]
        public void ObtenerTransportista()
        {
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>()))
                .Returns(new Transportista { Id = 1 });

            var resultado = target.ObtenerTransportista(It.IsAny<int>());
            Assert.IsNotNull(resultado);
        }

        [Test]
        public void ObtenerTransportistaPorCuit()
        {
            repositorioMock.Setup(s => s.Obtener(
                    It.IsAny<Expression<Func<Transportista, bool>>>()))
                .Returns(new Transportista { Id = 1 });

            var resultado = target.ObtenerTransportistaPorCuit(It.IsAny<string>());
            Assert.IsNotNull(resultado);
        }
        [Test]
        public void ObtenerTransportistaPorRazonSocial()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero(
                    It.IsAny<Expression<Func<Transportista, bool>>>()))
                .Returns(new Transportista { Id = 1 });

            var resultado = target.ObtenerTransportistaPorRazonSocial(It.IsAny<string>());
            Assert.IsNotNull(resultado);
        }

        [Test]
        public void ListarExcepcionesAlControl()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<ExcepcionAlControl, bool>>>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ExcepcionAlControl>(new List<ExcepcionAlControl> { new ExcepcionAlControl { Id = 1 } }, 1, 2, 3));

            var resultado = target.ListarExcepcionesAlControl("test", It.IsAny<Paginacion>());
            Assert.AreEqual(resultado.Items.Count, 1);
        }

        [Test]
        public void ListarExcepcionesAlDescuento()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<ExcepcionAlDescuento, bool>>>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ExcepcionAlDescuento>(new List<ExcepcionAlDescuento> { new ExcepcionAlDescuento { Id = 1 } }, 1, 2, 3));

            var resultado = target.ListarExcepcionesAlDescuento("test", It.IsAny<Paginacion>(), It.IsAny<int>());
            Assert.AreEqual(resultado.Items.Count, 1);
        }

        [Test]
        public void ListarInhabilitacionChoferes()
        {
            repositorioMock.Setup(s => s.Listar(
                    It.IsAny<Expression<Func<InhabilitacionChofer, bool>>>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<InhabilitacionChofer>(new List<InhabilitacionChofer> { new InhabilitacionChofer { Id = 1 } }, 1, 2, 3));

            var resultado = target.ListarInhabilitacionChoferes("test", It.IsAny<int>(), It.IsAny<Paginacion>());
            Assert.AreEqual(resultado.Items.Count, 1);
        }

        [Test]
        public void ChoferInhabilitado()
        {
            repositorioMock.Setup(s => s.Existe(
                    It.IsAny<Expression<Func<InhabilitacionChofer, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(s => s.Existe<Chofer>(
                   It.IsAny<Expression<Func<Chofer, bool>>>()))
               .Returns(true);

            var resultado = target.ChoferInhabilitado(It.IsAny<int>(), It.IsAny<int>());
            Assert.That(resultado);
        }

        [Test]
        public void ListarPaginadoRecorridosPorModificarDocumentoDeIngreso()
        {
            repositorioMock.Setup(s => s.Listar(
                   It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<Paginacion>()))
               .Returns(new ListaPaginada<Recorrido>(new List<Recorrido> { new Recorrido { Id = 1 } }, 1, 2, 3));

            var resultado = target.ListarPaginadoRecorridosPorModificarDocumentoDeIngreso(1, new Paginacion(), new ModificarDocumentoDeIngresoDto() { NumeroDocumentoIngreso = "" });
            Assert.AreEqual(resultado.Items.Count, 1);
        }

        [Test]
        public void ListarAlmacenesPorMaterialYCentroSustentableMixto()
        {
            repositorioMock.Setup(s => s.Listar(
                       It.IsAny<Expression<Func<Almacen, bool>>>()))
                   .Returns(new List<Almacen>() { new Almacen { Id = 1 } });

            var resultado = target.ListarAlmacenesPorMaterialYCentroSustentableMixto(It.IsAny<int>(), It.IsAny<int>());
            Assert.AreEqual(resultado.Count(), 1);
        }
        [Test]
        public void ListarHidraulicasPorCriterioSustentable()
        {
            repositorioMock.Setup(s => s.Listar(
                       It.IsAny<Expression<Func<PuestosDeCargaDescarga, bool>>>()))
                   .Returns(new List<PuestosDeCargaDescarga>() { new PuestosDeCargaDescarga { Id = 1 } });

            var resultado = target.ListarHidraulicasPorCriterioSustentable(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>());
            Assert.AreEqual(resultado.Count(), 1);
        }
        [Test]
        public void ListarMaterialesFiltroF515()
        {
            repositorioMock.Setup(s => s.ListarDistintos<Recorrido, Material>(
                       It.IsAny<Expression<Func<Recorrido, Material>>>(), It.IsAny<Expression<Func<Recorrido, bool>>>(), It.IsAny<int>()))
                   .Returns(new List<Material>() { new Material { Id = 1 } });

            var resultado = target.ListarMaterialesFiltroF515(It.IsAny<int>());
            Assert.AreEqual(resultado.Count(), 1);
        }
        [Test]
        public void ValidarCupoCartaPorte()
        {
            repositorioMock.Setup(s => s.Existe<CargaDeCupo>(
                       It.IsAny<Expression<Func<CargaDeCupo, bool>>>()))
                   .Returns(true);

            var resultado = target.ValidarCupoCartaPorte(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.That(resultado);
        }
        [Test]
        public void ListarClientes()
        {
            repositorioMock.Setup(s => s.Listar(
                   It.IsAny<Expression<Func<Cliente, bool>>>(), It.IsAny<Paginacion>()))
               .Returns(new ListaPaginada<Cliente>(new List<Cliente> { new Cliente { Id = 1 } }, 1, 2, 3));

            var resultado = target.ListarClientes("test", It.IsAny<Paginacion>());
            Assert.AreEqual(resultado.Items.Count, 1);
        }


        [Test]
        public void BuscarClientePorId()
        {
            repositorioMock.Setup(s => s.Obtener<Cliente>(It.IsAny<int>()))
                .Returns(new Cliente { Id = 1 });

            var resultado = target.ObtenerCliente(It.IsAny<int>());
            Assert.IsNotNull(resultado);
        }

        [Test]
        public void ObtenerPagoConMercadoPagoPorRecorridoId()
        {
            repositorioMock.Setup(s => s.ObtenerMayor<PagoConMercadoPago, int>(
                It.IsAny<Expression<Func<PagoConMercadoPago, bool>>>(), 
                It.IsAny<Expression<Func<PagoConMercadoPago, int>>>()))
                .Returns(new PagoConMercadoPago
                {
                    Id = 1,
                    DetalleDelEstado = "TEST"
                });

            var resultado = target.ObtenerPagoConMercadoPagoPorRecorridoId(It.IsAny<int>());
            Assert.IsNotNull(resultado);
        }

        [Test]
        public void ObtenerCampaniaPorCuitCorrecto()
        {
            string cuit = "27-06645944-2";
            string cosecha = "20-21";

            repositorioMock.Setup(x => x.ListarConsulta(It.IsAny<ListarCampaniaPorCuit>()))
                .Returns(new List<StockDeEstablecimientoDto>()
                {new StockDeEstablecimientoDto
                {
                    Id = 3,
                    CodigoEstablecimiento = "2",
                    Cosecha = "20-21",
                    FechaDesde = DateTime.Parse("2020-09-23 00:00:00.000"),
                    FechaHasta = DateTime.Parse("2021-01-22 00:00:00.000"),
                    StockDeclarado = (decimal)5000.00,
                    StockReservado = (decimal)0.00,
                    EstablecimientoAnulado = false,
                }});
                

            var resultado = target.ListarCampaniaPorCuit(cuit,cosecha);
            Assert.IsNotNull(resultado);

        }

        [Test]
        public void ObtenerRegistroInactividad()
        {
            repositorioMock.Setup(s => s.Obtener<RegistroInactividad>(It.IsAny<int>())).Returns(new RegistroInactividad { Id = 3, Usuario = "baufest", FechaInicio = DateTime.Now, FechaFinal = DateTime.Now });

            var result = target.ObtenerRegistroInactividad(3);

            repositorioMock.Verify(s => s.Obtener<RegistroInactividad>(It.IsAny<int>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Id, 3);
        }

        [Test]
        public void ExisteOrdenCargaFason_DebeDevolver_True_SiRecorridoNoEsRechazadoYNoEstaTerminado() 
        {
            string ordenExterno = "2122";
            var recorrido = new Recorrido { Rechazado = false, Terminado = false };
            var ordenCarga = new OrdenCargaInternaFason { NumeroOrdenExterno = ordenExterno, Recorrido = recorrido };

            repositorioMock.Setup(r => r.ObtenerMayor<OrdenCargaInternaFason, int>(
                It.IsAny<Expression<Func<OrdenCargaInternaFason, bool>>>(),
                It.IsAny<Expression<Func<OrdenCargaInternaFason, int>>>()))
                .Returns(ordenCarga);

            var result = target.ExisteOrdenCargaFason(ordenExterno);
            Assert.IsTrue(result);
        }

        [Test]
        public void ExisteOrdenCargaFason_DebeDevolver_False_SiRecorridoNoEsRechazadoYEstaTerminado()
        {
            string ordenExterno = "2122";
            var recorrido = new Recorrido { Rechazado = false, Terminado = true };
            var ordenCarga = new OrdenCargaInternaFason { NumeroOrdenExterno = ordenExterno, Recorrido = recorrido };

            repositorioMock.Setup(r => r.ObtenerMayor<OrdenCargaInternaFason, int>(
                It.IsAny<Expression<Func<OrdenCargaInternaFason, bool>>>(),
                It.IsAny<Expression<Func<OrdenCargaInternaFason, int>>>()))
                .Returns(ordenCarga);

            var result = target.ExisteOrdenCargaFason(ordenExterno);
            Assert.IsFalse(result);
        }

        [Test]
        public void ExisteOrdenCargaFason_DebeDevolver_False_SiRecorridoEsRechazadoYEstaTerminado()
        {
            string ordenExterno = "2122";
            var recorrido = new Recorrido { Rechazado = true, Terminado = true };
            var ordenCarga = new OrdenCargaInternaFason { NumeroOrdenExterno = ordenExterno, Recorrido = recorrido };

            repositorioMock.Setup(r => r.ObtenerMayor<OrdenCargaInternaFason, int>(
                It.IsAny<Expression<Func<OrdenCargaInternaFason, bool>>>(),
                It.IsAny<Expression<Func<OrdenCargaInternaFason, int>>>()))
                .Returns(ordenCarga);

            var result = target.ExisteOrdenCargaFason(ordenExterno);
            Assert.IsFalse(result);
        }

        [Test]
        public void ExisteOrdenCargaFason_DebeDevolver_True_SiRecorridoEsRechazadoYNoEstaTerminado() 
        {
            string ordenExterno = "2122";
            var recorrido = new Recorrido { Rechazado = true, Terminado = false };
            var ordenCarga = new OrdenCargaInternaFason { NumeroOrdenExterno = ordenExterno, Recorrido = recorrido };

            repositorioMock.Setup(r => r.ObtenerMayor<OrdenCargaInternaFason, int>(
                It.IsAny<Expression<Func<OrdenCargaInternaFason, bool>>>(),
                It.IsAny<Expression<Func<OrdenCargaInternaFason, int>>>()))
                .Returns(ordenCarga);

            var result = target.ExisteOrdenCargaFason(ordenExterno);
            Assert.IsTrue(result);
        }

        [Test]
        public void ExisteOrdenCargarFasonDemorado_ConOrdenRechazadaNoTerminada_RetornaFalse()
        {
            // Arrange
            string numeroOrden = "75018";
            var instanciaWorkflow = new InstanciaWorkflowDto
            {
                Id = new Guid("e661716d-384c-48a5-9d3a-53f5c8457330")
            };

            var recorrido = new Recorrido
            {
                NumeroDocumentoIngreso = "112625",
                Rechazado = true,
                Terminado = false,
                VehiculoDemorado = true,
                InstanciaWorkflow = new Guid("e661716d-384c-48a5-9d3a-53f5c8457330")
            };

            var ordenCarga = new OrdenCargaInternaFason
            {
                Id = 112625,
                NumeroOrdenExterno = numeroOrden,
                Recorrido = recorrido
            };

            repositorioMock
                 .Setup(r => r.ObtenerMayor<OrdenCargaInternaFason, int>(
                    It.Is<Expression<Func<OrdenCargaInternaFason, bool>>>(expr => true), // La condición se evaluará
                    It.IsAny<Expression<Func<OrdenCargaInternaFason, int>>>()))
                .Returns((OrdenCargaInternaFason)null); // Retorna null porque tienen la misma InstanciaWorkflow

            // Act
            var resultado = target.ExisteOrdenCargarFasonDemorado(numeroOrden, instanciaWorkflow);

            // Assert
            Assert.IsFalse(resultado);
        }

        [Test]
        public void ExisteOrdenCargarFasonDemorado_ConOrdenRechazadaNoTerminadaDistinta_RetornaTrue()
        {
            // Arrange
            string numeroOrden = "75018";
            var instanciaWorkflow = new InstanciaWorkflowDto
            {
                Id = new Guid("e661716d-384c-48a5-9d3a-53f5c8457330")
            };

            var recorrido = new Recorrido
            {
                NumeroDocumentoIngreso = "112625",
                Rechazado = true,
                Terminado = false,
                VehiculoDemorado = true,
                InstanciaWorkflow = new Guid("e700906d-300c-48a5-9d3a-53f5c8457330")
            };

            var ordenCarga = new OrdenCargaInternaFason
            {
                Id = 112625,
                NumeroOrdenExterno = numeroOrden,
                Recorrido = recorrido
            };

            repositorioMock
                 .Setup(r => r.ObtenerMayor<OrdenCargaInternaFason, int>(
                    It.Is<Expression<Func<OrdenCargaInternaFason, bool>>>(expr => true),
                    It.IsAny<Expression<Func<OrdenCargaInternaFason, int>>>()))
                .Returns(ordenCarga); // Retorna la orden porque tienen diferente InstanciaWorkflow

            // Act
            var resultado = target.ExisteOrdenCargarFasonDemorado(numeroOrden, instanciaWorkflow);

            // Assert
            Assert.IsTrue(resultado);
        }

        [Test]
        public void EstaDemoradoPorTasaAdeudada_CuandoRecorridoEstaDemoradoSinPago_RetornaTrue()
        {
            var instanceId = Guid.NewGuid();
            repositorioMock.Setup(r => r.Existe(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(r => r.Existe(It.IsAny<Expression<Func<PagosTasaMunicipal, bool>>>()))
                .Returns(false);

            var resultado = target.EstaDemoradoPorTasaAdeudada(instanceId);
         
            Assert.IsTrue(resultado);
        }

        [Test]
        public void EstaDemoradoPorTasaAdeudada_CuandoRecorridoEstaDemoradoConPago_RetornaFalse()
        {
            var instanceId = Guid.NewGuid();
            repositorioMock.Setup(r => r.Existe(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns(true);
            repositorioMock.Setup(r => r.Existe(It.IsAny<Expression<Func<PagosTasaMunicipal, bool>>>()))
                .Returns(true);
            
            var resultado = target.EstaDemoradoPorTasaAdeudada(instanceId);

            Assert.IsFalse(resultado);
        }

        [Test]
        public void EstaDemoradoPorTasaAdeudada_CuandoRecorridoNoEstaDemorado_RetornaFalse()
        {
            var instanceId = Guid.NewGuid();
            repositorioMock.Setup(r => r.Existe(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns(false);
            repositorioMock.Setup(r => r.Existe(It.IsAny<Expression<Func<PagosTasaMunicipal, bool>>>()))
                .Returns(false);

            var resultado = target.EstaDemoradoPorTasaAdeudada(instanceId);

            Assert.IsFalse(resultado);
        }
    }
}