using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class ValidarContratoEnSap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<string> Contrato { get; set; }
        [RequiredArgument]
        public InArgument<string> Material { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioSap = context.GetExtension<ZSDWS_SCATO>();
            var requestObj = new ValidaContratosRequest
                {
                  ValidaContratos = new ValidaContratos
                      {
                          Contrato = Contrato.Get<string>(context),
                          Material = Material.Get<string>(context)
                      }
                };
            var resultado = new Resultado();
            try
            {
                var respuesta = servicioSap.ValidaContratos(requestObj);
                if (Convert.ToInt32(respuesta.ValidaContratosResponse.Resultado.CODIGO) == 0)
                {
                    return resultado;
                }
                resultado.Errores.Add("Contrato", Textos.ValidarContrato_Inexistente);
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.IngresarLote_ErrorSap);
            }
            return resultado;
        }
    }
}