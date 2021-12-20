using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum AccionColaImpresion : int
    {
        Pausar = 0,
        Resumir = 1,
        Reiniciar = 2,
        Cancelar = 3
    }
}
