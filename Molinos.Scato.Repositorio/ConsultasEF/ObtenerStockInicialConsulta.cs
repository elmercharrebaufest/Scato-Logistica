using System;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerStockInicialConsulta : IConsultaEscalar<decimal?>
    {
        private readonly DateTime fecha;
        private readonly int materialId;
        private readonly int centroId;

        public ObtenerStockInicialConsulta(DateTime fecha, int materialId, int centroId)
        {
            this.fecha = fecha;
            this.materialId = materialId;
            this.centroId = centroId;
        }

        public static decimal? CalcularStockInicial(DbContext contexto, DateTime fecha, int materialId, int centroId)
        {
            var stockIngreso = contexto.Set<Recorrido>().Where(x => x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.Material.Id == materialId
                && x.PesoTaraFecha < fecha && x.Workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso && x.Centro.Id == centroId && x.Terminado && !x.Rechazado)
                .Sum(x => (long?)(x.PesoBruto - x.PesoTara - x.DescuentoEnKgOncca)) ?? 0;            

            var stockEgreso = contexto.Set<Recorrido>().Where(x => x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.Material.Id == materialId && x.PesoBrutoFecha < fecha && x.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso && x.Centro.Id == centroId && x.Terminado && !x.Rechazado)
                .Sum(x => (long?)(x.PesoBruto - x.PesoTara - x.DescuentoEnKgOncca)) ?? 0;

            var stockAjuste = contexto.Set<AjusteDeStock>().Where(x => x.Material.Id == materialId && x.Fecha < fecha && x.Centro.Id == centroId)
                .Sum(x => x.PesoNetoIngreso - x.PesoNetoEgreso) ?? 0;

            return (stockIngreso - stockEgreso + stockAjuste);
        }

        public virtual decimal? Ejecutar(DbContext contexto)
        {
            return CalcularStockInicial(contexto, fecha, materialId, centroId);
        }
    }

    public class Merma
    {
        public decimal DtoMermaSaldoInicial { get; set; }
    }
}
