using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarAlmacen : ProcesadorModificar<ModificarAlmacen>
    {
        public ProcesadorModificarAlmacen(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarAlmacen comando)
        {
            var almacenEditado = Repositorio.Obtener<Almacen>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, almacenEditado);
        }

        protected override void Validar(ModificarAlmacen comando, Resultado resultado)
        {
            if (Repositorio.Existe<Almacen>(e => e.Descripcion == comando.Dto.Descripcion && e.Centro.Id == comando.Dto.CentroId && e.Id != comando.Dto.Id))
            {
                resultado.Error("Descripcion", Textos.Almacen_DescripcionExistente);
            }

            var ConfiguracionGranoActivo = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica && x.Nombre == Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);
            var ConfiguracionNoGranoActivo = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.TableroComandoPuerto && x.Nombre == Constantes.ConfiguracionGeneral.LlamadoAutomatico.NoGranos); 
            
            if ((ConfiguracionGranoActivo.Valor.Equals("True")
                && Repositorio.Existe<AutomatismoGrano>(a => a.Activo && a.Almacen.Id == comando.Dto.Id))
                || (ConfiguracionNoGranoActivo.Valor.Equals("True")
                && Repositorio.Existe<AutomatismoNoGrano>(a => a.Activo && a.Almacen.Id == comando.Dto.Id)))
            {
                resultado.Error("PreBalanza", Textos.Automatismo_CalleUtilizadaEnAutomatismoActivo);
            }
        }
    }
}
