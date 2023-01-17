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
    public class ProcesadorEliminarExcepcionAlControlProveedor : ProcesadorComando<EliminarExcepcionAlControlProveedor>
    {
        public ProcesadorEliminarExcepcionAlControlProveedor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarExcepcionAlControlProveedor comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se está ejecutando ProcesadorEliminarExcepcionAlControl con Id = {0}", comando.Id);
                var excepcion = Repositorio.Obtener<ExcepcionAlControlProveedor>(comando.Id);
                excepcion.Motivo = MotivoExcepcionAlControl.B;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al borrar Excepcion Al Control con Id = {0}", comando.Id);
                resultado.Error("", Textos.Error_Generico);
            }

            return resultado;
        }
    }
}