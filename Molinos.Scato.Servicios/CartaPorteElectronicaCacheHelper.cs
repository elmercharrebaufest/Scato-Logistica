using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Molinos.Scato.Servicios.Helpers
{
    /// <summary>
    /// Persistencia de la caché de <see cref="CartaPorteElectronica"/>, compartida entre
    /// ProcesadorConsultarCPDigital (flujo interactivo) y ProcesadorCachearCPEAfip
    /// (job batch liviano). No dispara ninguna sincronización adicional (Proveedores/SAP) ni render de
    /// PDF: es la única responsabilidad de esta clase, separada del mapeo de la respuesta AFIP
    /// (CpeAfipRespuestaMapper) para no mezclar reglas de conversión con acceso a datos.
    /// </summary>
    public static class CartaPorteElectronicaCacheHelper
    {
        // Campos de bookkeeping que no representan un dato de la Carta Porte en si:
        // Id nunca cambia, FechaCacheado/FechaUltimaActualizacion son las fechas que esta clase calcula.
        private static readonly HashSet<string> PropiedadesExcluidasDeComparacion = new HashSet<string>
        {
            nameof(CartaPorteElectronica.Id),
            nameof(CartaPorteElectronica.FechaCacheado),
            nameof(CartaPorteElectronica.FechaUltimaActualizacion)
        };

        private static readonly PropertyInfo[] PropiedadesComparables = typeof(CartaPorteElectronica)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !PropiedadesExcluidasDeComparacion.Contains(p.Name))
            .ToArray();

        /// <summary>
        /// Inserta la <paramref name="cpe"/> si no existe una fila cacheada para su <see cref="CartaPorteElectronica.NroCTG"/>,
        /// o actualiza la existente en caso contrario (upsert).
        /// </summary>
        public static void Registrar(IRepositorio repositorio, IConversor conversor, CartaPorteElectronica cpe)
        {
            var cpeExistente = repositorio.Obtener<CartaPorteElectronica>(x => x.NroCTG == cpe.NroCTG);

            if (cpeExistente == null)
                Insertar(repositorio, cpe);
            else
                Actualizar(conversor, cpeExistente, cpe);

            repositorio.GuardarCambios();
        }

        private static void Insertar(IRepositorio repositorio, CartaPorteElectronica cpe)
        {
            repositorio.Agregar(cpe);
        }

        private static void Actualizar(IConversor conversor, CartaPorteElectronica destino, CartaPorteElectronica origen)
        {
            var idOriginal = destino.Id;
            var fechaCacheadoAnterior = destino.FechaCacheado;
            var fechaUltimaActualizacionAnterior = destino.FechaUltimaActualizacion;
            var valoresAnteriores = PropiedadesComparables.ToDictionary(p => p.Name, p => p.GetValue(destino));

            conversor.Convertir(origen, destino);

            destino.Id = idOriginal;

            if (origen.Pdf != null)
                destino.Pdf = origen.Pdf;

            var huboCambios = HuboCambios(valoresAnteriores, destino);
            var ahora = DateTime.Now;

            // FechaCacheado representa el momento del alta en la caché: se fija una unica vez en
            // Insertar (via CpeAfipRespuestaMapper) y nunca se vuelve a tocar en una actualizacion,
            // sea o no que haya novedades. Se restaura siempre al valor previo porque el conversor
            // pisa este campo con el "ahora" que trae la entidad recien mapeada desde AFIP.
            destino.FechaCacheado = fechaCacheadoAnterior;

            // FechaUltimaActualizacion solo avanza cuando algun dato de la Carta Porte realmente
            // cambio en AFIP (Estado o cualquier otro campo del comprobante). Si se re-consulta
            // (manual o por el job periodico) y no hay novedades, se conserva la fecha de la ultima
            // modificacion real (afecta el indicador de frescura en Monitor CPE Cacheada).
            destino.FechaUltimaActualizacion = huboCambios ? ahora : fechaUltimaActualizacionAnterior;
        }

        private static bool HuboCambios(IReadOnlyDictionary<string, object> valoresAnteriores, CartaPorteElectronica destino)
        {
            foreach (var propiedad in PropiedadesComparables)
            {
                var valorAnterior = valoresAnteriores[propiedad.Name];
                var valorNuevo = propiedad.GetValue(destino);

                if (!SonIguales(valorAnterior, valorNuevo))
                    return true;
            }

            return false;
        }

        private static bool SonIguales(object valorAnterior, object valorNuevo)
        {
            if (valorAnterior is byte[] bytesAnteriores && valorNuevo is byte[] bytesNuevos)
                return bytesAnteriores.SequenceEqual(bytesNuevos);

            return Equals(valorAnterior, valorNuevo);
        }
    }
}
