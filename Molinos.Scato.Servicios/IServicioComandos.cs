using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;

namespace Molinos.Scato.Servicios
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IServicioComandos
    {
        [OperationContract]
        Resultado Ejecutar(Comando comando);
    }
}
