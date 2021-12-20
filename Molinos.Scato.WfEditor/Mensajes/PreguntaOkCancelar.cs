using System;

namespace Molinos.Scato.WfEditor.Mensajes
{
    public class PreguntaOKCancelar
    {
        public string Titulo { get; set; }
        public string Texto { get; set; }

        public Action OK { get; set; }
        public Action Cancelar { get; set; }

        public PreguntaOKCancelar()
        {
            OK = () => { };
            Cancelar = () => { };
        }
    }
}
