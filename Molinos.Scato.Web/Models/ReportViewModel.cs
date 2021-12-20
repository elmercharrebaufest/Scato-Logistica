using System;
using System.Collections.Generic;

namespace Molinos.Scato.Web.Models
{
    public class ReportViewModel
    {
        public string ReportPath { get; internal set; }
        public IEnumerable<KeyValuePair<string, object>> ReportParameterList { get; internal set; }
        public int CentroId { get; internal set; }
        public string Language { get; internal set; }
    }
}