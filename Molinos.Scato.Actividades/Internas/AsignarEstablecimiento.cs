using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

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
            var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
            var servicioComandos = context.GetExtension<IServicioComandos>();

            var instanceId = InstanceId.Get<Guid>(context);
            var establecimientoId = EstablecimientoId.Get<int?>(context);
            var resultado = new Resultado();

            try
            {
                if (establecimientoId.HasValue)
                {
                    var establecimientoModel = servicioRepositorio.ObtenerEstablecimiento(establecimientoId ?? 0);
                    int.TryParse(establecimientoModel.CodigoDeEstablecimiento, out int codigoEstablecimiento);
                    if (codigoEstablecimiento <= Constantes.AsignacionDeEstablecimientoRangos.Desde
                       || codigoEstablecimiento >= Constantes.AsignacionDeEstablecimientoRangos.Hasta)
                    {
                        resultado = servicioRepositorio.ValidarStockEstablecimiento(establecimientoId.Value, instanceId);
                    }
                    if (!resultado.HayErrores)
                    {
                        var materialId = servicioRepositorio.ObtenerMaterialIdPorInstanceId(instanceId);
                        var tipoMaterialPorVariedad = servicioRepositorio.ObtenerVariedadIdPorMaterial(materialId, esEpa: establecimientoModel.EsSojaEPA, esSustentable: true, esEUDR: establecimientoModel.EsEUDR);

                        resultado = servicioComandos.Ejecutar(new ModificarRecorridoEstablecimiento
                        {
                            InstanceId = instanceId,
                            EstablecimientoId = establecimientoId.Value,
                            TipoVariedadId = tipoMaterialPorVariedad.GetValueOrDefault(),
                        });

                        var cartaPorte = servicioRepositorio.ObtenerCartaPortePorInstanceId(instanceId);
                        var recorrido = servicioRepositorio.ObtenerRecorridoPorGuid(instanceId);

                        servicioComandos.Ejecutar(new AgregarMarcaSustentable
                        {
                            RutaFotoCP = cartaPorte.FotoRutaDestino,
                            CodigoCentroSap = recorrido.Centro.CodigoSAP,
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