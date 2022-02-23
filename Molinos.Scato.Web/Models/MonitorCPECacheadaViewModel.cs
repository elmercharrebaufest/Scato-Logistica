using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Models
{
    public class MonitorCPECacheadaViewModel
    {
        public MonitorCPECacheadaViewModel()
        {
            MaterialList = new List<SelectListItem>();
        }
        public MonitorCPECacheadaFiltroDto Data { get; set; }
        public List<SelectListItem> MaterialList { get; set; }
    }
}