using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLogTarjetaSupervisor : ProcesadorComando<CrearLogTarjetaSupervisor>
    {
        public ProcesadorCrearLogTarjetaSupervisor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearLogTarjetaSupervisor comando)
        {
            var resultado = new Resultado();
            var puesto = Repositorio.Obtener<PuestoDeTrabajo>(comando.PuestoDeTrabajoId);
            var log = new LogTarjetaSupervisor
            {
                Fecha = DateTime.Now,
                PuestoDeTrabajo = puesto,
                NumeroTarjeta = comando.NumeroTarjeta,
                Motivo = comando.Motivo,
                NombreUsuario = comando.Usuario
            };

            Repositorio.Agregar(log);
            Repositorio.GuardarCambios();
            return resultado;
        }
    }
}
