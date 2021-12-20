using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearIngresoDeDatosDeExportacion : ProcesadorCrear<CrearIngresoDeDatosDeExportacion, IngresoDeDatosDeExportacion>
    {
        public ProcesadorCrearIngresoDeDatosDeExportacion(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override IngresoDeDatosDeExportacion CrearEntidad(CrearIngresoDeDatosDeExportacion comando)
        {
            return new IngresoDeDatosDeExportacion
            {
                PermisoEmbarque = comando.Dto.PermisoEmbarque,
                IdentificadorContenedor = comando.Dto.IdentificadorContenedor,
                Firma = Repositorio.Obtener<Firma>(comando.Dto.FirmaId),
                Recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.Dto.InstanciaWorkflow),
                Nacionalidad = Repositorio.Obtener<Pais>(comando.Dto.NacionalidadId),
                Transportista = Repositorio.Obtener<Transportista>(comando.Dto.TransportistaId),
                PesoNeto = comando.Dto.PesoNeto
            };
        }

        protected override void Validar(CrearIngresoDeDatosDeExportacion comando, Resultado resultado)
        {
            
        }
    }
}