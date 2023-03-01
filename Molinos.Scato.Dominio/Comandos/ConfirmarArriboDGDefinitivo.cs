using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ConfirmarArriboDGDefinitivo : Comando
    {
        public OrdenDeDescargaFasonDto Dto { get; set; }
    }
}
