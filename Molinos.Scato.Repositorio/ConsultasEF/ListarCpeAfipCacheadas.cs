using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarCpeAfipCacheadas : IConsulta<CartaPorteElectronicaDto>
    {
        private readonly List<CartaPorteResumenDto> ctgs;
        private readonly int pagina;
        public ListarCpeAfipCacheadas(List<CartaPorteResumenDto> ctgs)
        {
            this.ctgs = ctgs;
        }

        public List<CartaPorteElectronicaDto> Ejecutar(DbContext contexto)
        {
            var onlyCtgs = ctgs.Select(x => x.Ctg).ToList();

            var resultado =
                contexto.Set<CartaPorteElectronica>()
                        .Where(x=> onlyCtgs.Any(c=> c == x.NroCTG))
                        .Select(x => new CartaPorteElectronicaDto() { NroCtg = x.NroCTG.Value, FechaUltimaActualizacion = x.FechaUltimaActualizacion }).ToList();

            return resultado.Where(x=> ctgs.Any(c=>c.Ctg == x.NroCtg && c.FechaUltimaModificacion == x.FechaUltimaActualizacion)).ToList();
        }
    }
}
