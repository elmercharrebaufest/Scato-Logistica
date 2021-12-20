using System;
using System.Activities;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public sealed class ConsultarDatosMailStock : CodeActivity
    {
        public InArgument<Guid> WorkflowId { get; set; }
        public OutArgument<string> CodigoEstablecimiento { get; set; }
        public OutArgument<string> RazonesSociales { get; set; }
        public OutArgument<string> Corredores { get; set; }
        public OutArgument<string> Intermediarios { get; set; }
        public OutArgument<string> Remitentes { get; set; }
        public OutArgument<string> NombreEstablecimiento { get; set; }
        public OutArgument<string> Provincia { get; set; }
        public OutArgument<string> Localidad { get; set; } 
        public OutArgument<string> Cosecha { get; set; }
        public OutArgument<string> StockUtilizado { get; set; }
        public OutArgument<string> StockDeclarado { get; set; }
        public OutArgument<string> StockDisponible { get; set; }
        public OutArgument<bool> SeEnviaMail { get; set; } 
        public OutArgument<Resultado> Resultado { get; set; } 

        protected override void Execute(CodeActivityContext context)
        {
            var workflowId = WorkflowId.Get<Guid>(context);
            
            try{
                var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
                var dto = servicioRepositorio.ObtenerDatosMailAvisoStock(workflowId);
                if (dto.Error == null)
                {
                    CodigoEstablecimiento.Set(context, dto.CodigoDeEstablecimiento);
                    NombreEstablecimiento.Set(context, dto.NombreDeEstablecimiento);
                    Provincia.Set(context, dto.Provincia);
                    Localidad.Set(context, dto.Localidad);
                    Cosecha.Set(context, dto.Cosecha);
                    StockDeclarado.Set(context, dto.StockDeclarado.ToString(CultureInfo.CurrentCulture));
                    StockDisponible.Set(context, dto.StockDisponible.ToString(CultureInfo.CurrentCulture));
                    StockUtilizado.Set(context, dto.StockUtilizado.ToString(CultureInfo.CurrentCulture));
                    SeEnviaMail.Set(context, dto.SeEnviaMail);
                    string razones = "";
                    for (int i = 0; i < dto.RazonesSociales.Count; i++)
                    {
                        if (i == 0)
                        {
                            razones = dto.RazonesSociales[i];
                        }
                        else
                        {
                            razones = razones + ", " + dto.RazonesSociales[i];
                        }
                    }
                    RazonesSociales.Set(context, razones);

                    string corredores = "";
                    for (int i = 0; i < dto.Corredores.Count; i++)
                    {
                        if (i == 0)
                        {
                            corredores = dto.Corredores[i];
                        }
                        else
                        {
                            corredores = corredores + ", " + dto.Corredores[i];
                        }
                    }
                    Corredores.Set(context, corredores);

                    string intermediarios = "";
                    for (int i = 0; i < dto.Intermediarios.Count; i++)
                    {
                        if (i == 0)
                        {
                            intermediarios = dto.Intermediarios[i];
                        }
                        else
                        {
                            intermediarios = intermediarios + ", " + dto.Intermediarios[i];
                        }
                    }
                    Intermediarios.Set(context, intermediarios);

                    string remitentes = "";
                    for (int i = 0; i < dto.Remitentes.Count; i++)
                    {
                        if (i == 0)
                        {
                            remitentes = dto.Remitentes[i];
                        }
                        else
                        {
                            remitentes = remitentes + ", " + dto.Remitentes[i];
                        }
                    }
                    Remitentes.Set(context, remitentes);


                }
                else
                {
                    var resultado = new Resultado();
                    resultado.Errores.Add("Error", dto.Error);
                    Resultado.Set(context, resultado);
                }
            }
            catch (Exception e)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("", e.Message);
                Resultado.Set(context, resultado);
            }
        }
    }
}