using System.Collections.Generic;
using Molinos.Scato.Dominio.Consultas;

namespace Molinos.Scato.Servicios.Conversiones
{
    public interface IConversor
    {
        TSalida Convertir<TEntrada, TSalida>(TEntrada entrada);
        TSalida Convertir<TEntrada, TSalida>(TEntrada entrada, TSalida salida);
        ListaPaginada<TSalida> ConvertirListaPaginada<TEntrada, TSalida>(ListaPaginada<TEntrada> lista);
        IList<TSalida> ConvertirList<TEntrada, TSalida>(IList<TEntrada> lista);
    }
}
