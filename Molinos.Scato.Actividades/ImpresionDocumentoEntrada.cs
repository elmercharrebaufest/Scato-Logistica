using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionDocumentoEntrada : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroDeIngreso { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        public InArgument<DateTime?> FechaEmision { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var numeroDeIngreso = NumeroDeIngreso.Get<string>(context);
            var fechaEmision = FechaEmision.Get<DateTime>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Documento Ingreso",
                    ActividadXaml = "ImpresionDocumentoEntrada",
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

            var centro = repositorio.ObtenerCentro(centroId);
            
            try
            {
                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                var impresora = repositorio.ObtenerImpresora(documento.ImpresoraId);
                var dto = new ImpDocumentoDeEntradaDto
                {
                    Impresora = impresora != null ? impresora.Direccion : "",
                    Centro = centro.Descripcion,
                    Codigo = codigo,
                    Patente = patente,
                    WorkflowId = workflowId,
                    Fecha = DateTime.Now,
                    FechaImpresion = DateTime.Now,
                    NumeroDeIngreso = numeroDeIngreso,
                    FechaDocumentoDeIngreso = fechaEmision
                };

                resultado = servicio.Ejecutar(new ImprimirDocumentoDeEntrada() { Dto = dto });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionDocumentoEntrada", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
