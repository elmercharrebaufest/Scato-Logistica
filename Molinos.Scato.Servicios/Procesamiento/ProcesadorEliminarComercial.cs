using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarComercial : ProcesadorEliminar<EliminarComercial, Comercial>
    {
        public ProcesadorEliminarComercial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarComercial comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarComercial comando, Resultado resultado)
        {
            if (Repositorio.Existe<Establecimiento>(e => e.Comercial.Id == comando.Id))
            {
                resultado.Error("", Textos.Comercial_RelacionExistenteEstablecimiento);
            }
        }
    }
}
