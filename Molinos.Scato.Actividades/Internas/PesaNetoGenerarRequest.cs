using System;
using System.Activities;
using System.Configuration;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class PesaNetoGenerarRequest : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> NumeroDocumento { get; set; }
        [RequiredArgument]
        public InArgument<decimal> PesoBruto { get; set; }
        [RequiredArgument]
        public InArgument<decimal> PesoNeto { get; set; }
        public OutArgument<PesaNetoRequest> Request { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            PesaNetoRequest request = null;
            try
            {
                request = new PesaNetoRequest
                    { 
                        PesaNeto = new PesaNeto
                            {
                                Entrega = NumeroDocumento.Get<string>(context),
                                PesoBruto = PesoBruto.Get<Decimal>(context),
                                PesoNeto = PesoNeto.Get<Decimal>(context),
                                PesoBrutoSpecified = true,
                                PesoNetoSpecified = true
                            }
                    };


                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                    {
                        var srv = context.GetExtension<IServicioComandos>();
                        srv.Ejecutar(new CrearControlRecorrido
                            {
                                Dto = new ControlRecorridoDto
                                    {
                                        Actividad = "PesaNetoGenerarRequest",
                                        Fecha = DateTime.Now,
                                        Comentario = request.ToXml(),
                                        NombreUsuario = "",
                                        WorkflowInstanceId = context.WorkflowInstanceId,
                                    }
                            });
                    }
                }
                catch
                {
                }
                
            }
            catch (Exception e)
            {
                resultado.Errores.Add("", e.Message);
            }
            Request.Set(context,request);
            Resultado.Set(context, resultado);
        }
    }
}
