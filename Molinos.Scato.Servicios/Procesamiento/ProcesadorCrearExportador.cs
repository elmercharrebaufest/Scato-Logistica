using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearExportador : ProcesadorCrear<CrearExportador, Exportador>
    {
        public ProcesadorCrearExportador(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Exportador CrearEntidad(CrearExportador comando)
        {
            var entidad = Conversor.Convertir<ExportadorDto, Exportador>(comando.Dto);
            if (comando.Dto.Almacen_Id != null && comando.Dto.Almacen_Id != 0)
            {
                entidad.Almacen = Repositorio.Obtener<Almacen>(comando.Dto.Almacen_Id);
            }
            return entidad;
        }

        protected override void Validar(CrearExportador comando, Resultado resultado)
        {
            if (Repositorio.Existe<Exportador>(e => e.Nombre == comando.Dto.Nombre))
            {
                resultado.Error("Descripcion", Textos.Error_Existente);
            }
        }
    }
}
