namespace Molinos.Scato.Actividades.Servicios
{
    /// <summary>
    /// Factory para crear instancias de los servicios de actividades. Es necesaria porque no se puede usar Ninject directamente para crear las instancias, dado que se necesita el workflow como parámetro.
    /// </summary>
    /// <typeparam name="T">La interface del servicio que se quiere crear</typeparam>
    public interface IServicioActividadFactory<out T>
    {
        /// <summary>
        /// Crea el servicio utilizando el workflow pasado como parámetro y para construir el endpoint
        /// </summary>
        /// <param name="workflowDefinicionId"></param>
        /// 
        /// <returns>Devuelve una instancia del servicio lista para ejecutar</returns>
        T CrearServicio(int workflowDefinicionId);
    }
}
