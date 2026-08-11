using Molinos.Scato.Dominio;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Runtime.CompilerServices;
using static Molinos.Scato.Dominio.Constantes.ConfiguracionGeneral;

namespace Molinos.Scato.Actividades.Internas
{
    /// <summary>
    ///     Retorna True solo si el chofer existe y tanto el como el camion estan habilitados
    /// </summary>
    public class ExisteExcepcionAlControlByPass : ExisteExcepcionAlControl
    {
       protected override bool Execute(CodeActivityContext context)
        {
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var centroId = CentroId.Get<int>(context);
            var transportistaId = TransportistaId.Get<int>(context);
            var materialId = MaterialId.Get<int>(context);
            var clienteDestinoId = ClienteDestinoId.Get<int?>(context);
            var intermediarioId = IntermediarioId.Get<int>(context);
            var esCartaPorte = EsCartaPorte.Get<bool?>(context);

            var centro = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass, Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Centro);
            bool tieneException = false;
            if (centro != null && int.TryParse(centro.Valor, out int destinoCentroId))
            {
                tieneException = esCartaPorte.HasValue && esCartaPorte.Value
                ? ExisteExcepcionAlControlParaCartaPorte(repositorio, materialId, transportistaId, intermediarioId, centroId, DateTime.Today, destinoCentroId, clienteDestinoId)
                : ExisteExcepcionAlControlParaOrdenes(repositorio, materialId, transportistaId, intermediarioId, centroId, DateTime.Today, destinoCentroId, clienteDestinoId);
            }

            return tieneException;
        }
    }
}