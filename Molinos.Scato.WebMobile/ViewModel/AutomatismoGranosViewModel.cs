using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.ViewModel
{
    public class AutomatismoGranosViewModel
    {
        public AutomatismoGranosViewModel()
        {
            AutomatismoGrano = new AutomatismoGranoDto();
        }

        public AutomatismoGranoDto AutomatismoGrano { get; set; }
        public ListaPaginada<AutomatismoGranoDto> ListaAutomatismoGrano { get; set; }
        public List<SelectListItem> Materiales { get; set; }
        public List<SelectListItem> Variedades { get; set; }
        public List<SelectListItem> CallesHidraulica { get; set; }
        public List<SelectListItem> CallesPreBalanza { get; set; }
        public List<SelectListItem> Calidades { get; set; }
        public List<SelectListItem> Hidraulicas { get; set; }
        public List<SelectListItem> Almacenes { get; set; }
        public ListaPaginada<CalleDto> ConfiguracionCallesPreBalanza { get; set; }
        public ListaPaginada<CalleDto> ConfiguracionCallesPreHidraulica { get; set; }
        public ListaPaginada<LlamadoAutomaticoHidraulicaDto> ConfiguracionHidraulicas { get; set; }
        public bool AutomatismoLLamadoPrebalanza { get; set; }
        public bool AutomatismoGeneral { get; set; }

        public void CargarDatos(IServicioRepositorio servicio, int idCentro, AutomatismoGranoDto dto)
        {
            var callesPrebalanzaTotales = servicio.ListarCallesPorTipo(TipoCalle.PreBalanzaGranos).Where(c => !c.Deshabilitada).ToList();
            var callesHidraulicaTotales = servicio.ListarCallesPorTipo(TipoCalle.PlayaInterna).Where(c => c.CentroId == idCentro && !c.Deshabilitada).ToList();
            var callesPrebalanza = servicio.ListarCallesAutomatismoGrano(TipoCalle.PreBalanzaGranos, false, dto.CallePreBalanzaId).Where(c => !c.Deshabilitada).ToList();
            var callesHidraulica = servicio.ListarCallesAutomatismoGrano(TipoCalle.PlayaInterna, false, dto.CallePreHidraulicaId).Where(c => c.CentroId == idCentro && !c.Deshabilitada).ToList();
            var hidraulicas = servicio.ListarHidraulicasAutomatizadas().Where(c => c.Estado != EstadoHidraulica.Inhabilitado && c.CentroId == idCentro).ToList();

            Materiales = MapearMateriales(servicio.ListarMaterialGranoPorCentro(idCentro, true).Where(m => m.MostrarEnWebMobile).ToList(), dto.MaterialId);
            Variedades = MapearVariedades(servicio.ListarTipoVariedadPorMaterial(dto.MaterialId).Where(c => !c.Borrado).ToList(), dto.TipoVariedadId ?? 0);
            CallesPreBalanza = MapearCallesPreBalanzas(callesPrebalanza.Where(c => c.ActivoAutomatico == true).ToList(), dto.CallePreBalanzaId);
            Almacenes = dto.MaterialId > 0 ? MapearAlmacenes(servicio.ListarAlmacenesPorMaterial(idCentro, dto.MaterialId), dto.AlmacenId) : MapearAlmacenes(servicio.ListarAlmacenesPorCentro(idCentro), dto.AlmacenId);
            Hidraulicas = MapearHidraulicas(hidraulicas.Where(c => c.ActivoAutomatico == true).ToList(), AutomatismoGrano.Hidraulicas);
            Calidades = MapearCalidades(servicio.ListarCaracteristicasDeCalidadPorMaterial(dto.MaterialId, idCentro), dto.CalidadId ?? 0);
            CallesHidraulica = MapearCallesPreHidraulicas(callesHidraulica.Where(c => c.ActivoAutomatico == true).ToList(), dto.CallePreHidraulicaId);
            ConfiguracionCallesPreBalanza = new ListaPaginada<CalleDto>(callesPrebalanzaTotales, 1, callesPrebalanza.Count, callesPrebalanza.Count);
            ConfiguracionCallesPreHidraulica = new ListaPaginada<CalleDto>(callesHidraulicaTotales, 1, callesHidraulica.Count, callesHidraulica.Count);
            ConfiguracionHidraulicas = new ListaPaginada<LlamadoAutomaticoHidraulicaDto>(hidraulicas, 1, hidraulicas.Count, hidraulicas.Count);
            AutomatismoGeneral = Convert.ToBoolean(servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica, Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos).Valor);
            AutomatismoLLamadoPrebalanza = Convert.ToBoolean(servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica, Constantes.ConfiguracionGeneral.LlamadoAutomatico.PreBalanza).Valor);
        }

        private List<SelectListItem> MapearMateriales(IList<MaterialPorCentroDto> materiales, int idSeleccionado = 0)
        {
            var listaMateriales = materiales.Select(m => new SelectListItem
            {
                Value = m.MaterialId.ToString(),
                Text = m.DescripcionWebMobile,
                Selected = (m.Id == idSeleccionado)
            }).ToList();
            listaMateriales.Insert(0, new SelectListItem { Value = "", Text = Textos.Default_Material });

            return listaMateriales;
        }

        private List<SelectListItem> MapearVariedades(IList<TipoVariedadPorMaterialDto> variedadPorMaterial, int idSeleccionado = 0)
        {
            var listaVariedad = variedadPorMaterial.Select(m => new SelectListItem
            {
                Value = m.TipoVariedadId.ToString(),
                Text = m.TipoVariedadDescripcion,
                Selected = (m.TipoVariedadId == idSeleccionado)
            }).ToList();
            listaVariedad.Insert(0, new SelectListItem { Value = "", Text = Textos.Variedad_Estandar });

            return listaVariedad;
        }

        private List<SelectListItem> MapearCallesPreBalanzas(IList<CalleDto> callesPreBalanzas, int idSeleccionado = 0)
        {
            var cpBalanzas = callesPreBalanzas.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Nombre,
                Selected = (m.Id == idSeleccionado)
            }).ToList();
            cpBalanzas.Insert(0, new SelectListItem { Value = "", Text = Textos.Default_PreBalanza });

            return cpBalanzas;
        }

        private List<SelectListItem> MapearAlmacenes(IList<AlmacenDto> almacenes, int idSeleccionado = 0)
        {
            var ltsAlmacenes = almacenes.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Descripcion,
                Selected = (m.Id == idSeleccionado)
            }).ToList();
            ltsAlmacenes.Insert(0, new SelectListItem { Value = "", Text = Textos.Default_Almacen });

            return ltsAlmacenes;
        }

        private List<SelectListItem> MapearHidraulicas(IList<LlamadoAutomaticoHidraulicaDto> hidraulicas, List<int> idSeleccionados)
        {
            var opciones = hidraulicas.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = h.HidraulicaNombre,
                Selected = idSeleccionados.Contains(h.Id)
            }).ToList();

            return new MultiSelectList(opciones, "Value", "Text", idSeleccionados).ToList();
        }

        private List<SelectListItem> MapearCalidades(IList<CaracteristicaDeCalidadDto> calidades, int idSeleccionado = 0)
        {
            var ltsCalidades = calidades.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Descripcion,
                Selected = (m.Id == idSeleccionado)
            }).ToList();
            ltsCalidades.Insert(0, new SelectListItem { Value = "", Text = Textos.Default_Calidad });

            return ltsCalidades;
        }

        private List<SelectListItem> MapearCallesPreHidraulicas(IList<CalleDto> preHidraulicas, int idSeleccionado)
        {
            var ltsCalle = preHidraulicas.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Nombre,
                Selected = (m.Id == idSeleccionado)
            }).ToList();
            ltsCalle.Insert(0, new SelectListItem { Value = "", Text = Textos.Default_PreHidraulica });

            return ltsCalle;
        }
    }
}