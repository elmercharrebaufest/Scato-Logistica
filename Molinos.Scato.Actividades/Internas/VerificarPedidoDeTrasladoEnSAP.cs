using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{

    public sealed class VerificarPedidoDeTrasladoEnSAP : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> CentroDestinoId { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroEmisorId { get; set; }

        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }

        [RequiredArgument]
        public InArgument<int> TransportistaId { get; set; }

        [RequiredArgument]
        public InArgument<int> IntermediarioFleteId { get; set; }

        public OutArgument<bool> Existe { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {

            var resultado = new Resultado();

            try
            {
                var servicioSap = context.GetExtension<ZSDWS_SCATO>();
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var centroEmisor = srvRepositorio.ObtenerCentro(CentroEmisorId.Get(context));
                var centroDestino = srvRepositorio.ObtenerCentro(CentroDestinoId.Get(context));
                var material = srvRepositorio.ObtenerMaterial(MaterialId.Get(context));
                var transportista = srvRepositorio.ObtenerTransportista(TransportistaId.Get(context));
                var intermediarioFlete = srvRepositorio.ObtenerProveedor(IntermediarioFleteId.Get(context));

                var respuesta =
                    servicioSap.VerifPedTrasladoRedespacho(new VerifPedTrasladoRedespachoRequest
                    {
                        VerifPedTrasladoRedespacho = new VerifPedTrasladoRedespacho
                            {
                                CentroDestino = centroDestino.CodigoSAP,
                                CentroEmisor = centroEmisor.CodigoSAP,
                                Material = material.CodigoSAP,
                                Transportista = intermediarioFlete != null ? intermediarioFlete.Cuil.Replace("-", "") : transportista.Cuit.Replace("-", "")
                            }
                    });
                Existe.Set(context, respuesta.VerifPedTrasladoRedespachoResponse.Planificado == "X");
                //Existe.Set(context,true);
                if (!Existe.Get(context))
                {
                    resultado.Errores.Add("WorkflowId", Textos.ExistePedidoDeTraslado_Titulo);
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("WorkflowId", Textos.MovimientoStockSap_ErrorEnLaCarga + ": " + e.Message);
            }
            return resultado;

        }
    }
}
