using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorActualizarConfiguracionDeTablaTest
    {
        private ProcesadorActualizarConfiguracionDeTabla target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;
        private ConfiguracionDeTabla entidad;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            log = new NullLogger();
            target = new ProcesadorActualizarConfiguracionDeTabla(repositorioMock.Object, conversorMock.Object, log);

            entidad = new ConfiguracionDeTabla()
                {
                    Centro = new Centro(){Id = 1},
                    CaracteristicasDeCalidad = new Collection<CaracteristicaDeCalidad>(),
                    Material = new Material()
                };
        }

        [Test]
        public void TestModificar()
        {
            entidad.Id = 1;
            repositorioMock.Setup(s => s.Obtener<ConfiguracionDeTabla>(It.IsAny<Expression<Func<ConfiguracionDeTabla, bool>>>())).Returns(entidad);
            repositorioMock.Setup(s => s.Obtener<CaracteristicaDeCalidad>(It.IsAny<int>())).Returns(new CaracteristicaDeCalidad());


            var resultado = target.Ejecutar(new ActualizarConfiguracionDeTabla { Dto = new ConfiguracionDeTablaDto{ CaracteristicasDeCalidadId = new []{ 1 }, CentroId = 1} });

            repositorioMock.Verify(v => v.Agregar(It.IsAny<ConfiguracionDeTabla>()), Times.Never());
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrear()
        {
            entidad.Id = 0;
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(new Centro());
            repositorioMock.Setup(s => s.Obtener<Material>(It.IsAny<int>())).Returns(new Material());

            repositorioMock.Setup(s => s.Obtener<CaracteristicaDeCalidad>(It.IsAny<int>())).Returns(new CaracteristicaDeCalidad());


            var resultado = target.Ejecutar(new ActualizarConfiguracionDeTabla { Dto = new ConfiguracionDeTablaDto { CaracteristicasDeCalidadId = new[] { 1 } } });

            repositorioMock.Verify(v => v.Agregar(It.IsAny<ConfiguracionDeTabla>()), Times.Once());
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }   
    }
}