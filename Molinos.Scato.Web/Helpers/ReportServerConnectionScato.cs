using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Security.Principal;
using Microsoft.Reporting.WebForms;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Web.Helpers
{
    public sealed class ReportServerConnectionScato : IReportServerConnection2
    {
        public WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                // Read the user information from the web.config file.  
                // By reading the information on demand instead of 
                // storing it, the credentials will not be stored in 
                // session, reducing the vulnerable surface area to the
                // web.config file, which can be secured with an ACL.

                // User name
                string userName =
                    ConfigurationManager.AppSettings
                        ["Reportes.Username"];
                // Password
                string password = Encriptador.Decrypt(
                    ConfigurationManager.AppSettings
                        ["Reportes.Password"]);
                // Domain
                string domain =
                    ConfigurationManager.AppSettings
                        ["Reportes.Domain"];
                return new NetworkCredential(userName, password, domain);
            }
        }

        public bool GetFormsCredentials(out Cookie authCookie,
                                        out string userName, out string password,
                                        out string authority)
        {
            authCookie = null;
            userName = null;
            password = null;
            authority = null;

            // Not using form credentials
            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "No se puede convertir en un metodo porque se debe implementar la interface")]
        public Uri ReportServerUrl
        {
            get
            {
                string url =
                    ConfigurationManager.AppSettings[
                        "MvcReportViewer.ReportServerUrl"];

                if (string.IsNullOrEmpty(url))
                {
                    throw new Exception("Missing url from the Web.config file");
                }

                return new Uri(url);
            }
        }

        public int Timeout
        {
            get 
            { 
                return 60000; // 60 seconds
            }
        }

        public IEnumerable<Cookie> Cookies
        {
            get
            {
                // No custom cookies
                return null;
            }
        }

        public IEnumerable<string> Headers
        {
            get
            {
                // No custom headers
                return null;
            }
        }
    }
}