using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;
using System.ServiceModel;

namespace Molinos.Scato.Servicios
{
    [ServiceContract]
    public interface IAdministradorDeCalles
    {
        [OperationContract]
        Calle AsignarCalle(TipoCalle tipoCalle, Material material, TipoCalidad calidad, int centroId, bool llegoEnHorarioCircular = false, Guid? instanceId = null);
        [OperationContract]
        Calle ObtenerSiguienteCalle(int materialId);
        [OperationContract]
        int ObtenerEspacioDisponible(TipoCalle tipoCalle, int materialId, TipoCalidad calidad, int? calleId = null);
        [OperationContract]
        bool ObtenerEspacioDisponibleEnCalle(int calleId);
    }
}
