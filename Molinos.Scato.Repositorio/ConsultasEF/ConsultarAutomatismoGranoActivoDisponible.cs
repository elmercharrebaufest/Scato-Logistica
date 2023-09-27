using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ConsultarAutomatismoGranoActivoDisponible : IConsultaEscalar<AutomatismoGranoDto>
    {
        private readonly Guid workflowInstanceId;
        private readonly List<TipoVehiculo> tipoVehiculosEscalables;

        private int materialId;
        private int? tipoVariedadId;
        private TipoVehiculo tipoVehiculo;
        private IList<AnalisisPorCaracteristica> calidadesAnalizadas;
        private ICollection<CaladoPorCaracteristica> calidadesCalados;

        public ConsultarAutomatismoGranoActivoDisponible(Guid workflowInstanceId)
        {
            this.workflowInstanceId = workflowInstanceId;
            this.tipoVehiculosEscalables = new List<TipoVehiculo> { TipoVehiculo.CamiónC, TipoVehiculo.CamiónD, TipoVehiculo.CamiónE };
        }

        public AutomatismoGranoDto Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            ObtenerDatosDelRecorrido(contexto);

            var automatismoGrano = ListarAutomatismosActivosConMismasCaracteristicas(contexto)
                                            .Where(x => FiltrarCalidad(x))
                                            .Where(x => FiltrarEscalabilidad(x))
                                            .FirstOrDefault();

            if (automatismoGrano == null)
                return null;

            return new AutomatismoGranoDto
            {
                Id = automatismoGrano.Id,
                CallePreBalanzaId = automatismoGrano.CallePreBalanza.Id,
                CallePBDescripcion = automatismoGrano.CallePreBalanza.Nombre,
                CallePreHidraulicaId = automatismoGrano.CallePreHidraulica.Id,
                CallePHDescripcion = automatismoGrano.CallePreHidraulica.Nombre,
                AlmacenId = automatismoGrano.Almacen.Id,
                AlmacenDescripcion = automatismoGrano.Almacen.Descripcion,
                Hidraulicas = automatismoGrano.Hidraulicas.Select(x => x.Id).ToList(),
                HidraulicaDescripcion = string.Join(",", automatismoGrano.Hidraulicas.Select(x => x.Nombre))
            };
        }

        private void ObtenerDatosDelRecorrido(DbContext contexto)
        {
            var recorrido = contexto.Set<Recorrido>().First(x => x.InstanciaWorkflow == workflowInstanceId);
            this.materialId = recorrido.Material.Id;
            this.tipoVariedadId = recorrido.TipoVariedadId;
            this.tipoVehiculo = recorrido.TipoVehiculo;

            this.calidadesAnalizadas = recorrido.AnalisisDeCalidad?.CaracteristicasAnalizadas;
            this.calidadesCalados = recorrido.Calado?.CaladosPorCaracteristica;
        }

        private List<AutomatismoGrano> ListarAutomatismosActivosConMismasCaracteristicas(DbContext contexto)
        {
            Func<AutomatismoGrano, bool> FiltrarAutomatismoActivoConMismoMaterialYVariedad = x => x.Activo && x.MaterialId == this.materialId && x.TipoVariedadId == this.tipoVariedadId;

            var automatismosGranos = contexto.Set<AutomatismoGrano>()
                                            .Include(x => x.Hidraulicas)
                                            .Include(x => x.CallePreBalanza)
                                            .Include(x => x.CallePreHidraulica)
                                            .Include(x => x.Almacen)
                                            .Where(FiltrarAutomatismoActivoConMismoMaterialYVariedad)
                                            .ToList();

            return automatismosGranos;
        }

        private bool FiltrarCalidad(AutomatismoGrano automatismo)
        {
            if (!automatismo.AplicaFiltroCalidad)
                return true;

            if (!automatismo.CalidadId.HasValue)
                return false;

            if (calidadesAnalizadas != null)
            {
                var calidadAnalizada = calidadesAnalizadas.FirstOrDefault(r => r.CaracteristicaDeCalidad.Id == automatismo.CalidadId);
                if (calidadAnalizada != null)
                    return calidadAnalizada.ValorAnalisis.GetValueOrDefault() <= automatismo.Maximo.GetValueOrDefault()
                        && calidadAnalizada.ValorAnalisis.GetValueOrDefault() >= automatismo.Minimo.GetValueOrDefault();
            }

            if (calidadesCalados != null)
            {
                var calidadCalado = calidadesCalados.FirstOrDefault(r => r.CaracteristicaDeCalidad.Id == automatismo.CalidadId);
                if (calidadCalado != null)
                    return calidadCalado.ValorCalado.GetValueOrDefault() <= automatismo.Maximo.GetValueOrDefault()
                        && calidadCalado.ValorCalado.GetValueOrDefault() >= automatismo.Minimo.GetValueOrDefault();
            }

            return false;
        }

        private bool FiltrarEscalabilidad(AutomatismoGrano automatismo)
        {
            if (automatismo.CamionEscalable)
                return this.tipoVehiculo == TipoVehiculo.Camión || tipoVehiculosEscalables.Contains(this.tipoVehiculo);
            else
                return this.tipoVehiculo == TipoVehiculo.Camión;
        }
    }
}