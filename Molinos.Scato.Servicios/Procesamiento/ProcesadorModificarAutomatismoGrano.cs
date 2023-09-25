using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarAutomatismoGrano : ProcesadorModificar<ModificarAutomatismoGranos>
    {
        public ProcesadorModificarAutomatismoGrano(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarAutomatismoGranos comando)
        {
            var includes = new List<Expression<Func<AutomatismoGrano, object>>> { x => x.Material, x => x.CallePreBalanza, x => x.CallePreHidraulica, x => x.TipoVariedad, x => x.Almacen, x => x.Hidraulicas };
            var automatismoEditado = Repositorio.Obtener<AutomatismoGrano>(includes, c => c.Id == comando.Dto.Id);

            var hidraulicas = Repositorio.Listar<PuestosDeCargaDescarga>(p => comando.Dto.Hidraulicas.Contains(p.Id));

            var automatismo = Conversor.Convertir(comando.Dto, automatismoEditado);
            if (comando.Dto.MaterialId == 0 && comando.Dto.TipoVariedadId == null)
            {
                automatismo.MaterialId = automatismoEditado.Material.Id;
                automatismo.TipoVariedadId = comando.Dto.TipoVariedadId;
            }
            automatismo.EsPasoDirecto = comando.Dto.Llamado1a1 ? false : true;
            automatismo.Hidraulicas = hidraulicas;
        }

        protected override void Validar(ModificarAutomatismoGranos comando, Resultado resultado)
        {
            if (!Repositorio.Existe<AutomatismoGrano>(a => a.Id == comando.Dto.Id))
            {
                resultado.Error("Id Automatismo", Textos.Automatismo_IdExistente);
            }

            if (Repositorio.Existe<AutomatismoGrano>(a => a.MaterialId == comando.Dto.MaterialId && a.TipoVariedadId == comando.Dto.TipoVariedadId && a.AplicaFiltroCalidad == comando.Dto.AplicaFiltroCalidad && a.CamionEscalable == comando.Dto.CamionEscalable))
            {
                resultado.Error("Id Variedad , Id Material, AplicaFiltroCalidad, CamionEscalable", Textos.Automatismo_IdExistente);
            }
        }
    }
}