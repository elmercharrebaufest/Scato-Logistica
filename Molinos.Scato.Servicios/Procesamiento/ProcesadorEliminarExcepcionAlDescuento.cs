using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarExcepcionAlDescuento : ProcesadorEliminar<EliminarExcepcionAlDescuento, ExcepcionAlDescuento>
    {
        public ProcesadorEliminarExcepcionAlDescuento(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarExcepcionAlDescuento comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarExcepcionAlDescuento comando, Resultado resultado)
        {
        }
    }
}