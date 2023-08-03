using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IPuestoComandoPuertoService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado PuestoComandoPuerto(Guid instanceId, ControlRecorridoDto controlRecorrido);
    }
}
