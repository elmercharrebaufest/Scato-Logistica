using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;

namespace Molinos.Scato.ModuloImpresor
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IServicioImpresion
    {
        [OperationContract]
        Resultado Ejecutar(Comando comando);
    }
}
