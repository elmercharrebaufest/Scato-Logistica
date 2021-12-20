using System;

namespace Molinos.Scato.Web.Models.ArchivosTxt
{
    public class TxtColumnaAttribute : Attribute
    {
        public int Order { get; set; }
        public int Longitud { get; set; }
        public TxtColumnaAttribute()
        {
            Order = int.MaxValue; //Unordered columns are at the end
        }

    }
}