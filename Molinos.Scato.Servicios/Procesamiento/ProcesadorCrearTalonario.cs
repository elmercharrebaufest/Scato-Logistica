using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearTalonario: ProcesadorCrear<CrearTalonario,Talonario>
    {
        public ProcesadorCrearTalonario(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Talonario CrearEntidad(CrearTalonario comando)
        {
            var talonarioEditada = Conversor.Convertir<TalonarioDto,Talonario>(comando.Dto);
            talonarioEditada.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            return talonarioEditada;
        }

        protected override void Validar(CrearTalonario comando, Resultado resultado)
        {
            if (Repositorio.Existe<Talonario>(e => e.Descripcion == comando.Dto.Descripcion && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", Textos.Talonario_DescripcionExistente);
            }
        }
    }
}
