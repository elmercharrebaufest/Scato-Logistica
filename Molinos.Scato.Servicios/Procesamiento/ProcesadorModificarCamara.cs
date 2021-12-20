using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCamara : ProcesadorModificar<ModificarCamara>
    {
        public ProcesadorModificarCamara(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCamara comando)
        {
            var camara = Repositorio.Obtener<Camara>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, camara);
        }

        protected override void Validar(ModificarCamara comando, Resultado resultado)
        {
            if (Repositorio.Existe<Camara>(e => e.CodigoSAP == comando.Dto.CodigoSAP && (comando.Dto.Id == 0 || e.Id != comando.Dto.Id)))
            {
                resultado.Error("CodigoSap", string.Format(Textos.Error_Existente, Textos.Camara_CodigoSAP));
            }
        }
    }
}
