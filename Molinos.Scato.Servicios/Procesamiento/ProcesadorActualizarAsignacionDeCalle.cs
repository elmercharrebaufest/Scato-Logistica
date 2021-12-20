using System;
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
    public class ProcesadorActualizarAsignacionDeCalle : ProcesadorComando<ActualizarAsignacionDeCalle>
    {
        public ProcesadorActualizarAsignacionDeCalle(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarAsignacionDeCalle comando)
        {
            var resultado = new Resultado();
            try
            {
                var calle = Repositorio.Obtener<Calle>(comando.CalleId);
                var hidraulicas = Repositorio.Listar<PuestosDeCargaDescarga>(x => comando.HidraulicasId.Contains(x.Id));
                if (hidraulicas.Any(x=>x.Codigo.Trim() == "HID6" || x.Codigo.Trim() == "HID7"))
                {
                    if (calle.HidraulicaAsignada == 0)
                    {
                        calle.CantidadDeCamiones += 2;
                        calle.HidraulicaAsignada = 6;
                    }
                    if (calle.HidraulicaAsignada == 5)
                    {
                        calle.CantidadDeCamiones += 1;
                        calle.HidraulicaAsignada = 6;
                    }
                }
                else if(hidraulicas.Any(x => x.Codigo.Trim() == "HID5"))
                {
                    if (calle.HidraulicaAsignada == 0)
                    {
                        calle.CantidadDeCamiones += 1;
                        calle.HidraulicaAsignada = 5;
                    }
                    if (calle.HidraulicaAsignada == 6)
                    {
                        calle.CantidadDeCamiones -= 1;
                        calle.HidraulicaAsignada = 5;
                    }
                }
                {
                    if (calle.HidraulicaAsignada == 6)
                    {
                        calle.CantidadDeCamiones -= 2;
                        calle.HidraulicaAsignada = 0;
                    }
                    if (calle.HidraulicaAsignada == 5)
                    {
                        calle.CantidadDeCamiones -= 1;
                        calle.HidraulicaAsignada = 0;
                    }
                }
            }
            catch (Exception)
            {
                resultado.Error("", Textos.Observacion_ErrorEnLaCarga);
            }
            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }
            return resultado;
        }
    }
}
