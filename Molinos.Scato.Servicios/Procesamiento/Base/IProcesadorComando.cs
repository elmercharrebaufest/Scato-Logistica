using Molinos.Scato.Dominio.Comandos;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public interface IProcesadorComando
    {
        Resultado Ejecutar(Comando comando);
    }

    public interface IProcesadorComando<in TComando> : IProcesadorComando
    {
        Resultado Ejecutar(TComando comando);
    }
    
}
