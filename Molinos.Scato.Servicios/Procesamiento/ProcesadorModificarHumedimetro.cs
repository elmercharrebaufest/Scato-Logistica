using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarHumedimetro : ProcesadorModificar<ModificarHumedimetro>
    {
        public ProcesadorModificarHumedimetro(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarHumedimetro comando)
        {
            var humedimetro = Repositorio.Obtener<Humedimetro>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, humedimetro);
        }

        protected override void Validar(ModificarHumedimetro comando, Resultado resultado)
        {
            if (Repositorio.Existe<Humedimetro>(e => e.Descripcion == comando.Dto.Descripcion && e.Centro.Id == comando.Dto.CentroId && e.Id != comando.Dto.Id))
            {
                resultado.Error("Descripcion", Textos.Humedimetro_DescripcionExistente);
            }
            if (Repositorio.Existe<Humedimetro>(e => e.PuestoDeTrabajo == comando.Dto.PuestoDeTrabajo && e.Centro.Id == comando.Dto.CentroId && e.Id != comando.Dto.Id))
            {
                resultado.Error("PuestoDeTrabajo", Textos.Humedimetro_PuestoDeTrabajoExistente);
            }
        }
    }
}
