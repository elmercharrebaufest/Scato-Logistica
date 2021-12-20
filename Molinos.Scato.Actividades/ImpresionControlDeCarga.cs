using System;
using System.Activities;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionControlDeCarga : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaDocumentoDeIngreso { get; set; }
        public InArgument<decimal?> PesoBruto { get; set; }
        public InArgument<decimal?> PesoTara { get; set; }
        public InArgument<decimal?> PesoNeto { get; set; }
        public InArgument<decimal?> TotalDescargado { get; set; }
        public InArgument<decimal?> Diferencia { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();
            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var fechaDocumento = FechaDocumentoDeIngreso.Get<DateTime>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            var pesoBruto = PesoBruto.Get<decimal?>(context);
            var pesoTara= PesoTara.Get<decimal?>(context);
            var pesoNeto = PesoNeto.Get<decimal?>(context);
            var totalDescargado = Diferencia.Get<decimal?>(context);
            var diferencia = TotalDescargado.Get<decimal?>(context);

            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Control de Carga",
                    ActividadXaml = "ImpresionControlDeCarga",
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

                if (documento == null)
                {
                    resultado.Errores.Add("1", Textos.Impresion_DocumentoDeImpresionPorCentroCodigoError);
                }
                else
                {
                    var impresora = repositorio.ObtenerImpresora(documento.ImpresoraId);
                    var dto = new ImpControlDeCargaDto()
                    {
                        Impresora = impresora != null ? impresora.Direccion : "",
                        Centro = centro.Descripcion,
                        Codigo = codigo,
                        Patente = patente,
                        WorkflowId = workflowId,
                        FechaImpresion = DateTime.Now,
                        FechaDocumentoDeIngreso = fechaDocumento,
                        NumeroControl = centro.CodigoSAP ?? "" + "-" + repositorio.ObtenerNumeroControlDeCargaGenerado().ToString(CultureInfo.InvariantCulture).PadLeft(8, '0'),
                        PesoBruto = pesoBruto,
                        PesoNeto = pesoNeto,
                        PesoTara = pesoTara,
                        Diferencia = diferencia,
                        TotalDescargado = totalDescargado
                    };
                    resultado = servicio.Ejecutar(new ImprimirControlDeCarga { Dto = dto });
                }

            }
            catch (Exception e)
            {
                resultado.Errores.Add("2", e.Message);
            }
            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionControlDeCarga", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("3", Textos.FinDeActividad_ErrorEnLaCarga);
            }
            return resultado;
        }
    }
}
