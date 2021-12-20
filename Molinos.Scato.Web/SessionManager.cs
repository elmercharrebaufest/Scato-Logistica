using System.Globalization;
using System.Threading;

namespace Molinos.Scato.Web
{
    public static class SessionManager
    {
        public static CultureInfo CurrentCulture
        {
            set
            {
                Thread.CurrentThread.CurrentUICulture = value;
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            }
            get { return Thread.CurrentThread.CurrentCulture; }
        }
    }
}