using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorEliminarRecorridoTest
    {
        private ProcesadorEliminarRecorrido target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IServicioComandos> comandosMock;

        private IConversor conversor;
        private RecorridoDto tipoDto;
        private Recorrido tipo;
        private MaterialPorCentro material;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            comandosMock = new Mock<IServicioComandos>();
            target = new ProcesadorEliminarRecorrido(repositorioMock.Object, conversor, new NullLogger(), comandosMock.Object);
            tipoDto = new RecorridoDto
            {
                Id = 1,
            };
            tipo = new Recorrido
                {
                    Id = 1,
                    Patente = "AAA111",
                    TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                    NumeroDocumentoIngreso = "12345678",
                    InstanciaWorkflow = new Guid(),
                    Vehiculo = new Vehiculo{Id = 12,CartaPorte = new CartaPorte{Id = 2}},
                    Centro = new Centro{ Id = 2},
                    Material = new Material{Descripcion = "Semilla Soja"},
                    Chofer = new Chofer{Nombre = "Rodolfo",Apellido = "Pantero",Cuil = "30-11111111-6"},
                    TarjetaDeAcceso = "0000012141"
                    

                };
            material = new MaterialPorCentro
                {
                    Id = 1,
                    RequiereTecnologia = true
                };

            comandosMock.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarRecorrido { Id = tipoDto.Id };
            repositorioMock.Setup(r => r.Obtener<Recorrido>(comando.Id)).Returns(tipo);
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Returns(material);
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<AjusteDeCalidad, bool>>>())).Returns(new AjusteDeCalidad{Id = 1});
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, bool>>>()))
                .Returns(new List<Dominio.Entidades.AnalisisDeCalidad>{new Dominio.Entidades.AnalisisDeCalidad
                    {
                        Id = 1,
                        CaracteristicasAnalizadas = new List<AnalisisPorCaracteristica> {new AnalisisPorCaracteristica {Id = 1}}
                    }});
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<DistribucionDeAlmacenes, bool>>>())).Returns(new List<DistribucionDeAlmacenes>());
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<DistribucionDeAlmacen, bool>>>())).Returns(new List<DistribucionDeAlmacen>());
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Calado, bool>>>())).Returns(new List<Calado>());
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<ControlRecorrido, bool>>>()))
                .Returns(new List<ControlRecorrido> {new ControlRecorrido {Id = 1}});
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<LogActividad, bool>>>()))
                .Returns(new List<LogActividad> { new LogActividad { Id = 1 } });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<MuestraDeHumedad, bool>>>()))
                .Returns(new List<MuestraDeHumedad> { new MuestraDeHumedad { Id = 1 } });
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<Observacion, bool>>>())).Returns(new Observacion{Id = 1});
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Precinto, bool>>>()))
                .Returns(new List<Precinto> { new Precinto { Id = 1 }, new Precinto{Id = 2} });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Impresion, bool>>>()))
                .Returns(new List<Impresion> { new ImpImpresionGenerica { Id = 1 }});
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Romaneo, bool>>>())).Returns(new List<Romaneo>());
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<DescargaUnidad, bool>>>())).Returns(new List<DescargaUnidad>());
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<OrdenCargaInterna, bool>>>())).Returns(new OrdenCargaInterna { Id = 1 });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<MuestraEnvioACamaraBiotecnologia, bool>>>()))
                .Returns(new List<MuestraEnvioACamaraBiotecnologia> { new MuestraEnvioACamaraBiotecnologia { Id = 1 } });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<IngresoDeDatosDeExportacion, bool>>>())).Returns(new List<IngresoDeDatosDeExportacion> { new IngresoDeDatosDeExportacion() });
            repositorioMock.Setup(r => r.ObtenerMayor(It.IsAny<Expression<Func<ControlRecorrido, bool>>>(),It.IsAny<Expression<Func<ControlRecorrido, int>>>())).Returns(new ControlRecorrido{ Actividad = "Baja CTG"});
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<ImpReciboMunicipal, bool>>>())).Returns(new List<ImpReciboMunicipal> { new ImpReciboMunicipal { FechaImpresion = DateTime.Now, TicketNro = "001415" } });

            repositorioMock.Setup(r => r.Existe(It.IsAny<Expression<Func<LogActividadHistorico, bool>>>())).Returns(true);
            repositorioMock.Setup(r => r.ObtenerMayor(It.IsAny<Expression<Func<LogActividad, bool>>>(), It.IsAny<Expression<Func<LogActividad, int>>>())).Returns(new LogActividad { Id = 1, ActividadXaml = "PesadaTara" });

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.Remover(It.IsAny<AjusteDeCalidad>()), Times.Exactly(1));

            repositorioMock.Verify(s => s.Remover(It.IsAny<Recorrido>()), Times.Exactly(1));

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            
            repositorioMock.Verify(r => r.ObtenerMayor(It.IsAny<Expression<Func<ControlRecorrido, bool>>>(),It.IsAny<Expression<Func<ControlRecorrido, int>>>()),Times.Exactly(1));
            repositorioMock.Verify(r => r.Listar<ImpReciboMunicipal>(It.IsAny<Expression<Func<ImpReciboMunicipal, bool>>>()), Times.Exactly(1));
            repositorioMock.Verify(r => r.Agregar(It.IsAny<TicketMunicipalBorrado>()), Times.Exactly(1));

            repositorioMock.Verify(r => r.Existe(It.IsAny<Expression<Func<LogActividadHistorico, bool>>>()));
            repositorioMock.Verify(r => r.ObtenerMayor(It.IsAny<Expression<Func<LogActividad, bool>>>(), It.IsAny<Expression<Func<LogActividad, int>>>()));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void EliminarEntidad2()
        {
            tipo.TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte;
            var comando = new EliminarRecorrido { Id = tipoDto.Id };
            repositorioMock.Setup(r => r.Obtener<Recorrido>(comando.Id)).Returns(tipo);
            repositorioMock.Setup(r => r.Existe(It.IsAny<Expression<Func<LogActividad, bool>>>())).Returns(true);
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<AltaCTG, bool>>>())).Returns(new AltaCTG());
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<CartaPorte, bool>>>())).Returns(new CartaPorte());
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<BajaCTG, bool>>>())).Returns(new BajaCTG());
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<CartaDePorteRegistradaServicioMonsanto, bool>>>())).Returns(new CartaDePorteRegistradaServicioMonsanto());
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Calado, bool>>>())).Returns(new List<Calado>{new Calado
                    {   
                        CaladosPorCaracteristica = new List<CaladoPorCaracteristica> {new CaladoPorCaracteristica {Id = 1}}                        
                    }});
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<MuestraEnvioACamara, bool>>>())).Returns(new List<MuestraEnvioACamara>());
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<MicroMuestrasPorCasillero, bool>>>())).Returns(new List<MicroMuestrasPorCasillero>{new MicroMuestrasPorCasillero
                    {                       
                        Id = 1,Muestra = new MuestraEnvioACamara{Id = 2}
                    }});
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Returns(material);
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<AjusteDeCalidad, bool>>>())).Returns(new AjusteDeCalidad { Id = 1 });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Dominio.Entidades.AnalisisDeCalidad, bool>>>())).Returns(new List<Dominio.Entidades.AnalisisDeCalidad>{new Dominio.Entidades.AnalisisDeCalidad
                    {
                        Id = 1,
                        CaracteristicasAnalizadas = new List<AnalisisPorCaracteristica> {new AnalisisPorCaracteristica {Id = 1}}
                    }});
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<DistribucionDeAlmacenes, bool>>>())).Returns(new List<DistribucionDeAlmacenes>());
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<DistribucionDeAlmacen, bool>>>())).Returns(new List<DistribucionDeAlmacen>());
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<ControlRecorrido, bool>>>()))
                .Returns(new List<ControlRecorrido> { new ControlRecorrido { Id = 1 } });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<LogActividad, bool>>>()))
                .Returns(new List<LogActividad> { new LogActividad { Id = 1 } });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<MuestraDeHumedad, bool>>>()))
                .Returns(new List<MuestraDeHumedad> { new MuestraDeHumedad { Id = 1 } });
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<Observacion, bool>>>())).Returns(new Observacion { Id = 1 });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Precinto, bool>>>()))
                .Returns(new List<Precinto> { new Precinto { Id = 1 }, new Precinto { Id = 2 } });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Impresion, bool>>>()))
                .Returns(new List<Impresion> { new ImpImpresionGenerica { Id = 1 } });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<Romaneo, bool>>>())).Returns(new List<Romaneo>{new Romaneo()});
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<RomaneoItem, bool>>>())).Returns(new List<RomaneoItem> { new RomaneoItem() });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<RomaneoItemPedido, bool>>>())).Returns(new List<RomaneoItemPedido> { new RomaneoItemPedido() });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<DescargaUnidad, bool>>>())).Returns(new List<DescargaUnidad>{new DescargaUnidad()});
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<OrdenCargaInterna, bool>>>())).Returns(new OrdenCargaInterna { Id = 1 });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<DescargaUnidadItem, bool>>>())).Returns(new List<DescargaUnidadItem> { new DescargaUnidadItem() });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<DescargaUnidadItemPedido, bool>>>())).Returns(new List<DescargaUnidadItemPedido> { new DescargaUnidadItemPedido() });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<DescargaDeBines, bool>>>())).Returns(new List<DescargaDeBines> { new DescargaDeBines() });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<DistribucionDeAlmacenes, bool>>>())).Returns(new List<DistribucionDeAlmacenes> { new DistribucionDeAlmacenes() });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<DistribucionDeAlmacen, bool>>>())).Returns(new List<DistribucionDeAlmacen> { new DistribucionDeAlmacen() });
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<MuestraEnvioACamaraBiotecnologia, bool>>>())).Returns(new List<MuestraEnvioACamaraBiotecnologia> ());
            repositorioMock.Setup(r => r.Listar(It.IsAny<Expression<Func<IngresoDeDatosDeExportacion, bool>>>())).Returns(new List<IngresoDeDatosDeExportacion>{new IngresoDeDatosDeExportacion()});
            repositorioMock.Setup(r => r.ObtenerMayor(It.IsAny<Expression<Func<ControlRecorrido, bool>>>(), It.IsAny<Expression<Func<ControlRecorrido, int>>>())).Returns(new ControlRecorrido { Actividad = "Baja CTG" });
            repositorioMock.Setup(r => r.Existe(It.IsAny<Expression<Func<LogActividadHistorico, bool>>>())).Returns(true);
            repositorioMock.Setup(r => r.ObtenerMayor(It.IsAny<Expression<Func<LogActividad, bool>>>(), It.IsAny<Expression<Func<LogActividad, int>>>())).Returns(new LogActividad { Id = 1, ActividadXaml = "PesadaTara" });

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.Remover(It.IsAny<AjusteDeCalidad>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Remover(It.IsAny<CartaPorte>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Remover(It.IsAny<Recorrido>()), Times.Exactly(1));
            repositorioMock.Verify(r => r.ObtenerMayor(It.IsAny<Expression<Func<ControlRecorrido, bool>>>(), It.IsAny<Expression<Func<ControlRecorrido, int>>>()), Times.Exactly(1));
            repositorioMock.Verify(r => r.Listar<ImpReciboMunicipal>(It.IsAny<Expression<Func<ImpReciboMunicipal, bool>>>()), Times.Exactly(1));
            repositorioMock.Verify(r => r.Agregar(It.IsAny<TicketMunicipalBorrado>()), Times.Exactly(0));
            repositorioMock.Verify(r => r.Existe(It.IsAny<Expression<Func<LogActividadHistorico, bool>>>()));
            repositorioMock.Verify(r => r.ObtenerMayor(It.IsAny<Expression<Func<LogActividad, bool>>>(), It.IsAny<Expression<Func<LogActividad, int>>>()));

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
