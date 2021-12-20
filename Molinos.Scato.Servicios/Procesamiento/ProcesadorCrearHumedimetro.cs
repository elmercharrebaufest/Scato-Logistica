using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearHumedimetro : ProcesadorCrear<CrearHumedimetro, Humedimetro>
    {
        public ProcesadorCrearHumedimetro(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Humedimetro CrearEntidad(CrearHumedimetro comando)
        {
            var humedimetro = Conversor.Convertir<HumedimetroDto, Humedimetro>(comando.Dto);
            humedimetro.Centro = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroId);
            return humedimetro;
        }

        protected override void Validar(CrearHumedimetro comando, Resultado resultado)
        {
            if (Repositorio.Existe<Humedimetro>(e => e.Descripcion == comando.Dto.Descripcion && e.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Descripcion", Textos.Humedimetro_DescripcionExistente);
            }

            if (Repositorio.Existe<Humedimetro>(e => e.PuestoDeTrabajo == comando.Dto.PuestoDeTrabajo && e.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("PuestoDeTrabajo", Textos.Humedimetro_PuestoDeTrabajoExistente);
            }
        }
    }
}
