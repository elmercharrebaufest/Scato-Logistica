using System;
using System.Text;
using System.Web;

namespace Molinos.Scato.Web.Models
{
    public class CookieUsuario
    {
        private const string NombreCookie = "datosUsuario";
        private void Inicializar()
        {
            if (HttpContext.Current.Request.Cookies[NombreCookie] != null)
            {
                var cookie = HttpContext.Current.Request.Cookies[NombreCookie];
                cookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Current.Response.SetCookie(cookie);
            }
            else
            {
                NuevaCookie();
            }
        }

        private static HttpCookie NuevaCookie()
        {
            var nuevacookie = new HttpCookie(NombreCookie);
            nuevacookie.Values["Grupo"] = "";
            nuevacookie.Values["CentroId"] = "0";
            nuevacookie.Values["CentroDescripcion"] = "";
            nuevacookie.Values["CentroCodigoSap"] = "";
            nuevacookie.Values["BalanzaId"] = "0";
            nuevacookie.Values["CurrentCulture"] = "";
            nuevacookie.Values["visibles"] = "Patente|ProximaEtapa|Material|Transportista";
            nuevacookie.Values["Refresco"] = "false";
            nuevacookie.Values["Workflow"] = "";
            nuevacookie.Values["Patente"] = "";
            nuevacookie.Values["TipoDocumentoDeIngreso"] = "";
            nuevacookie.Values["ProximaAccion"] = "";
            nuevacookie.Values["NumeroDocumentoDeIngreso"] = "";
            nuevacookie.Values["SoloDemorados"] = "false";

            nuevacookie.Expires = DateTime.Now.AddYears(1);
            HttpContext.Current.Response.Cookies.Remove(NombreCookie);
            HttpContext.Current.Response.SetCookie(nuevacookie);
            return nuevacookie;
        }

        public string Valor(string clave)
        {
            var cookie = HttpContext.Current.Request.Cookies[NombreCookie];
            if (cookie == null)
            {
                cookie = NuevaCookie();
            }
            return cookie.Values[clave];
        }

        public void ActualizarValor(string clave, string valor)
        {
            var cookie = HttpContext.Current.Response.Cookies[NombreCookie];
            if (cookie == null || !cookie.HasKeys)
            {
                Inicializar();
                cookie = HttpContext.Current.Response.Cookies[NombreCookie];
            }
            cookie.Values[clave] = valor;
            HttpContext.Current.Response.Cookies.Remove(NombreCookie);
            HttpContext.Current.Response.SetCookie(cookie);
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append("Grupo=").Append(Valor("Grupo")).Append("; ")
                .Append("CentroId=").Append(Valor("CentroId")).Append("; ")
                .Append("CentroDescripcion=").Append(Valor("CentroDescripcion")).Append("; ")
                .Append("CentroCodigoSap=").Append(Valor("CentroCodigoSap")).Append("; ")
                .Append("BalanzaId=").Append(Valor("BalanzaId")).Append("; ")
                .Append("CurrentCulture=").Append(Valor("CurrentCulture")).Append("; ")
                .Append("visibles=").Append(Valor("visibles")).Append("; ")
                .Append("Refresco=").Append(Valor("Refresco")).Append("; ")
                .ToString();
        }
    }
}