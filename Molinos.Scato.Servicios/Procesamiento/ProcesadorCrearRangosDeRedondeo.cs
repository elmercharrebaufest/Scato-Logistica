using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearRangosDeRedondeo : ProcesadorCrear<CrearRangosDeRedondeo, RangosDeRedondeo>
    {
        public ProcesadorCrearRangosDeRedondeo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override RangosDeRedondeo CrearEntidad(CrearRangosDeRedondeo comando)
        {
            var rangosDeRedondeoEditado = Conversor.Convertir<RangosDeRedondeoDto, RangosDeRedondeo>(comando.Dto);
            rangosDeRedondeoEditado.MaterialPorCentro = Repositorio.Obtener<MaterialPorCentro>(comando.Dto.MaterialPorCentroId);

            return rangosDeRedondeoEditado;
        }

        protected override void Validar(CrearRangosDeRedondeo comando, Resultado resultado)
        {
            if (Repositorio.Existe<RangosDeRedondeo>(e => e.MaterialPorCentro.Id == comando.Dto.MaterialPorCentroId &&
                                                    ((e.ValorDesde <= comando.Dto.ValorDesde && e.ValorHasta >= comando.Dto.ValorHasta) ||
                                                    (comando.Dto.ValorDesde <= e.ValorDesde && comando.Dto.ValorHasta >= e.ValorDesde) ||
                                                    (comando.Dto.ValorDesde <= e.ValorHasta && comando.Dto.ValorHasta >= e.ValorHasta)
                                                    )))
            {
                resultado.Error("MaterialPorCentroId", Textos.RangosDeRedondeo_Existente);
            }

            if (comando.Dto.ValorDesde >= comando.Dto.ValorHasta)
            {
                resultado.Error("ValorDesde", Textos.RangosDeRedondeo_ValorDesdeEsMayor);
            }
        }
    }
}
