using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public abstract class ProcesadorComandoImpresion<TComandoImpresion> 
        : ProcesadorImpresionAsync<TComandoImpresion> where TComandoImpresion : ComandoImpresion
    {
        protected ProcesadorComandoImpresion(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioImpresorFactory servicioImpresorFactory)
            : base(repositorio, conversor, log, servicioImpresorFactory)
        {
        }

        protected abstract Func<MaterialPorWorkflow, bool> PropiedadConfiguracionDebeImprimir { get; }

        protected override bool DoDebeImprimir(TComandoImpresion comando)
        {
            bool debeImprimir = DebeImprimirPorDefecto;

            if (comando != null && comando.Dto != null)
            {
                var materialPorWorkflow = this.ObtenerMaterialPorWorkflow(comando.Dto);

                if (materialPorWorkflow != null && this.PropiedadConfiguracionDebeImprimir != null)
                    debeImprimir = this.PropiedadConfiguracionDebeImprimir(materialPorWorkflow);
            }

            return debeImprimir;
        }

        /// <summary>
        /// Método para obtener la configuracion de impresion a utilizar en DebeImprimir
        /// </summary>
        protected virtual MaterialPorWorkflow ObtenerMaterialPorWorkflow(IDtoConCentroIdMaterialIdWorkflowId dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "El dto no puede ser nulo.");

            MaterialPorWorkflow materialPorWorkflow = null;
            Log.Debug($"Parámetros: materialId={dto.MaterialId}, workflowId{dto.WorkflowId}, centroId={dto.CentroId}");

            try
            {
                var recorrido =
                    Repositorio.Obtener(
                        new List<Expression<Func<Recorrido, object>>> { x => x.Workflow },
                        r => r.InstanciaWorkflow == dto.WorkflowId);

                if (recorrido != null && recorrido.Workflow != null)
                {
                    materialPorWorkflow =
                       Repositorio.Obtener<MaterialPorWorkflow>(m =>
                           m.Material.Id == dto.MaterialId &&
                           m.Workflow.Id == recorrido.Workflow.Id &&
                           m.Centro.Id == dto.CentroId &&
                           m.Cliente == null);
                }
                else
                    Log.Debug($"No se puedo obtener recorrido o el workflow del recorrido es nulo");
            }
            catch (Exception ex)
            {
                Log.Error(
                    ex,
                    "Error al obtener configuración de impresión en MaterialPorWorkflow con los parametros: materialId={0}, workflowGuid={1}, centroId={2}.\n" +
                    "Posibles causas: \n" +
                    "1. Revisar que no exista más de una configuración para el mismo material, workflow y centro (el cliente no es considerado en este caso)\n" +
                    "2. Recorrido o workflow inexistente o erróneo",
                    dto.MaterialId,
                    dto.WorkflowId,
                    dto.CentroId);
            }

            return materialPorWorkflow;
        }
    }
}
