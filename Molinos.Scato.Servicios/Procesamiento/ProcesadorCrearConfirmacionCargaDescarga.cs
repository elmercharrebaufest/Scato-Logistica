using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearConfirmacionCargaDescarga : ProcesadorComando<CrearConfirmacionCargaDescarga>
    {
        private readonly IServicioComandos servicioComandos;
        public ProcesadorCrearConfirmacionCargaDescarga(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(CrearConfirmacionCargaDescarga comando)
        {
            var resultado = new Resultado();
            
            try
            {
                var confirmacionBase = Repositorio.Obtener<ConfirmacionCargaDescarga>(x => x.Recorrido.Id == comando.Dto.RecorridoId);

               

                if(confirmacionBase == null)
                {
                    var confirmacion = new ConfirmacionCargaDescarga()
                    {
                        FechaCreacion = DateTime.Now,
                        Recorrido = Repositorio.Obtener<Recorrido>(x => x.Id == comando.Dto.RecorridoId),
                        Confirmado = comando.Dto.Confirmado,
                        PendienteConfirmacion = comando.Dto.PendienteConfirmacion
                    };
                    Repositorio.Agregar(confirmacion);
                }
                else
                {
                    confirmacionBase.Confirmado = comando.Dto.Confirmado;
                    confirmacionBase.PendienteConfirmacion = comando.Dto.PendienteConfirmacion;
                }

                
                Repositorio.GuardarCambios();
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Ocurrió un error al registrar la confirmación carga/descarga al workflow");
                resultado.Error("confirmacion", "Ocurrió un error al registrar la confirmación carga/descarga al recorrido");
            }
            return resultado;
        }

        public void Validar(Resultado resultado, int recorridoId)
        {
            if(Repositorio.Existe<ConfirmacionCargaDescarga>(x => x.Recorrido.Id == recorridoId))
            {
                resultado.Error("confirmacion", "La carga/descarga del recorrido ya fue confirmado");
            }
        }
    }
}
