using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarConfiguracionSensores : ProcesadorEliminar<EliminarConfiguracionSensores, ConfigSensores>
    {
        public ProcesadorEliminarConfiguracionSensores(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarConfiguracionSensores comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarConfiguracionSensores comando, Resultado resultado)
        {
        }
    }
}
