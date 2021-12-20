using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarTecnologia : ProcesadorModificar<ModificarTecnologia>
    {
        public ProcesadorModificarTecnologia(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarTecnologia comando)
        {
            var tecnologia = Repositorio.Obtener<Tecnologia>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, tecnologia);
            tecnologia.Empresa = Repositorio.Obtener<Empresa>(comando.Dto.EmpresaId);
        }

        protected override void Validar(ModificarTecnologia comando, Resultado resultado)
        {
            if (Repositorio.Existe<Tecnologia>(e => e.Codigo == comando.Dto.Codigo && e.Id != comando.Dto.Id))
            {
                resultado.Error("Codigo", Textos.Tecnologia_CodigoExistente);
            }
        }
    }
}
