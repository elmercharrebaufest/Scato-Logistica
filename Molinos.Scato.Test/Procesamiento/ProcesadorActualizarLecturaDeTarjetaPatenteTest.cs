using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ProcesadorActualizarLecturaDeTarjetaPatenteTest
    {
        private ProcesadorActualizarLecturaDeTarjetaPatente target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;
        private Recorrido recorrido;

        [SetUp]
        public void SetUp()
        {
            conversorMock = new Mock<IConversor>();
            repositorioMock = new Mock<IRepositorio>();
            log = new NullLogger();

            target = new ProcesadorActualizarLecturaDeTarjetaPatente(repositorioMock.Object, conversorMock.Object, log);
        }

        [Test]
        public void TestPatenteTolerancia0Diferencia0Ok()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new PuestoDeTrabajo() { Centro = new Centro { ToleranciaPatenteLeida = 0 } });
            repositorioMock.Setup(s => s.ObtenerMayor<LecturaDeTarjeta, int>(It.IsAny<Expression<Func<LecturaDeTarjeta, bool>>>(), It.IsAny<Expression<Func<LecturaDeTarjeta, int>>>())).Returns(new LecturaDeTarjeta());

            target.Ejecutar(new ActualizarLecturaDeTarjetaPatente { Patente = "a", PatenteLeida = "a"});

            repositorioMock.Verify(x => x.Agregar<LogLecturaDeTarjeta>(It.Is<LogLecturaDeTarjeta>(y => y.ReconocimientoExitoso == true)));
            repositorioMock.Verify(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()), Times.Exactly(0));
        }

        [Test]
        public void TestPatenteTolerancia0Diferencia1Error()
        {
            repositorioMock.Setup(s => s.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new PuestoDeTrabajo() { Centro = new Centro { ToleranciaPatenteLeida = 0 } });
            repositorioMock.Setup(s => s.ObtenerMayor<LecturaDeTarjeta, int>(It.IsAny<Expression<Func<LecturaDeTarjeta, bool>>>(), It.IsAny<Expression<Func<LecturaDeTarjeta, int>>>())).Returns(new LecturaDeTarjeta());

            target.Ejecutar(new ActualizarLecturaDeTarjetaPatente { Patente = "a", PatenteLeida = "da" });

            repositorioMock.Verify(x => x.Agregar<LogLecturaDeTarjeta>(It.Is<LogLecturaDeTarjeta>(y => y.ReconocimientoExitoso == false)));
            repositorioMock.Verify(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()), Times.Exactly(0));
        }

        [Test]
        public void TestPatenteTolerancia1Diferencia2Error()
        {
            repositorioMock.Setup(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new PuestoDeTrabajo() { Centro = new Centro { ToleranciaPatenteLeida = 2 } });
            repositorioMock.Setup(s => s.ObtenerMayor<LecturaDeTarjeta, int>(It.IsAny<Expression<Func<LecturaDeTarjeta, bool>>>(), It.IsAny<Expression<Func<LecturaDeTarjeta, int>>>())).Returns(new LecturaDeTarjeta());

            target.Ejecutar(new ActualizarLecturaDeTarjetaPatente { Patente = "a3", PatenteLeida = "da" });

            repositorioMock.Verify(x => x.Agregar<LogLecturaDeTarjeta>(It.Is<LogLecturaDeTarjeta>(y => y.ReconocimientoExitoso == false)));
            repositorioMock.Verify(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()), Times.Exactly(1));
        }

        [Test]
        public void TestPatenteTolerancia1Diferencia1OK()
        {
            repositorioMock.Setup(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(false);
            repositorioMock.Setup(s => s.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new PuestoDeTrabajo() { Centro = new Centro { ToleranciaPatenteLeida = 1 } });
            repositorioMock.Setup(s => s.ObtenerMayor<LecturaDeTarjeta, int>(It.IsAny<Expression<Func<LecturaDeTarjeta, bool>>>(), It.IsAny<Expression<Func<LecturaDeTarjeta, int>>>())).Returns(new LecturaDeTarjeta());

            target.Ejecutar(new ActualizarLecturaDeTarjetaPatente { Patente = "sa", PatenteLeida = "da" });

            repositorioMock.Verify(x => x.Agregar<LogLecturaDeTarjeta>(It.Is<LogLecturaDeTarjeta>(y => y.ReconocimientoExitoso == true)));
            repositorioMock.Verify(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()), Times.Exactly(1));
        }

        [Test]
        public void TestPatenteTolerancia1Diferencia1ExisteCamionError()
        {
            repositorioMock.Setup(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new PuestoDeTrabajo() { Centro = new Centro { ToleranciaPatenteLeida = 1 } });
            repositorioMock.Setup(s => s.ObtenerMayor<LecturaDeTarjeta, int>(It.IsAny<Expression<Func<LecturaDeTarjeta, bool>>>(), It.IsAny<Expression<Func<LecturaDeTarjeta, int>>>())).Returns(new LecturaDeTarjeta());

            target.Ejecutar(new ActualizarLecturaDeTarjetaPatente { Patente = "sa", PatenteLeida = "da" });

            repositorioMock.Verify(x => x.Agregar<LogLecturaDeTarjeta>(It.Is<LogLecturaDeTarjeta>(y => y.ReconocimientoExitoso == false)));
            repositorioMock.Verify(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()), Times.Exactly(1));
        }

        [Test]
        public void TestPatenteTolerancia2Diferencia1Ok()
        {
            repositorioMock.Setup(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(false);
            repositorioMock.Setup(s => s.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new PuestoDeTrabajo() { Centro = new Centro { ToleranciaPatenteLeida = 2 } });
            repositorioMock.Setup(s => s.ObtenerMayor<LecturaDeTarjeta, int>(It.IsAny<Expression<Func<LecturaDeTarjeta, bool>>>(), It.IsAny<Expression<Func<LecturaDeTarjeta, int>>>())).Returns(new LecturaDeTarjeta());

            target.Ejecutar(new ActualizarLecturaDeTarjetaPatente { Patente = "sa", PatenteLeida = "da" });

            repositorioMock.Verify(x => x.Agregar<LogLecturaDeTarjeta>(It.Is<LogLecturaDeTarjeta>(y => y.ReconocimientoExitoso == true)));
            repositorioMock.Verify(x => x.Existe<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()), Times.Exactly(1));
        }

        [Test]
        public void TestCalcularDiferencia1()
        {
            Assert.That(target.CalcularDiferencia("a","a"), Is.EqualTo(0));
        }
        
        [Test]
        public void TestCalcularDiferencia2()
        {
            Assert.That(target.CalcularDiferencia("a",""), Is.EqualTo(1));
        }

        [Test]
        public void TestCalcularDiferencia3()
        {
            Assert.That(target.CalcularDiferencia("a", "b"), Is.EqualTo(1));
        }

        [Test]
        public void TestCalcularDiferencia4()
        {
            Assert.That(target.CalcularDiferencia("aa", "a"), Is.EqualTo(1));
        }

        [Test]
        public void TestCalcularDiferencia5()
        {
            Assert.That(target.CalcularDiferencia("aa", "aa"), Is.EqualTo(0));
        }

        [Test]
        public void TestCalcularDiferencia6()
        {
            Assert.That(target.CalcularDiferencia("aa", "ab"), Is.EqualTo(1));
        }

        [Test]
        public void TestCalcularDiferencia7()
        {
            Assert.That(target.CalcularDiferencia("a", "aa"), Is.EqualTo(1));
        }

        [Test]
        public void TestCalcularDiferencia8()
        {
            Assert.That(target.CalcularDiferencia("aBb321", "abb322"), Is.EqualTo(2));
        }

        [Test]
        public void TestCalcularDiferencia9()
        {
            Assert.That(target.CalcularDiferencia("", ""), Is.EqualTo(0));
        }
    }
}