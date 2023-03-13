using System;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ConfirmarArriboDG : Comando
    {
        public OrdenDeDescargaFasonDto Dto { get; set; }
    }
}
