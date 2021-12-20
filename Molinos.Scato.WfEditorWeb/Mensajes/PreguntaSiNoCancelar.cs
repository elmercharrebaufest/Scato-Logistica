using System;

namespace Molinos.Scato.WfEditorWeb.Mensajes
{
    public class PreguntaSiNoCancelar
    {
        public string Titulo { get; set; }
        public string Texto { get; set; }

        public Action Si { get; set; }
        public Action No { get; set; }
        public Action Cancelar { get; set; }

        public PreguntaSiNoCancelar()
        {
            Si = () => { };
            No = () => { };
            Cancelar = () => { };
        }
    }
}
