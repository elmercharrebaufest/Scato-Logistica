using Hangfire;
using Molinos.Scato.Servicios.Behavior;

namespace Molinos.Scato.Servicios.Impl
{
    [AiErrorHandlerBehaviorAttribute]
    public class ServicioHangfireQueue : IServicioHangfireQueue
    {
        private readonly IBackgroundJobClient backgroundJobClient;

        public ServicioHangfireQueue(IBackgroundJobClient backgroundJobClient)
        {
            this.backgroundJobClient = backgroundJobClient;
        }

        public void EncolarImportarCartaPorteVisec(int id)
        {
            backgroundJobClient.Enqueue<IHangfireQueue>(x => x.EncolarImportarCartaPorteVisec(id));
        }
    }
}