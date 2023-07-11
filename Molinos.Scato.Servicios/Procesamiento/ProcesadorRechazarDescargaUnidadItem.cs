using System;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorRechazarDescargaUnidadItem : ProcesadorComando<RechazarDescargaUnidadItem>
    {
        private ZSDWS_SCATO servicioSap;

        public ProcesadorRechazarDescargaUnidadItem(IRepositorio repositorio, IConversor conversor, ILogger log,
                                             ZSDWS_SCATO servicioSap)
            : base(repositorio, conversor, log)
        {
            this.servicioSap = servicioSap;
        }

        public override Resultado Ejecutar(RechazarDescargaUnidadItem comando)
        {
            var resultado = new Resultado();

            foreach (var item in comando.Dto.Select(itemDto => Repositorio.Obtener<DescargaUnidadItem>(itemDto.Id)))
            {
                try
                {
                    var respuesta =
                        servicioSap.AnulaContabilizacion(new AnulaContabilizacionRequest(new AnulaContabilizacion
                            {
                                DocMaterial = item.DocMaterial,
                                Ejercicio = item.EjercicioDocMaterial
                            }));
                    if (respuesta.AnulaContabilizacionResponse.Return == null ||
                        !respuesta.AnulaContabilizacionResponse.Return.Any())
                    {
                        item.Rechazado = true;
                        item.EjercicioDocMaterialAnulacion = respuesta.AnulaContabilizacionResponse.EjercicioDocMaterial;
                    }
                    else
                    {
                        resultado.Errores.Add(item.Id.ToString(CultureInfo.InvariantCulture), respuesta.AnulaContabilizacionResponse.Return[0].MESSAGE);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error al anular contabilización item {0}", item.Id);
                    resultado.Errores.Add(item.Id.ToString(CultureInfo.InvariantCulture) + "exception", e.Message);
                }
            }
            try
            {
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar dar de baja el item #{0}",comando.Dto.LastOrDefault()?.ItemNro);
                resultado.Errores.Add("",Textos.Error_ActualizarGenerico + ":" + e.Message);
            }
            return resultado;
        }
    }
}