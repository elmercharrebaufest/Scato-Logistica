using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarCampaniaPorCuit : IConsulta<StockDeEstablecimientoDto>
    {
        private readonly string cuit;
        private readonly string cosecha;

        public ListarCampaniaPorCuit(string cuit,string cosecha)
        {
            this.cuit = cuit;
            this.cosecha = cosecha;
        }

        public List<StockDeEstablecimientoDto> Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
           

            var resultado = (from stockEstablecimeinto in contexto.Set<StockDeEstablecimiento>()
                            join estab in contexto.Set<Establecimiento>() on stockEstablecimeinto.CodigoEstablecimiento equals estab.CodigoDeEstablecimiento
                            where estab.Proveedor.Cuil == cuit && stockEstablecimeinto.Cosecha == cosecha                           
                            select new StockDeEstablecimientoDto
                            {
                                Id = stockEstablecimeinto.Id,
                                CodigoEstablecimiento = stockEstablecimeinto.CodigoEstablecimiento,
                                Cosecha = stockEstablecimeinto.Cosecha,
                                FechaDesde = stockEstablecimeinto.FechaDesde,
                                FechaHasta = stockEstablecimeinto.FechaHasta,
                                StockDeclarado = stockEstablecimeinto.StockDeclarado,
                                StockUtilizado = contexto.Set<RegistroStockEPA>()
                                .Where(x => x.CodigoEstablecimiento == stockEstablecimeinto.CodigoEstablecimiento &&
                                    x.Cosecha == stockEstablecimeinto.Cosecha).Any() ? contexto.Set<RegistroStockEPA>()
                                .Where(x => x.CodigoEstablecimiento == stockEstablecimeinto.CodigoEstablecimiento &&
                                    x.Cosecha == stockEstablecimeinto.Cosecha)
                                .Sum(x => x.PesoNeto) : 0 + (contexto.Set<RegistroStockOtrosPuertos>()
                                .Where(r => r.CodigoEstablecimiento == stockEstablecimeinto.CodigoEstablecimiento && 
                                    r.Cosecha == stockEstablecimeinto.Cosecha).Any() ? contexto.Set<RegistroStockOtrosPuertos>()
                                .Where(r => r.CodigoEstablecimiento == stockEstablecimeinto.CodigoEstablecimiento &&  r.Cosecha == stockEstablecimeinto.Cosecha).Sum(r => r.PesoNeto) : 0),
                                StockReservado = stockEstablecimeinto.StockReservado,
                                NombreEstablecimiento = estab.NombreDeEstablecimiento
                            }                               
                           
                ).ToList();

            return resultado;
        }
    }
}
