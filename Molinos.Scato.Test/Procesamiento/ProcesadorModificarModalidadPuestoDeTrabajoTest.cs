using System;
using Molinos.Scato.Dominio.Comandos;
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
    public class ProcesadorModificarModalidadPuestoDeTrabajoTest
    {
        private ProcesadorModificarModalidadPuestoDeTrabajo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private PuestoDeTrabajo puesto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarModalidadPuestoDeTrabajo(repositorioMock.Object, conversorMock.Object,
                                                                     new NullLogger());
            puesto = new PuestoDeTrabajo
                {
                    Id = 1,
                    Centro = new Centro {Id = 1},
                    NombrePuesto = "Puesto 1",
                    Automatico = false,
                };
        }

        [Test]
        public void TestModificarModalidad()
        {
            PuestoDeTrabajoModificacionModalidad cambio = null;

            repositorioMock.Setup(r => r.Agregar(It.IsAny<PuestoDeTrabajoModificacionModalidad>()))
                           .Returns<PuestoDeTrabajoModificacionModalidad>(x =>
                               {
                                   cambio = x;
                                   return cambio;
                               });

            repositorioMock.Setup(s => s.Obtener<PuestoDeTrabajo>(1)).Returns(puesto);
            var comando = new ModificarModalidadPuestoDeTrabajo
                {
                    IdPuesto = 1,
                    Automatico = true,
                    Motivo = "por que si",
                    Usuario = "diego"
                };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<PuestoDeTrabajoModificacionModalidad>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(puesto.Automatico, Is.True);
            Assert.That(cambio, Is.Not.Null);
            Assert.That(cambio.Automatico, Is.EqualTo(comando.Automatico));
            Assert.That(cambio.Motivo, Is.EqualTo(comando.Motivo));
            Assert.That(cambio.NombreUsuarioResponsable, Is.EqualTo(comando.Usuario));
            Assert.That(cambio.PuestoDeTrabajo.Id, Is.EqualTo(comando.IdPuesto));
            Assert.That(cambio.Fecha.Date, Is.EqualTo(DateTime.Now.Date));
        }
    }
}
