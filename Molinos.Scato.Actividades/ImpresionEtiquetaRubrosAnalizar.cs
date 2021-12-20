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
    public class ImpresionEtiquetaRubrosAnalizar : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroCartaPorte { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        [RequiredArgument]
        public InArgument<string> NombreUsuario { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        public InArgument<int?> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }
        [RequiredArgument]
        public InArgument<string> Material { get; set; }
        [RequiredArgument]
        public InArgument<CaladoDto> Calado { get; set; }
        
        
        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);

            var codigo = CodigoDeImpresion.Get<string>(context);
            var numeroCartaPorte = NumeroCartaPorte.Get<string>(context);

            var caladoDto = Calado.Get<CaladoDto>(context);

            var patente = Patente.Get<string>(context);
            var nombreUsuario = NombreUsuario.Get<string>(context);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var logActividad = new LogActividadDto
            {
                Actividad = "Impresion Etiqueta Rubros Analizar",
                ActividadXaml = "ImpresionEtiquetaRubrosAnalizar",
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


            string analisisSeleccionados = string.Empty;
            if (caladoDto != null)
            {
                var caladosPorCaracteristica = caladoDto.CaladosPorCaracteristica.Where(c => c.AnalisisPreliminar);
                if (caladosPorCaracteristica.Any())
                {
                    foreach (var item in caladosPorCaracteristica)
                    {

                        analisisSeleccionados += item.CaracteristicaDescripcionCorta + ";";
                    }

                    try
                    {
                        var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                        if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }

                        var dto = new ImpEtiquetaRubrosAnalizarDto
                        {
                            Impresora = documento.ImpresoraDireccion ?? "",
                            Centro = documento.CentroDescripcion,
                            Codigo = codigo,
                            Patente = patente,
                            NumeroCartaPorte = numeroCartaPorte,
                            NombreUsuario = nombreUsuario,
                            WorkflowId = workflowId,
                            AnalisisSeleccionados = analisisSeleccionados
                        };

                        resultado = servicio.Ejecutar(new ImprimirEtiquetaRubrosAnalizar { Dto = dto, CantidadCopias = cantCopias });
                    }
                    catch (Exception e)
                    {
                        resultado.Errores.Add("1", e.Message);
                    }
                }
            }
            
            return resultado;
        }

    }
}
