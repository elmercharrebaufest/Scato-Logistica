using System;
using System.Activities;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionLoteACamara : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> MuestraId { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
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

            var muestraId = MuestraId.Get<int>(context);
            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var calado = Calado.Get<CaladoDto>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var numeroDeOrden = NumeroDeOrden.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Lote A Camara",
                    ActividadXaml = "ImpresioLoteACamara",
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
                var muestra = repositorio.ObtenerNumeroMuestraEnvioACamara(muestraId);
                if (!string.IsNullOrEmpty(muestra))
                {
                    var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                    if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }
                    var precintos = repositorio.ListarPrecintos(calado.WorkflowInstanceId);

                    var dto = new ImpIdentificacionEnvioLoteACamaraDto
                    {
                        Impresora = documento.ImpresoraDireccion ?? "",
                        Centro = documento.CentroDescripcion,
                        Codigo = codigo,
                        NumeroDeOrden = numeroDeOrden,
                        Patente = patente,
                        FechaCalado = calado.FechaCreacion.HasValue ? calado.FechaCreacion.Value : DateTime.MinValue,
                        NumeroDeMuestra = muestra,
                        Precinto = precintos.Count == 0 ? " – S/N" : (String.Join(";", precintos.Select(x => x.Detalle))),
                        WorkflowId = workflowId,
                    };

                    resultado = servicio.Ejecutar(new ImprimirEnvioLoteACamara { Dto = dto, CantidadCopias = cantCopias });
                }
                
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresioLoteACamara", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
