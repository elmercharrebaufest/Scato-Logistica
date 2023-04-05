using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.WebMobile.ViewModel
{
    public class PesadaHistoricaViewModel
    {
        public PesadaHistoricaViewModel()
        {
            DateStart = DateTime.Now;
            DateFin = DateTime.Now;
            BeginTime = TimeSpan.Parse("00:00");
            EndTime = TimeSpan.Parse("23:59");
        }
        public DateTime DateStart { get; set; }
        public DateTime DateFin { get; set; }
        public TimeSpan BeginTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}