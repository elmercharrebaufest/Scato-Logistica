using System;

namespace Molinos.Scato.Dominio
{
    public interface ITipeable : IIdentificable
    {
        Type ObtenerTipoObjeto();
    }
}
