using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarEstadoHidraulica : ProcesadorModificar<ModificarEstadoHidraulica>
    {
        public ProcesadorModificarEstadoHidraulica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarEstadoHidraulica comando)
        {
            var automatismos = TodosLosAutomatismosActivos();
            var hidraulica = Repositorio.Obtener<PuestosDeCargaDescarga>(comando.Id);

            var hiautomatismoEditar = automatismos.Where(c => c.Hidraulicas.Count == 1 && c.Hidraulicas.Contains(hidraulica)).ToList();

            foreach (var automatismo in hiautomatismoEditar)
            {
                automatismo.Activo = false;
            }

            hidraulica.ActivoAutomatico = comando.ActivoAutomatico;
        }

        private IList<AutomatismoGrano> TodosLosAutomatismosActivos()
        {
            var includesGrano = new List<Expression<Func<AutomatismoGrano, object>>> { 
                x => x.Material, 
                x => x.CallePreBalanza, 
                x => x.CallePreHidraulica, 
                x => x.TipoVariedades, 
                x => x.Almacen, 
                x => x.Hidraulicas };
            return Repositorio.Listar<AutomatismoGrano>(includesGrano, c => c.Activo == true);
        }

        protected override void Validar(ModificarEstadoHidraulica comando, Resultado resultado)
        {
            var configuracion = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica && x.Nombre == Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);

            if (!comando.ActivoAutomatico && configuracion.Valor.Equals("True"))
            {
                var todosLosAutomatismosActivos = TodosLosAutomatismosActivos();

                // Obtener los automatismos activos que contienen la hidráulica
                var automatismosActivosConLaHidraulica = todosLosAutomatismosActivos
                    .Where(a => a.Hidraulicas.Any(h => h.Id == comando.Id))
                    .ToList();

                // Verificar si la hidráulica es la única en algún automatismo activo
                bool hidraulicaUnicaEnAlgunAutomatismoActivo = automatismosActivosConLaHidraulica
                    .Any(a => a.Hidraulicas.Count(h => h.ActivoAutomatico) == 1);

                // Verificar si la hidráulica está en varios automatismos activos y es la única en cada uno
                bool hidraulicaUnicaEnVariosAutomatismosActivos = automatismosActivosConLaHidraulica.Count > 1 &&
                    automatismosActivosConLaHidraulica.All(a => a.Hidraulicas.Count(h => h.ActivoAutomatico) == 1);

                // Obtener una lista de IDs de los automatismos que contienen la hidráulica
                var idsAutomatismosConLaHidraulica = string.Join(", ", automatismosActivosConLaHidraulica.Select(a => a.Id.ToString()));

                // Generar error si se cumplen las condiciones para no permitir la deshabilitación
                if (hidraulicaUnicaEnAlgunAutomatismoActivo || hidraulicaUnicaEnVariosAutomatismosActivos)
                {
                    resultado.Error("Hidraulica", string.Format(Textos.HidraulicaUtilizadaEnVariosAutomatismoActivo, idsAutomatismosConLaHidraulica));
                    return;
                }

            }
        }
    }
}