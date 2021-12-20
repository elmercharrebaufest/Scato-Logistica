using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCategoria : ProcesadorModificar<ModificarCategoria>
    {
        public ProcesadorModificarCategoria(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCategoria comando)
        {
            var categoria = Repositorio.Obtener<Categoria>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, categoria);
        }

        protected override void Validar(ModificarCategoria comando, Resultado resultado)
        {
            //if (Repositorio.Existe<Categoria>(e => e.Clasificacion == comando.Dto.Clasificacion && (comando.Dto.Id == 0 || e.Id != comando.Dto.Id)))
            //{
            //    resultado.Error("CodigoSap", string.Format(Textos.Error_Existente, Textos.Camara_CodigoSAP));
            //}
        }
    }
}