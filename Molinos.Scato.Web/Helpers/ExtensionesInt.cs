using System;
using System.Web;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Helpers
{
    public static class ExtensionesInt
    {
        public static string ToStringAbreviado(this int number)
        {
            if (number > 1000000)
            {
                return (number / 1000000).ToString() + "M";
            }                
            else if (number > 1000)
            {
                 return (number / 1000).ToString() + "K";
            }
            else
            {
                 return number.ToString();
            }
                
        } 

    }
}
