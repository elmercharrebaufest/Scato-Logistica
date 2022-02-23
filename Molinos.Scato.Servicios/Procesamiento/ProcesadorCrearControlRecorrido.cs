using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearControlRecorrido : ProcesadorCrear<CrearControlRecorrido, ControlRecorrido>
    {
        public ProcesadorCrearControlRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ControlRecorrido CrearEntidad(CrearControlRecorrido comando)
        {
            Log.Debug($"Crear Control recorrido para {comando.Dto.ActividadXaml}, por usuario {comando.Dto.NombreUsuario}, en puesto {comando.Dto.PuestoDeTrabajoId}");
            var control = Conversor.Convertir<ControlRecorridoDto, ControlRecorrido>(comando.Dto);
            control.Fecha = DateTime.Now;
            control.PuestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(comando.Dto.PuestoDeTrabajoId);
            return control;
        }

        protected override void Validar(CrearControlRecorrido comando, Resultado resultado)
        {
        }
    }
}
