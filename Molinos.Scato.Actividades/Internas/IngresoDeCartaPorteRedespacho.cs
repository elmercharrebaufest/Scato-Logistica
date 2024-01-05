using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresoDeCartaPorteRedespacho : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<CartaPorteDto> CartaPorte { get; set; }

        public InArgument<Pesada> Pesada { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var servicioRepositorio = context.GetExtension<IServicioRepositorio>();

            var cartaPorte = CartaPorte.Get<CartaPorteDto>(context);
            var resultado = new Resultado();
            try
            {
                var tipoMaterialPorVariedad = servicioRepositorio.ObtenerVariedadIdPorMaterial(cartaPorte.MaterialId, cartaPorte.TitularCartaPorteCodigoSap);
                servicioComandos.Ejecutar(new Dominio.Comandos.CrearCartaPorte
                {
                    Orden = cartaPorte,
                    NombreWorkflow = "",
                    InstanciaWorkflowId = context.WorkflowInstanceId,
                    TipoVariedadId = tipoMaterialPorVariedad,
                });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Transportista_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}