using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearHumedimetroModificarModalidad : ProcesadorCrear<CrearHumedimetroModificarModalidad, HumedimetroModificacionModalidad>
    {
        public ProcesadorCrearHumedimetroModificarModalidad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override HumedimetroModificacionModalidad CrearEntidad(CrearHumedimetroModificarModalidad comando)
        {
            var cambioDeModalidad = Conversor.Convertir<HumedimetroModificacionModalidadDto, HumedimetroModificacionModalidad>(comando.Dto);
            cambioDeModalidad.Humedimetro = Repositorio.Obtener<Humedimetro>(comando.Dto.HumedimetroId);
            cambioDeModalidad.Humedimetro.Modalidad = comando.Dto.Modalidad;
            return cambioDeModalidad;
        }

        protected override void Validar(CrearHumedimetroModificarModalidad comando, Resultado resultado)
        {
        }
    }
}
