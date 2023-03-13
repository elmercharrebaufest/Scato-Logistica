using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Models
{
    public class ConfiguracionEficienciaCaladoViewModel
    {
        public ConfiguracionesEficienciaCaladoDto Configuraciones { get; set; }
        public ListaPaginada<EficienciaCaladoValoresDto> Items { get; set; }
        public ConfiguracionHorarioDto Horarios { get; set; }
    }
}