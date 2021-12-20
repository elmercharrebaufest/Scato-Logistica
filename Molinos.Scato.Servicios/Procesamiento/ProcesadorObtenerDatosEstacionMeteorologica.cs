using System;
using System.Collections.Generic;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorObtenerDatosEstacionMeteorologica : ProcesadorComando<ObtenerDatosEstacionMeteorologica>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;

        public ProcesadorObtenerDatosEstacionMeteorologica(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioOrquestador orquestador) : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
        }        

        public override Resultado Ejecutar(ObtenerDatosEstacionMeteorologica comando)
        {
            var resultado = new ResultadoEstacionMeteorologica() {
                Imagenes = new List<EstacionMeteorologicaDto>()
            };

            try
            {
                Log.Info("Se está ejecutando ProcesadorObtenerDatosEstacionMeteorologica", comando.CodigoDispositivo);

                var respuesta = (ResultadoMeteorologica) orquestador.Ejecutar(new EjecutarEstacionMeteorologica { CodigoDispositivo = comando.CodigoDispositivo });
                Log.Info("Llamada al orquestador exitosa.");

                if (respuesta.Mensaje.Codigo != 0)
                {
                    Log.Error("La información de la estación meteorológica fue recibida con error.");
                    resultado.Error("999", "La información de la estación meteorológica fue recibida con error");
                    return resultado;
                }

                foreach (var info in respuesta.Imagenes) {
                    resultado.Imagenes.Add(new EstacionMeteorologicaDto() {
                         Descripcion = info.Descripcion,
                         Detalle = info.Detalle,
                         Imagenes = info.Imagenes
                    });
                }

                return resultado;
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorObtenerDatosEstacionMeteorologica", comando.CodigoDispositivo);
                resultado.Error("", e.Message);
            }

            return resultado;
        }


    }
}
