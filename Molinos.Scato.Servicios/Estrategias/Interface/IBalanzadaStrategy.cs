using Molinos.Scato.Repositorio;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios.Estrategias
{
    public interface IBalanzadaStrategy
    {
        string Nombre { get; }
        bool RegistrarBalanzada(Dictionary<string, string> datos);
    }
}