using Microsoft.AspNetCore.Mvc;

namespace QaTools.Dao
{
    public interface IAdministracionDao
    {
        IEnumerable<int> GetRecorridos(int centroId, int workflowId);
        JsonResult EliminarRecorridos(IEnumerable<int> recorridos);
    }
}
