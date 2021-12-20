using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarCargaOpuesta : Comando
    {
        public int Carga_Id { get; set; }

        public string NumeroBalanza { get; set; }

        public int CargaOpuesta_Id { get; set; }
    }
}
