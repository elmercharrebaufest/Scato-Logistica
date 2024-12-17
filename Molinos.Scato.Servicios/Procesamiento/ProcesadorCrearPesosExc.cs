using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearPesosExc : ProcesadorComando<CrearPesosExc>
    {
        public ProcesadorCrearPesosExc(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearPesosExc comando)
        {
            var resultado = new Resultado();
            var entity = new PesosExc();
            entity.PesoTomado = comando.PesoTomado;
            var recorrido = Repositorio.Obtener<Recorrido>(x => x.Id == comando.RecorridoId);
            recorrido.PasoPorContingenciaPesoExc = true;
            entity.Recorrido = recorrido;
            Repositorio.Agregar(entity);
            Repositorio.GuardarCambios();
            return resultado;
        }
    }
}
