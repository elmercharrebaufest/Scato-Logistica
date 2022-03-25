using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;


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
            Conversor.Convertir(comando.Dto, configuracion);
        }

        protected override void Validar(ModificarConfiguracionGeneral comando, Resultado resultado)
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
