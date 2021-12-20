using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorNotificacionAplicacion : ProcesadorComando<NotificacionAplicacionComando>
    {
        public ProcesadorNotificacionAplicacion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(NotificacionAplicacionComando comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se está ejecutando ProcesadorNotificacionAplicacion");
                var mensaje = Conversor.Convertir<NotificacionAplicacionDto, NotificacionAplicacion>(comando.Dto);

                mensaje.TipoAccion = comando.Dto.TipoAccion;
                mensaje.FechaAccion = comando.Dto.FechaAccion;
                mensaje.Titulo = comando.Dto.Titulo;
                mensaje.Detalle = comando.Dto.Detalle;
                mensaje.Usuario = comando.Dto.Usuario;

                Repositorio.Agregar(mensaje);
                Repositorio.GuardarCambios();

                //enviar email
                
                resultado.Id = mensaje.Id;
                return resultado;
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorNotificacionAplicacion");
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
