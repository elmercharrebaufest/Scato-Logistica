using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearImpresora : ProcesadorCrear<CrearImpresora, Impresora>
    {
        public ProcesadorCrearImpresora(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override Impresora CrearEntidad(CrearImpresora comando)
        {
            var entidad = Conversor.Convertir<ImpresoraDto, Impresora>(comando.Dto);
            entidad.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            return entidad;
        }

        protected override void Validar(CrearImpresora comando, Resultado resultado)
        {
            if (Repositorio.Existe<Impresora>(x => x.Id != comando.Dto.Id && x.Descripcion == comando.Dto.Descripcion && x.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Descripcion", Textos.Impresora_Existente);
            }
        }
    }
}