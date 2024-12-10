using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarConfirmacionCargaDescarga : ProcesadorComando<ActualizarConfirmacionCargaDescarga>
    {
        public ProcesadorActualizarConfirmacionCargaDescarga(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarConfirmacionCargaDescarga comando)
        {
            var resultado = new Resultado();
            Validar(resultado, comando.RecorridoId);
            if (resultado.HayErrores)
                return resultado;

            try
            {
                ModificarEntidad(comando);
                Repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Ocurrió un error al actualizar la confirmación carga/descarga del workflow");
                resultado.Error("confirmacion", "Ocurrió un error al actualizar la confirmación carga/descarga del recorrido");
            }
            return resultado;
        }

        private void ModificarEntidad(ActualizarConfirmacionCargaDescarga comando)
        {
            var confirmacion = Repositorio.Obtener<ConfirmacionCargaDescarga>(x => x.Recorrido.Id == comando.RecorridoId);
            if(comando.DeshabilitarConfirmacion)
            {
                confirmacion.PendienteConfirmacion = false;
                return;
            }

            if(comando.Confirmar)
            {
                confirmacion.PendienteConfirmacion = false;
                confirmacion.Confirmado = true;
                confirmacion.FechaConfirmacion = DateTime.Now;
                confirmacion.NombreUsuario = comando.Usuario;
            }
        }

        public void Validar(Resultado resultado, int recorridoId)
        {
            if (!Repositorio.Existe<ConfirmacionCargaDescarga>(x => x.Recorrido.Id == recorridoId))
            {
                resultado.Error("confirmacion", "No existe un recorrido para confirmar la carga/descarga");
                return;
            }

            if (Repositorio.Existe<ConfirmacionCargaDescarga>(x => x.Recorrido.Id == recorridoId && !x.PendienteConfirmacion))
            {
                resultado.Error("confirmacion", "El recorrido no tiene una confirmación de carga/descarga pendiente");
                return;
            }
        }
    }
}