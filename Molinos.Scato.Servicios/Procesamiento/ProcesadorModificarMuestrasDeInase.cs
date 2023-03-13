using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarMuestraDeInase : ProcesadorModificar<ModificarMuestraDeInase>
    {
        public ProcesadorModificarMuestraDeInase(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarMuestraDeInase comando)
        {
            var muestras = Repositorio.Listar<MuestraDeInase>(x=> comando.Lista.Contains(x.Id));
            foreach (var muestra in muestras)
            {
                muestra.MuestraEnviada = true;
            }
        }

        protected override void Validar(ModificarMuestraDeInase comando, Resultado resultado)
        {

        }
    }
}
