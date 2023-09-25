using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearComercial : ProcesadorCrear<CrearComercial, Comercial>
    {
        public ProcesadorCrearComercial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Comercial CrearEntidad(CrearComercial comando)
        {
            var comercialEditado = Conversor.Convertir<ComercialDto, Comercial>(comando.Dto);
            comercialEditado.Activo = true;
            return comercialEditado;
        }

        protected override void Validar(CrearComercial comando, Resultado resultado)
        {
            if (Repositorio.Existe<Comercial>(e => e.CodigoSap == comando.Dto.CodigoSap ))
            {
                resultado.Error("CodigoSap",Textos.Comercial_CodigoSapExistente);
            }
        }
    }
}
