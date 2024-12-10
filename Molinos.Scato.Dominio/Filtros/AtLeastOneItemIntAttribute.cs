using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Filtros
{
    public class AtLeastOneItemIntAttribute : ValidationAttribute
    {
        public AtLeastOneItemIntAttribute(string errMsg)
            : base(errMsg)
        { }

        public override bool IsValid(object value)
        {
            if (value is IList<int> list)
            {
                if (list.Count == 0)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
