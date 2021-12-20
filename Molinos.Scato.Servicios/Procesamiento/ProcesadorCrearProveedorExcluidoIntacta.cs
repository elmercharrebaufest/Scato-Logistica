using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearProveedorExcluidoIntacta : ProcesadorCrear<CrearProveedorExcluidoIntacta, ProveedorExcluidoIntacta>
    {
        public ProcesadorCrearProveedorExcluidoIntacta(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ProveedorExcluidoIntacta CrearEntidad(CrearProveedorExcluidoIntacta comando)
        {
            Log.Debug("Se va a crear el Proveedor Excluido Intacta con Id {0}", comando.ProveedorId);
            var proveedor = Repositorio.Obtener<Proveedor>(comando.ProveedorId);
            return  new ProveedorExcluidoIntacta {Proveedor = proveedor};
        }

        protected override void Validar(CrearProveedorExcluidoIntacta comando, Resultado resultado)
        {
            if (comando.ProveedorId == 0)
            {
                resultado.Error("Proveedor", Textos.Proveedor_Invalido);
            }
            else if (Repositorio.Existe<ProveedorExcluidoIntacta>(e => e.Proveedor.Id == comando.ProveedorId))
            {
                resultado.Error("Proveedor", Textos.ProveedorExcluidoIntacta_Existente);
            }
            
        }
    }
}
