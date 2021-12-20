using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class TransmisionesCuposErrorConsulta : IConsulta<InformarCupoTransmisionASap>
    {
        private readonly FiltroPanelDeTransaccionesSapDto filtro;


        public TransmisionesCuposErrorConsulta(FiltroPanelDeTransaccionesSapDto filtro)
        {
            this.filtro = filtro;
        }

        public List<InformarCupoTransmisionASap> Ejecutar(DbContext contexto)
        {

            IQueryable<InformarCupoTransmisionASap> resultados = from tra in contexto.Set<InformarCupoTransmisionASap>()
                                                        join rec in contexto.Set<Recorrido>() on tra.InstanciaWorkflow equals rec.InstanciaWorkflow
                                                                 where filtro.CentroId == rec.Centro.Id && ((!filtro.EstadoTransmisionASap.HasValue || filtro.EstadoTransmisionASap == EstadoTransmisionASap.Error) && EstadoTransmisionASap.Error == tra.Estado)
                                                        && (tra.Fecha >= filtro.FechaDesde && tra.Fecha <= filtro.FechaHasta) && rec.NumeroDocumentoIngreso.Contains(filtro.NumeroDocumentoIngreso)
                                                        && rec.Patente.Contains(filtro.Patente) && (!filtro.TipoDocumentoIngreso.HasValue || filtro.TipoDocumentoIngreso == rec.TipoDocumentoIngreso)
                                                        && tra.FuncionSap == FuncionSAP.InformarCupo
                            select tra;
            
            return resultados.ToList();
        }
    }
}
