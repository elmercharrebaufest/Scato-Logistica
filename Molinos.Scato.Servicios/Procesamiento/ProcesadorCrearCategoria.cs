using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;


namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCategoria : ProcesadorCrear<CrearCategoria, Categoria>
    {

        public ProcesadorCrearCategoria(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Categoria CrearEntidad(CrearCategoria comando)
        {
            var entidad = Conversor.Convertir<CategoriaDto, Categoria>(comando.Dto);
            return entidad;
        }

        protected override void Validar(CrearCategoria comando, Resultado resultado)
        {
            if (Repositorio.Existe<Categoria>(e => e.Clasificacion == comando.Dto.Clasificacion && (comando.Dto.Id == 0 || e.Id != comando.Dto.Id)))
            {
                resultado.Error("CodigoSap", string.Format(Textos.Error_Existente, Textos.Camara_CodigoSAP));
            }
        }


    }
}
