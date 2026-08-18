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
    public class ProcesadorCrearNotificacionTest
    {
        private ProcesadorCrearNotificacion target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NotificacionDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearNotificacion(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new NotificacionDto
                {
                    Id = 1,
                    Mensaje = "test"
                    
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearNotificacion {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Notificacion>(o => o.Mensaje == tipoDto.Mensaje)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void CasoA_AutomaticaConPuestoId_MarcaPreviasNoLeidasComoLeidas()
        {
            const int puestoId = 1;
            var previa = new Notificacion { Id = 5, PuestoId = puestoId, TipoAlerta = TipoAlerta.Automatica, Leido = false };
            var pool = new List<Notificacion> { previa };

            repositorioMock
                .Setup(s => s.Listar<Notificacion>(It.IsAny<Expression<Func<Notificacion, bool>>>()))
                .Returns<Expression<Func<Notificacion, bool>>>(p => pool.Where(p.Compile()).ToList());

            // Leido=true simula notificación exitosa (sin error)
            var dto = new NotificacionDto { Mensaje = "ok", TipoAlerta = TipoAlerta.Automatica, PuestoId = puestoId, Leido = true };
            var resultado = target.Ejecutar(new CrearNotificacion { Dto = dto });

            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(previa.Leido, Is.EqualTo(true));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
        }

        [Test]
        public void CasoB_AutomaticaConLeido0OTipoDistinto_NoDisparaLimpieza()
        {
            // Notificación Automatica con error (Leido=false) no debe limpiar previas
            var dtoError = new NotificacionDto { Mensaje = "falla", TipoAlerta = TipoAlerta.Automatica, PuestoId = 1, Leido = false };
            target.Ejecutar(new CrearNotificacion { Dto = dtoError });
            repositorioMock.Verify(s => s.Listar<Notificacion>(It.IsAny<Expression<Func<Notificacion, bool>>>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));

            repositorioMock = new Mock<IRepositorio>();
            target = new ProcesadorCrearNotificacion(
                repositorioMock.Object,
                conversorMock.Object,
                new NullLogger());

            // Tipo distinto de Automatica tampoco debe limpiar previas
            var dtoOtroTipo = new NotificacionDto { Mensaje = "info", TipoAlerta = TipoAlerta.Error, PuestoId = 1, Leido = true };
            target.Ejecutar(new CrearNotificacion { Dto = dtoOtroTipo });
            repositorioMock.Verify(s => s.Listar<Notificacion>(It.IsAny<Expression<Func<Notificacion, bool>>>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void CasoC_AutomaticaConPuestoId_SinPreviasSinLeer_NoSegundoGuardarCambios()
        {
            const int puestoId = 2;
            repositorioMock
                .Setup(s => s.Listar<Notificacion>(It.IsAny<Expression<Func<Notificacion, bool>>>()))
                .Returns<Expression<Func<Notificacion, bool>>>(_ => new List<Notificacion>());

            var dto = new NotificacionDto { Mensaje = "ok", TipoAlerta = TipoAlerta.Automatica, PuestoId = puestoId, Leido = true };
            var resultado = target.Ejecutar(new CrearNotificacion { Dto = dto });

            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
        }

       
    }
}