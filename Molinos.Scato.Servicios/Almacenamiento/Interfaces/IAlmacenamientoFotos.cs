namespace Molinos.Scato.Servicios.Almacenamiento.Interfaces
{
    public interface IAlmacenamientoFotos
    {
        string CopiarImagenDesdeRuta(string rutaOrigen, string directorioDestino, string nombreArchivo, bool sobreescribir = true);
    }
}
