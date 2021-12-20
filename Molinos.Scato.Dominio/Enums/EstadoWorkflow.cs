using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Enums
{
    public enum EstadoWorkflow
    {
        Active,
        Idle,
        Exception,
        UserSuspension,
        Successful,
        Canceled,
        Terminated,
    }
}
