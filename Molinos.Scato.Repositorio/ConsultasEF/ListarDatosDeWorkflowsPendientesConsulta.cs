using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarDatosDeWorkflowsPendientesConsulta : IConsulta<InstanciaWorkflowDto>
    {
        private readonly int centroId;
        private readonly int cantidad;

        public ListarDatosDeWorkflowsPendientesConsulta(int centroId, int cantidad)
        {
            this.centroId = centroId;
            this.cantidad = cantidad;
        }

        public List<InstanciaWorkflowDto> Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var accion = PermisosScato.CamionesPendientesMesa.Text();
            var accionNoGranos = PermisosScato.CamionesPendientesNoGranos.ToString();
            var fechaMinima = DateTime.Now.AddDays(-1);
            IQueryable<InstanciaWorkflowDto> resultado = (from x in contexto.Set<CargaDeCupo>()
                             
                             where x.Recorrido == null && x.Centro.Id == centroId && x.Fecha > fechaMinima && x.Cupo != null && x.Numero != null && !x.EnProgresoAutomatico
                             select new InstanciaWorkflowDto
                             {
                                 Patente = x.Patente,
                                 ProximaAccion = x.Material != null && x.Material.EsGrano ? accion : accionNoGranos,
                                 //Workflow = ,
                                 MaterialCodigoSap = x.Material != null ? x.Material.CodigoSAP : string.Empty,
                                 Material = x.Material != null ? x.Material.Descripcion : string.Empty,
                                 TipoDocumentoDeIngreso = TipoDocumentoIngreso.CartaPorte,
                                 NumeroDocumentoDeIngreso = x.NumeroCartaPorte,
                                 //CentroCodigoSap = ,
                                 NumeroDeTarjeta = x.Numero,
                                 CentroId = x.Centro.Id,
                                 //Centro = ,
                                 MaterialId = x.Material != null ? x.Material.Id : 0,
                                 //Codigo = , 
                                 Id = Guid.Empty,
                                 RecorridoId = x.Id,
                                 LlegoEnHorario = x.LlegoEnHorario,
                                 FechaCreacion = x.Fecha,
                                 FechaUltimaModificacion = x.Fecha,
                                 PuestoDeTrabajo = x.PuestoDeTrabajo.NombrePuesto, 
                                 Reingreso = x.Reingresado,
                                 Calle = contexto.Set<CallePorRecorrido>().Where(y => y.CargaDeCupo.Id == x.Id).FirstOrDefault().Calle.Nombre,
                                 NoGranos = x.Material== null || !x.Material.EsGrano,
                                 CPE = x.CPE,
                                 CTG = x.CTG,
                                 FleteMOA = x.FleteMOA
                             }).OrderByDescending(x => x.FechaCreacion);
    
            if(cantidad > 0)
            {
                resultado = resultado.Take(cantidad);
            }
            return resultado.ToList();
        }
    }
}
