using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Molinos.Scato.Web.ServicioHub.Server
{
    /// <summary>
    /// Hub SignalR para el Panel de Avance de Camión por Puesto.
    /// Grupo: puestoId (id del PuestoDeTrabajo).
    /// Mensajes cliente:
    ///   - nuevaContingencia(data)      → nueva alerta sin recorrido activo
    ///   - contingenciaEditando(data)   → un operador abrió la pantalla de resolución
    ///   - contingenciaLiberada(data)   → contingencia resuelta/liberada
    /// </summary>
    [HubName("panelAvanceCamion")]
    public class PanelAvanceCamionHub : Hub
    {
        /// <summary>
        /// El cliente se suscribe al grupo del PuestoDeTrabajo para recibir alertas por rol.
        /// </summary>
        public void SuscribirseAlPuesto(int puestoId)
        {
            if (puestoId > 0)
                Groups.Add(Context.ConnectionId, puestoId.ToString());
        }

        /// <summary>
        /// Emite una nueva contingencia a todos los clientes del grupo.
        /// Llamado desde el servidor (IHubContext) — no directamente por el cliente.
        /// </summary>
        public static void NotificarNuevaContingencia(int puestoId, object datos)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<PanelAvanceCamionHub>();
            hub.Clients.Group(puestoId.ToString()).nuevaContingencia(datos);
        }

        /// <summary>
        /// Emite cambio de estado a "Editando" a todos los clientes del grupo.
        /// Llamado desde el servidor (IHubContext) — no directamente por el cliente.
        /// </summary>
        public static void NotificarContingenciaEditando(int puestoId, object datos)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<PanelAvanceCamionHub>();
            hub.Clients.Group(puestoId.ToString()).contingenciaEditando(datos);
        }

        /// <summary>
        /// Emite liberación de contingencia (Atendido vuelve a 0) a todos los clientes del grupo.
        /// Llamado desde el servidor (IHubContext) — no directamente por el cliente.
        /// </summary>
        public static void NotificarContingenciaLiberada(int puestoId, object datos)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<PanelAvanceCamionHub>();
            hub.Clients.Group(puestoId.ToString()).contingenciaLiberada(datos);
        }
    }
}
