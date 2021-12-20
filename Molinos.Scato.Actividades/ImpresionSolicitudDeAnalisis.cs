using System;
using System.Activities;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionSolicitudDeAnalisis : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<CaladoDto> Calado { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroDeOrden { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        public InArgument<int?> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var materialId = MaterialId.Get<int>(context);
            var calado = Calado.Get<CaladoDto>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var numeroDeOrden = NumeroDeOrden.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Solicitud De Analisis",
                    ActividadXaml = "ImpresionSolicitudDeAnalisis",
                    WorkflowInstanceId = workflowId,
                    Fecha = DateTime.Now
                };
            try
            {
                resultado = servicio.Ejecutar(new CrearLogActividad { Dto = logActividad });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }
            
            try
            {
                var material = repositorio.ObtenerMaterial(materialId);

                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }


                var dto = new ImpSolicitudDeAnalisisDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Centro = documento.CentroDescripcion,
                    Codigo = codigo,
                    Material = material.Descripcion,
                    NumeroDeOrden = numeroDeOrden,
                    Patente = patente,
                    CaracteristicaDeCalidad = calado.CaladosPorCaracteristica.Where(x => x.AnalisisPreliminar).Select(x => x.Caracteristica).ToList(),
                    FechaCalado = calado.FechaCreacion.HasValue ? calado.FechaCreacion.Value : DateTime.MinValue,
                    NumeroAnalisis = calado.Id.ToString(CultureInfo.InvariantCulture),
                    WorkflowId = workflowId,
                };

                resultado = servicio.Ejecutar(new ImprimirSolicitudDeAnalisis { Dto = dto, CantidadCopias = cantCopias });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionSolicitudDeAnalisis", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
