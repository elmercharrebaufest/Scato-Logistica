using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarTarjetaRango : ProcesadorModificar<ModificarTarjetaRango>
    {
        public ProcesadorModificarTarjetaRango(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarTarjetaRango comando)
        {
            var tarjetaRango = Repositorio.Obtener<TarjetaRango>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, tarjetaRango);
            if (tarjetaRango.Centro.Id != comando.Dto.CentroId)
            {
                tarjetaRango.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            }
        }

        protected override void Validar(ModificarTarjetaRango comando, Resultado resultado)
        {
            if (Repositorio.Existe<TarjetaRango>(t => ((t.Codigo == comando.Dto.Codigo) && (t.RangoHasta.CompareTo(comando.Dto.RangoDesde) >= 0) && (t.RangoDesde.CompareTo(comando.Dto.RangoHasta) <= 0) && (t.Id != comando.Dto.Id))
                & ((t.ValidoHasta >= comando.Dto.ValidoDesde) && (t.ValidoDesde <= comando.Dto.ValidoHasta) && (t.Id != comando.Dto.Id) && t.Centro.Id == comando.Dto.CentroId)))
            {
                resultado.Error("", Textos.TarjetaRango_ErrorRangoExistente);
            }
        }
    }
}
