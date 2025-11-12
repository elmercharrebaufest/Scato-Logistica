using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos.Consultas
{
    public class ConsultarPanelPagoMunicipal : Comando
    {
        public FiltroPanelPagoMunicipalDto Filtro {  get; set; }
        public Paginacion Paginacion { get; set; }
    }
}
