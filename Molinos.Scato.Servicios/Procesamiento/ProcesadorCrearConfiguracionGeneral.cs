using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearConfiguracionGeneral : ProcesadorCrear<CrearConfiguracionGeneral, ConfiguracionGeneral>
    {
        public ProcesadorCrearConfiguracionGeneral(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override ConfiguracionGeneral CrearEntidad(CrearConfiguracionGeneral comando)
        {
            return new ConfiguracionGeneral
            {
                Pantalla = comando.Dto.Pantalla,
                Nombre = comando.Dto.Nombre,
                Valor = comando.Dto.Valor,
                CentroId = comando.Dto.CentroId,
                FechaCreacion = comando.Dto.FechaCreacion,
                UsuarioCreacion = comando.Dto.UsuarioCreacion,
            };
        }

        protected override void Validar(CrearConfiguracionGeneral comando, Resultado resultado)
        {
            if (
                Repositorio.Existe<ConfiguracionGeneral>(
                    x =>
                    x.Id != comando.Dto.Id && x.Pantalla == comando.Dto.Pantalla && x.Nombre == comando.Dto.Nombre && x.CentroId == comando.Dto.CentroId))
            {
                resultado.Error("ConfiguracionGeneral", Textos.CategoriaCamiones_PatenteExistente);
            }
        }
    }
}
