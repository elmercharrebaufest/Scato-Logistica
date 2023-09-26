using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarComercial : ProcesadorModificar<ModificarComercial>
    {
        public ProcesadorModificarComercial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarComercial comando)
        {
            var comercialEditado = Repositorio.Obtener<Comercial>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, comercialEditado);
        }

        protected override void Validar(ModificarComercial comando, Resultado resultado)
        {
            if (Repositorio.Existe<Comercial>(e => e.CodigoSap == comando.Dto.CodigoSap && e.Id !=comando.Dto.Id))
            {
                resultado.Error("CodigoSap", Textos.Comercial_CodigoSapExistente);
            }
            
        }
    }
}
