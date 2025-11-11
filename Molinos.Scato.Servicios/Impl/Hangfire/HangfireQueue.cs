using Molinos.Scato.Dominio.Comandos;

namespace Molinos.Scato.Servicios.Impl.Hangfire
{
    public class HangfireQueue : IHangfireQueue
    {
        private readonly IServicioComandos comandos;

        public HangfireQueue(IServicioComandos comandos)
        {
            this.comandos = comandos;
        }

        public Resultado EncolarImportarCartaPorteVisec(int id)
        {
            var comando = new ImportarCartaPorteVisec { Id = id };
            var resultado = comandos.Ejecutar(comando);
            return resultado;
        }
    }
}