using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Molinos.Scato.WebMobile.ViewModel
{
    public class PesadaOnlineViewModel
    {
        public DateTime DateNow { get; set; }
        public TimeSpan BeginTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}