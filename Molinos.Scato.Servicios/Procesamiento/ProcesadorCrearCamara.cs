using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCamara : ProcesadorCrear<CrearCamara, Camara>
    {
        public ProcesadorCrearCamara(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Camara CrearEntidad(CrearCamara comando)
        {
            var entidad = Conversor.Convertir<CamaraDto, Camara>(comando.Dto);
            return entidad;
        }

        protected override void Validar(CrearCamara comando, Resultado resultado)
        {
            if (Repositorio.Existe<Camara>(e => e.CodigoSAP == comando.Dto.CodigoSAP && (comando.Dto.Id == 0 || e.Id != comando.Dto.Id)))
            {
                resultado.Error("CodigoSap", string.Format(Textos.Error_Existente, Textos.Camara_CodigoSAP));
            }
        }
    }
}
