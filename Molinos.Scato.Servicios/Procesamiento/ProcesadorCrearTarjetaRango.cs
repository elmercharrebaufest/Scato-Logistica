using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearTarjetaRango : ProcesadorCrear<CrearTarjetaRango, TarjetaRango>
    {
        public ProcesadorCrearTarjetaRango(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override TarjetaRango CrearEntidad(CrearTarjetaRango comando)
        {
            var editado = Conversor.Convertir<TarjetaRangoDto, TarjetaRango>(comando.Dto);
            editado.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            return editado;
        }

        protected override void Validar(CrearTarjetaRango comando, Resultado resultado)
        {
            if (Repositorio.Existe<TarjetaRango>(t => ((t.Codigo == comando.Dto.Codigo) && (t.RangoHasta.CompareTo(comando.Dto.RangoDesde) >= 0) && (t.RangoDesde.CompareTo(comando.Dto.RangoHasta) <= 0) && (t.Id != comando.Dto.Id))
                & ((t.ValidoHasta >= comando.Dto.ValidoDesde) && (t.ValidoDesde <= comando.Dto.ValidoHasta) && (t.Id != comando.Dto.Id) && t.Centro.Id == comando.Dto.CentroId)))
                {
                    resultado.Error("", Textos.TarjetaRango_ErrorRangoExistente);
                }
        }
    }
}
