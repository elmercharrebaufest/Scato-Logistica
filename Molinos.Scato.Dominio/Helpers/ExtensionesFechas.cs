using System;

namespace Molinos.Scato.Dominio.Helpers
{
    public static class ExtensionesFechas
    {
        public static DateTime FinDelDia(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day,23, 59, 59, 999);
        }

        public static DateTime? FinDelDia(this DateTime? date)
        {
            return date.HasValue ? new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, 23, 59, 59, 999) : date;
        }

        public static DateTime InicioDelDia(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }
    }
}
