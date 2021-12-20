using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarBajaCTGConErrorConsulta : IConsultaEscalar<IList<BajaCTGRetransmisionDto>>
    {
        private readonly int centro;
        public ListarBajaCTGConErrorConsulta(int centro)
        {
            this.centro = centro;
        }

        public IList<BajaCTGRetransmisionDto> Ejecutar(DbContext contexto)
        {

            var previos = from tra in contexto.Set<BajaCTG>()
                        join rec in contexto.Set<Recorrido>() on tra.WorkflowId equals rec.InstanciaWorkflow
                        where rec.Centro.Id == this.centro &&
                        (tra.CodigoDeBaja == null || 
                        (tra.CodigoDeBajaDefinitivo == null && rec.Terminado == true)) && 
                        !rec.Rechazado && 
                        tra.Fecha >= (DateTime)EntityFunctions.AddDays(DateTime.Now,-1)
                        group new { tra, rec } by tra.WorkflowId into g
                        select new
                        {
                            bajas = g.FirstOrDefault(x => x.tra.Id == g.Max(y => y.tra.Id)),

                        };
            IQueryable<BajaCTGRetransmisionDto> resultados = previos.Select(x => new BajaCTGRetransmisionDto
            {
                CentroId = x.bajas.rec.Centro.Id,
                WorkflowId = x.bajas.tra.WorkflowId,
                Dto = new CartaPorteDto {
                    Id = x.bajas.tra.CartaPorte.Id,
                    NroCartaPorte = x.bajas.tra.CartaPorte.NroCartaPorte,
                    CTG = x.bajas.tra.CartaPorte.CTG,
                    Chofer = new ChoferDto
                    {
                        Cuil = x.bajas.tra.CartaPorte.Chofer.Cuil
                    },
                    TransportistaId = x.bajas.tra.CartaPorte.Transportista.Id,
                    Cpe = x.bajas.tra.CartaPorte.Cpe.HasValue && x.bajas.tra.CartaPorte.Cpe.Value,
                    TitularCartaPorteCuil = x.bajas.tra.CartaPorte.TitularCartaPorte.Cuil,
                    TipoVehiculo = x.bajas.tra.CartaPorte.TipoVehiculo
                },
                Vehiculo = new VehiculoDto
                {
                    Id = x.bajas.rec.Vehiculo.Id,
                    Patente = x.bajas.rec.Patente,
                    PesoNetoOrigen = x.bajas.rec.PesoTaraOrigen
                },
                EstadoCtg = (x.bajas.tra.CodigoDeBaja == null) ? EstadoTransmisionASap.Error : EstadoTransmisionASap.Correcto,
                EstadoCtgDefinitivo = (x.bajas.tra.CodigoDeBajaDefinitivo == null) ? (x.bajas.rec.Terminado ? EstadoTransmisionASap.Error : EstadoTransmisionASap.Pendiente) : EstadoTransmisionASap.Correcto,
            });

            return resultados.ToList();
        }
    }
}
