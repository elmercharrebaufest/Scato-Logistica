using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarConfiguracionGeneral : ProcesadorModificar<ModificarConfiguracionGeneral>
    {
        public ProcesadorModificarConfiguracionGeneral(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarConfiguracionGeneral comando)
        {
            var configuracion = Repositorio.Obtener<ConfiguracionGeneral>(comando.Dto.Id);
            configuracion.Valor = comando.Dto.Valor;
            configuracion.UsuarioUltimaModificacion = comando.Dto.UsuarioUltimaModificacion;
            configuracion.FechaUltimaModificacion = DateTime.Now;
        }

        protected override void Validar(ModificarConfiguracionGeneral comando, Resultado resultado)
        {
            if (!Repositorio.Existe<ConfiguracionGeneral>(x => x.Id == comando.Dto.Id))
            {
                resultado.Error("ConfiguracionGeneral", $"{Textos.ConfiguracionGeneral_Inexistente} a modificar");
            }
        }
    }
}
