using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
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
            var includesGrano = new List<Expression<Func<AutomatismoGrano, object>>> { x => x.Material, x => x.CallePreBalanza, x => x.CallePreHidraulica, x => x.TipoVariedad, x => x.Almacen, x => x.Hidraulicas };
            var automatismos = Repositorio.Listar<AutomatismoGrano>(includesGrano, c => c.Activo == true);
            var hidraulica = Repositorio.Obtener<PuestosDeCargaDescarga>(comando.Id);

            var hiautomatismoEditar = automatismos.Where(c => c.Hidraulicas.Count == 1 && c.Hidraulicas.Contains(hidraulica)).ToList();

            foreach (var automatismo in hiautomatismoEditar)
            {
                automatismo.Activo = false;
            }

            hidraulica.ActivoAutomatico = comando.ActivoAutomatico;
        }

        protected override void Validar(ModificarEstadoHidraulica comando, Resultado resultado)
        {
        }
    }
}