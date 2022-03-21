using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class AsignarEstablecimiento : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<int?> EstablecimientoId { get; set; }


        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get<Guid>(context);
            var establecimientoId = EstablecimientoId.Get<int?>(context);
            var resultado = new Resultado();
            try
            {
                if (establecimientoId.HasValue)
                {
                    resultado = context.GetExtension<IServicioRepositorio>().ValidarStockEstablecimiento(establecimientoId.Value, instanceId);
                    if (!resultado.HayErrores)
                    {
                        var servicioComandos = context.GetExtension<IServicioComandos>();
                        resultado = servicioComandos.Ejecutar(new ModificarRecorridoEstablecimiento
                            {
                                InstanceId = instanceId,
                                EstablecimientoId = establecimientoId.Value
                            });

                        var cartaPorte = context.GetExtension<IServicioRepositorio>().ObtenerCartaPortePorInstanceId(instanceId);
                        var recorrido = context.GetExtension<IServicioRepositorio>().ObtenerRecorridoPorGuid(instanceId);
                        var centroId = context.GetExtension<IServicioRepositorio>().ObtenerCentroIdPorInstanceId(instanceId);
                        var centroSap = context.GetExtension<IServicioRepositorio>().ObtenerCentroCodigoSap(centroId);

                        servicioComandos.Ejecutar(new AgregarMarcaSustentable
                        {
                            RutaFotoCP = cartaPorte.FotoRutaDestino,
                            CodigoCentroSap = centroSap,
                            NroDocumento = cartaPorte.NroCartaPorte,
                            Patente = recorrido.Patente
                        });

                        if (resultado.HayErrores)
                        {
                            resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
                        }
                    }
                    
                }
            }
            catch (Exception)
            {
                resultado.Errores.Add("1", Textos.Error_ActualizarGenerico);
            }
            return resultado;          
        }
    }
}
