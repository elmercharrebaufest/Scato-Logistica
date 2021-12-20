using System;
using System.Activities;
using System.Configuration;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class LlegadaADestinoEnRedespachosGenerarRequest : CodeActivity
    {
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<string> Documento { get; set; }
        [RequiredArgument]
        public InArgument<int> Ejercicio { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaContab { get; set; }
        public InArgument<string> DocumentoInternoSap { get; set; }

        public OutArgument<Mov305Request> Request { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            Mov305Request request = null;
            try
            {
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var esCPE = srvRepositorio.ObtenerSiEsCPEporWf(InstanceId.Get<Guid>(context));
                var numeroDocumento = Documento.Get<string>(context);
                var ejercicio = Ejercicio.Get<int>(context);
                var fechaContab = FechaContab.Get<DateTime>(context);
                var documentoInternoSap = DocumentoInternoSap.Get<string>(context);

                if (esCPE.HasValue && esCPE.Value)
                {
                    if (numeroDocumento.PadLeft(12, '0').IndexOf("-", StringComparison.Ordinal) == -1)
                    {
                        numeroDocumento = numeroDocumento.PadLeft(12, '0').Substring(0, 4) + "-" + numeroDocumento.PadLeft(12, '0').Substring(4, 8);
                    }
                } else if (numeroDocumento.Length == 12 && numeroDocumento[4] != '-')
                {
                        numeroDocumento = numeroDocumento.Substring(0, 4) + "-" + numeroDocumento.Substring(4, 8);
                }

                request = new Mov305Request(new Mov305
                {
                    Documento = documentoInternoSap ?? "",
                    DocLegal = numeroDocumento,
                    Ejercicio = String.IsNullOrEmpty(documentoInternoSap) ? "" : ejercicio.ToString(CultureInfo.InvariantCulture),
                    FechaContab = fechaContab.ToString("yyyy-MM-dd")
                });


                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                    {
                        var srv = context.GetExtension<IServicioComandos>();
                        srv.Ejecutar(new CrearControlRecorrido
                            {
                                Dto = new ControlRecorridoDto
                                    {
                                        Actividad = "LlegadaADestinoEnRedespachosGenerarRequest",
                                        Fecha = DateTime.Now,
                                        Comentario = request.ToXml(),
                                        NombreUsuario = "",
                                        WorkflowInstanceId = context.WorkflowInstanceId,
                                    }
                            });
                    }
                }
                catch (Exception e)
                {
                    resultado.Errores.Add("ControlRecorrido", e.Message);
                }
                
            }
            catch (Exception e)
            {
                resultado.Errores.Add("", e.Message);

            }
            Request.Set(context, request);
            Resultado.Set(context, resultado);
        }
    }
}
