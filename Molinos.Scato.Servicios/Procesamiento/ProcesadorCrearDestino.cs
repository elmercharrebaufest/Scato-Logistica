using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearDestino : ProcesadorCrear<CrearDestino, Destino>
    {
        public ProcesadorCrearDestino(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Destino CrearEntidad(CrearDestino comando)
        {
            return Conversor.Convertir<DestinoDto, Destino>(comando.Dto);
        }

        protected override void Validar(CrearDestino comando, Resultado resultado)
        {
            if (Repositorio.Existe<Destino>(e => e.Nombre == comando.Dto.Nombre))
            {
                resultado.Error("Descripcion", Textos.Error_Existente);
            }
        }
    }
}
