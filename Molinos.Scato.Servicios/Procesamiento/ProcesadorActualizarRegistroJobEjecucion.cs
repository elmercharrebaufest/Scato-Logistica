using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarRegistroJobEjecucion : ProcesadorComando<ActualizarRegistroJobEjecucion>
    {
        private ZSDWS_SCATO servicioSap;

        public ProcesadorActualizarRegistroJobEjecucion(IRepositorio repositorio, IConversor conversor, ILogger log, ZSDWS_SCATO servicioSap)
            : base(repositorio, conversor, log)
        {
            this.servicioSap = servicioSap;
        }

        public override Resultado Ejecutar(ActualizarRegistroJobEjecucion comando)
        {
            var resultado = new ResultadoCrear();
            var model = Repositorio.Obtener<RegistroJobEjecucion>(x => (x.NombreProceso == comando.Dto.NombreProceso));
            try
            {
                if (model == null)
                {
                    model = new RegistroJobEjecucion
                    {
                        NombreProceso = comando.Dto.NombreProceso,
                        Descripcion = comando.Dto.Descripcion,
                        FechaEjecucion = comando.Dto.FechaEjecucion,
                    };
                    Repositorio.Agregar(model);
                }
                else
                {
                    model.FechaEjecucion = comando.Dto.FechaEjecucion;
                }

                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                resultado.Errores.Add("", e.Message);
            }
            return resultado;
        }
    }
}