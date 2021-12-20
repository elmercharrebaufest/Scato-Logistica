using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearEmpresa : ProcesadorCrear<CrearEmpresa, Empresa>
    {
        public ProcesadorCrearEmpresa(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Empresa CrearEntidad(CrearEmpresa comando)
        {
            return Conversor.Convertir<EmpresaDto, Empresa>(comando.Dto);
        }

        protected override void Validar(CrearEmpresa comando, Resultado resultado)
        {
            if (Repositorio.Existe<Empresa>(x => x.Nombre == comando.Dto.Nombre))
            {
                resultado.Error("Nombre", string.Format(Textos.Error_Existente,"Nombre"));
            }
        }
    }
}
