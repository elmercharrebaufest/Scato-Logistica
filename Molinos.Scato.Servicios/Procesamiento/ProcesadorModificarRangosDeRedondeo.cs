using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRangosDeRedondeo : ProcesadorModificar<ModificarRangosDeRedondeo>
    {
        public ProcesadorModificarRangosDeRedondeo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarRangosDeRedondeo comando)
        {
            var rangosDeRedondeoEditado = Repositorio.Obtener<RangosDeRedondeo>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, rangosDeRedondeoEditado);

            rangosDeRedondeoEditado.ValorDesde = comando.Dto.ValorDesde;
            rangosDeRedondeoEditado.ValorHasta = comando.Dto.ValorHasta;
            rangosDeRedondeoEditado.ValorRedondeado = comando.Dto.ValorRedondeado;
        }

        protected override void Validar(ModificarRangosDeRedondeo comando, Resultado resultado)
        {
            if (Repositorio.Existe<RangosDeRedondeo>(e => e.Id != comando.Dto.Id &&
                                                    (e.MaterialPorCentro.Id == comando.Dto.MaterialPorCentroId) &&
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
