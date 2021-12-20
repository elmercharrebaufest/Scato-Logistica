using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarIngresoDatosExportacion : ProcesadorModificar<ModificarIngresoDatosExportacion>
    {
        public ProcesadorModificarIngresoDatosExportacion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarIngresoDatosExportacion comando)
        {
            var ingresoDeDatosDeExportacion = Repositorio.Obtener<IngresoDeDatosDeExportacion>(comando.Dto.Id);
            //Conversor.Convertir(comando.Dto, ingresoDeDatosDeExportacion);
            ingresoDeDatosDeExportacion.PermisoEmbarque = comando.Dto.PermisoEmbarque;
            ingresoDeDatosDeExportacion.IdentificadorContenedor = comando.Dto.IdentificadorContenedor;
            ingresoDeDatosDeExportacion.Firma = Repositorio.Obtener<Firma>(comando.Dto.FirmaId);
            ingresoDeDatosDeExportacion.Recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.Dto.InstanciaWorkflow);
            ingresoDeDatosDeExportacion.Nacionalidad = Repositorio.Obtener<Pais>(comando.Dto.NacionalidadId);
            ingresoDeDatosDeExportacion.Transportista = Repositorio.Obtener<Transportista>(comando.Dto.TransportistaId);
            ingresoDeDatosDeExportacion.PesoNeto = comando.Dto.PesoNeto;
        }

        protected override void Validar(ModificarIngresoDatosExportacion comando, Resultado resultado)
        {

        }
    }
}
