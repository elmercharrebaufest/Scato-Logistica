using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarImpresora : ProcesadorModificar<ModificarImpresora>
    {
        public ProcesadorModificarImpresora(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarImpresora comando)
        {
            var impresora = Repositorio.Obtener<Impresora>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, impresora);
            impresora.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
        }

        protected override void Validar(ModificarImpresora comando, Resultado resultado)
        {
            if (Repositorio.Existe<Impresora>(x => x.Id != comando.Dto.Id && x.Descripcion == comando.Dto.Descripcion && x.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Descripcion", Textos.Impresora_Existente);
            }
        }
    }
}
