namespace QaTools.Dao
{
    public interface IAdministracionDao
    {
        IEnumerable<int> GetRecorridos(int centroId, int workflowId);
        void EliminarRecorrido(int recorridoId);
    }
}
