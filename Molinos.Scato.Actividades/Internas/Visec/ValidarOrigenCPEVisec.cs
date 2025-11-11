using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class ValidarOrigenCPEVisec : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<CartaPorteDto> CartaPorte { get; set; }

        [RequiredArgument]
        public OutArgument<bool> EsUnidadProductiva { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resulado = new Resultado();
            var cartaPorte = CartaPorte.Get<CartaPorteDto>(context);
            var plantaOrigen = int.Parse(cartaPorte.CodEstab);
            var tipoOrigenCPE = VisecHelper.ObtenerTipoOrigenCPE(plantaOrigen);
            EsUnidadProductiva.Set(context, tipoOrigenCPE == TipoOrigenCPE.UnidadProductiva);
            return resulado;
        }
    }
}