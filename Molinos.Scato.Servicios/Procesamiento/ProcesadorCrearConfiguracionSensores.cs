using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearConfiguracionSensores : ProcesadorCrear<CrearConfiguracionSensores, ConfigSensores>
    {
        public ProcesadorCrearConfiguracionSensores(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ConfigSensores CrearEntidad(CrearConfiguracionSensores comando)
        {
            var entidad = Conversor.Convertir<ConfigSensoresDto, ConfigSensores>(comando.Dto);
            entidad.Centro = Repositorio.Obtener<Centro>(comando.Dto.Centro_Id);
            return entidad;
        }

        protected override void Validar(CrearConfiguracionSensores comando, Resultado resultado)
        {
            if (Repositorio.Existe<ConfigSensores>(e => e.Descripcion == comando.Dto.Descripcion && (comando.Dto.Id == 0 || e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", string.Format(Textos.Error_Existente, Textos.Descripcion));
            }
        }
    }
}
