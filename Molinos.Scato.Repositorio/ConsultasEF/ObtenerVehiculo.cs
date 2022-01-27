using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Transactions;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerVehiculo : IConsultaEscalar<CategoriaVehiculoDto>
    {
        private readonly string patente;
        private readonly string acoplado;
        private readonly string acoplado2;
        public ObtenerVehiculo(string patente, string acoplado, string acoplado2)
        {
            this.patente = patente;
            this.acoplado = acoplado;
            this.acoplado2 = acoplado2;
        }

        public virtual CategoriaVehiculoDto Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var query = contexto.Set<CategoriaVehiculo>().AsQueryable();

                if (!string.IsNullOrWhiteSpace(this.acoplado2))
                    query = query.Where(q => q.PatenteAcoplado2.Equals(this.acoplado2));
                else
                    query = query.Where(q => (q.PatenteAcoplado2 == null || q.PatenteAcoplado2 == string.Empty));

                if (!string.IsNullOrWhiteSpace(this.acoplado))
                    query = query.Where(q => q.PatenteAcoplado.Equals(this.acoplado));
                else
                    query = query.Where(q => (q.PatenteAcoplado == null || q.PatenteAcoplado == string.Empty));

                if (!string.IsNullOrWhiteSpace(this.patente))
                    query = query.Where(q => q.Patente.Equals(this.patente));

                var result = query.Select(x => new CategoriaVehiculoDto()
                {
                    Id = x.Id,
                    Patente = x.Patente,
                    PatenteAcoplado = x.PatenteAcoplado,
                    PatenteAcoplado2 = x.PatenteAcoplado2,
                    TipoVehiculo = x.TipoVehiculo
                }).FirstOrDefault();

                return result;
            }
        }
    }
}
