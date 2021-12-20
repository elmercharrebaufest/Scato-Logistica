using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarMuestrasDeHumedad : IConsulta<MuestraDeHumedadDto>
    {
        private readonly int? centroId;
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int? humedimetroId;

        public ListarMuestrasDeHumedad(int? centroId,int? humedimetroId, DateTime fechaDesde, DateTime fechaHasta )
        {
            this.centroId = centroId;
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
            this.humedimetroId = humedimetroId;
        }

        private static List<MuestraDeHumedadDto> ListadoDeMuestrasDeHumedad(DbContext contexto, int? centroId,int? humedimetroId, DateTime fechaDesde, DateTime fechaHasta)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var resultado = from muestradehumedad in contexto.Set<MuestraDeHumedad>()
                            join r in contexto.Set<Recorrido>().DefaultIfEmpty() on muestradehumedad.WorkflowInstanceId equals r.InstanciaWorkflow into remitoJoined
                            from r in remitoJoined.DefaultIfEmpty()
                            where (centroId == null || centroId == 0 || muestradehumedad.Centro.Id == centroId) &&
                                  (fechaDesde <= muestradehumedad.Fecha && muestradehumedad.Fecha <= fechaHasta)&&
                                  (humedimetroId == null || humedimetroId == 0 || muestradehumedad.Humedimetro.Id == humedimetroId)

                            orderby muestradehumedad.Fecha

                            select new
                                {
                                    Centro = muestradehumedad.Centro.CodigoSAP,
                                    Humedimetro = muestradehumedad.Humedimetro.Descripcion,
                                    muestradehumedad.Modalidad,
                                    muestradehumedad.NumeroOrden,
                                    NumeroDocumentoIngreso = muestradehumedad.NumeroDocumentoIngreso.Substring(0,4) + "-" + muestradehumedad.NumeroDocumentoIngreso.Substring(4,8),
                                    muestradehumedad.CicloDeCalado,
                                    muestradehumedad.NroDeToma,
                                    muestradehumedad.ValorLeido,
                                    muestradehumedad.ValorFinal,
                                    muestradehumedad.Fecha,
                                    muestradehumedad.Usuario,
                                    muestradehumedad.MotivoHumedadManual,
                                    r.Material.Descripcion
                                };

            return resultado.ToList().Select(muestradehumedad => new MuestraDeHumedadDto
                {
                    Centro = muestradehumedad.Centro,
                    Humedimetro = muestradehumedad.Humedimetro,
                    Modalidad = muestradehumedad.Modalidad,
                    NumeroOrden = muestradehumedad.NumeroOrden,
                    NumeroDocumentoIngreso = muestradehumedad.NumeroDocumentoIngreso,
                    CicloDeCalado = muestradehumedad.CicloDeCalado,
                    Fecha = muestradehumedad.Fecha,
                    NroDeToma = muestradehumedad.NroDeToma,
                    ValorLeido = muestradehumedad.ValorLeido,
                    ValorFinal = muestradehumedad.ValorFinal,
                    Usuario = muestradehumedad.Usuario,
                    MotivoHumedadManualDesc = muestradehumedad.MotivoHumedadManual != null ?  muestradehumedad.MotivoHumedadManual.Descripcion : "",
                    Material = muestradehumedad.Descripcion
            }).ToList();
                    
                    
        }

        public virtual List<MuestraDeHumedadDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return ListadoDeMuestrasDeHumedad(contexto, centroId,humedimetroId, fechaDesde, fechaHasta);
            }
        }
    }
}
