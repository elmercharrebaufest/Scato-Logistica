using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class EliminarTipoComercial : Comando
    {
        public int Id { get; set; }
    }
}
