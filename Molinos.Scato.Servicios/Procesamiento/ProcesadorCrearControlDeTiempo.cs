using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearControlDeTiempo : ProcesadorCrear<CrearControlDeTiempo, ControlDeTiempo>
    {
        public ProcesadorCrearControlDeTiempo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ControlDeTiempo CrearEntidad(CrearControlDeTiempo comando)
        {
            return new ControlDeTiempo
            {
                Workflow = Repositorio.Obtener<Workflow>(x => x.Id == comando.Dto.WorkflowId),
                CodigoControl = comando.Dto.CodigoControl,
                ActividadDesde = comando.Dto.ActividadDesde,
                ActividadHasta = comando.Dto.ActividadHasta,
                TiempoMaximo = comando.Dto.TiempoMaximo,
            };
        }

        protected override void Validar(CrearControlDeTiempo comando, Resultado resultado)
        {
            if (comando.Dto.WorkflowId == 0)
            {
                resultado.Error("WorkflowId", String.Format(Textos.Error_Requerido, Textos.Workflow));
            }
            else if (Repositorio.Existe<ControlDeTiempo>(x => comando.Dto.ActividadDesde == comando.Dto.ActividadHasta))
            {
                resultado.Error(string.Empty, Textos.ControlDeTiempo_Iguales);
            }
            else if (Repositorio.Existe<ControlDeTiempo>(x => x.ActividadDesde == comando.Dto.ActividadDesde && x.ActividadHasta == comando.Dto.ActividadHasta && x.Workflow.Id == comando.Dto.WorkflowId))
            {
                resultado.Error(string.Empty, Textos.ControlDeTiempo_Existente);
            }
            else if (Repositorio.Existe<ControlDeTiempo>(x => x.CodigoControl == comando.Dto.CodigoControl && x.Workflow.Id == comando.Dto.WorkflowId))
            {
                resultado.Error(string.Empty, Textos.ControlDeTiempo_CodigoExistente);
            }
        }
    }
}
