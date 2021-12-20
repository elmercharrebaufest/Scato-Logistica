using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Molinos.Scato.ModuloImpresor
{
    public static class Extensor
    {
        public static string GetValueOrDefault(this object entity, string propertyName)
        {
            var direccion = propertyName.Split('/').ToList();
            if (direccion.Count == 0)
            {
                return null;
            }

            var aux = direccion[0];
            var propiedad = entity.GetType().GetProperty(aux);

            if (direccion.Count > 1)
            {
                direccion.RemoveAt(0);
                return propiedad == null ? null : GetValueOrDefault(propiedad.GetValue(entity, null), String.Join("/", direccion.ToArray()));
            }
            var value = propiedad.GetValue(entity, null);
            if (value == null)
            {
                return null;
            }
            if (value is DateTime)
            {
                return String.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", ((DateTime)value));
            }
            return value.ToString();
        }

        public static string GetTotalMesOrDefault(this object entity, string propertyName)
        {
            var direccion = propertyName.Split('/').ToList();
            if (direccion.Count == 0)
            {
                return null;
            }

            var aux = direccion[0];
            var propiedad = entity.GetType().GetProperty(aux);

            if (direccion.Count > 1)
            {
                direccion.RemoveAt(0);
                return propiedad == null ? null : GetTotalMesOrDefault(propiedad.GetValue(entity, null), String.Join("/", direccion.ToArray()));
            }
            var value = propiedad.GetValue(entity, null);
            if (value == null)
            {
                return null;
            }
            if (value is DateTime)
            {
                return String.Format(CultureInfo.CurrentCulture, "{0:M/yyyy}", ((DateTime)value));
            }
            return value.ToString();
        }





        public static List<List<T>> Split<T>(this IEnumerable<T> list, int partes)
        {
            return list
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / partes)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
