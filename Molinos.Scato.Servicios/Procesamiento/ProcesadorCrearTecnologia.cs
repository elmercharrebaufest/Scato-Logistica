using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearTecnologia : ProcesadorCrear<CrearTecnologia, Tecnologia>
    {
        public ProcesadorCrearTecnologia(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Tecnologia CrearEntidad(CrearTecnologia comando)
        {
            var entidad =  Conversor.Convertir<TecnologiaDto, Tecnologia>(comando.Dto);
            entidad.Empresa = Repositorio.Obtener<Empresa>(comando.Dto.EmpresaId);
            return entidad;
        }

        protected override void Validar(CrearTecnologia comando, Resultado resultado)
        {
            if (Repositorio.Existe<Tecnologia>(e => e.Codigo == comando.Dto.Codigo))
            {
                resultado.Error("Codigo", Textos.Tecnologia_CodigoExistente);
            }
        }
    }
}
