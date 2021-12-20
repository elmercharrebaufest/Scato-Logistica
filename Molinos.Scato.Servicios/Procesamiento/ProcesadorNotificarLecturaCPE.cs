using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorNotificarLecturaCPE : ProcesadorComando<NotificarLecturaCPE>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IServicioNotificarUsuario servicioNotificar;

        public ProcesadorNotificarLecturaCPE(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos,
            IServicioRepositorio servicioRepositorio, IServicioNotificarUsuario servicioNotificar) : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.servicioRepositorio = servicioRepositorio;
            this.servicioNotificar = servicioNotificar;
        }        

        public override Resultado Ejecutar(NotificarLecturaCPE comando)
        {
            Log.Debug($"Obteniendo datos de la cp {comando.NroCtg}");
            var resultado = new Resultado();

            servicioNotificar.NotificarLectura(new LecturaCpeDto
            {
                CentroId = comando.CentroId,
                NroCtg = comando.NroCtg,
                PuestoId = comando.PuestoId
            });
            return resultado;
        }
    }
}
