using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarEmpresa : ProcesadorModificar<ModificarEmpresa>
    {
        public ProcesadorModificarEmpresa(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarEmpresa comando)
        {
            var empresa = Repositorio.Obtener<Empresa>(comando.Dto.Id);
            empresa.Nombre = comando.Dto.Nombre;
        }

        protected override void Validar(ModificarEmpresa comando, Resultado resultado)
        {
            if (Repositorio.Existe<Empresa>(x => x.Nombre == comando.Dto.Nombre && x.Id != comando.Dto.Id))
            {
                resultado.Error("Nombre", string.Format(Textos.Error_Existente, "Nombre"));
            }
        }
    }
}
