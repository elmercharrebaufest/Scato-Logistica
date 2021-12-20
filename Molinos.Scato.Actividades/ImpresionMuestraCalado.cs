using System;
using System.Activities;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionMuestraCalado : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<CaladoDto> Calado { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroDeOrden { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroCartaPorte { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        [RequiredArgument]
        public InArgument<string> NombreUsuario { get; set; }
        [RequiredArgument]
        public InArgument<string> PesoNeto { get; set; }
        [RequiredArgument]
        public InArgument<string> Procedencia { get; set; }
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
            var calado = Calado.Get<CaladoDto>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var numeroDeOrden = NumeroDeOrden.Get<string>(context);
            var numeroCartaPorte = NumeroCartaPorte.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var nombreUsuario = NombreUsuario.Get<string>(context);
            var pesoNeto = PesoNeto.Get<string>(context);
            var procedencia = Procedencia.Get<string>(context);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Muestra Calado",
                    ActividadXaml = "ImpresionMuestraCalado",
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
                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }

                var dto = new ImpIdentificacionMuestraCaladoDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Centro = documento.CentroDescripcion,
                    Codigo = codigo,
                    NumeroDeOrden = numeroDeOrden,
                    Patente = patente,
                    FechaCalado = calado.FechaCreacion.HasValue ? calado.FechaCreacion.Value : DateTime.MinValue,
                    Humedad = calado != null ? calado.CaladosPorCaracteristica.Where(x => x.EsHumedad).Select(x => x.ValorCalado).FirstOrDefault().ToString() : "",
                    NombreUsuario = nombreUsuario,
                    NumeroCartaPorte = numeroCartaPorte,
                    PesoNeto = pesoNeto,
                    Procedencia = procedencia,
                    WorkflowId = workflowId
                };

                resultado = servicio.Ejecutar(new ImprimirMuestraCalado { Dto = dto, CantidadCopias = cantCopias });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionMuestraCalado", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
