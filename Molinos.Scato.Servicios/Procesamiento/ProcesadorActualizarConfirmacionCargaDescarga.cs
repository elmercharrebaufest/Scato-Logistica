using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
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
            Validar(resultado, comando.Dto.RecorridoId);
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
            var confirmacion = Repositorio.Obtener<ConfirmacionCargaDescarga>(x => x.Recorrido.Id == comando.Dto.RecorridoId);
            confirmacion.FechaConfirmacion = comando.Dto.FechaConfirmacion;
            confirmacion.Confirmado = comando.Dto.Confirmado;
            confirmacion.PendienteConfirmacion = comando.Dto.PendienteConfirmacion;
            confirmacion.NombreUsuario = comando.Dto.NombreUsuario;
        }

        public void Validar(Resultado resultado, int recorridoId)
        {
            if (!Repositorio.Existe<ConfirmacionCargaDescarga>(x => x.Recorrido.Id == recorridoId))
            {
                resultado.Error("confirmacion", "No existe un recorrido para confirmar la carga/descarga");
            }
        }
    }
}