using System;
using System.Activities;
using System.Linq;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Impl;

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
                    var establecimientoModel = context.GetExtension<IServicioRepositorio>().ObtenerEstablecimiento(establecimientoId ?? 0);
                    int.TryParse(establecimientoModel.CodigoDeEstablecimiento, out int codigoEstablecimiento);
                    if (codigoEstablecimiento <= Constantes.AsignacionDeEstablecimientoRangos.Desde
                       || codigoEstablecimiento >= Constantes.AsignacionDeEstablecimientoRangos.Hasta) { 
                        resultado = context.GetExtension<IServicioRepositorio>().ValidarStockEstablecimiento(establecimientoId.Value, instanceId);
                    }
                    if (!resultado.HayErrores)
                    {
                        var tipoMaterialPorVariedad = context.GetExtension<IServicioRepositorio>().ObtenerTipoVariedadMaterial(instanceId);
                        var servicioComandos = context.GetExtension<IServicioComandos>();
                        
                        resultado = servicioComandos.Ejecutar(new ModificarRecorridoEstablecimiento
                        {
                            InstanceId = instanceId,
                            EstablecimientoId = establecimientoId.Value,
                            TipoVariedadId = tipoMaterialPorVariedad.First().Key
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
