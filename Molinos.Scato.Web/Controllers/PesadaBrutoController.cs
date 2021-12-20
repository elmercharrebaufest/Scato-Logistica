using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadPesadaBruto)]
    public class PesadaBrutoController : PesadaController
    {
        public PesadaBrutoController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<IPesadaService> factory, IServicioComandos comando, IServicioOrquestador orquestador, IListaDeWorkflows workflows)
            : base(log, servicio, factory, comando, orquestador, workflows)
        {
            TipoPesada = TipoPesada.Bruto;
            ActividadXaml = "PesadaBruto";
        }
    }
}