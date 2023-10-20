using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAutomatismoGranos : ProcesadorCrear<CrearAutomatismoGranos, AutomatismoGrano>
    {
        public ProcesadorCrearAutomatismoGranos(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override AutomatismoGrano CrearEntidad(CrearAutomatismoGranos comando)

        {
            var callePrebalanza = Repositorio.Obtener<Calle>(c => c.Id == comando.Dto.CallePreBalanzaId);
            callePrebalanza.Material = Repositorio.Obtener<Material>(c => c.Id == comando.Dto.MaterialId);
            var automatismo = new AutomatismoGrano
            {
                Activo = comando.Dto.Activo,
                AlmacenId = comando.Dto.AlmacenId,
                AplicaFiltroCalidad = comando.Dto.AplicaFiltroCalidad,
                CalidadId = comando.Dto.CalidadId,
                CallePreHidraulicaId = comando.Dto.CallePreHidraulicaId,
                CamionEscalable = comando.Dto.CamionEscalable,
                EsPasoDirecto = comando.Dto.Llamado1a1 ? false : comando.Dto.EsPasoDirecto,
                Llamado1a1 = comando.Dto.Llamado1a1,
                MaterialId = comando.Dto.MaterialId,
                Maximo = comando.Dto.Maximo,
                Minimo = comando.Dto.Minimo,
                TipoVariedadId = comando.Dto.TipoVariedadId,
                Almacen = Repositorio.Obtener<Almacen>(c => c.Id == comando.Dto.AlmacenId),
                Calidad = Repositorio.Obtener<CaracteristicaDeCalidad>(c => c.Id == 1),
                CallePreBalanza = callePrebalanza,
                CallePreHidraulica = Repositorio.Obtener<Calle>(c => c.Id == comando.Dto.CallePreHidraulicaId),
                TipoVariedad = Repositorio.Obtener<TipoVariedad>(c => c.Id == comando.Dto.TipoVariedadId)
            };

            return automatismo;
        }

        protected override void Validar(CrearAutomatismoGranos comando, Resultado resultado)
        {
            bool validacionConfigExistente = false;

            if (comando.Dto.TipoVariedadId == null)
                validacionConfigExistente = Repositorio.Existe<AutomatismoGrano>(a => a.MaterialId == comando.Dto.MaterialId && a.TipoVariedadId.Equals(comando.Dto.TipoVariedadId) && a.AplicaFiltroCalidad == comando.Dto.AplicaFiltroCalidad && a.CamionEscalable == comando.Dto.CamionEscalable);
            else if (comando.Dto.TipoVariedadId.HasValue)
                validacionConfigExistente = Repositorio.Existe<AutomatismoGrano>(a => a.MaterialId == comando.Dto.MaterialId && a.TipoVariedadId == comando.Dto.TipoVariedadId && a.AplicaFiltroCalidad == comando.Dto.AplicaFiltroCalidad && a.CamionEscalable == comando.Dto.CamionEscalable);

            if (Repositorio.Existe<AutomatismoGrano>(a => a.Id == comando.Dto.Id))
            {
                resultado.Error("Id Automatismo", Textos.Automatismo_IdExistente);
            }

            if (validacionConfigExistente)
            {
                resultado.Error("Id Variedad , Id Material, AplicaFiltroCalidad, CamionEscalable", Textos.Automatismo_ConfiguracionExistente);
            }
            if (Repositorio.Existe<AutomatismoGrano>(a => a.CallePreBalanzaId == comando.Dto.CallePreBalanzaId))
            {
                resultado.Error("Calle Prebalanza", Textos.Automatismo_CallePrebalanzaExistente);
            }
        }
    }
}